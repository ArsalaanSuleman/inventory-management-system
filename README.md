<<<<<<< HEAD
# inventory-management-system
=======
# Inventory Management System (DDD + Clean Architecture)

A backend-focused Inventory & Order Management system built in **C# / .NET 9** to demonstrate
professional object-oriented design, Domain-Driven Design (DDD), and clean architecture principles.

## 🎯 Purpose
This project is intentionally designed as a **portfolio project** showcasing:
- Rich domain models
- Explicit invariants
- Domain events
- Clean separation of concerns
- Test-driven development
- Infrastructure ready for production patterns (Outbox, EF Core)

## 🧱 Architecture

## 🧠 Domain Model

### Core Aggregates
- **Order**
  - Owns `OrderLine`
  - Emits `OrderPlaced`
- **Warehouse**
  - Owns `StockItem`
  - Enforces stock invariants
- **Shipment**
  - Created only when stock is reserved
  - Emits `ShipmentCreated`

### Value Objects
- `Sku`
- `Quantity`

### Domain Events
- `OrderPlaced`
- `StockReserved`
- `ShipmentCreated`

All events are collected on aggregates and persisted via an **Outbox-ready design**.


## 🔒 Business Rules
- Stock cannot be reserved beyond available quantity
- Orders cannot be shipped unless fully reserved
- Aggregates protect their own invariants
- No anemic domain models


## 🧪 Testing
- Unit tests for domain behavior
- Use case tests with in-memory repositories
- No infrastructure dependencies in tests


## 🛠 Technology
- .NET 9
- C#
- EF Core (PostgreSQL-ready)
- xUnit
- Clean Architecture
- Domain-Driven Design (DDD)

## 🚀 Status
This project is complete and intentionally not deployed.
Its purpose is architectural demonstration rather than runtime hosting.

## 📌 Notes
Infrastructure is production-ready (EF Core mappings, Outbox pattern),
but database migrations and runtime configuration are intentionally omitted
to keep the repository focused and clean.

## 👤 Author
Built as a professional portfolio project.

>>>>>>> bc4a8c0 (Initial commit: Inventory Management System (DDD + Clean Architecture))
