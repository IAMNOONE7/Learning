## Flask Basic — Project Overview

This project demonstrates how to build a **REST API using Flask** with PostgreSQL, focusing on understanding what Flask does *not* give you out of the box.

The goal is to learn:
- how routing, validation, and serialization are handled manually
- how Flask compares to FastAPI and Django Rest Framework
- how to design a clean API with minimal abstractions

### Key characteristics

- Flask is **unopinionated and lightweight**
- No automatic request validation 
- No built-in API docs (a simple custom UI is provided)
- Explicit control over:
  - request parsing
  - response formatting
  - error handling
  - database session usage

This part of the project highlights how Flask gives **maximum control**, at the cost of more boilerplate, making it ideal for learning how APIs work at a low level.

---

## schemas.py — Request Validation

This module defines **Pydantic schemas** used to validate incoming JSON data.

In Flask:
- Request bodies are **not validated automatically**
- JSON is parsed manually and passed to Pydantic models

### Purpose

- Enforce data shape and constraints (types, lengths, ranges)
- Separate validation logic from HTTP and database code
- Provide clear schemas for:
  - creating books (`BookCreate`)
  - partially updating books (`BookUpdate`)

This mirrors FastAPI’s validation system

---

## models.py — Database Models

This module defines **SQLAlchemy ORM models** for the Flask application.

### Purpose

- Describe the database table structure (`books`)
- Map Python objects to PostgreSQL rows
- Contain **no HTTP or request logic**

In Flask:
- Models are purely about persistence
- Serialization is manual (`to_dict()`), unlike FastAPI or DRF

This file represents the **data layer** of the application and is reused by all routes that interact with the database.

---

## routes/books.py — Books API Endpoints (Flask)

This module defines the **HTTP layer** for the Books API using a Flask **Blueprint**.

### Purpose

- Registers the `/books` routes (GET, POST, PUT, DELETE)
- Reads input from `request` (query params + JSON bodies)
- Validates JSON bodies manually using **Pydantic** (`BookCreate`, `BookUpdate`)
- Uses **Flask-SQLAlchemy** to query and mutate the `books` table
- Returns consistent JSON responses via `jsonify()`

### What it supports

- **List books** with:
  - pagination (`page`, `page_size`)
  - filtering (`title_contains`, `author`, `year_from`, `year_to`)
  - safe sorting (`sort_by`, `sort_dir`) using an allowlist to avoid SQL injection
- **Get by id**: `GET /books/<id>`
- **Create**: `POST /books`
- **Update** (partial fields allowed): `PUT /books/<id>`
- **Delete**: `DELETE /books/<id>`

### Key Flask takeaways

- No automatic request validation (unlike FastAPI) -> you validate manually
- No dependency injection system -> you directly use `db.session` and `request`
- You build error handling yourself (`_error()` helper)

---

## routes/ui.py — Minimal Testing UI (Flask)

This module provides a **very small in-browser UI** to manually test the Books API without external tools.

### Why this exists

- Flask has no built-in interactive docs (unlike FastAPI / Swagger)
- This UI acts as a lightweight, zero-dependency playground
- Makes it easy to visually inspect responses and HTTP status codes

### Key takeaway

This file is **not production UI** — it’s a learning and debugging tool.

