# .NET Core Fundamentals Demo

This project demonstrates the core architecture principles of modern .NET using a simple Console Application.

Even though it runs from the command line, it uses the same hosting model, dependency injection, configuration, and logging system as ASP.NET Core and Worker Services — showing what powers every modern .NET application under the hood.

---

## What This Project Demonstrates

-  **HostBuilder** — the central object that manages configuration, dependency injection (DI), and logging  
-  **Dependency Injection (DI)** — registering and resolving services instead of creating them manually  
-  **Configuration System** — loading structured data from `appsettings.json`  
-  **Logging** — using `ILogger<T>` to produce structured log output  

This is essentially a **console-based version of an ASP.NET Core app**, meant for understanding how .NET applications are built internally.

---

##  Understanding .NET, .NET Framework, and .NET Core

###  .NET Framework
- The original .NET platform 
- Works **only on Windows**
- Used for classic **WPF**, **WinForms**
- Heavy, tightly integrated with Windows
- Still supported for legacy systems, but **no longer actively developed**

### .NET Core
- A complete **rebuild** of .NET
- **Cross-platform** — runs on Windows, Linux, and macOS
- Open-source and modular
- Introduced modern concepts:
  - Built-in **Dependency Injection**
  - Unified **Configuration** and **Logging**
  - The **HostBuilder** pattern
- The foundation of everything that came after (.NET 5+)

###  Modern .NET (5, 6, 7, 8, 9...)
- Microsoft **merged .NET Framework and .NET Core** into one platform
- Officially just called **“.NET”**
- Continues from **.NET Core**, not from .NET Framework
- Cross-platform, cloud-native, open source, and highly performant
- The same runtime and libraries are used by:
  - Console apps  
  - Web APIs (ASP.NET Core)  
  - Worker Services  
  - MAUI apps  
  - Blazor and more  

---

##  Why This App Is “.NET Core Style”

Although it’s a console app, it follows the **.NET Core architecture pattern**:

| Feature | Description |
|----------|--------------|
| **HostBuilder** | Creates and configures the runtime environment for the app |
| **Dependency Injection** | All services (`OrderProcessor`, `DateTimeProvider`) are registered and injected automatically |
| **Configuration** | Settings are stored in `appsettings.json` and mapped to a C# class |
| **Logging** | Uses the built-in logging system (same as ASP.NET Core) |
| **Cross-Platform** | Runs on Windows, Linux, and macOS without code changes |

Essentially, this app behaves like a **Worker Service** — just without background threads or hosted tasks.

