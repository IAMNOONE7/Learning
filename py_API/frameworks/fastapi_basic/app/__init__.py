"""
models.py define what data is,
db.py defines how to reach the database,
main.py wires the application together,
books.py defines how users interact with data through HTTP,
and SQLAlchemy translates Python intent into SQL execution.

"""

"""
User (HTTP request)
   ↓
FastAPI Router (books.py)
   ↓
Dependency Injection (get_db from db.py)
   ↓
SQLAlchemy Session (db.py)
   ↓
ORM Models (models.py)
   ↓
SQLAlchemy generates SQL
   ↓
PostgreSQL executes SQL
   ↓
Result bubbles back up
"""

"""
AUTH FLOW (Register -> Login -> Use API -> Refresh -> Logout/Role)

Vocabulary
- Access token: short-lived JWT used on normal API requests (minutes).
- Refresh token: long-lived JWT used ONLY to get new access tokens (days).
- JWT: signed token; server verifies signature + exp; client stores/sends it.
- Bearer token: "Authorization: Bearer <access_token>" header.
- jti: unique token id inside JWT; used to track/rotate/revoke refresh tokens in DB.

0) Prereqs (once)
- Ensure DB tables exist: users, refresh_tokens
- Ensure .env contains:
  JWT_SECRET=...
  JWT_ACCESS_MINUTES=15
  JWT_REFRESH_DAYS=7

1) REGISTER (create account)
Request:
  POST /auth/register
  Body:
    {
      "username": "alice",
      "password": "alice12345"
    }

What server does:
- Validate input (Pydantic).
- Check "users" table for existing username.
- Hash password (bcrypt) -> password_hash.
- Insert new row into users (username, password_hash, role="user").
- Return public user info (id, username, role).  (NO password / hash returned)

Response (example):
  {
    "id": 1,
    "username": "alice",
    "role": "user"
  }

2) LOGIN (prove identity and receive tokens)
Request:
  POST /auth/login
  Body:
    {
      "username": "alice",
      "password": "alice12345"
    }

What server does:
- Load user by username from DB.
- Verify password vs stored hash.
- Create ACCESS token (JWT) with:
    type="access", sub=user_id, role=user.role, exp=now+15min, jti=uuid
- Create REFRESH token (JWT) with:
    type="refresh", sub=user_id, exp=now+7days, jti=uuid
- Store refresh token jti in DB table refresh_tokens:
    (jti, user_id, expires_at, revoked=false)
- Return both tokens.

Response (example):
  {
    "access_token":  "<JWT_ACCESS>",
    "refresh_token": "<JWT_REFRESH>",
    "token_type": "bearer"
  }

3) CALL PROTECTED ENDPOINTS (use access token)
Client sends access token in header:
  Authorization: Bearer <JWT_ACCESS>

Example:
  GET /auth/me
  Header: Authorization: Bearer <JWT_ACCESS>

What server does (FastAPI dependency chain):
- oauth2_scheme extracts token from Authorization header.
- decode_token() verifies signature + exp.
- Ensure token "type" == "access".
- Read user_id from "sub".
- Load user from DB and attach it to endpoint as `user`.
- Endpoint executes and returns data.

If token expired/invalid -> 401 Unauthorized.

4) ROLE-BASED ACCESS (admin-only example)
Example:
  GET /auth/admin-only
  Header: Authorization: Bearer <JWT_ACCESS>

What server does:
- Same steps as protected endpoint to resolve current user.
- require_role("admin") checks user.role == "admin".
- If not admin -> 403 Forbidden.
- If admin -> endpoint executes.

NOTE: To test admin, you can manually update user role in DB:
  UPDATE users SET role='admin' WHERE username='alice';

5) REFRESH (get new access token when access expires)
When access token expires (401), client uses refresh token:

Request:
  POST /auth/refresh
  Body (as implemented in the lab):
    refresh_token=<JWT_REFRESH>
  (If you later change it to JSON, it would be: {"refresh_token": "..."} )

What server does:
- decode_token(refresh_token) verifies signature + exp.
- Ensure token "type" == "refresh".
- Read jti and user_id (sub).
- Look up refresh token record by jti in DB:
    - must exist
    - must NOT be revoked
- ROTATION (recommended): revoke old refresh token:
    rt.revoked = True
- Create NEW access token for user (includes role).
- Create NEW refresh token (new jti, new exp).
- Store NEW refresh token jti in DB (revoked=false).
- Return new token pair.

Client action:
- Replace stored access token with new access token.
- Replace stored refresh token with new refresh token.

If refresh token is expired/revoked/unknown -> 401 Unauthorized.

6) LOGOUT 
- Client calls /auth/logout with refresh token (or current session id).
- Server marks that refresh token’s DB row as revoked=true.
- After logout:
    - Access token may still work until it expires (short-lived).
    - Refresh token can no longer mint new access tokens (revoked).

7) Why this design prevents common attacks
- Passwords are never stored in plain text (only hashes).
- Access tokens are short-lived (limits damage if stolen).
- Refresh tokens are tracked in DB and can be revoked/rotated.
- Sorting/filtering queries remain parameterized (no SQL injection).


"""