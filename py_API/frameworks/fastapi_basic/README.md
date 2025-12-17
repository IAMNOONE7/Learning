## fastapi_basic — Overview

This part of the project focuses on **building REST API using FastAPI** and understanding *why* FastAPI is structured the way it is.

The goal here was not just to “make endpoints work”, but to learn:
- how modern Python APIs are designed
- how responsibilities are split across layers
- how infrastructure concerns (auth, caching, rate limits) fit into the request lifecycle

### What this implementation demonstrates

- **FastAPI fundamentals**
  - Path operations, routers, dependencies
  - Automatic request validation and OpenAPI docs
  - Async-first design

- **Data & persistence**
  - PostgreSQL as the database
  - Async SQLAlchemy ORM
  - Clear separation between ORM models and API schemas (Pydantic)

- **Authentication & authorization**
  - OAuth2 with password flow
  - JWT access & refresh tokens
  - Role-based access control
  - Secure password hashing

- **API quality features**
  - Pagination, filtering, searching, sorting
  - Safe query construction (SQL injection prevention)
  - Consistent response envelopes

- **Performance & security**
  - Redis caching for read-heavy endpoints
  - API rate limiting
  - Brute-force protection (Redis-backed)
  - Proper HTTP status codes and error handling

### Design philosophy

- dependencies instead of globals
- clear data contracts via Pydantic
- infrastructure isolated from business logic
- async I/O for scalability

---

### main.py — Application entry point

This file assembles the FastAPI application.

- Creates the FastAPI app instance and defines API metadata
- Registers all routers (books, auth, rate-limit demos, brute-force demos)
- Configures logging
- Defines global lifecycle hooks (e.g. Redis cleanup on shutdown)
- Exposes the ASGI app for Uvicorn

No business logic or database code lives here — this file is only responsible for **wiring the application together**.

---

### models.py — Data definitions

This file defines **all data structures** used by the application.

- **SQLAlchemy ORM models** (`BookORM`, `UserORM`, `RefreshTokenORM`)  
  Describe how data is stored in PostgreSQL (tables, columns, relationships).

- **Pydantic models** (`Book`, `BookCreate`, `BookUpdate`, `Page`, etc.)  
  Define how data is validated, serialized, and returned through the API.

Key ideas:
- Clear separation between **database models** and **API schemas**
- No database access, no HTTP logic
- Pure, passive data definitions shared across the app

---

### deps.py — Authentication dependencies

This module defines **reusable FastAPI dependencies** for authentication and authorization.

- Extracts and validates JWT access tokens
- Loads the current user from the database
- Provides role-based guards (e.g. admin-only endpoints)

Purpose:
- Centralize security logic
- Keep route handlers clean and declarative
- Leverage FastAPI’s dependency injection system

---

### init_db.py — Database initialization

This script initializes the database schema.

- Creates all tables defined by SQLAlchemy ORM models
- Intended for local development and learning

Run once before using the API to prepare the database.

---

### db.py — Database infrastructure

This module defines how the application connects to PostgreSQL.

- Loads database configuration from environment variables
- Converts a DSN-style connection string into a SQLAlchemy async URL
- Creates the async SQLAlchemy engine and session factory
- Exposes `get_db()` as a FastAPI dependency for per-request DB sessions

This file is purely infrastructural: it knows nothing about routes, models, or business logic.

---

### cache.py — Redis caching

This module provides a small async Redis cache layer.

- Caches JSON responses (e.g. paginated `/books` results)
- Uses a single shared Redis client for the whole app
- Supports TTL-based caching
- Implements versioned cache keys, allowing global cache invalidation by incrementing one value

Used to reduce database load and demonstrate practical API-level caching patterns.

---

### rate_limit.py — API rate limiting

This module implements Redis-backed rate limiting for FastAPI endpoints.

- Limits requests to **N per time window**
- Uses Redis counters with TTL for efficient tracking
- Enforces limits via a reusable FastAPI dependency
- Supports different identities (IP, user, custom key)

Demonstrates a production-style approach to protecting APIs from abuse and excessive traffic.

---

### bruteforce.py — Brute-force protection

This module implements Redis-backed brute-force protection for authentication flows.

- Tracks failed login attempts per **IP**
- Locks further attempts after a configurable threshold
- Uses Redis TTLs to automatically release locks after a cooldown
- Clears all counters on successful authentication

Designed to demonstrate how real-world login protection is built outside of the main auth logic.

---

### security.py — Authentication & token security

This module contains all security-related primitives used by the FastAPI application.

- Password hashing and verification using **bcrypt**
- Creation and validation of **JWT access and refresh tokens**
- Token metadata (`exp`, `iat`, `jti`, `role`) for proper authorization and rotation
- Centralized JWT configuration loaded from environment variables

It cleanly separates **security concerns** (hashing, tokens, signing) from HTTP routes and business logic.

---

### routes/books.py — Books API (CRUD + filtering + sorting + pagination + caching)

This module is the **HTTP boundary** for everything related to `Book` resources. It defines the `/books` endpoints and translates an incoming request (query params + JSON body + auth) into safe SQLAlchemy queries and consistent JSON responses.

What it includes:

- **CRUD endpoints**
  - `GET /books` (list, paginated)
  - `GET /books/{id}` (detail)
  - `POST /books` (create, authenticated)
  - `PUT /books/{id}` (update, authenticated)
  - `DELETE /books/{id}` (delete, admin-only)

- **Filtering + pagination**
  - Query params like `title_contains`, `author`, `year_from`, `year_to`
  - `page` + `page_size` mapped to SQL `OFFSET/LIMIT`

- **Safe sorting (SQL injection awareness)**
  - Sorting is allowlisted (`id`, `title`, `author`, `year`)
  - Supports multi-sort like `sort_by=author:desc,title:asc`

- **Redis caching for list endpoint**
  - `GET /books` uses a **versioned cache key** so repeated requests can skip the database
  - On write operations (POST/PUT/DELETE) it calls `bump_cache_version()` to invalidate all list caches at once

In short: this file is where “API behavior” lives — validation via Pydantic schemas, authorization via dependencies, SQL queries via SQLAlchemy, and performance via Redis caching.

---

### routes/auth.py — Authentication & Authorization API

This module implements **user authentication and authorization** for the FastAPI application using **JWT + OAuth2** patterns.

What it provides:

- **User registration**
  - Creates new users
  - Hashes passwords securely (never stores plain text)
  - Enforces unique usernames

- **Login (OAuth2 password flow)**
  - Validates credentials
  - Issues a short-lived **access token** and a long-lived **refresh token**
  - Designed to work seamlessly with FastAPI’s Swagger UI

- **Refresh token rotation**
  - Exchanges a refresh token for a new access token
  - Stores refresh token identifiers (JTI) in the database
  - Revokes old refresh tokens to prevent reuse

- **User identity endpoint**
  - `/auth/me` returns the currently authenticated user
  - Demonstrates dependency-based authentication (`get_current_user`)

- **Role-based authorization**
  - Example admin-only endpoint using `require_role("admin")`
  - Shows how roles embedded in JWTs can control access

Overall, this file demonstrates a **production-style auth setup**:
secure password handling, JWT lifecycle management, token revocation, and clean separation between authentication logic and business endpoints.

---

### routes/rl_demo.py — Rate Limiting Demo

This module provides **simple demo endpoints** to experiment with and understand **API rate limiting** using Redis.

What it demonstrates:

- **IP-based rate limiting**
  - Limits how many requests a client can make within a time window
  - Returns HTTP `429 Too Many Requests` when exceeded

- **Reusable limiter dependency**
  - Uses a generic `RateLimit` configuration
  - Applied via FastAPI dependencies (`Depends(limiter(...))`)

- **Two demo scenarios**
  - `/rl-demo/ping` — relaxed limit to easily observe counters and resets
  - `/rl-demo/login-sim` — stricter limit mimicking a login endpoint

Purpose:

This file is a **safe playground** to spam endpoints, watch Redis behavior, and see how rate limiting integrates.

---

### routes/bf_demo.py — Brute-Force Protection Demo

This module provides a **safe, isolated playground** to experiment with **brute-force login protection** using Redis.

