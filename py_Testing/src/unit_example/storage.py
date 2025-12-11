from __future__ import annotations

import json
from pathlib import Path
from typing import List, Dict, Any


def init_storage(path: Path) -> None:
    """
    Initialize the storage file.

    If the file does not exist, it creates it with an empty list: [].
    If it exists, it leaves it as is.
    """
    if not path.exists():
        path.write_text("[]", encoding="utf-8")


def load_orders(path: Path) -> List[Dict[str, Any]]:
    """
    Load all orders from the JSON file at 'path'.

    Raises:
        json.JSONDecodeError if the file content is invalid.
    """
    text = path.read_text(encoding="utf-8")
    return json.loads(text)


def save_orders(path: Path, orders: List[Dict[str, Any]]) -> None:
    """
    Save the list of orders to the JSON file at 'path'.
    """
    text = json.dumps(orders, indent=2)
    path.write_text(text, encoding="utf-8")
