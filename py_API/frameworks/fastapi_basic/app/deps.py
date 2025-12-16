from fastapi import Depends, HTTPException
from fastapi.security import OAuth2PasswordBearer
from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession

from .db import get_db
from .models import UserORM
from .security import decode_token

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/auth/login")  # tells Swagger how to get a token

async def get_current_user(
    token: str = Depends(oauth2_scheme),     # reads Authorization: Bearer <token>
    db: AsyncSession = Depends(get_db),      # DB session for user lookup
) -> UserORM:
    try:
        claims = decode_token(token)         # verifies signature + exp
    except Exception:
        raise HTTPException(status_code=401, detail="Invalid or expired access token")

    if claims.get("type") != "access":       # prevents using refresh token as access
        raise HTTPException(status_code=401, detail="Not an access token")

    user_id = int(claims["sub"])             # user id stored in token
    res = await db.execute(select(UserORM).where(UserORM.id == user_id))
    user = res.scalar_one_or_none()
    if not user:
        raise HTTPException(status_code=401, detail="User not found")
    return user


def require_role(required: str):
    async def _guard(user: UserORM = Depends(get_current_user)) -> UserORM:
        if user.role != required:
            raise HTTPException(status_code=403, detail="Forbidden")
        return user
    return _guard

