"""
Goal:
- Understand what threads are (in practice).
- Use ThreadPoolExecutor to run blocking functions concurrently.
- See why threads help mainly with I/O-bound tasks (not CPU-bound).

KEY IDEAS:

- A thread is a separate flow of execution inside one process.
- In CPython, the GIL means only ONE thread runs Python bytecode at a time.
    -> Threads are still GREAT for I/O-bound tasks (waiting on network, disk, etc.)
    -> Threads are NOT great for CPU-heavy pure Python code (we'll use multiprocessing later).

- ThreadPoolExecutor:
    -> A "pool" of worker threads.
    -> You submit functions to be executed in those threads.
    -> You get back Future objects (like promises of results).
"""

from __future__ import annotations

import time
import threading
from concurrent.futures import ThreadPoolExecutor, as_completed
from typing import List


# ------------------------------------------------------------
# 1) A fake I/O-bound function
# ------------------------------------------------------------

def fake_download(url: str, delay: float) -> str:
    """
    Simulate a blocking I/O task (e.g. HTTP download).

    - Uses time.sleep to block the thread (like waiting for network).
    - Prints which thread is doing the work.
    """
    thread_name = threading.current_thread().name
    print(f"[START] {url} on {thread_name}, sleeping for {delay:.1f}s")
    time.sleep(delay)  # BLOCKS this thread
    print(f"[END]   {url} on {thread_name}")
    return f"content-of-{url}"


def run_io_sequential(urls: List[str]) -> None:
    """
    Run fake_download sequentially (no threads).
    """
    start = time.perf_counter()
    results = []

    for url in urls:
        # Each call finishes before the next starts.
        result = fake_download(url, delay=1.0)
        results.append(result)

    elapsed = time.perf_counter() - start
    print(f"[SEQUENTIAL] Got {len(results)} results in {elapsed:.2f}s\n")


def run_io_with_threads(urls: List[str], max_workers: int = 4) -> None:
    """
    Run fake_download concurrently using ThreadPoolExecutor.

    How it works:
    - We create a pool with N threads.
    - We submit tasks to this pool.
    - Each task runs in a worker thread (and can block independently).
    """
    start = time.perf_counter()
    results = []

    # Create a pool of worker threads
    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        # Submit returns a Future immediately (task scheduled)
        future_to_url = {
            executor.submit(fake_download, url, 1.0): url
            for url in urls
        }

        # as_completed iterates futures as they finish (any order)
        for future in as_completed(future_to_url):
            url = future_to_url[future]
            try:
                data = future.result()
            except Exception as exc:
                print(f"[THREAD-ERROR] {url!r} generated an exception: {exc!r}")
            else:
                print(f"[THREAD-OK] {url!r} -> {data!r}")
                results.append(data)

    elapsed = time.perf_counter() - start
    print(f"[THREADED] Got {len(results)} results in {elapsed:.2f}s with max_workers={max_workers}\n")


# ------------------------------------------------------------
# 2) A fake CPU-bound function (to show threads don't help much)
# ------------------------------------------------------------

def cpu_heavy(n: int) -> int:
    """
    Simulate a CPU-heavy task: sum of squares up to n.

    This is pure Python number crunching, no I/O.

    Because of the GIL:
    - Multiple threads doing this in parallel will NOT speed it up
      much (and can even be slower due to thread overhead).

    For real CPU-bound speedups, we'll later use multiprocessing.
    """
    thread_name = threading.current_thread().name
    print(f"[CPU] Starting heavy computation({n}) on {thread_name}")
    total = 0
    for i in range(n):
        total += i * i
    print(f"[CPU] Done heavy computation({n}) on {thread_name}")
    return total


def run_cpu_sequential(tasks: List[int]) -> None:
    """
    Run cpu_heavy sequentially.
    """
    start = time.perf_counter()
    results = [cpu_heavy(n) for n in tasks]
    elapsed = time.perf_counter() - start
    print(f"[CPU-SEQUENTIAL] Completed {len(results)} tasks in {elapsed:.2f}s\n")


def run_cpu_with_threads(tasks: List[int], max_workers: int = 4) -> None:
    """
    Run cpu_heavy with a ThreadPoolExecutor.

    EXPECTATION in CPython:
    - Not much faster than sequential (sometimes even slower),
      because GIL prevents true parallel CPU execution.
    """
    start = time.perf_counter()
    results = []

    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        futures = [executor.submit(cpu_heavy, n) for n in tasks]

        for future in as_completed(futures):
            try:
                result = future.result()
            except Exception as exc:
                print(f"[CPU-THREAD-ERROR] Exception: {exc!r}")
            else:
                results.append(result)

    elapsed = time.perf_counter() - start
    print(f"[CPU-THREADED] Completed {len(results)} tasks in {elapsed:.2f}s with max_workers={max_workers}\n")


# ------------------------------------------------------------
# MAIN DEMO
# ------------------------------------------------------------

def main() -> None:
    """
    Run demonstrations:

    1) I/O-bound fake download (sequential vs threads)
    2) CPU-bound work (sequential vs threads)
    """
    urls = [f"http://example.com/resource-{i}" for i in range(6)]

    print("=== I/O-BOUND: SEQUENTIAL ===")
    run_io_sequential(urls)

    print("=== I/O-BOUND: THREADED ===")
    run_io_with_threads(urls, max_workers=4)

    # For CPU demo, use moderately big numbers
    cpu_tasks = [5_000_00, 6_000_00, 7_000_00, 8_000_00]  # adjust if needed

    print("=== CPU-BOUND: SEQUENTIAL ===")
    run_cpu_sequential(cpu_tasks)

    print("=== CPU-BOUND: THREADED ===")
    run_cpu_with_threads(cpu_tasks, max_workers=4)


if __name__ == "__main__":
    main()
