# cs_RestApiDemo

A simple **ASP.NET Core Web API** project demonstrating how to build a REST API in **C#**.  
The example simulates managing industrial devices (like PLCs, HMIs, and drives).

---

## Overview

This project shows how to:
- Build a REST API using **ASP.NET Core** and **Controllers**
- Define routes and handle HTTP methods (`GET`, `POST`, `PUT`, `DELETE`)
- Work with in-memory data (no database required)
- Use **Swagger (OpenAPI)** for testing and documentation

---

## How It Works

- The API exposes endpoints under `/api/devices`
- Uses **in-memory List<Device>** as a fake database
- Each endpoint corresponds to an HTTP verb:
  | Method | Route | Description |
  |---------|-------|--------------|
  | `GET` | `/api/devices` | Get all devices |
  | `GET` | `/api/devices/{id}` | Get a device by ID |
  | `POST` | `/api/devices` | Create a new device |
  | `PUT` | `/api/devices/{id}` | Update existing device |
  | `DELETE` | `/api/devices/{id}` | Delete a device |

Example JSON for creating a new device:
```json
{
  "name": "Main PLC",
  "ipAddress": "192.168.0.10",
  "location": "Line 1",
  "isOnline": true
}

