from unittest.mock import Mock, patch

import pytest
import requests

from src.network import fetch_data


# A) side_effect as an exception: simulate network / HTTP error
def test_fetch_data_raises_error():
    """
    Example A:
    side_effect is set to an exception, so the mock raises it
    when called. We use this to test error handling without
    making a real HTTP request.
    """
    with patch("src.network.requests.get") as mock_get:
        mock_get.side_effect = requests.exceptions.HTTPError("Network boom")

        with pytest.raises(requests.exceptions.HTTPError):
            fetch_data("https://example.com")

    mock_get.assert_called_once_with("https://example.com")


# B) side_effect as a sequence: multiple different responses
def test_fetch_data_multiple_responses():
    """
    Example B:
    side_effect is a list. Each call to the mock returns the next
    item in the list. This lets us simulate different responses
    on successive calls.
    """
    # First fake response
    fake_response_1 = Mock()
    fake_response_1.raise_for_status.return_value = None
    fake_response_1.text = "first call"

    # Second fake response
    fake_response_2 = Mock()
    fake_response_2.raise_for_status.return_value = None
    fake_response_2.text = "second call"

    with patch("src.network.requests.get") as mock_get:
        mock_get.side_effect = [fake_response_1, fake_response_2]

        r1 = fetch_data("https://example.com/1")
        r2 = fetch_data("https://example.com/2")

    assert r1 == "first call"
    assert r2 == "second call"
    assert mock_get.call_count == 2
    mock_get.assert_any_call("https://example.com/1")
    mock_get.assert_any_call("https://example.com/2")


# C) side_effect as a callable: custom logic based on arguments
def test_fetch_data_custom_side_effect():
    """
    Example C:
    side_effect is a function. The mock will CALL this function
    instead of just returning a fixed value. We can implement
    custom behavior: return a fake response or raise an error
    depending on the URL.
    """

    def fake_get(url: str):
        # Raise an error for certain URLs
        if "bad" in url:
            raise ValueError("invalid url")

        # Minimal fake response object with the attributes
        # used by fetch_data: raise_for_status() and .text
        class FakeResponse:
            def raise_for_status(self):
                return None

        resp = FakeResponse()
        resp.text = f"DATA:{url}"
        return resp

    with patch("src.network.requests.get", side_effect=fake_get) as mock_get:
        ok_result = fetch_data("good-url")
        assert ok_result == "DATA:good-url"

        # Now the fake_get will raise ValueError for "bad" in url
        with pytest.raises(ValueError):
            fetch_data("bad-url")

    # The mock still tracks how it was called
    assert mock_get.call_count == 2
    mock_get.assert_any_call("good-url")
    mock_get.assert_any_call("bad-url")
