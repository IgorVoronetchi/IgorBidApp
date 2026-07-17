"""E2E: inregistrare, login, delogare."""

from conftest import DEMO_EMAIL, DEMO_PASSWORD, click, login


def test_register_new_account_logs_in_automatically(driver, wait, base_url, unique_suffix):
    driver.get(f"{base_url}/register")

    driver.find_element("css selector", "input[autocomplete='username']").send_keys(f"sel_{unique_suffix}")
    driver.find_element("css selector", "input[autocomplete='name']").send_keys("Selenium Test")
    driver.find_element("css selector", "input[autocomplete='email']").send_keys(f"sel_{unique_suffix}@test.ro")
    driver.find_element("css selector", "input[autocomplete='new-password']").send_keys("SeleniumPass123")
    click(driver, driver.find_element("css selector", "button.submit-btn"))

    # Register-ul reusit navigheaza la /auctions si arata user-chip-ul logat in header.
    wait.until(lambda d: "/auctions" in d.current_url)
    assert driver.find_elements("css selector", ".user-chip"), "Nu s-a afisat user-chip-ul dupa register."


def test_register_with_short_password_shows_error(driver, wait, base_url, unique_suffix):
    driver.get(f"{base_url}/register")

    driver.find_element("css selector", "input[autocomplete='username']").send_keys(f"shortpw_{unique_suffix}")
    driver.find_element("css selector", "input[autocomplete='name']").send_keys("Short Pw")
    driver.find_element("css selector", "input[autocomplete='email']").send_keys(f"shortpw_{unique_suffix}@test.ro")
    driver.find_element("css selector", "input[autocomplete='new-password']").send_keys("abc")
    click(driver, driver.find_element("css selector", "button.submit-btn"))

    error_box = wait.until(lambda d: d.find_element("css selector", ".error-box"))
    assert error_box.text.strip() != ""
    assert "/register" in driver.current_url, "Nu ar trebui sa navigheze departe la o parola invalida."


def test_login_with_wrong_password_shows_error(driver, wait, base_url):
    driver.get(f"{base_url}/login")
    driver.find_element("css selector", "input[autocomplete='email']").send_keys(DEMO_EMAIL)
    driver.find_element("css selector", "input[autocomplete='current-password']").send_keys("parola-gresita")
    click(driver, driver.find_element("css selector", "button.submit-btn"))

    error_box = wait.until(lambda d: d.find_element("css selector", ".error-box"))
    assert error_box.text.strip() != ""
    assert driver.find_elements("css selector", ".signin-link"), "Nu ar trebui sa fie logat dupa o parola gresita."


def test_login_then_logout(driver, wait, base_url):
    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)
    assert driver.find_elements("css selector", ".user-chip")

    click(driver, driver.find_element("css selector", ".logout-btn"))
    wait.until(lambda d: d.find_elements("css selector", ".signin-link"))
    assert not driver.find_elements("css selector", ".user-chip"), "Userul tot pare logat dupa logout."
