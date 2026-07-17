"""E2E: catalogul de licitatii - cautare, filtrare, wishlist."""

import time

from conftest import DEMO_EMAIL, DEMO_PASSWORD, click, login


def test_auctions_page_lists_items(driver, wait, base_url):
    driver.get(f"{base_url}/auctions")
    cards = wait.until(lambda d: d.find_elements("css selector", ".item-card"))
    assert len(cards) > 0, "Nu apare niciun item in grila de licitatii."


def test_search_filters_results(driver, wait, base_url):
    driver.get(f"{base_url}/auctions")
    wait.until(lambda d: d.find_elements("css selector", ".item-card"))

    search = driver.find_element("css selector", "input.search")
    search.send_keys("Dacia")
    time.sleep(0.5)  # signal-ul de search se aplica sincron, dar lasam randarea sa se stabilizeze

    cards = driver.find_elements("css selector", ".item-card")
    assert len(cards) >= 1
    assert all("Dacia" in c.text or "dacia" in c.text.lower() for c in cards), \
        "Rezultatele de cautare contin iteme care nu se potrivesc."


def test_category_filter_narrows_results(driver, wait, base_url):
    driver.get(f"{base_url}/auctions")
    wait.until(lambda d: d.find_elements("css selector", ".item-card"))
    total_count = len(driver.find_elements("css selector", ".item-card"))

    chips = driver.find_elements("css selector", "button.chip")
    # primul chip e "All"/"Toate"; alegem urmatorul (Electronics)
    assert len(chips) > 1, "Nu exista chip-uri de categorie."
    click(driver, chips[1])
    time.sleep(0.5)

    filtered_count = len(driver.find_elements("css selector", ".item-card"))
    assert 0 < filtered_count <= total_count, "Filtrarea pe categorie nu a redus rezultatele."


def test_wishlist_requires_login_and_redirects_back(driver, wait, base_url):
    driver.get(f"{base_url}/auctions")
    wait.until(lambda d: d.find_elements("css selector", ".item-card"))

    click(driver, driver.find_element("css selector", ".wish-btn"))
    wait.until(lambda d: "/login" in d.current_url)
    assert "returnUrl" in driver.current_url, "Login-ul ar trebui sa primeasca un returnUrl."

    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)
    wait.until(lambda d: "/auctions" in d.current_url)


def test_wishlist_toggle_updates_heart_icon(driver, wait, base_url):
    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)
    driver.get(f"{base_url}/auctions")
    wait.until(lambda d: d.find_elements("css selector", ".item-card"))
    time.sleep(0.5)  # lasam wishlist.load() sa se aplice inainte de a citi starea initiala

    wish_btn = driver.find_element("css selector", ".wish-btn")
    before = wish_btn.text
    click(driver, wish_btn)

    wait.until(lambda d: d.find_element("css selector", ".wish-btn").text != before)
