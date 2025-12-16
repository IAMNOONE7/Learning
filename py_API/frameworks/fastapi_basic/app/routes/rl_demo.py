"""
Rate limiting demo endpoints (safe playground).

Goal:
- Endpoint we can spam to see Redis counters + 429 responses.
"""

from fastapi import APIRouter, Depends
from ..rate_limit import RateLimit, limiter


router = APIRouter(prefix="/rl-demo", tags=["rate-limit-demo"])

# Example: allow 5 requests per 10 seconds per IP
demo_limit = RateLimit(key_prefix="ping", limit=5, window_seconds=10)


@router.get("/ping")
async def ping(_: None = Depends(limiter(demo_limit))):
    return {"ok": True, "message": "pong"}


# Brute-force style: much stricter, per IP
login_sim_limit = RateLimit(key_prefix="login-sim", limit=3, window_seconds=60)


@router.post("/login-sim")
async def login_sim(_: None = Depends(limiter(login_sim_limit))):
    # Pretend this is a login endpoint (we only care about rate limiting behavior here)
    return {"ok": True, "message": "login attempt accepted (simulated)"}

