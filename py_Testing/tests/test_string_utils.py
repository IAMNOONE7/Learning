from unittest.mock import Mock, patch

from src import string_utils


def test_download_text_mocked():
    # Create a fake "response" object that will mimic what requests.get() returns.
    fake_response = Mock()
    fake_response.text = "hello world"
    fake_response.raise_for_status.return_value = None

    # Any call to requests.get() during this context will return fake_response.
    with patch("src.string_utils.requests.get", return_value=fake_response) as mock_get:
        # Call the function under test.
        # It will internally call requests.get(), but we’ve replaced that call
        # with our fake version, so no real HTTP request is made.
        result = string_utils.download_text("https://example.com")

    assert result == "hello world"
    # Make sure requests.get() was called exactly once, with the expected URL.
    mock_get.assert_called_once_with("https://example.com")



def test_word_count():
    assert string_utils.word_count("hello world") == 2
    assert string_utils.word_count("") == 0
    assert string_utils.word_count("one two  three") == 3
