from fastapi import APIRouter, Depends, HTTPException, status  # FastAPI building blocks
from sqlalchemy import select  # SQL SELECT builder
from sqlalchemy.ext.asyncio import AsyncSession  # Async DB session
from fastapi.security import OAuth2PasswordRequestForm
from ..db import get_db  # DB dependency
from ..models import RefreshTokenORM, TokenPair, UserCreate, UserORM, UserPublic  # Models/schemas
from ..security import (
    create_access_token,  # Generate access JWT
    create_refresh_token,  # Generate refresh JWT
    decode_token,  # Decode JWT
    hash_password,  # Hash user passwords
    verify_password,  # Verify passwords
)
from ..deps import get_current_user, require_role

router = APIRouter(prefix="/auth", tags=["auth"])  # Group auth endpoints

@router.post("/register", response_model=UserPublic, status_code=status.HTTP_201_CREATED)
async def register(payload: UserCreate, db: AsyncSession = Depends(get_db)) -> UserPublic:
    """Create a new user account"""
    existing = await db.execute(
        select(UserORM).where(UserORM.username == payload.username)
    )
    if existing.scalar_one_or_none():
        raise HTTPException(status_code=400, detail="Username already exists")

    user = UserORM(
        username=payload.username,
        password_hash=hash_password(payload.password),  # Never store plain passwords
        role="user",  # Default role
    )
    db.add(user)
    await db.commit()
    await db.refresh(user)

    return UserPublic.model_validate(user)


@router.post("/login", response_model=TokenPair)
async def login(
    form: OAuth2PasswordRequestForm = Depends(),  # <-- reads username/password as form fields
    db: AsyncSession = Depends(get_db),
) -> TokenPair:
    # Load user by username
    res = await db.execute(select(UserORM).where(UserORM.username == form.username))
    user = res.scalar_one_or_none()

    # Verify password
    if not user or not verify_password(form.password, user.password_hash):
        raise HTTPException(status_code=401, detail="Invalid credentials")

    # Create tokens
    access = create_access_token(user_id=user.id, role=user.role)
    refresh, refresh_exp = create_refresh_token(user_id=user.id)

    # Store refresh token jti in DB
    refresh_claims = decode_token(refresh)
    db.add(
        RefreshTokenORM(
            jti=refresh_claims["jti"],
            user_id=user.id,
            expires_at=refresh_exp,
            revoked=False,
        )
    )
    await db.commit()

    return TokenPair(access_token=access, refresh_token=refresh)


@router.post("/refresh", response_model=TokenPair)
async def refresh_token(refresh_token: str, db: AsyncSession = Depends(get_db)) -> TokenPair:
    """Exchange refresh token for a new access token"""
    try:
        claims = decode_token(refresh_token)
    except Exception:
        raise HTTPException(status_code=401, detail="Invalid or expired refresh token")

    if claims.get("type") != "refresh":
        raise HTTPException(status_code=401, detail="Not a refresh token")

    jti = claims.get("jti")
    user_id = int(claims.get("sub"))

    res = await db.execute(
        select(RefreshTokenORM).where(RefreshTokenORM.jti == jti)
    )
    rt = res.scalar_one_or_none()

    if not rt or rt.revoked:
        raise HTTPException(status_code=401, detail="Refresh token revoked or unknown")

    rt.revoked = True  # Rotate refresh token
    await db.commit()

    ures = await db.execute(select(UserORM).where(UserORM.id == user_id))
    user = ures.scalar_one_or_none()

    if not user:
        raise HTTPException(status_code=401, detail="User not found")

    access = create_access_token(user_id=user.id, role=user.role)
    new_refresh, new_refresh_exp = create_refresh_token(user_id=user.id)
    new_claims = decode_token(new_refresh)

    db.add(
        RefreshTokenORM(
            jti=new_claims["jti"],
            user_id=user.id,
            expires_at=new_refresh_exp,
            revoked=False,
        )
    )
    await db.commit()

    return TokenPair(access_token=access, refresh_token=new_refresh)

@router.get("/me", response_model=UserPublic)
async def me(user: UserORM = Depends(get_current_user)) -> UserPublic:
    """Return the currently authenticated user"""
    return UserPublic.model_validate(user)


@router.get("/admin-only")
async def admin_only(user: UserORM = Depends(require_role("admin"))):
    """Example endpoint protected by role-based authorization"""
    return {"message": f"Hello admin {user.username}!"}
