from __future__ import annotations

import asyncio
from typing import Protocol


class AsyncClient(Protocol):
    """
    Simple protocol describing an async dependency.

    Anything passed as 'client' just needs to implement:
        async def get_data(self, key: str) -> str
    """

    async def get_data(self, key: str) -> str:
        ...


async def async_add(a: int, b: int) -> int:
    """
    Tiny async function that simulates IO latency then returns a + b.
    Useful for basic async testing with pytest.
    """
    await asyncio.sleep(0.01)
    return a + b


async def delayed_double(x: int, delay: float = 0.01) -> int:
    """
    Waits for 'delay' seconds, then returns 2 * x.
    Helpful for testing timing and timeouts.
    """
    await asyncio.sleep(delay)
    return 2 * x


async def fetch_and_upper(client: AsyncClient, key: str) -> str:
    """
    Uses an async client dependency to fetch some data and transform it.

    This is designed for testing AsyncMock:
      - we await an async method (client.get_data)
      - we do some extra async work (sleep)
      - we return a transformed value
    """
    raw = await client.get_data(key)
    await asyncio.sleep(0.01)
    return raw.upper()


async def wait_for_value(coro, timeout: float):
    """
    Wait for the given coroutine with a timeout.

    If the coroutine doesn't finish in time, asyncio.TimeoutError is raised.
    """
    return await asyncio.wait_for(coro, timeout=timeout)
