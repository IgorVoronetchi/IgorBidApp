"""
E2E: pagina de admin (validare listari).

Nota: testul de aprobare consuma din coada de validare seed-uita (doar 4 iteme
la pornire). Daca se ruleaza suita de multe ori pe aceeasi baza de date fara
reset, coada se goleste si testul de aprobare se sare automat (pytest.skip).
"""

import pytest

from conftest import ADMIN_EMAIL, ADMIN_PASSWORD, DEMO_EMAIL, DEMO_PASSWORD, click, login


def test_non_admin_is_redirected_away_from_admin_page(driver, wait, base_url):
    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)
    driver.get(f"{base_url}/admin")
    wait.until(lambda d: d.current_url.rstrip("/") == base_url)
    assert not driver.find_elements("css selector", ".queue"), \
        "Un user non-admin nu ar trebui sa vada coada de validare."


def test_admin_can_see_validation_queue(driver, wait, base_url):
    login(driver, wait, base_url, ADMIN_EMAIL, ADMIN_PASSWORD)
    driver.get(f"{base_url}/admin")

    wait.until(lambda d: d.find_elements("css selector", ".queue") or d.find_elements("css selector", ".empty-box"))
    assert driver.find_elements("css selector", ".admin-head"), "Nu s-a incarcat pagina de admin."


def test_admin_can_approve_a_pending_item(driver, wait, base_url):
    login(driver, wait, base_url, ADMIN_EMAIL, ADMIN_PASSWORD)
    driver.get(f"{base_url}/admin")

    wait.until(lambda d: d.find_elements("css selector", ".queue-item") or d.find_elements("css selector", ".empty-box"))
    queue_items = driver.find_elements("css selector", ".queue-item")
    if not queue_items:
        pytest.skip("Coada de validare e goala - nimic de aprobat in acest moment.")

    items_before = len(queue_items)
    click(driver, queue_items[0].find_element("css selector", "button.btn-accent"))

    wait.until(lambda d: d.find_elements("css selector", ".ok-box"))
    items_after = len(driver.find_elements("css selector", ".queue-item"))
    assert items_after == items_before - 1, "Coada nu s-a micsorat dupa aprobare."
