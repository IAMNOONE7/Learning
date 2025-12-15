import os
import uuid  # Generate unique token IDs (jti)
from datetime import datetime, timedelta, timezone  # Handle token timestamps
from typing import Any, Dict  # Type hints for JWT payloads

from dotenv import load_dotenv  # Load .env configuration
from jose import jwt  # JWT encode/decode
from passlib.context import CryptContext  # Password hashing abstraction

load_dotenv(override=False)  # Load environment variables once

# Password hashing context (bcrypt is industry standard)
_pwd = CryptContext(schemes=["bcrypt"], deprecated="auto")

# JWT = JSON Web Token
JWT_SECRET = os.getenv("JWT_SECRET", "dev-secret")  # Secret key for signing tokens
JWT_ALG = "HS256"  # Symmetric JWT algorithm
ACCESS_MIN = int(os.getenv("JWT_ACCESS_MINUTES", "15"))  # Access token lifetime
REFRESH_DAYS = int(os.getenv("JWT_REFRESH_DAYS", "7"))  # Refresh token lifetime


def hash_password(password: str) -> str:
    """Hash a plain-text password before storing it"""
    return _pwd.hash(password)


def verify_password(password: str, password_hash: str) -> bool:
    """Verify a plain-text password against its hash"""
    return _pwd.verify(password, password_hash)


def _utcnow() -> datetime:
    """Return current UTC time (JWTs must use UTC)"""
    return datetime.now(timezone.utc)


def create_access_token(user_id: int, role: str) -> str:
    """Create a short-lived access JWT"""
    now = _utcnow()
    payload: Dict[str, Any] = {
        "type": "access",  # Token type discriminator
        "sub": str(user_id),  # Subject = user id
        "role": role,  # Embed user role for authorization
        "iat": int(now.timestamp()),  # Issued-at time
        "exp": int((now + timedelta(minutes=ACCESS_MIN)).timestamp()),  # Expiration time
        "jti": str(uuid.uuid4()),  # Unique token identifier
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALG)  # Sign and encode JWT


def create_refresh_token(user_id: int) -> tuple[str, datetime]:
    """Create a long-lived refresh JWT"""
    now = _utcnow()
    exp_dt = now + timedelta(days=REFRESH_DAYS)
    payload: Dict[str, Any] = {
        "type": "refresh",  # Mark as refresh token
        "sub": str(user_id),  # Subject = user id
        "iat": int(now.timestamp()),  # Issued-at time
        "exp": int(exp_dt.timestamp()),  # Expiration time
        "jti": str(uuid.uuid4()),  # Unique token identifier
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALG), exp_dt  # Return token + expiry


def decode_token(token: str) -> Dict[str, Any]:
    """Decode and verify a JWT (raises if invalid or expired)"""
    return jwt.decode(token, JWT_SECRET, algorithms=[JWT_ALG])
