"""
Brute-force protection helpers (Redis-backed).

Mental model:
- Every failed login increments a counter for (username + ip).
- If failures exceed a threshold, we set a "lock" key with a TTL (cooldown).
- While locked, we reject immediately with HTTP 429 + Retry-After.
- On successful login, we clear the failure counter and lock.
"""

from __future__ import annotations

from dataclasses import dataclass
from fastapi import HTTPException, status
from redis.asyncio import Redis

# We'll pass `ip` and `username` in explicitly.


@dataclass(frozen=True)
class BruteForceConfig:
    key_prefix: str            # namespace for redis keys, e.g. "login-demo"
    max_failures: int          # how many failures allowed before locking
    window_seconds: int        # failures counted within this rolling window
    lock_seconds: int          # lock duration once threshold exceeded


async def _incr_with_ttl(r: Redis, key: str, ttl_seconds: int) -> int:
    """
    Atomic INCR + EXPIRE on first hit.
    (Same pattern as rate_limit.py, reused here on purpose.)
    """
    lua = """
    local current = redis.call('INCR', KEYS[1])
    if current == 1 then
        redis.call('EXPIRE', KEYS[1], ARGV[1])
    end
    return current
    """
    return int(await r.eval(lua, 1, key, str(ttl_seconds)))


async def _ttl(r: Redis, key: str) -> int:
    """TTL for Retry-After."""
    return int(await r.ttl(key))


def _fail_key(cfg: BruteForceConfig, username: str, ip: str) -> str:
    return f"bf:{cfg.key_prefix}:fail:{username}:{ip}"


def _lock_key(cfg: BruteForceConfig, username: str, ip: str) -> str:
    return f"bf:{cfg.key_prefix}:lock:{username}:{ip}"


async def ensure_not_locked(r: Redis, cfg: BruteForceConfig, username: str, ip: str) -> None:
    """
    Reject early if (username + ip) is currently locked.
    """
    lk = _lock_key(cfg, username, ip)
    is_locked = await r.exists(lk)
    if is_locked:
        retry_after = await _ttl(r, lk)
        raise HTTPException(
            status_code=status.HTTP_429_TOO_MANY_REQUESTS,
            detail="Too many login attempts. Try again later.",
            headers={"Retry-After": str(max(retry_after, 1))},
        )


async def register_failure(r: Redis, cfg: BruteForceConfig, username: str, ip: str) -> int:
    """
    Record a failed login attempt. Returns current failure count.
    If threshold exceeded, sets a lock key.
    """
    fk = _fail_key(cfg, username, ip)
    current = await _incr_with_ttl(r, fk, cfg.window_seconds)

    if current >= cfg.max_failures:
        # Set lock key with TTL. Value doesn't matter; we just need existence + TTL.
        lk = _lock_key(cfg, username, ip)
        await r.set(lk, "1", ex=cfg.lock_seconds)

    return current


async def clear_state(r: Redis, cfg: BruteForceConfig, username: str, ip: str) -> None:
    """
    On successful login, clear failures + lock for (username + ip).
    """
    await r.delete(_fail_key(cfg, username, ip), _lock_key(cfg, username, ip))
