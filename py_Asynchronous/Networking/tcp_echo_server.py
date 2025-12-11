"""
Goal:
- Build a simple asynchronous TCP echo server using asyncio.
- Understand:
    - asyncio.start_server
    - reader / writer streams
    - handling multiple clients concurrently

HOW TO RUN:
    1) Start the server:
        python tcp_echo_server.py

    2) In another terminal, use either:
        - our async client (tcp_echo_client.py)

    3) Type text, press Enter; server will echo back the line.
"""

import asyncio
from typing import Tuple


HOST = "127.0.0.1"   # localhost
PORT = 8888          # typical test port


async def handle_client(reader: asyncio.StreamReader,
                        writer: asyncio.StreamWriter) -> None:
    """
    This is called for each new client connection.

    - `reader`  = async interface to read bytes from the socket
    - `writer`  = async interface to write bytes to the socket

    The function runs in its own Task. Many handle_client coroutines can run
    "concurrently" on a single event loop (one per client connection).
    """
    # Get client address for logging
    peername: Tuple[str, int] = writer.get_extra_info("peername")
    print(f"[SERVER] New connection from {peername}")

    try:
        while True:
            # Wait for a line of data ending with '\n'
            # This suspends the coroutine until data arrives.
            data: bytes = await reader.readline()

            # If data is empty => client closed the connection.
            if not data:
                print(f"[SERVER] Client {peername} disconnected.")
                break

            # Decode bytes to string for logging
            message = data.decode().rstrip("\n")
            print(f"[SERVER] Received from {peername}: {message!r}")

            # Prepare the echo message
            response = f"ECHO: {message}\n"
            # Encode as bytes for sending
            writer.write(response.encode("utf-8"))

            # drain() is where the coroutine yields control to the event loop
            # until the data is actually sent (or buffered safely).
            await writer.drain()

    except asyncio.CancelledError:
        # If server shuts down and cancels client tasks:
        print(f"[SERVER] Connection task cancelled for {peername}")
        raise

    except Exception as exc:
        print(f"[SERVER] Error with {peername}: {exc!r}")

    finally:
        # Always close the writer (half-close connection)
        print(f"[SERVER] Closing connection to {peername}")
        writer.close()
        # Wait until the underlying socket is closed
        await writer.wait_closed()


async def main() -> None:
    """
    Creates the TCP server and starts serving clients.

    asyncio.start_server:
        - binds to HOST:PORT
        - returns a Server object which holds listening sockets
        - for each new client, it creates:
            Task(target=handle_client, args=(reader, writer))

    The server itself runs within the event loop, single-threaded.
    """
    server = await asyncio.start_server(
        handle_client,  # callback for each client
        host=HOST,
        port=PORT,
    )

    addr = ", ".join(str(sock.getsockname()) for sock in server.sockets)
    print(f"[SERVER] Serving on {addr}")
    print("[SERVER] Press Ctrl+C to stop.\n")

    # async with ensures proper closing on exit
    async with server:
        try:
            # serve_forever() never returns unless cancelled (e.g. Ctrl+C)
            await server.serve_forever()
        except asyncio.CancelledError:
            print("[SERVER] Server task cancelled, shutting down...")


if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        # This is raised when you hit Ctrl+C.
        print("\n[SERVER] KeyboardInterrupt received, exiting.")
