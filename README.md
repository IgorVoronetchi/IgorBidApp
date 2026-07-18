# Bid-App-Igor — Marketplace de licitații online

Aplicație full-stack de licitații (un „eBay simplificat").

- **Backend:** ASP.NET Core Web API (.NET 10) + EF Core + SQLite + JWT — folder `backend/`
- **Frontend:** Angular 21 (standalone components + signals) — folder `frontend/`

## Cum rulezi aplicația

### 1. Backend (port 5000)

```powershell
cd backend
dotnet run
```

La prima pornire se creează automat baza de date `bidapp.db` (SQLite) cu date demo.
Pentru a reseta demo-ul, șterge `backend/bidapp.db` și repornește.

### 2. Frontend (port 4200)

```powershell
cd frontend
npm install   # doar prima dată
npx ng serve
```

Apoi deschide **http://localhost:4200**.

## Conturi demo

| Rol   | Email             | Parolă     | Observații                                  |
|-------|-------------------|------------|---------------------------------------------|
| Admin | igor@bidapp.ro    | Igor123    | Vede pagina **Admin** (validare obiecte)    |
| User  | radu@demo.ro      | parola123  | Are recenzii, un obiect câștigat, wishlist  |
| User  | ana@demo.ro       | parola123  | Licitator activ                             |
| User  | vlad@demo.ro      | parola123  | Vânzător (camera Zenit)                     |

Poți crea oricând un cont nou din **Join free**.

## Testare

- **Swagger** (doar Development): `http://localhost:5000/swagger` — testează orice endpoint direct din browser,
  inclusiv cele protejate (buton **Authorize**, lipești tokenul primit de la login).
- **Inspectarea bazei de date**: `backend/bidapp.db` e un fișier SQLite obișnuit — deschide-l cu
  [DB Browser for SQLite](https://sqlitebrowser.org/) (gratuit) ca să vezi tabelele/datele direct.
  Nu există legătură cu SQL Server/LocalDB.
- **Test de regresie API** (fără browser): `powershell -File tests/api-smoke-test.ps1` — 25 de verificări
  automate (auth, validări, bidding, wishlist, forum, recenzii, permisiuni).
- **E2E prin browser** (Selenium + Python): vezi [tests/selenium/README.md](tests/selenium/README.md).
  ```powershell
  pip install -r tests/selenium/requirements.txt
  cd tests/selenium && pytest --headless
  ```

## Deploy / demo public

Vrei să trimiți un link cuiva să testeze? Vezi [HOSTING.md](HOSTING.md) — ghid pas cu pas
pentru Netlify (frontend) + Render (backend), gratuit.

## Funcționalități

- **Autentificare JWT** — register/login, parole cu hash PBKDF2, roluri User/Admin
- **Catalog licitații** — căutare, filtrare pe categorii, sortare, countdown live
- **Bidding** — entitate `Bid` cu istoric complet, validări (ofertă > prețul curent,
  nu poți licita pe obiectul tău, doar licitații active), quick-bid +10/+25/+50
- **Ciclu de viață** — `Added → Validated/ActiveBid → Sold / NoWinner` (+ `Rejected`),
  tranzițiile rulează automat într-un `BackgroundService` (winner-ul se setează la expirare)
- **Validare admin** — obiectele noi intră într-o coadă; adminul le aprobă sau le respinge
- **Wishlist** — many-to-many User↔Item, inimioară pe fiecare card
- **Recenzii** — 0–5 stele + comentariu; câștigătorul poate lăsa recenzie vânzătorului;
  rating-ul mediu apare pe profil și la vânzător
- **Forum** — postări + comentarii (răspunsuri pliabile)
- **Profil** — taburi: Obiectele mele / Câștigate / Wishlist / Recenzii
- **Profil public vânzător** — `/user/:id`, accesibil din „View profile" și din istoricul ofertelor
- **Poze la listări** — upload opțional pe pagina Sell, redimensionat client-side (canvas, max 900px) înainte de trimitere
- **Auto-logout la token expirat** + `returnUrl` — după login ești adus înapoi exact unde ai rămas
- **Live refresh** pe pagina unui item — se reîmprospătează la 5s cât timp licitația e activă
- **„Load more"** pe pagina de licitații, în loc să randeze tot grid-ul dintr-o dată
- **i18n EN/RO** — buton de comutare în header
- **Design albastru „tech"** — paletă completă bazată pe gradient albastru→cyan, glow-uri pe hover,
  ilustrații SVG generate server-side per categorie pentru fiecare listare demo
- **Securitate** — rate limiting per-IP pe login/register/bid, validare DataAnnotations pe toate DTO-urile,
  parolă minimă 8 caractere, upload de imagini restricționat strict la JPEG, handler global de excepții
  (nu se scurg detalii interne către client)

## API (principalele endpoint-uri)

| Metodă | Rută                        | Auth  | Descriere                       |
|--------|-----------------------------|-------|---------------------------------|
| POST   | /api/user/register          | —     | Creare cont (întoarce JWT)      |
| POST   | /api/user/login             | —     | Login (întoarce JWT)            |
| GET    | /api/user/me                | user  | Profil complet                  |
| GET    | /api/user/{id}               | —     | Profil public (rating, recenzii)|
| GET    | /api/item                   | —     | Toate licitațiile vizibile      |
| GET    | /api/item/{id}              | —     | Detaliu + istoric oferte        |
| POST   | /api/item                   | user  | Creare listare (status Added)   |
| GET    | /api/item/pending           | admin | Coada de validare               |
| POST   | /api/item/{id}/approve      | admin | Aprobă listarea                 |
| POST   | /api/item/{id}/reject       | admin | Respinge listarea               |
| POST   | /api/bid                    | user  | Plasează ofertă                 |
| GET    | /api/category               | —     | Categorii                       |
| GET/POST | /api/wishlist(/{itemId})  | user  | Listă / toggle wishlist         |
| GET/POST | /api/forum                | —/user| Postări forum                   |
| POST   | /api/forum/{id}/comments    | user  | Comentariu                      |
| POST   | /api/review                 | user  | Recenzie pentru alt utilizator  |
| GET    | /api/review/stats            | —     | Rating mediu pe toată platforma |
