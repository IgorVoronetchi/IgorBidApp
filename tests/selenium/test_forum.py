"""E2E: forumul comunitatii - postari si comentarii."""

from conftest import DEMO_EMAIL, DEMO_PASSWORD, click, login, set_value


def test_forum_lists_existing_posts(driver, wait, base_url):
    driver.get(f"{base_url}/forum")
    posts = wait.until(lambda d: d.find_elements("css selector", ".post"))
    assert len(posts) > 0, "Nu apare nicio postare pe forum."


def test_anonymous_user_sees_signin_prompt_instead_of_form(driver, wait, base_url):
    driver.get(f"{base_url}/forum")
    wait.until(lambda d: d.find_elements("css selector", ".post"))
    assert driver.find_elements("css selector", ".signin-note"), \
        "Un vizitator neautentificat ar trebui sa vada un mesaj de sign-in, nu formularul de postare."


def test_logged_in_user_can_publish_a_post(driver, wait, base_url, unique_suffix):
    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)
    driver.get(f"{base_url}/forum")

    title = f"Selenium test post {unique_suffix}"
    body = "Postare creata automat de suita Selenium."

    title_input = wait.until(lambda d: d.find_element("css selector", ".new-post input.input"))
    set_value(driver, title_input, title)
    set_value(driver, driver.find_element("css selector", ".new-post textarea.input"), body)
    click(driver, driver.find_element("css selector", ".new-post button.btn-accent"))

    wait.until(lambda d: title in d.page_source)
    posts_text = driver.find_element("css selector", ".posts").text
    assert title in posts_text


def test_logged_in_user_can_comment_on_a_post(driver, wait, base_url, unique_suffix):
    login(driver, wait, base_url, DEMO_EMAIL, DEMO_PASSWORD)
    driver.get(f"{base_url}/forum")

    wait.until(lambda d: d.find_elements("css selector", ".post"))
    click(driver, driver.find_element("css selector", ".toggle-btn"))

    comment_text = f"Comentariu automat {unique_suffix}"
    reply_input = wait.until(lambda d: d.find_element("css selector", ".reply-input"))
    set_value(driver, reply_input, comment_text)
    click(driver, driver.find_element("css selector", ".reply-btn"))

    wait.until(lambda d: comment_text in d.page_source)
