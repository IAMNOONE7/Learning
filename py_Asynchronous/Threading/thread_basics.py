"""
Goal:
- Understand what a thread REALLY is.
- Learn how to create, start, and join threads manually.
- Understand daemon threads.
- Observe concurrent execution.
- See why shared data across threads is dangerous (race conditions).
"""

from __future__ import annotations

import threading
import time

# ======================================================================
# 1) Minimal example: What is a Thread?
# ======================================================================

def worker_simple():
    """
    A very simple worker function.

    A thread just means this function will run in parallel (concurrently)
    with the main program.
    NOTE:
    - In CPython, due to the GIL, threads don't run TRUE parallel CPU,
      but they do overlap when doing I/O or sleeping.
    """
    print("[worker_simple] Starting...")
    time.sleep(1)
    print("[worker_simple] Done!")


def demo_basic_thread():
    print("\n=== DEMO: basic thread ===")

    # Create a Thread object
    t = threading.Thread(
        target=worker_simple,   # function that will run in this new thread
        name="MyThread-1"       # optional but good for debugging
    )

    print("[main] Starting thread...")
    t.start()  # IMPORTANT: this actually kicks off the thread

    print("[main] Now main continues while thread runs in background...")

    # Wait until thread finishes
    t.join()

    print("[main] Thread has finished!")


# ======================================================================
# 2) Threads with arguments
# ======================================================================

def worker_with_args(name: str, delay: float):
    """
    Workers often need arguments.
    """
    print(f"[{name}] Starting, sleeping {delay}s...")
    time.sleep(delay)
    print(f"[{name}] Done!")


def demo_thread_args():
    print("\n=== DEMO: thread with arguments ===")

    t = threading.Thread(
        target=worker_with_args,
        args=("Worker-A", 1.5),
        name="Thread-A",
    )

    t.start()
    t.join()
    print("[main] Worker-A finished.")


# ======================================================================
# 3) Multiple concurrent threads
# ======================================================================

def worker_counting(name: str, count: int):
    """
    Visible concurrency:
    Workers repeatedly print something while sleeping a bit.
    """
    for i in range(count):
        print(f"[{name}] Step {i}")
        time.sleep(0.2)
    print(f"[{name}] Completed.")


def demo_multiple_threads():
    print("\n=== DEMO: multiple threads ===")

    t1 = threading.Thread(target=worker_counting, args=("T1", 5), name="T1")
    t2 = threading.Thread(target=worker_counting, args=("T2", 5), name="T2")

    t1.start()
    t2.start()

    # Notice how their prints interleave — this is real concurrency!
    t1.join()
    t2.join()

    print("[main] Both threads finished.")


# ======================================================================
# 4) Daemon vs Non-daemon threads
# ======================================================================

def worker_daemon():
    """
    Daemon threads automatically stop when the main program ends.
    Useful for background tasks that should NOT block shutdown.
    """
    for i in range(10):
        print(f"[daemon] Working... {i}")
        time.sleep(0.3)


def demo_daemon_thread():
    print("\n=== DEMO: daemon thread ===")

    t = threading.Thread(target=worker_daemon, daemon=True, name="DaemonThread")

    t.start()

    print("[main] Main thread sleeps briefly then ends...")
    time.sleep(1)  # give daemon thread some time

    # NO join()
    print("[main] Exiting now. Daemon thread will be killed instantly.")


# ======================================================================
# 5) Danger: Shared state & race condition intro
# ======================================================================

shared_counter = 0  # This is shared across all threads


def worker_increment(name: str, times: int):
    """
    Demonstrates race conditions.

    Each thread increments a global counter.
    This is NOT safe without a Lock.
    """
    global shared_counter

    for _ in range(times):
        # Read-modify-write sequence is NOT atomic.
        # Threads can overlap between read and write = corrupted result.
        local_copy = shared_counter
        local_copy += 1
        time.sleep(0.0001)  # force switching to make race more visible
        shared_counter = local_copy

    print(f"[{name}] Finished increments.")


def demo_race_condition():
    print("\n=== DEMO: race condition (unsafe shared state) ===")

    global shared_counter
    shared_counter = 0  # reset

    t1 = threading.Thread(target=worker_increment, args=("T1", 100))
    t2 = threading.Thread(target=worker_increment, args=("T2", 100))

    t1.start()
    t2.start()
    t1.join()
    t2.join()

    print(f"[main] Expected counter = 200")
    print(f"[main] Actual counter   = {shared_counter}\n")
    print("This shows why shared data across threads is dangerous.")


# ======================================================================
# MAIN: Run all demos
# ======================================================================

def main():
    demo_basic_thread()
    demo_thread_args()
    demo_multiple_threads()
    demo_daemon_thread()
    demo_race_condition()


if __name__ == "__main__":
    main()
