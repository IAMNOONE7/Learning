"""
Goal:
- Understand the MOST BASIC way to use multiprocessing in Python:
    * multiprocessing.Process
    * target function + args
    * start()
    * join()

Key ideas:
- A process is like starting a second Python interpreter.
- Each process has its own memory space (unlike threads).
- Processes can truly run in parallel on multiple CPU cores.
"""

from __future__ import annotations

import time
from multiprocessing import Process, current_process


# ============================================================
# 1) The simplest worker function
# ============================================================

def simple_worker():
    """
    This function will run in a SEPARATE process.

    current_process() gives us info about which process is running.
    """
    proc = current_process()
    print(f"[{proc.name}] Starting work...")
    time.sleep(1.0)
    print(f"[{proc.name}] Finished work.")


def demo_single_process():
    print("\n=== DEMO: single child process ===")

    # Create a Process object
    p = Process(
        target=simple_worker,   # function to run in the new process
        name="ChildProcess-1"   # optional name for nicer logs
    )

    print("[main] Starting child process...")
    p.start()   # actually starts the new process

    print("[main] Child started; main is free to do other stuff...")
    # Here we just wait.
    p.join()    # wait until child process finishes

    print("[main] Child process has finished.")


# ============================================================
# 2) Processes with arguments
# ============================================================

def worker_with_args(name: str, delay: float, steps: int):
    """
    Worker that prints a few steps with a delay.

    This lets us SEE that two processes run in parallel.
    """
    proc = current_process()
    print(f"[{proc.name}] ({name}) starting, delay={delay}s, steps={steps}")

    for i in range(steps):
        print(f"[{proc.name}] ({name}) step {i}")
        time.sleep(delay)

    print(f"[{proc.name}] ({name}) finished.")


def demo_two_processes():
    print("\n=== DEMO: two processes running in parallel ===")

    # Create two process objects with different arguments
    p1 = Process(
        target=worker_with_args,
        args=("Worker-A", 0.3, 5),
        name="Process-A",
    )

    p2 = Process(
        target=worker_with_args,
        args=("Worker-B", 0.5, 5),
        name="Process-B",
    )

    print("[main] Starting both processes...")
    p1.start()
    p2.start()

    print("[main] Both started. main() continues while they run.")

    # Wait for both to finish
    p1.join()
    p2.join()

    print("[main] Both processes finished.")


# ============================================================
# 3) Small CPU example: sequential vs 2 processes
# ============================================================

def cpu_heavy(n: int) -> int:
    """
    Simple CPU-heavy task: sum of squares up to n.
    """
    total = 0
    for i in range(n):
        total += i * i
    return total


def cpu_worker(label: str, n: int):
    """
    Wrapper around cpu_heavy that logs which process runs it.
    """
    proc = current_process()
    print(f"[{proc.name}] ({label}) starting cpu_heavy({n})...")
    start = time.perf_counter()
    result = cpu_heavy(n)
    elapsed = time.perf_counter() - start
    print(f"[{proc.name}] ({label}) done. result={result} (ignored), time={elapsed:.2f}s")


def demo_cpu_sequential_vs_processes():
    print("\n=== DEMO: CPU work sequential vs two processes ===")

    N = 120_000_000

    print("\n-- Sequential in main process --")
    start = time.perf_counter()
    cpu_heavy(N)
    cpu_heavy(N)
    elapsed = time.perf_counter() - start
    print(f"[main] Sequential time: {elapsed:.2f}s")

    print("\n-- In two separate processes --")
    p1 = Process(target=cpu_worker, args=("P1-job", N), name="CPU-Proc-1")
    p2 = Process(target=cpu_worker, args=("P2-job", N), name="CPU-Proc-2")

    start = time.perf_counter()
    p1.start()
    p2.start()
    p1.join()
    p2.join()
    elapsed = time.perf_counter() - start
    print(f"[main] Two-process time: {elapsed:.2f}s")
    print("On a multi-core machine, two-process time should be noticeably smaller.\n")


# ============================================================
# MAIN ENTRY POINT
# ============================================================

def main():
    demo_single_process()
    demo_two_processes()
    demo_cpu_sequential_vs_processes()


if __name__ == "__main__":
    # This guard is VERY IMPORTANT on Windows and macOS for multiprocessing.
    # It prevents the child processes from re-running the whole module on import.
    main()
