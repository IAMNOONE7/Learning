"""
Goal:
- Learn how to use queue.Queue with worker threads.
- Implement a classic producer–consumer pattern.

Why Queue?
- It gives you:
    - Thread-safe put() and get()
    - Built-in blocking (workers wait for work)
    - A natural way to avoid sharing complex mutable state directly

We will:
1) Create a Queue of jobs
2) Start N worker threads that:
       while True:
           item = queue.get()
           if item is sentinel: break
           process item
           queue.task_done()
3) A producer thread (or main thread) puts jobs into the queue
4) When done, producer puts N sentinel values to stop workers
5) Use queue.join() to wait until all jobs are processed
"""

from __future__ import annotations

import threading
import time
from queue import Queue
from typing import Any, List


# =====================================================================
# 1) Worker function using a Queue
# =====================================================================

def worker_thread(name: str, jobs: "Queue[Any]"):
    """
    Worker thread that processes items from the queue.

    Pattern:
        for each item:
            - get blockingly (wait if no item)
            - if item is sentinel, break
            - otherwise process it
            - call task_done() when finished

    This loop runs until it receives the sentinel object.
    """
    while True:
        job = jobs.get()  # blocks until item is available
        if job is None:
            # None will be our sentinel value meaning "no more jobs"
            print(f"[{name}] Received sentinel, exiting.")
            jobs.task_done()
            break

        print(f"[{name}] Got job: {job!r}")
        # Simulate some work on the job
        time.sleep(0.2)
        print(f"[{name}] Finished job: {job!r}")

        # IMPORTANT: signal that this job is completed
        jobs.task_done()

    print(f"[{name}] Worker stopped.")


# =====================================================================
# 2) Simple demo: main thread as producer
# =====================================================================

def demo_basic_queue_workers():
    print("\n=== DEMO: Queue + worker threads (basic) ===")

    num_workers = 3
    jobs: "Queue[Any]" = Queue() # Forward reference - avoids Python trying to evaluate the annotation immediately

    # Start worker threads
    workers: List[threading.Thread] = []
    for i in range(num_workers):
        t = threading.Thread(
            target=worker_thread,
            args=(f"Worker-{i}", jobs),
            daemon=True,   # daemon so they don't block program exit in case of bug
        )
        t.start()
        workers.append(t)

    # Main thread is producer: put some jobs into the queue
    for j in range(10):
        print(f"[main] Putting job {j}")
        jobs.put(j)

    # Tell workers to stop: put one sentinel per worker
    for _ in range(num_workers):
        jobs.put(None)

    # Wait until all jobs (including sentinels) are processed
    print("[main] Waiting for all jobs to be processed with jobs.join()...")
    jobs.join()
    print("[main] All jobs processed, workers should be stopping now.\n")

    # Optionally join non-daemon workers. (Here they are daemon, so not required.)
    for t in workers:
        t.join(timeout=0.1)  # timeout just to not block forever if bug

    print("[main] demo_basic_queue_workers finished.\n")


# =====================================================================
# 3) Producer thread + workers
# =====================================================================

def producer_thread(name: str, jobs: "Queue[Any]", num_jobs: int, delay: float = 0.1):
    """
    Producer that generates some jobs and puts them into the queue.

    This could be:
    - reading lines from a file
    - reading requests from network
    - polling some external system
    """
    print(f"[{name}] Starting, will produce {num_jobs} jobs.")
    for i in range(num_jobs):
        item = f"job-{i}"
        print(f"[{name}] -> putting {item}")
        jobs.put(item)
        time.sleep(delay)
    print(f"[{name}] Finished producing.")


def demo_producer_and_workers():
    print("\n=== DEMO: Producer thread + worker threads ===")

    num_workers = 4
    total_jobs = 12
    jobs: "Queue[Any]" = Queue()

    # Start workers
    workers: List[threading.Thread] = []
    for i in range(num_workers):
        t = threading.Thread(
            target=worker_thread,
            args=(f"Worker-{i}", jobs),
            daemon=True,
        )
        t.start()
        workers.append(t)

    # Start producer in its own thread
    producer = threading.Thread(
        target=producer_thread,
        args=("Producer", jobs, total_jobs, 0.05),
        daemon=True,
    )
    producer.start()

    # Wait for producer to finish
    producer.join()
    print("[main] Producer finished, sending sentinels to workers...")

    # Send sentinels
    for _ in range(num_workers):
        jobs.put(None)

    # Wait for all jobs to be processed
    jobs.join()
    print("[main] All jobs processed.\n")

    for t in workers:
        t.join(timeout=0.1)

    print("[main] demo_producer_and_workers finished.\n")


# =====================================================================
# 4) Why Queue helps with thread safety
# =====================================================================

# === Queue vs Locks ===
#
# - With shared counters/lists, you must use threading.Lock to protect
#   your own critical sections.
#
# - Queue gives you built-in thread-safe operations:
#       put()
#       get()
#       task_done()
#       join()
#
# - In a producer/consumer design:
#       * Producers put work items into the queue.
#       * Worker threads block on queue.get() when there is no work.
#       * Workers call task_done() when the job is finished.
#       * No manual locking is required around queue operations.
#
# - This reduces shared mutable state and avoids many race conditions.
#
# - The pattern scales well: to handle more I/O-bound tasks,
#   you simply add more worker threads.

# =====================================================================
# MAIN
# =====================================================================

def main():
    demo_basic_queue_workers()
    demo_producer_and_workers()


if __name__ == "__main__":
    main()
