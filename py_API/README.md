# Python API Learning Project

## Overview

This is a **learning and practice project** focused on building modern APIs in Python.

The main goal of the project is to understand **how APIs are designed, structured, secured, and optimized** using the three most common Python API frameworks:

- **FastAPI**
- **Flask**
- **Django Rest Framework (DRF)**

Each framework is implemented **independently** so their design philosophies and developer experience can be clearly compared.

---

## Purpose of This Project

This project exists to:

- Practice **real-world API patterns**
- Understand how the **same backend problems** are solved differently across frameworks
- Build a long-term reference for:
  - future projects  
  - architectural decision-making

All code is **well commented**, primarily for future reference, but also so others can easily follow the reasoning behind each implementation.

---

## What Is Practiced in This Repository

### Core API Concepts
- RESTful API design
- CRUD operations
- Proper HTTP status codes
- Consistent JSON response structures

### Database & ORM
- PostgreSQL as the main database
- SQLAlchemy (async usage)
- ORM modeling and query construction
- Understanding how SQL is generated
- SQL injection prevention through safe query building

### Authentication & Security
- User authentication
- Password hashing
- JWT access tokens
- Refresh tokens
- OAuth2-style login flows
- Role-based authorization

### Performance & Scalability
- Pagination
- Filtering
- Sorting with allowlisted fields
- Redis-based caching
- Cache invalidation strategies

### API Protection
- Rate limiting (Redis-backed)
- Per-IP and per-user limits
- Brute-force protection for login attempts
- Lockout windows and retry handling
- Proper use of HTTP `429 Too Many Requests`

### Framework-Level Differences
- Dependency Injection (FastAPI)
- Explicit request handling (Flask)
- ViewSets and serializers (Django Rest Framework)
- Automatic vs manual validation
- Framework-provided abstractions vs explicit control

---

## Learning Philosophy

This repository is intentionally:

- **Explicit** instead of magical
- **Verbose** instead of clever
- **Educational** instead of minimal

In many places, code is written in a more verbose way **on purpose** to make:
- control flow visible
- security decisions explicit
- architectural trade-offs clear

---

## Status

This is an **active learning repository**.

The goal is not perfection, but continuous improvement and deeper understanding of Python backend development.

Each framework has its own dedicated README with deeper explanations and framework-specific details.

---

## Final Note

This project represents a hands-on approach to learning:

- backend API development
- security-aware design
- scalable and maintainable architectures

If some parts feel “over-explained”, that is intentional — this repository is meant to be revisited and learned from over time.
