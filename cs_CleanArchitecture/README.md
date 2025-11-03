# Clean Architecture WPF Example

This project is a WPF desktop app built to demonstrate the **Clean Architecture** approach in C#.  
It’s a small, working example showing how to structure an app so it stays maintainable, testable, and easy to extend.

---

## Purpose

The goal was to understand how Clean Architecture works in a real WPF project — not just theory.  
I wanted to clearly see where each type of code belongs and how each layer communicates.

---

## Architecture Overview

The solution is split into four projects:

```
CsClean.Domain/
CsClean.Application/
CsClean.Infrastructure/
CsClean.Presentation.Wpf/
```

Each layer has a specific job.

| Layer | Depends on | Description | Real-life example |
|:--|:--|:--|:--|
| **Domain** | — | The core business logic — just data, rules, and validation. | A plain Excel sheet structure or the “data model” of the app. |
| **Application** | Domain | Coordinates use cases, defines interfaces (ports). No UI or IO. | “What should happen” when user clicks Save/Delete. |
| **Infrastructure** | Application, Domain | Implements those interfaces — talks to real world (files, DB, APIs). | Database, Excel, file system, REST API. |
| **Presentation (WPF)** | Application, Domain | UI layer (Views + ViewModels). Only calls **use cases** from Application. | The WPF window the user actually interacts with. |

---

## How It Works (flow)

```
[User clicks button]
     ↓
[ViewModel Command]
     ↓
[Use Case (Application layer)]
     ↓
[Repository Interface (Port)]
     ↓
[InMemory Repository (Infrastructure)]
     ↓
[Domain Entities → back to UI]
```

### Example

1. In WPF, the user presses **“Add”**.  
2. `MainViewModel.AddCommand` creates a `LocalMessage` (Domain entity).  
3. It calls the **use case** `SaveLocalMessage.ExecuteAsync()`.  
4. That use case depends on `ILocalMessageRepository` (an interface).  
5. DI injects the **InMemory implementation**, which stores the message.  
6. `GetAllLocalMessages` reloads the list → UI updates automatically.

---
## Why It’s Done This Way

### Flexibility
If I want to replace the in-memory data with Excel or SQL,  
I only change one line in DI registration — the rest of the app stays the same.

```csharp
services.AddSingleton<ILocalMessageRepository, ExcelLocalMessageRepository>();
```

### Clear separation of responsibilities
- **Domain** → rules and data
- **Application** → what happens
- **Infrastructure** → how it happens
- **Presentation** → how it looks and interacts

### Easier testing
I can test all use cases without running the UI or connecting to a database.

### Scalable and maintainable
New features = new use cases.  
No spaghetti dependencies across UI, logic, and storage.

---

## Example Use Cases

| Use Case | Description |
|-----------|-------------|
| `GetAllLocalMessages` | Load all existing messages. |
| `SaveLocalMessage` | Add or update a message. |
| `DeleteLocalMessageByName` | Remove message by name. |

---
