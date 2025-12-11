"""

We will:
- Implement a fake I/O-bound task synchronously (using time.sleep)
- Implement the same task asynchronously (using asyncio.sleep)
- Compare total runtime

KEY IDEAS:
- "async def" defines a coroutine function (not a normal function).
- Calling a coroutine function returns a coroutine object (it does NOT run immediately).
- To actually run a coroutine, you must "await" it (inside async code),
  or use `asyncio.run(...)` at the top level.
"""

import time
import asyncio


def sync_fetch_data(source: str, delay: float) -> str:
    """
    Synchronous (blocking) version.

    Pretend we are doing some I/O here (HTTP request, DB query, etc.).
    time.sleep(...) blocks the current thread – nothing else can run meanwhile.
    """
    print(f"[SYNC] Start fetching from {source}")
    time.sleep(delay)  # BLOCKS this thread
    print(f"[SYNC] Finished fetching from {source}")
    return f"data-from-{source}"


def sync_main() -> None:
    """
    Run 3 blocking tasks one after another.

    Total time = sum of all delays.
    """
    start = time.perf_counter()

    # Each call BLOCKS until it's finished
    result1 = sync_fetch_data("server-1", 1.0)
    result2 = sync_fetch_data("server-2", 1.0)
    result3 = sync_fetch_data("server-3", 1.0)

    elapsed = time.perf_counter() - start
    print(f"[SYNC] Results: {result1}, {result2}, {result3}")
    print(f"[SYNC] Total time: {elapsed:.2f} seconds\n")


# ------------------- ASYNC VERSION ------------------- #

async def async_fetch_data(source: str, delay: float) -> str:
    """
    Asynchronous version of fetch_data.

    Differences:
    - "async def" -> defines a coroutine function.
    - We use `await asyncio.sleep(delay)` instead of time.sleep(delay).

    `asyncio.sleep`:
    - DOES NOT block the OS thread.
    - It tells the event loop:
        "Pause this coroutine for `delay` seconds and let
         other tasks run in the meantime."
    """
    print(f"[ASYNC] Start fetching from {source}")
    # This is a SUSPENSION point: control returns to event loop.
    await asyncio.sleep(delay)
    print(f"[ASYNC] Finished fetching from {source}")
    return f"data-from-{source}"


async def async_main() -> None:
    """
    Run 3 async tasks "concurrently".

    Because the only thing these tasks do is await asyncio.sleep,
    the event loop can interleave them, so total time = max(delay) not sum(delay).
    """
    start = time.perf_counter()

    # IMPORTANT:
    # We create the coroutines (they don't run yet)
    coro1 = async_fetch_data("server-1", 1.0)
    coro2 = async_fetch_data("server-2", 1.0)
    coro3 = async_fetch_data("server-3", 1.0)

    # OPTION 1 – sequential awaiting (still "async", but not concurrent):
    # result1 = await coro1
    # result2 = await coro2
    # result3 = await coro3

    # OPTION 2 – run them concurrently with asyncio.gather:
    # - gather schedules all coroutines as Tasks and awaits their completion.
    result1, result2, result3 = await asyncio.gather(coro1, coro2, coro3)

    elapsed = time.perf_counter() - start
    print(f"[ASYNC] Results: {result1}, {result2}, {result3}")
    print(f"[ASYNC] Total time: {elapsed:.2f} seconds\n")


if __name__ == "__main__":
    print("=== SYNC VERSION ===")
    sync_main()

    print("=== ASYNC VERSION ===")
    # asyncio.run(...) creates an event loop, runs async_main, and closes the loop.
    asyncio.run(async_main())

    # At this point, event loop is closed and program exits.
