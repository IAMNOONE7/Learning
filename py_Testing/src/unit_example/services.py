from __future__ import annotations

from pathlib import Path
from typing import Dict, Any, List

from src.unit_example import storage


def create_order(path: Path, customer: str, amount: float) -> Dict[str, Any]:
    """
    Create a new order and persist it to disk.

    - Uses storage.init_storage() to ensure file exists
    - Loads existing orders
    - Assigns a new incremental 'id'
    - Saves all orders back to the file
    - Returns the created order dict
    """
    storage.init_storage(path)
    orders = storage.load_orders(path)

    # simple incremental ID (not for production, just for demo)
    new_id = (max(o["id"] for o in orders) + 1) if orders else 1

    order = {
        "id": new_id,
        "customer": customer,
        "amount": amount,
    }

    orders.append(order)
    storage.save_orders(path, orders)
    return order


def get_orders_for_customer(path: Path, customer: str) -> List[Dict[str, Any]]:
    """
    Return all orders for a given customer, reading from the storage file.
    """
    storage.init_storage(path)
    orders = storage.load_orders(path)
    return [o for o in orders if o["customer"] == customer]
