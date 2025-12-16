"""
Redis-backed rate limiting helpers.

Mental model:
- A rate limit = allow at most N requests per window (e.g. 10 per 60s)
- We store a counter in Redis with an expiry (TTL) equal to the window.
- Each request increments the counter.
- If counter > N -> reject with HTTP 429.
"""

from __future__ import annotations

from dataclasses import dataclass
from typing import Callable, Optional

from fastapi import Depends, HTTPException, Request, status
from redis.asyncio import Redis

from .cache import get_redis  # reuse Redis connection factory


# -----------------------------
# Identity helpers (who to limit)
# -----------------------------

def client_ip(request: Request) -> str:
    """
    Extract the client IP.

    Note:
    - With reverse proxies (nginx, cloudflare), we'd read X-Forwarded-For,
      but for local learning, request.client.host is fine.
    """
    if request.client is None:
        return "unknown"
    return request.client.host

@dataclass(frozen=True)
class RateLimit:
    """Configuration of a limit: N requests per window_seconds."""
    key_prefix: str         # Namespace for Redis keys (e.g. "login", "books")
    limit: int
    window_seconds: int


async def _incr_with_ttl(r: Redis, key: str, window_seconds: int) -> int:
    """
    Atomically increment a counter and ensure it expires.

    Why this Lua script:
    - INCR is atomic, but setting expiry only on the first hit must be atomic too.
    - This script:
        - increments
        - if value == 1 -> sets TTL
        - returns the counter
    """
    lua = """
    local current = redis.call('INCR', KEYS[1])
    if current == 1 then
        redis.call('EXPIRE', KEYS[1], ARGV[1])
    end
    return current
    """
    return int(await r.eval(lua, 1, key, str(window_seconds)))


async def _ttl_seconds(r: Redis, key: str) -> int:
    """Return TTL in seconds for Retry-After (or -1/-2 if none/missing)."""
    return int(await r.ttl(key))


def limiter(
    config: RateLimit,
    *,
    key_fn: Optional[Callable[[Request], str]] = None,
) -> Callable:
    """
    Factory that returns a FastAPI dependency.
    Usage:
        @router.get("/ping")
        async def ping(_: None = Depends(limiter(RateLimit("demo", 5, 10)))):
            ...

    key_fn:
    - lets you choose what you rate limit by (IP, user, username, etc.)
    """
    async def _dep(
        request: Request,
        r: Redis = Depends(get_redis),
    ) -> None:
        ident = key_fn(request) if key_fn else client_ip(request)      # Determine identity (IP by default, custom if provided)
        key = f"rl:{config.key_prefix}:{ident}"                         # Build Redis key: rl:<prefix>:<identity>

        current = await _incr_with_ttl(r, key, config.window_seconds)   # Increment counter and ensure TTL is set

        if current > config.limit:                                      # If limit exceeded, reject request
            retry_after = await _ttl_seconds(r, key)
            # HTTP 429 is the standard response for rate limiting
            raise HTTPException(
                status_code=status.HTTP_429_TOO_MANY_REQUESTS,
                detail=f"Rate limit exceeded: {config.limit}/{config.window_seconds}s",
                headers={"Retry-After": str(max(retry_after, 1))},
            )

    return _dep
