"""
Goal:
- Show how to fix race conditions with threading.Lock.
- Compare:
    - UNSAFE increments (no lock, lost updates)
    - SAFE increments (lock-protected critical section)

Key ideas:
- A race condition happens when multiple threads read/modify/write shared data
  without coordination.
- Lock (mutex) ensures that only ONE thread at a time enters the critical section.
"""

from __future__ import annotations

import threading
import time


# =====================================================================
# 1) UNSAFE SHARED COUNTER (same as before)
# =====================================================================

shared_counter = 0  # shared across threads (intentional global)


def unsafe_worker(name: str, times: int):
    """
    Increment a shared counter WITHOUT any lock.
    This is prone to race conditions.
    """
    global shared_counter

    for _ in range(times):
        local_copy = shared_counter      # read
        local_copy += 1                  # modify
        time.sleep(0.0001)               # force thread switching
        shared_counter = local_copy      # write

    print(f"[{name}] Finished UNSAFE increments")


def demo_unsafe():
    """
    Run two threads that both increment the shared counter without a lock.
    """
    global shared_counter
    shared_counter = 0

    print("\n=== DEMO: UNSAFE increments (no lock) ===")

    t1 = threading.Thread(target=unsafe_worker, args=("T1", 100))
    t2 = threading.Thread(target=unsafe_worker, args=("T2", 100))

    t1.start()
    t2.start()
    t1.join()
    t2.join()

    print(f"[UNSAFE] Expected counter = 200")
    print(f"[UNSAFE] Actual counter   = {shared_counter}")
    print("-> Lost updates due to race condition.\n")


# =====================================================================
# 2) SAFE SHARED COUNTER USING LOCK
# =====================================================================

safe_counter = 0
counter_lock = threading.Lock()  # mutex protecting safe_counter


def safe_worker(name: str, times: int):
    """
    Increment a shared counter WITH a lock.

    CRITICAL SECTION:
        safe_counter += 1

    Only one thread at a time is allowed to execute that piece of code,
    because they must acquire counter_lock first.
    """
    global safe_counter

    for _ in range(times):
        # Version 1: explicit acquire/release
        # counter_lock.acquire()
        # try:
        #     safe_counter += 1
        # finally:
        #     counter_lock.release()

        # Version 2: preferred Python style using context manager
        with counter_lock:
            safe_counter += 1

        # Small sleep to keep similar execution profile as unsafe version
        time.sleep(0.0001)

    print(f"[{name}] Finished SAFE increments")


def demo_safe():
    """
    Run two threads that both increment the shared counter with a lock.
    """
    global safe_counter
    safe_counter = 0

    print("\n=== DEMO: SAFE increments (with lock) ===")

    t1 = threading.Thread(target=safe_worker, args=("T1", 100))
    t2 = threading.Thread(target=safe_worker, args=("T2", 100))

    t1.start()
    t2.start()
    t1.join()
    t2.join()

    print(f"[SAFE] Expected counter = 200")
    print(f"[SAFE] Actual counter   = {safe_counter}")
    print("-> Lock guarantees no lost updates.\n")


# =====================================================================
# 3) EXAMPLE: PROTECTING MORE COMPLEX SHARED STATE
# =====================================================================

shared_list = []
list_lock = threading.Lock()


def list_worker(name: str, items: int):
    """
    Example where we share a list between threads.

    Even for operations that LOOK atomic, it's safer to lock around
    multi-step operations or invariants.
    """
    for i in range(items):
        with list_lock:
            # "critical section" on shared_list
            shared_list.append((name, i))
        time.sleep(0.00005)
    print(f"[{name}] Finished adding to shared_list")


def demo_shared_list():
    """
    Run multiple threads appending to a shared list using a lock.
    """
    global shared_list
    shared_list = []

    print("\n=== DEMO: shared list WITH lock ===")

    threads = [
        threading.Thread(target=list_worker, args=(f"T{i}", 50))
        for i in range(4)
    ]

    for t in threads:
        t.start()
    for t in threads:
        t.join()

    print(f"[LIST] Expected length = 4 * 50 = 200")
    print(f"[LIST] Actual length   = {len(shared_list)}")
    print("-> All appends preserved, thanks to the lock.\n")


# =====================================================================
# MAIN
# =====================================================================

def main():
    demo_unsafe()
    demo_safe()
    demo_shared_list()


if __name__ == "__main__":
    main()
