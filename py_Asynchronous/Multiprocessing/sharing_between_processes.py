"""
Goal:
- Understand how data is (NOT) shared between processes.
- Show three patterns:

  1) Globals are NOT shared
     - Each process has its own copy of global variables.

  2) Sharing by RETURNING values (recommended)
     - Using ProcessPoolExecutor: each process returns a result
       and the main process aggregates them.

  3) Shared mutable state with multiprocessing.Manager
     - A Manager gives you process-safe shared lists/dicts, etc.,
       at the cost of some overhead.
"""

from __future__ import annotations

import time
from concurrent.futures import ProcessPoolExecutor, as_completed
from multiprocessing import Process, Manager, current_process


# ============================================================
# 1) Globals are NOT shared between processes
# ============================================================

counter = 0  # this looks global, but each process gets its own copy


def increment_global(times: int) -> None:
    """
    Increment the global counter many times.

    Each PROCESS has its own separate memory space.
    So this 'counter' here is NOT the same as the one in the parent.
    """
    global counter
    for _ in range(times):
        counter += 1

    proc = current_process()
    print(f"[{proc.name}] Local counter in this process: {counter}")


def demo_globals_not_shared():
    print("\n=== DEMO 1: Globals are NOT shared between processes ===")

    global counter
    counter = 0  # reset in main process

    # Create two processes, each incrementing its own 'counter'
    p1 = Process(target=increment_global, args=(100_000,), name="Child-1")
    p2 = Process(target=increment_global, args=(100_000,), name="Child-2")

    p1.start()
    p2.start()
    p1.join()
    p2.join()

    print(f"[main] Global counter in MAIN process: {counter}")
    # Explanation:
    # - Each child process had its OWN copy of counter, starting from 0.
    # - They modified their own local copy inside their separate memory space.
    # - The main process's counter remained 0 because processes do NOT share memory.


# ============================================================
# 2) Sharing via RETURN VALUES (recommended way)
# ============================================================

def cpu_heavy_with_id(task_id: int, n: int) -> tuple[int, int]:
    """
    CPU-heavy function that returns (task_id, result).

    This is the cleanest pattern:
    - Processes don't touch shared globals.
    - They just compute something based on their arguments
      and return the result to the parent.
    """
    total = 0
    for i in range(n):
        total += i * i
    return task_id, total


def demo_share_via_results():
    print("\n=== DEMO 2: Sharing via return values (ProcessPoolExecutor) ===")

    # (task_id, n) pairs
    tasks = [
        (0, 5_000_00),
        (1, 6_000_00),
        (2, 7_000_00),
        (3, 8_000_00),
    ]

    aggregated: dict[int, int] = {}

    start = time.perf_counter()
    # Process pool takes care of starting worker processes for us
    with ProcessPoolExecutor(max_workers=4) as pool:
        futures = [
            pool.submit(cpu_heavy_with_id, task_id, n)
            for task_id, n in tasks
        ]

        for fut in as_completed(futures):
            task_id, result = fut.result()
            aggregated[task_id] = result

    elapsed = time.perf_counter() - start

    print("[main] Aggregated results (task_id -> result):")
    for k in sorted(aggregated):
        print(f"  task {k}: {aggregated[k]}")

    print(f"\nTime: {elapsed:.2f}s")
    # Explanation:
    # - Each worker process received its own arguments and returned a value.
    # - The main process collected these results from the futures.
    # - This approach is usually the BEST way to "share" data between processes.


# ============================================================
# 3) Shared mutable state with multiprocessing.Manager
# ============================================================

def append_to_shared_list(shared_list, name: str, count: int):
    """
    Append items to a Manager-managed list.

    shared_list is NOT a normal list; it is a proxy object.
    Operations on it are sent over a pipe to a Manager process,
    which safely updates the underlying list.
    """
    proc = current_process()
    for i in range(count):
        item = (name, i)
        print(f"[{proc.name}] {name} appending {item}")
        shared_list.append(item)
        time.sleep(0.01)  # just to slow down for nicer output


def demo_manager_shared_list():
    print("\n=== DEMO 3: Shared list via multiprocessing.Manager ===")

    # Manager starts a special server process that holds the real list.
    with Manager() as manager:
        # shared_list is a proxy to a list living in the Manager process
        shared_list = manager.list()

        processes: list[Process] = []
        for idx in range(3):
            name = f"Proc-{idx}"
            p = Process(
                target=append_to_shared_list,
                args=(shared_list, name, 5),
                name=name,
            )
            p.start()
            processes.append(p)

        for p in processes:
            p.join()

        print(f"\n[main] Length of shared_list: {len(shared_list)}")
        print("[main] Contents of shared_list:")
        for item in list(shared_list):
            print(" ", item)

    # Explanation:
    # - Manager-based objects (such as manager.list and manager.dict) are truly
    #   shared and safe across processes.
    # - They are slower than normal Python data structures because they rely on
    #   inter-process communication (IPC) under the hood.
    # - Use them only when you REALLY need live, shared, mutable state across processes.


# ============================================================
# MAIN
# ============================================================

def main():
    demo_globals_not_shared()
    demo_share_via_results()
    demo_manager_shared_list()


if __name__ == "__main__":
    # The guard is required on Windows/macOS so that child processes
    # don't re-execute the whole module on import.
    main()
