"""
Goal:
- Build a simple async TCP client that talks to our echo server.

We will:
- Use asyncio.open_connection(host, port) -> (reader, writer)
- Send some messages
- Read echo responses
- Show how multiple clients could run concurrently.

HOW TO RUN:

1) Make sure echo server is running:
       python tcp_echo_server.py

2) In another terminal, run this client:
       python tcp_echo_client.py

3) Watch the interaction in both consoles.
"""

import asyncio
from typing import List


HOST = "127.0.0.1"
PORT = 8888


async def simple_client(name: str, messages: List[str]) -> None:
    """
    Single client that connects to the server and exchanges a few messages.

    - Connects via asyncio.open_connection
    - For each message:
        * sends line (with '\n')
        * waits for echo from server
    """

    print(f"[{name}] Connecting to {HOST}:{PORT} ...")
    reader, writer = await asyncio.open_connection(HOST, PORT)
    print(f"[{name}] Connected")

    try:
        for msg in messages:
            line = msg + "\n"
            print(f"[{name}] -> {msg!r}")
            writer.write(line.encode("utf-8"))
            await writer.drain()

            # Read one line back (server echoes with newline)
            data = await reader.readline()
            if not data:
                print(f"[{name}] Server closed the connection.")
                break

            response = data.decode().rstrip("\n")
            print(f"[{name}] <- {response!r}")

        print(f"[{name}] Done sending messages, closing connection")

    finally:
        writer.close()
        await writer.wait_closed()
        print(f"[{name}] Connection closed")


async def main() -> None:
    """
    Demo:
    - Starts several clients concurrently to show that
      the server can handle multiple connections at once.

    Note: Clients themselves are also coroutines; here they run
    concurrently in the same event loop process as well.
    """
    clients = [
        simple_client("Client-1", ["hello", "world"]),
        simple_client("Client-2", ["foo", "bar", "baz"]),
        simple_client("Client-3", ["Python", "asyncio", "async"]),
    ]

    # Run all clients concurrently and wait until all finish
    await asyncio.gather(*clients)


if __name__ == "__main__":
    asyncio.run(main())
