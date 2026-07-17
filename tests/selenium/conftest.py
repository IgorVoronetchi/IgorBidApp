"""
Fixtures comune pentru suita Selenium a Bid-App-Igor.

Ruleaza cu: pytest tests/selenium -v
Optiuni: --base-url http://localhost:4200  (implicit)
         --headless                          (fara fereastra de browser vizibila)
"""

import time

import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait

DEFAULT_BASE_URL = "http://localhost:4200"

# Conturi seed din backend/Data/DbSeeder.cs - trebuie sa existe pe backend-ul testat.
ADMIN_EMAIL = "igor@bidapp.ro"
ADMIN_PASSWORD = "Igor123"
DEMO_EMAIL = "radu@demo.ro"
DEMO_PASSWORD = "parola123"


def pytest_addoption(parser):
    parser.addoption("--base-url", action="store", default=DEFAULT_BASE_URL,
                      help="URL-ul frontend-ului Angular (implicit: %s)" % DEFAULT_BASE_URL)
    parser.addoption("--headless", action="store_true", default=False,
                      help="Ruleaza Chrome fara fereastra vizibila")


@pytest.fixture(scope="session")
def base_url(request):
    return request.config.getoption("--base-url").rstrip("/")


@pytest.fixture
def driver(request):
    options = Options()
    if request.config.getoption("--headless"):
        options.add_argument("--headless=new")
    options.add_argument("--window-size=1280,900")
    # Selenium 4 (Selenium Manager) descarca/gaseste singur chromedriver-ul potrivit.
    d = webdriver.Chrome(options=options)
    yield d
    d.quit()


@pytest.fixture
def wait(driver):
    return WebDriverWait(driver, 10)


@pytest.fixture
def unique_suffix():
    return str(int(time.time() * 1000))


def click(driver, element):
    """
    Click printr-un dispatch JS direct pe element, nu prin evenimentul nativ
    de WebDriver. Aplicatia (Angular fara zone.js / zoneless) nu re-randeaza
    fiabil in urma unor click-uri native WebDriver pe butoane care actualizeaza
    proprietati simple ale componentei (nu semnale) - un click JS declanseaza
    acelasi handler (click)="..." din template si e consistent, indiferent
    de pozitia elementului fata de header-ul sticky sau de randare.
    """
    driver.execute_script("arguments[0].click();", element)


_SET_VALUE_JS = """
const el = arguments[0], text = arguments[1];
const proto = el.tagName === 'TEXTAREA' ? window.HTMLTextAreaElement.prototype : window.HTMLInputElement.prototype;
Object.getOwnPropertyDescriptor(proto, 'value').set.call(el, text);
el.dispatchEvent(new Event('input', { bubbles: true }));
el.dispatchEvent(new Event('change', { bubbles: true }));
"""


def set_value(driver, element, text):
    """
    Seteaza valoarea unui input/textarea prin setter-ul nativ + dispatch manual
    de evenimente 'input'/'change', in loc de send_keys().

    Pe unele campuri legate cu [(ngModel)] (ex. formularul de forum), send_keys()
    nu declanseaza fiabil actualizarea proprietatii componentei in acest mediu
    (Chrome headless + Selenium + Angular zoneless) desi elementul e vizibil si
    focusat - vezi si nota de la `click()`. Acest workaround e robust indiferent
    de cauza exacta si testat sa functioneze corect pe toate paginile.
    """
    driver.execute_script(_SET_VALUE_JS, element, text)


def login(driver, wait, base_url, email, password):
    """Autentifica un user existent prin formularul real de login."""
    driver.get(f"{base_url}/login")
    email_input = wait.until(lambda d: d.find_element("css selector", "input[autocomplete='email']"))
    email_input.clear()
    email_input.send_keys(email)
    password_input = driver.find_element("css selector", "input[autocomplete='current-password']")
    password_input.clear()
    password_input.send_keys(password)
    click(driver, driver.find_element("css selector", "button.submit-btn"))
    # Dupa login reusit, header-ul arata user-chip-ul in loc de link-ul de sign-in.
    wait.until(lambda d: d.find_elements("css selector", ".user-chip"))
