"""
Brute-force protection demo endpoints (clean playground).

This does NOT touch /auth/login.
We simulate login success/failure to learn brute-force protection patterns.
"""

from fastapi import APIRouter, Depends, Request
from pydantic import BaseModel, Field
from redis.asyncio import Redis

from ..cache import get_redis
from ..rate_limit import client_ip
from ..bruteforce import (
    BruteForceConfig,
    ensure_not_locked,
    register_failure,
    clear_state,
)

router = APIRouter(prefix="/bf-demo", tags=["bruteforce-demo"])


class LoginSimPayload(BaseModel):
    username: str = Field(min_length=1, max_length=50)
    password: str = Field(min_length=1, max_length=200)


CFG = BruteForceConfig(
    key_prefix="login-sim",
    max_failures=5,        # after 5 bad tries...
    window_seconds=120,    # ...within 2 minutes
    lock_seconds=60,       # lock for 60 seconds
)


@router.post("/login")
async def login_sim(
    payload: LoginSimPayload,
    request: Request,
    r: Redis = Depends(get_redis),
):
    ip = client_ip(request)
    username = payload.username.strip().lower()  # normalize to avoid "User" vs "user" bypass

    # 1) If locked, reject immediately
    await ensure_not_locked(r, CFG, username, ip)

    # 2) Simulated authentication
    # For learning: only one password is considered "correct"
    ok = payload.password == "letmein"

    if not ok:
        # record failure and maybe lock
        count = await register_failure(r, CFG, username, ip)
        # keep error generic
        return {
            "ok": False,
            "message": "Invalid credentials",
            "failures_for_this_user_ip": count,  # keep for learning; remove in real app
        }

    # 3) On success, clear failure + lock state for this identity
    await clear_state(r, CFG, username, ip)

    return {"ok": True, "message": "Login successful (simulated)"}
