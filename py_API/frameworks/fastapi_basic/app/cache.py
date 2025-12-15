"""
Redis cache helper (async).

WHAT:
- Store small JSON responses (like paginated /books results) with TTL.

WHY:
- Avoid repeating the same DB queries when the same request is called often.
- "Versioned keys" let us invalidate all book-list caches by bumping one number.
"""

import json  # serialize cached objects
import os  # read REDIS_URL from env
from typing import Any, Optional  # type hints

from dotenv import load_dotenv  # load .env
from redis.asyncio import Redis  # async Redis client

load_dotenv(override=False)  # load env vars once (safe to call multiple times)

_redis: Optional[Redis] = None  # cached Redis client (singleton)

CACHE_VERSION_KEY = "books:cache_version"  # one key controls invalidation for all /books caches


def _redis_url() -> str:
    """Read Redis connection string from environment."""
    return os.getenv("REDIS_URL", "redis://localhost:6379/0")  # default for local Memurai


async def get_redis() -> Redis:
    """
    Return shared Redis client.

    WHY:
    - Creating a new client/connection per request is wasteful.
    - A single client is reused for the whole app process.
    """
    global _redis
    if _redis is None:
        _redis = Redis.from_url(_redis_url(), decode_responses=True)  # decode text responses to str
    return _redis


async def close_redis() -> None:
    """Close Redis connection on app shutdown."""
    global _redis
    if _redis is not None:
        await _redis.aclose()
        _redis = None


async def get_cache_version() -> int:
    """
    Return current cache version.

    WHY:
    - If we change the version, old cached keys become unreachable (instant invalidation).
    """
    r = await get_redis()
    v = await r.get(CACHE_VERSION_KEY)
    if v is None:
        await r.set(CACHE_VERSION_KEY, "1")  # initialize version on first use
        return 1
    return int(v)


async def bump_cache_version() -> int:
    """
    Increment cache version to invalidate all cached /books list results.

    Call after:
    - POST /books
    - PUT /books/{id}
    - DELETE /books/{id}
    """
    r = await get_redis()
    return int(await r.incr(CACHE_VERSION_KEY))  # INCR is atomic in Redis


async def cache_get_json(key: str) -> Optional[Any]:
    """Return cached JSON decoded into Python object (or None if missing)."""
    r = await get_redis()
    raw = await r.get(key)
    if raw is None:
        return None
    return json.loads(raw)


async def cache_set_json(key: str, value: Any, ttl_seconds: int) -> None:
    """Store Python object as JSON with a TTL."""
    r = await get_redis()
    await r.set(key, json.dumps(value), ex=ttl_seconds)  # ex = expiry in seconds
