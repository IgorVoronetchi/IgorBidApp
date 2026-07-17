"""E2E: pagina de detaliu a unui item si plasarea unei oferte."""

import time

import pytest
from selenium.common.exceptions import NoSuchElementException

from conftest import DEMO_EMAIL, DEMO_PASSWORD, click, login


def _open_a_biddable_item(driver, wait, base_url):
    """Deschide primul item din grila pe care se poate licita (activ, nu al userului logat)."""
    driver.get(f"{base_url}/auctions")
    wait.until(lambda d: d.find_elements("css selector", ".item-card"))
    links = driver.find_elements("css selector", "a.card-name")
    hrefs = [a.get_attribute("href") for a in links][:6]

    for href in hrefs:
        driver.get(href)
        wait.until(lambda d: d.find_elements("css selector", ".item-title"))
        try:
            driver.find_element("css selector", ".bid-input")
            return True
        except NoSuchElementException:
            continue
    return False


def test_item_detail_shows_core_info(driver, wait, base_url):
    driver.get(f"{base_url}/auctions")
    wait.until(lambda d: d.find_elements("css selector", ".item-card"))
    click(driver, driver.find_element("css selector", "a.card-name"))

    wait.until(lambda d: d.find_elements("css selector", ".item-title"))
    assert driver.find_element("css selector", ".bid-price").text.strip() != ""
    assert driver.find_elements("css selector", ".history-card"), "Lipseste sectiunea de istoric oferte."


def test_bid_requires_login(driver, wait, base_url):
    found = _open_a_biddable_item(driver, wait, base_url)
    if not found:
        pytest.skip("Niciun item activ disponibil pentru licitare in acest moment.")

    driver.find_element("css selector", ".bid-input").send_keys("999999")
    click(driver, driver.find_element("css selector", ".bid-btn"))

    error_box = wait.until(lambda d: d.find_element("css selector", ".bid-error"))
    assert error_box.text.strip() != "", "Ar trebui sa ceara autentificare inainte de a licita."


def test_placing_a_valid_bid_updates_price(driver, wait, base_url):
    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)

    found = _open_a_biddable_item(driver, wait, base_url)
    if not found:
        pytest.skip("Niciun item activ disponibil pentru licitare in acest moment.")

    time.sleep(0.5)
    price_before = driver.find_element("css selector", ".bid-price").text

    quick_buttons = driver.find_elements("css selector", ".quick-btn")
    assert quick_buttons, "Lipsesc butoanele de quick-bid."
    click(driver, quick_buttons[0])  # completeaza inputul cu pretul curent + primul increment
    click(driver, driver.find_element("css selector", ".bid-btn"))

    wait.until(lambda d: d.find_elements("css selector", ".bid-ok"))
    price_after = driver.find_element("css selector", ".bid-price").text
    assert price_after != price_before, "Pretul curent nu s-a actualizat dupa oferta."


def test_bid_below_current_price_is_rejected(driver, wait, base_url):
    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)

    found = _open_a_biddable_item(driver, wait, base_url)
    if not found:
        pytest.skip("Niciun item activ disponibil pentru licitare in acest moment.")

    bid_input = driver.find_element("css selector", ".bid-input")
    bid_input.send_keys("1")
    click(driver, driver.find_element("css selector", ".bid-btn"))

    error_box = wait.until(lambda d: d.find_element("css selector", ".bid-error"))
    assert error_box.text.strip() != ""
