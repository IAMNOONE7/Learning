# Entity Framework Core Demo

> **Note to self:** Get back to this later — not enough code and full understanding yet.  

---

## Overview

This folder demonstrates both major approaches of **Entity Framework Core (EF Core)**:
1. **Code-First** — database is created from C# classes and migrations  
2. **Database-First** — C# classes and `DbContext` are generated from an existing database  

EF Core allows you to:
- Work with databases using **C# objects** instead of SQL directly  
- Automatically map classes - database tables  
- Perform CRUD operations (`Create`, `Read`, `Update`, `Delete`) using **LINQ**  
- Manage schema changes using **migrations**

---

## Project Structure

- `CodeFirstDemo/` - manually written C# classes and `DbContext`
- `DbFirstDemo/` - auto-generated from an existing SQLite database
- `Program.cs` - small demo performing insert, read, update, and delete operations