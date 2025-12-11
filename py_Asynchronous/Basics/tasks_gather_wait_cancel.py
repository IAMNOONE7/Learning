"""

Goal:
- Understand asyncio.gather()
- Understand asyncio.wait() and its modes
- Task cancellation
- Timeouts (asyncio.wait_for / asyncio.timeout)

These are CRITICAL to real-world async apps:
    - servers
    - API clients
    - background workers
"""

import asyncio

# -------------------- WORKER --------------------

async def worker(name: str, delay: float) -> str:
    """
    Simulates async work with variable delay.
    """
    print(f"[{name}] Started (delay={delay})")
    try:
        await asyncio.sleep(delay)
    except asyncio.CancelledError:
        print(f"[{name}] CANCELLED!")
        raise  # Don't swallow cancellation!
    print(f"[{name}] Finished")
    return f"{name}-result"


# ================================================================
# 1) asyncio.gather — run tasks concurrently and collect results
# ================================================================

async def demo_gather():
    print("\n=== DEMO: asyncio.gather ===")

    tasks = [
        worker("A", 1.0),
        worker("B", 0.5),
        worker("C", 1.5),
    ]

    # gather returns results in same ORDER as tasks are passed
    results = await asyncio.gather(*tasks)
    print("[GATHER] Results:", results)


# =================================================================
# 2) asyncio.wait — low-level control (modes: ALL, FIRST_COMPLETED)
# =================================================================

async def demo_wait():
    print("\n=== DEMO: asyncio.wait ===")

    t1 = asyncio.create_task(worker("W1", 3.0))
    t2 = asyncio.create_task(worker("W2", 2.0))
    t3 = asyncio.create_task(worker("W3", 1.0))

    # Wait until *FIRST_COMPLETED*
    done, pending = await asyncio.wait(
        {t1, t2, t3},
        return_when=asyncio.FIRST_COMPLETED
    )

    print("[WAIT] First completed:", {d.get_name() if hasattr(d, 'get_name') else d for d in done})
    print("[WAIT] Pending tasks:", pending)

    # Cancel pending tasks:
    for task in pending:
        print("[WAIT] Cancelling:", task)
        task.cancel()

    # Allow cancellation to propagate
    await asyncio.gather(*pending, return_exceptions=True)


# =================================================================
# 3) TIMEOUTS — wait_for / timeout context manager
# =================================================================

async def demo_timeout():
    print("\n=== DEMO: timeout ===")

    async def long_task():
        print("[long_task] running...")
        await asyncio.sleep(3)
        return "done"

    try:
        # Using classic wait_for
        result = await asyncio.wait_for(long_task(), timeout=1.0)
        print("[TIMEOUT] Result:", result)
    except asyncio.TimeoutError:
        print("[TIMEOUT] Task exceeded 1.0s -> TimeoutError")


    print("\n[New timeout] Using asyncio.timeout() context manager")
    try:
        async with asyncio.timeout(1.0):
            await long_task()
    except TimeoutError:
        print("[New timeout] TimeoutError triggered")


# =================================================================
# 4) CANCELLATION of running tasks
# =================================================================

async def demo_cancellation():
    print("\n=== DEMO: cancelling running tasks ===")

    task = asyncio.create_task(worker("TO_CANCEL", 5.0))

    await asyncio.sleep(1.0)  # Let the task start
    print("[CANCEL DEMO] Cancelling task...")
    task.cancel()

    try:
        await task
    except asyncio.CancelledError:
        print("[CANCEL DEMO] Task confirmed cancelled")


# =================================================================
# MAIN
# =================================================================

async def main():
    await demo_gather()
    await demo_wait()
    await demo_timeout()
    await demo_cancellation()


if __name__ == "__main__":
    asyncio.run(main())
