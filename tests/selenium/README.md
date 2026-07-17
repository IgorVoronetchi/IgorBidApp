# Selenium E2E — Bid-App-Igor

Teste automate prin browser (Chrome) peste aplicatia reala, folosind Selenium 4 + pytest.

## Instalare (o singura data)

```powershell
pip install -r requirements.txt
```

Necesita Google Chrome instalat. Selenium 4 (Selenium Manager) descarca automat
driverul potrivit — nu trebuie sa instalezi chromedriver manual.

## Rulare

Backend-ul (`:5000`) si frontend-ul (`:4200`) trebuie sa ruleze deja local.

```powershell
cd tests/selenium
pytest                          # ruleaza toate testele, cu fereastra Chrome vizibila
pytest --headless               # fara fereastra vizibila (mai rapid, bun pentru CI)
pytest test_bidding.py -v       # doar un fisier
pytest --base-url http://localhost:4200   # explicit, pentru alt mediu (ex. staging deployed)
```

## Ce acopera

| Fisier              | Acopera                                                          |
|----------------------|-------------------------------------------------------------------|
| `test_auth.py`       | Register, parola prea scurta, login gresit, login + logout        |
| `test_auctions.py`   | Grila de licitatii, cautare, filtrare pe categorie, wishlist       |
| `test_bidding.py`    | Pagina de detaliu, plasare oferta (validare, succes, pret minim)  |
| `test_forum.py`      | Listare postari, restrictie pentru vizitatori, postare + comentariu |
| `test_admin.py`      | Acces restrictionat, coada de validare, aprobare listare           |

## Note

- Testele creeaza propriile date (email-uri unice cu timestamp) — nu sterg nimic la final,
  ruleaza pe o baza de date demo, nu de productie.
- `test_bidding.py` si `test_admin.py` sar automat (`pytest.skip`) daca nu gasesc un item
  activ pe care sa liciteze / o listare in asteptare de aprobat — coada de admin are doar
  4 iteme seed, deci ruleaza suita de prea multe ori fara reset de baza de date si se goleste.
- Pentru un mediu deployat (Netlify + Render), ruleaza cu `--base-url https://numele-tau.netlify.app`.
