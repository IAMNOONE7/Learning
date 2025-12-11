import requests

def download_text(url: str) -> str:
    """Download text content from a URL."""
    resp = requests.get(url)
    resp.raise_for_status()
    return resp.text


def word_count(text: str) -> int:
    """Return the number of words in a string."""
    return len(text.split())
