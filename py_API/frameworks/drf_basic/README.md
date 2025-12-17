# drf_basic — Django Rest Framework API

## Overview

This project demonstrates how to build a REST API using **Django Rest Framework (DRF)** backed by **PostgreSQL**, and how this approach compares to **FastAPI** and **Flask**.

The goal is to understand Django’s philosophy and see how much functionality DRF provides with minimal code and strong conventions.

---

## What This Project Focuses On

- Clean data modeling with Django ORM  
- Automatic CRUD API generation  
- Built-in filtering, searching, and ordering  
- Minimal configuration for common API patterns  
- Understanding high-level abstractions vs manual control  

---

## What DRF Gives You Out of the Box

- Full CRUD endpoints (list, detail, create, update, delete)
- Filtering, searching, and ordering via configuration
- Consistent API patterns with very little boilerplate
- Integrated admin and debugging tools

---

## Key Takeaways

- DRF favors productivity and convention over low-level control.
- Many features that are manual in Flask or explicit in FastAPI are declarative in DRF.

---

## Purpose of This Project

This project exists to:
- Learn Django Rest Framework in isolation
- Compare it conceptually with FastAPI and Flask
- Understand trade-offs between explicit control and built-in abstractions

The goal is learning and comparison, not choosing a “best” framework.

---

## views.py — API behavior

This file defines the **HTTP behavior** of the Books API using Django Rest Framework.

- Connects HTTP requests to database operations via a `ModelViewSet`
- Automatically provides full CRUD endpoints
- Enables filtering, searching, and ordering through configuration
- Relies on serializers for validation and data transformation

The key idea is that **behavior is declared, not coded** — DRF handles the heavy lifting based on metadata and conventions.

---

## serializers.py — Data validation & transformation

This file defines how **Book data is converted between JSON and database objects**.
In DRF, serializers replace the role of Pydantic schemas and form the bridge between HTTP and the database.

---

## models.py — Database schema & ORM mapping

This file defines the **database structure and ORM mapping** for the application.

- Describes what data exists and how it is stored
- Maps Python objects to PostgreSQL tables
- Contains no HTTP or API logic
- Serves as the foundation for serializers and views

In Django, models combine **schema definition and ORM behavior** in one place, unlike FastAPI where these concerns are usually split.

