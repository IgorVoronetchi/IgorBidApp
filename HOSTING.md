# Ghid de hosting — Bid-App-Igor

Cum publici aplicația ca s-o poată testa oricine, printr-un link.

**Recomandare:** frontend pe **Netlify**, backend pe **Render**. Ambele au un tier gratuit
suficient pentru un demo, deploy automat la fiecare push pe GitHub, și configurare simplă.

> **Alternative pentru backend**, dacă vrei să compari: **Railway** (developer experience
> la fel de bună, credit gratuit la înregistrare, apoi plată la consum) sau **Fly.io**
> (mai potrivit dacă ai nevoie de disc persistent între restart-uri — nu e cazul aici,
> vezi nota despre SQLite mai jos). Dacă nu ai un motiv anume, rămâi la Render — e cel mai
> simplu de configurat pentru un API ASP.NET Core.

## De ce contează arhitectura noastră aici

- Baza de date e **SQLite** (un singur fișier). Majoritatea platformelor gratuite au disc
  efemer (se resetează la fiecare redeploy/restart). **Pentru noi asta e o caracteristică,
  nu o problemă** — aplicația repopulează automat datele demo la fiecare pornire
  (`DbSeeder.Seed`), deci un reset accidental de disc înseamnă doar date demo proaspete,
  nu pierdere reală.
- Frontend-ul (Angular) și backend-ul (.NET) vor fi pe **domenii diferite** odată deployate,
  deci browser-ul aplică CORS — de-asta am făcut originile permise configurabile (vezi mai jos).
- Free tier-ul de Render "adoarme" backend-ul după ~15 min de inactivitate; primul request
  după somn poate dura 30-60s. Normal pentru un demo, dar avertizează-l pe cel care testează.

---

## Pasul 0 — Pune codul pe GitHub

Proiectul nu e încă un repo git. Rulează tu (nu eu) aceste comenzi din rădăcina proiectului:

```powershell
cd C:\Users\Igor\Desktop\BidAppDemo
git init
git add .
git commit -m "Initial commit"
```

Apoi creează un repo nou pe [github.com/new](https://github.com/new) (fără README/gitignore,
le ai deja), și:

```powershell
git remote add origin https://github.com/<user-ul-tau>/<nume-repo>.git
git branch -M main
git push -u origin main
```

`.gitignore`-ul de la rădăcină exclude deja `bin/`, `obj/`, `bidapp.db`, `node_modules/`, etc.

---

## Pasul 1 — Backend pe Render

1. Cont gratuit pe [render.com](https://render.com) (login cu GitHub e cel mai rapid).
2. **New +** → **Web Service** → conectează repo-ul.
3. Setări:
   - **Root Directory:** `backend`
   - **Runtime:** `Docker` (Render detectează automat `backend/Dockerfile`, deja pregătit)
   - **Instance Type:** Free
4. **Environment Variables** (tab Environment, înainte de primul deploy):

   | Key | Value |
   |---|---|
   | `Jwt__Secret` | un string lung, random, generat de tine — **nu** cel din appsettings.json |

   > ⚠️ **Important:** `appsettings.json` conține un secret JWT de exemplu, comis în repo.
   > Dacă repo-ul e public pe GitHub, oricine poate genera token-uri valide cu acel secret.
   > Setează `Jwt__Secret` ca variabilă de mediu pe Render cu o valoare nouă, random
   > (ex: generată cu `openssl rand -base64 48` sau orice generator de parole lungi) —
   > variabilele de mediu au prioritate peste appsettings.json automat, fără cod suplimentar.

5. Click **Create Web Service**. Primul build durează câteva minute (compilează imaginea Docker).
6. Copiază URL-ul public alocat (ceva de forma `https://bid-app-igor-api.onrender.com`).
7. Verifică rapid: `https://<url-ul-tau>/swagger` ar trebui să arate Swagger UI.

---

## Pasul 2 — Frontend pe Netlify

1. Înainte de push, editează **`frontend/src/environments/environment.ts`**:

   ```ts
   export const environment = {
     apiUrl: 'https://bid-app-igor-api.onrender.com/api',  // URL-ul tau de la Render + /api
   };
   ```

   Fă commit + push la această modificare.

2. Cont gratuit pe [netlify.com](https://netlify.com) (login cu GitHub).
3. **Add new site** → **Import an existing project** → alege repo-ul.
   `netlify.toml` de la rădăcina repo-ului configurează deja totul automat
   (`base = frontend`, comanda de build, folderul de publish, și redirect-ul SPA
   necesar ca refresh-ul pe `/auctions` sau orice altă rută să nu dea 404).
4. Click **Deploy site**. Copiază URL-ul alocat (ex: `https://bid-app-igor.netlify.app`) —
   sau setează unul propriu din **Site settings → Change site name**.

---

## Pasul 3 — Închide bucla CORS

Acum că ai URL-ul final de Netlify, întoarce-te pe Render:

1. **Environment** → adaugă:

   | Key | Value |
   |---|---|
   | `Cors__AllowedOrigins__0` | `https://bid-app-igor.netlify.app` (URL-ul tau exact de Netlify) |

2. Salvează — Render redeploy-uiește automat cu noua configurație.

---

## Pasul 4 — Trimite linkul

Link-ul de Netlify e tot ce trebuie trimis. Cel care testează poate:

- să-și creeze cont propriu din **Join free**, sau
- să folosească un cont demo direct:

  | Rol | Email | Parolă |
  |---|---|---|
  | Admin | igor@bidapp.ro | Igor123 |
  | User | radu@demo.ro | parola123 |

Menționează-i că **primul request poate dura 30-60s** dacă backend-ul "a adormit"
din lipsă de activitate (tier gratuit) — nu e o eroare, doar trezirea containerului.

---

## Depanare rapidă

| Simptom | Cauză probabilă |
|---|---|
| Pagina se încarcă dar nu apar licitații | `environment.ts` încă are URL-ul placeholder, sau ai uitat push-ul după editare |
| Eroare CORS în consolă (F12) | `Cors__AllowedOrigins__0` pe Render nu se potrivește exact cu URL-ul Netlify (fără `/` la final) |
| 404 la refresh pe orice pagină non-home | `netlify.toml` nu a fost detectat — verifică ca fișierul e la rădăcina repo-ului, nu în `frontend/` |
| Backend nu pornește pe Render | Verifică log-urile de build din dashboard; cel mai des e o eroare in Dockerfile sau lipsa `Jwt__Secret` |
