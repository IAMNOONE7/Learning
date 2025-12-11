"""

Focus:
- What is the event loop (conceptually)?
- What is an asyncio.Task?
- How to schedule coroutines using create_task(...)
- Differences: awaiting tasks vs 'fire-and-forget'

IMPORTANT MENTAL MODEL:
- Event loop = single-threaded scheduler.
    - Keeps a queue of "ready to run" tasks.
    - Picks a task, runs it until it hits an await that needs to pause.
    - Then switches to another ready task.
- Task = wrapper around a coroutine that the event loop can schedule.
"""

import asyncio
import time


async def do_work(name: str, delay: float) -> str:
    """
    A small async worker.

    It:
    - prints when it starts
    - awaits a sleep (simulating I/O)
    - prints when it finishes

    The 'await asyncio.sleep' is where control returns to the event loop.
    """
    print(f"[{name}] Start (delay={delay})")
    await asyncio.sleep(delay)
    print(f"[{name}] Finished")
    return f"result-from-{name}"


async def demo_create_task() -> None:
    """
    Show how asyncio.create_task works.

    create_task:
    - takes a coroutine object and schedules it to run "soon" on the event loop.
    - returns a Task instance.
    - Task starts running as soon as the event loop gets control.

    We then await the tasks to get their results.
    """
    start = time.perf_counter()

    # Create three tasks. The coroutines -> Tasks.
    task1 = asyncio.create_task(do_work("task-1", 2.0))
    task2 = asyncio.create_task(do_work("task-2", 1.0))
    task3 = asyncio.create_task(do_work("task-3", 3.0))

    print("[MAIN] Three tasks created, now awaiting them...")

    # Awaiting them one by one DOES NOT make them sequential.
    # They are already scheduled to run concurrently.
    # Awaiting just "waits for completion".
    result1 = await task1
    result2 = await task2
    result3 = await task3

    elapsed = time.perf_counter() - start
    print(f"[MAIN] Results: {result1}, {result2}, {result3}")
    print(f"[MAIN] Total time: {elapsed:.2f} seconds\n")


async def demo_gather_vs_individual_awaits() -> None:
    """
    Show that asyncio.gather is essentially a convenience wrapper.

    Both patterns below are concurrent:
    - creating tasks manually and awaiting them
    - using asyncio.gather on coroutines directly
    """

    print("[GATHER DEMO] Using create_task + individual awaits")
    t1 = asyncio.create_task(do_work("c1", 1.0))
    t2 = asyncio.create_task(do_work("c2", 1.5))
    t3 = asyncio.create_task(do_work("c3", 0.5))

    r1 = await t1
    r2 = await t2
    r3 = await t3
    print("[GATHER DEMO] Results via individual awaits:", r1, r2, r3)

    print("\n[GATHER DEMO] Using asyncio.gather on coroutines directly")
    # asyncio.gather will internally create tasks (if needed) and await them all.
    r1, r2, r3 = await asyncio.gather(
        do_work("g1", 1.0),
        do_work("g2", 1.5),
        do_work("g3", 0.5),
    )
    print("[GATHER DEMO] Results via gather:", r1, r2, r3)
    print()


async def demo_fire_and_forget() -> None:
    """
    Show a fire-and-forget pattern (and why it is dangerous).

    Sometimes people do:
        asyncio.create_task(do_work(...))
    without keeping the task and without awaiting it.

    This starts the task, but:
    - You don't know if/when it fails.
    - At program exit, task may be cancelled if the loop stops.

    We'll show it here and then do the SAFE pattern.
    """

    print("[F&F] Starting fire-and-forget task")
    task = asyncio.create_task(do_work("fire-and-forget", 2.0))

    # Imagine main code "finishes" before this task ends:
    print("[F&F] Doing some quick main work...")
    await asyncio.sleep(0.5)
    print("[F&F] Main work done; we are NOT awaiting the task explicitly.")

    # If the event loop keeps running long enough, the task will finish.
    # But if we end the program now, the task may be cancelled.
    # -> In real apps, prefer tracking tasks and explicitly awaiting them
    #    or using helpers like asyncio.gather or TaskGroups
    await asyncio.sleep(2.0)
    print("[F&F] End of demo\n")

    # NOTE: In a real application, you'd avoid fire-and-forget unless:
    # - You manage their lifecycle somehow (like a background supervisor),
    # - or you truly don't care about the result or errors.


async def main() -> None:
    """
    Our main coroutine for this file.

    It:
    - Shows create_task + concurrent tasks
    - Compares create_task + awaits vs gather
    - Demonstrates fire-and-forget pitfall
    """
    print("=== DEMO: create_task & concurrent execution ===")
    await demo_create_task()

    print("=== DEMO: gather vs individual awaits ===")
    await demo_gather_vs_individual_awaits()

    print("=== DEMO: fire-and-forget task ===")
    await demo_fire_and_forget()


if __name__ == "__main__":
    # Event loop entry point:
    # - creates a new event loop
    # - runs main() until completion
    # - closes the loop
    asyncio.run(main())
