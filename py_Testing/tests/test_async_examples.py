# tests/test_async_examples.py

import asyncio
from unittest.mock import AsyncMock

import pytest
import pytest_asyncio

from src.async_examples import (
    async_add,
    delayed_double,
    fetch_and_upper,
    wait_for_value,
)


# -----------------------------------------------------------------------------
# 1) Basic async test with @pytest.mark.asyncio
# -----------------------------------------------------------------------------

@pytest.mark.asyncio
async def test_async_add_basic():
    result = await async_add(2, 3)
    assert result == 5

    result2 = await async_add(-1, 1)
    assert result2 == 0


# -----------------------------------------------------------------------------
# 2) Testing delays & time-based behavior
# -----------------------------------------------------------------------------

@pytest.mark.asyncio
async def test_delayed_double_value():
    # We only care about the result, not the real wall-clock time.
    result = await delayed_double(10, delay=0.01)
    assert result == 20


@pytest.mark.asyncio
async def test_wait_for_value_timeout():
    # Pass a coroutine that will take longer than the timeout.
    long_coro = delayed_double(5, delay=0.2)

    with pytest.raises(asyncio.TimeoutError):
        await wait_for_value(long_coro, timeout=0.01)


# -----------------------------------------------------------------------------
# 3) Async fixture using pytest-asyncio
# -----------------------------------------------------------------------------

@pytest_asyncio.fixture
async def prepared_number():
    """
    Example async fixture: simulates async setup,
    then provides a value to tests.
    """
    await asyncio.sleep(0.01)
    return 7


@pytest.mark.asyncio
async def test_async_fixture_works(prepared_number):
    # prepared_number comes from the async fixture above
    result = await async_add(prepared_number, 3)
    assert result == 10


# -----------------------------------------------------------------------------
# 4) Using AsyncMock for async dependencies
# -----------------------------------------------------------------------------

@pytest_asyncio.fixture
async def fake_async_client():
    """
    Async fixture returning an AsyncMock that behaves like AsyncClient.
    """
    client = AsyncMock()
    # Configure the async method: when awaited, it should return this value.
    client.get_data.return_value = "hello async"
    return client


@pytest.mark.asyncio
async def test_fetch_and_upper_with_asyncmock(fake_async_client):
    """
    Demonstrates how to test code that awaits an async dependency.
    We use AsyncMock so that 'await client.get_data(...)' works correctly.
    """
    result = await fetch_and_upper(fake_async_client, "key-123")

    assert result == "HELLO ASYNC"
    fake_async_client.get_data.assert_awaited_once_with("key-123")


@pytest.mark.asyncio
async def test_fetch_and_upper_different_values():
    """
    Same function as above, but we manually create an AsyncMock inside the test
    and use side_effect to return different values.
    """
    client = AsyncMock()
    client.get_data.side_effect = ["foo", "bar"]

    r1 = await fetch_and_upper(client, "k1")
    r2 = await fetch_and_upper(client, "k2")

    assert r1 == "FOO"
    assert r2 == "BAR"
    assert client.get_data.await_count == 2
    client.get_data.assert_any_await("k1")
    client.get_data.assert_any_await("k2")
