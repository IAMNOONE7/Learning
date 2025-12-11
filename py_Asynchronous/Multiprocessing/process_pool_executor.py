"""
Goal:
- Learn how to use ProcessPoolExecutor for CPU-bound work.
- Show:
    1) Sequential execution of a CPU-heavy function.
    2) Running the same function in parallel using ProcessPoolExecutor.
    3) Two usage styles:
        - submit(...) + as_completed(...)
        - pool.map(...)

Key ideas:
- A process pool keeps a fixed number of worker processes alive.
- You submit CPU-heavy jobs to the pool; it distributes them across
  processes (and thus CPU cores).
- Much easier than manually creating Process objects.
"""

from __future__ import annotations

import time
from concurrent.futures import ProcessPoolExecutor, as_completed


# ============================================================
# 1) CPU-heavy function
# ============================================================

def cpu_heavy(n: int) -> int:
    """
    Simple CPU-heavy task: sum of squares up to n.
    """
    total = 0
    for i in range(n):
        total += i * i
    return total


# ============================================================
# 2) Sequential baseline
# ============================================================

def demo_sequential(tasks: list[int]) -> None:
    """
    Run cpu_heavy for each n in tasks, one after another.
    This is our baseline for comparison.
    """
    print("\n=== SEQUENTIAL ===")
    print(f"Tasks: {tasks}")

    start = time.perf_counter()
    results = [cpu_heavy(n) for n in tasks]
    elapsed = time.perf_counter() - start

    print(f"First 3 results: {results[:3]} (values are not important)")
    print(f"Sequential time: {elapsed:.2f}s\n")


# ============================================================
# 3) ProcessPoolExecutor with submit + as_completed
# ============================================================

def demo_process_pool_submit(tasks: list[int], max_workers: int = 4) -> None:
    """
    Use ProcessPoolExecutor + submit(...) + as_completed(...).

    Pattern:
        with ProcessPoolExecutor(...) as pool:
            future = pool.submit(cpu_heavy, n)
            ...
    """
    print("=== PROCESS POOL: submit + as_completed ===")
    print(f"Tasks: {tasks}, max_workers={max_workers}")

    start = time.perf_counter()

    results: list[int] = []

    # IMPORTANT: this must be under "if __name__ == '__main__'" on Windows/macOS.
    with ProcessPoolExecutor(max_workers=max_workers) as pool:
        # submit(...) immediately schedules the job on the pool
        futures = [pool.submit(cpu_heavy, n) for n in tasks]

        # as_completed gives futures as soon as they finish (any order)
        for fut in as_completed(futures):
            result = fut.result()  # re-raise worker exception if any
            results.append(result)

    elapsed = time.perf_counter() - start

    print(f"Collected {len(results)} results.")
    print(f"Process pool time (submit/as_completed): {elapsed:.2f}s\n")


# ============================================================
# 4) ProcessPoolExecutor with map(...)
# ============================================================

def demo_process_pool_map(tasks: list[int], max_workers: int = 4) -> None:
    """
    Use ProcessPoolExecutor.map(...) which is a convenience wrapper.

    Pattern:
        with ProcessPoolExecutor(...) as pool:
            for result in pool.map(cpu_heavy, tasks):
                ...

    - Results are yielded in the SAME ORDER as input tasks.
    - map(...) blocks until each result is ready when iterated.
    """
    print("=== PROCESS POOL: map() ===")
    print(f"Tasks: {tasks}, max_workers={max_workers}")

    start = time.perf_counter()

    with ProcessPoolExecutor(max_workers=max_workers) as pool:
        # pool.map returns an iterator over results
        results = list(pool.map(cpu_heavy, tasks))

    elapsed = time.perf_counter() - start

    print(f"First 3 results: {results[:3]}")
    print(f"Process pool time (map): {elapsed:.2f}s\n")


# ============================================================
# MAIN
# ============================================================

def main() -> None:
    # Choose n big enough so each task takes some noticeable CPU time.
    # Adjust if too slow/fast on your machine.
    tasks = [40_000_000, 42_000_000, 44_000_000, 46_000_000]

    demo_sequential(tasks)
    demo_process_pool_submit(tasks, max_workers=4)
    demo_process_pool_map(tasks, max_workers=4)


if __name__ == "__main__":
    # This guard is REQUIRED on Windows/macOS for multiprocessing.
    main()
