# Python Concurrency Playground

This project is a **playground for learning and testing concurrency in Python**, covering:

- **Async/Await (asyncio)**
- **Threading**
- **Multiprocessing**
- **Async Networking**

It is **not** a production framework — the goal is to understand how each model works, what problems it solves, and where its limitations are.  
Every folder contains small, focused examples with heavy comments explaining *what* is happening and *why*.

---

## What Each Concurrency Model Is Good For

### Async/Await (`asyncio`)
- Best for **I/O-bound** tasks where you have **many concurrent connections**.
- Excellent for:
  - TCP/HTTP clients and servers  
  - Network-heavy workloads  
  - High-level async libraries  
- Runs on a **single thread**, switching between coroutines at `await`.

### Threading
- Best for **blocking I/O** using **non-async libraries**.
- Good for:
  - Background workers  
  - File I/O  
  - Database drivers that block  
- Shares memory -> requires synchronization (`Lock`, `Queue`).

### Multiprocessing
- Best for **CPU-bound** workloads that need **real parallelism**.
- Good for:
  - Heavy computation  
  - Data processing  
  - Simulations / analysis  
- Each process has its **own memory**


---

##  Projects Structure

- **Basics**
- **Threading**
- **Multiprocessing**
- **Networking**

Each subfolder contains stand-alone `.py` scripts designed to be run individually.  
They include extensive comments to help you understand the mechanics step-by-step.

---

## Codes

---

### Basics

**async_vs_sync.py** 
This example compares synchronous blocking code with asynchronous code using asyncio.  
The sync version blocks on `time.sleep`, so tasks run one-by-one.  
The async version uses `await asyncio.sleep`, allowing tasks to run concurrently.  
As a result, the async version finishes much faster because waiting time overlaps.

**event_loop_and_tasks.py**
This file demonstrates how the asyncio event loop schedules and runs coroutines using Tasks.  
It shows how `asyncio.create_task` lets coroutines run concurrently and compares this with using `asyncio.gather`.  
Examples illustrate that tasks run concurrently once scheduled, even if awaited individually.  
A fire-and-forget demo also highlights why running tasks without awaiting them can be unsafe.

**tasks_gather_wait_cancel.py**
This file demonstrates advanced asyncio task handling: running multiple coroutines with `gather`, and using `wait` for lower-level control.  
It shows how to detect the first completed task, cancel the remaining ones, and handle cancellation safely.  
Timeout examples illustrate how to stop long-running tasks using `asyncio.wait_for` or the `asyncio.timeout` context manager.  
Overall, it teaches essential patterns needed for real-world async apps: coordination, cancellation, and timeout management.

---

### Threading

**thread_basics.py**
This file introduces the fundamentals of Python threading, including creating, starting, and joining threads.  
It demonstrates passing arguments to threads, running multiple threads concurrently, and the behavior of daemon threads.  
It also shows why sharing data between threads can be unsafe by triggering a race condition on a shared counter.  
Overall, the script provides a clear foundation for understanding how threads work and where problems can occur.

**thread_pool_executor.py**
This file demonstrates how to use `ThreadPoolExecutor` to run blocking I/O tasks concurrently.  
It compares sequential execution with threaded execution, showing how threads improve performance when tasks spend most of their time waiting.  
A second example runs CPU-heavy functions to show that threads do *not* speed up CPU-bound work.  
Overall, the script highlights when threads are useful and when they should be avoided in favor of multiprocessing.

**locks_and_thread_safety.py**
This file demonstrates how race conditions occur when multiple threads modify shared data without coordination.  
It first shows incorrect behavior from unsafe increments, where threads overwrite each other’s updates.  
A second example fixes this using `threading.Lock` to ensure only one thread at a time enters the critical section.  
The script also illustrates locking around more complex shared structures, like a list, to maintain thread safety.

**queues_and_worker_threads.py**
This file demonstrates how to use `queue.Queue` to build a safe and scalable producer–consumer system with worker threads.  
It shows how workers pull jobs from the queue, process them, and report completion using `task_done()`.  
Sentinel values (`None`) are used to cleanly shut down worker threads once all jobs are done.  
Overall, the script highlights how queues eliminate the need for manual locks while enabling simple and safe concurrency.

---

### Multiprocessing

**process_basics.py**
This file introduces the fundamentals of multiprocessing using the raw `Process` class.  
It shows how each process runs in its own memory space and can execute truly in parallel on multiple CPU cores.  
Examples demonstrate creating processes, passing arguments, and running CPU-heavy tasks both sequentially and in parallel.  
Overall, the script highlights how multiprocessing solves CPU-bound problems where threads cannot help.

**process_pool_executor.py**
This file shows how to use `ProcessPoolExecutor` to run CPU-heavy work in parallel across multiple CPU cores.  
It compares sequential execution with two process-pool patterns: using `submit` with `as_completed`, and using the simpler `map` interface.  
Both pool techniques distribute tasks to worker processes, providing true parallelism unlike threads.  
The examples demonstrate how process pools make multiprocessing easier and cleaner than managing `Process` objects manually.

**sharing_between_processes.py**
This file demonstrates how processes do **not** share global variables, as each process has its own independent memory space.  
It then shows the recommended way to exchange data between processes: returning results via a `ProcessPoolExecutor`.  
A third example uses `multiprocessing.Manager` to create truly shared objects like lists, which allow coordinated updates across processes.  
Together, the examples illustrate when to return data, when to use shared objects, and why direct global sharing does not work in multiprocessing.

---

### Networking

**tcp_echo_server.py**
This file implements a simple asynchronous TCP echo server using `asyncio.start_server`.  
Each client connection is handled by its own coroutine, allowing many clients to be served concurrently on a single event loop.  
The server reads data from clients, prints it, and sends it back using async stream readers and writers.  
It demonstrates the core structure of async networking: non-blocking I/O, task-per-connection handling, and clean shutdown behavior.

**tcp_echo_client.py**
This file implements an asynchronous TCP client that connects to the echo server and exchanges messages.  
It uses `asyncio.open_connection` to create reader and writer streams, sending lines and reading echoed responses.  
The main function launches several clients concurrently, demonstrating how multiple connections can run in parallel on one event loop.  
Together with the server example, it illustrates the fundamentals of async client–server communication.



