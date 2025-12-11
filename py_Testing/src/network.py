import requests

def fetch_data(url: str) -> str:
    """
    Fetch text data from a URL.

    - Uses requests.get(url)
    - Raises any HTTP-related errors via response.raise_for_status()
    - Returns response.text on success
    """
    response = requests.get(url)
    response.raise_for_status()
    return response.text
