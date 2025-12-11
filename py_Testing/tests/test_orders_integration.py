"""
Integration tests for the orders system.

Here we test:
- services.create_order() + storage working together
- Real JSON read/write on disk (using pytest's tmp_path fixture)
- No mocks: this is closer to a real-world scenario than a unit test
"""

from pathlib import Path

import pytest

from src.unit_example import services, storage


@pytest.mark.integration
def test_create_and_read_orders_integration(tmp_path: Path):
    # tmp_path is a pytest fixture that gives us a temporary directory.
    # We create a file path inside it for our JSON storage.
    storage_path = tmp_path / "orders.json"

    # Initially, the file doesn't exist. The service should handle that.
    assert not storage_path.exists()

    # Create some orders through the HIGH-LEVEL service function.
    order1 = services.create_order(storage_path, customer="Alice", amount=100.0)
    order2 = services.create_order(storage_path, customer="Bob", amount=50.5)
    order3 = services.create_order(storage_path, customer="Alice", amount=200.0)

    # Now the file SHOULD exist and contain the persisted orders.
    assert storage_path.exists()

    # Use another high-level function to get orders for a specific customer.
    alice_orders = services.get_orders_for_customer(storage_path, "Alice")
    bob_orders = services.get_orders_for_customer(storage_path, "Bob")

    # Check that we get the right data back.
    assert len(alice_orders) == 2
    assert len(bob_orders) == 1

    # IDs should be incremental and persisted correctly.
    assert {o["id"] for o in alice_orders} == {order1["id"], order3["id"]}
    assert bob_orders[0]["id"] == order2["id"]

    # Total amount for Alice (just to show we can also inspect values)
    assert {o["amount"] for o in alice_orders} == {100.0, 200.0}


@pytest.mark.integration
def test_storage_file_is_valid_json(tmp_path: Path):
    # Another integration test focusing more on the storage layer,
    # but still going through the services API.
    storage_path = tmp_path / "orders.json"

    # Create one order
    services.create_order(storage_path, customer="Charlie", amount=123.45)

    # The file should contain valid JSON list of orders.
    # We call storage.load_orders() directly to simulate another part of
    # the application that also uses the storage layer.
    orders = storage.load_orders(storage_path)

    assert isinstance(orders, list)
    assert len(orders) == 1
    assert orders[0]["customer"] == "Charlie"
    assert orders[0]["amount"] == 123.45
