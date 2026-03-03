# 🚀 SmartInventory API

> A production-oriented ASP.NET Core 9 Web API demonstrating clean architecture, EF Core with SQLite, automated testing, CI/CD, and modern C# 13 patterns. Includes a React/TypeScript frontend with dashboard, product listing, pagination, and filtering.

---

## 📌 Overview

**SmartInventory** is a layered, production-ready backend API for managing products and categories within an inventory system.

This project is intentionally structured as a real-world engineering foundation, focusing on:

- Clean separation of concerns
- Scalable architecture
- Testability
- Modern C# language features
- Continuous Integration
- Migration-driven database management

It is designed both as a learning vehicle and as a demonstration of production-level backend engineering practices.

---

## 🧱 Architecture

The solution follows a Clean Layered Architecture approach.

```
SmartInventory/
│
├── smart-inventory-frontend/ → React
│  ├── src/
│  ├── api
│  ├── pages
│
├── src/
│  ├── SmartInventory.Web → API Layer (Controllers, DI, Middleware)
│  ├── SmartInventory.Services → Application / Business Logic
│  ├── SmartInventory.Data → EF Core, DbContext, Migrations
│  └── SmartInventory.Core → Domain Entities & Contracts
│
├── tests/
│  └── SmartInventory.Tests → Unit Tests (mirrors src structure)
│
└── .github/workflows/
    └── ci.yml → CI pipeline (Windows: build backend + tests, build/lint frontend)
```

### Layer Responsibilities

| Layer | Responsibility |
|--------|----------------|
| Core | Domain entities and abstractions |
| Data | Persistence logic, EF Core configuration |
| Services | Business rules and application logic |
| Web | HTTP layer, DI wiring, middleware, Swagger |
| Tests | Unit testing with isolated in-memory database |

---

## 🏗 Design Principles

This project follows:

- SOLID principles
- Separation of Concerns
- Dependency Injection
- Test-driven friendly structure
- Explicit migration-based schema evolution
- Async-first design

---

## 🛠 Technology Stack

| Technology | Purpose |
|------------|----------|
| .NET 9 | Runtime & SDK |
| C# 13 | Modern language features |
| ASP.NET Core | Web API framework |
| EF Core | ORM |
| SQLite | Lightweight relational DB |
| xUnit | Testing framework |
| Moq | Mocking framework |
| GitHub Actions | CI pipeline |
| Swagger (Swashbuckle) | API documentation |
| React + TypeScript | Frontend dashboard |

## Backend Features

- Fully seeded database with **150 products across 15 categories**  
- `FakeProductService` for testing without database  
- `GetPagedAsync` API supports:
  - Pagination (`page`, `pageSize`)  
  - Category filtering (`categoryId`)  
  - Search by product name (`search`)  
- Products and Categories include a `Description` field  

## Frontend Features

- React + TypeScript frontend (`smart-inventory-frontend`)  
- Dashboard displays:
  - Quick summary of first 5 products  
  - Full paginated **ProductsPage** table  
- ProductsPage supports:
  - Pagination controls  
  - Search by product name  
  - Category filtering  

  ## CI (Continuous Integration)

- GitHub Actions on Windows
- Backend: dotnet restore/build/test
- Frontend: npm install/lint/build
- Ensures code quality and full-stack compilation on each push/PR

---

## 🗄 Database Strategy

Development uses SQLite for simplicity and portability.

### Connection String

```json
{
  "ConnectionStrings": {
    "InventoryDb": "Data Source=SmartInventory.db"
  }
}
```

### Migration-Driven Schema

Schema changes are versioned using EF Core migrations:

```bash
dotnet ef migrations add InitialCreate \
  --project src/SmartInventory.Data \
  --startup-project src/SmartInventory.Web

dotnet ef database update \
  --project src/SmartInventory.Data \
  --startup-project src/SmartInventory.Web
```

This ensures:

- Reproducible database state
- Version-controlled schema
- Production-ready migration workflow

---

## 🧪 Testing Strategy

The project uses:

- xUnit
- EF Core InMemory provider
- Service-level unit tests

Tests mirror the source structure:

```
tests/SmartInventory.Tests/Services/ProductServiceTests.cs
```

Run all tests:

```bash
dotnet test
```

CI automatically runs tests on push.

---
---
## 🔄 Continuous Integration

GitHub Actions pipeline:

- Restores dependencies
- Builds solution
- Runs all tests

See `.github/workflows/ci.yml` for details.

---

## 📡 Running the API

```bash
dotnet run --project src/SmartInventory.Web
```

Swagger UI (local): https://localhost:5001/swagger

---

## 💡 Modern C# Features Demonstrated

- File-scoped namespaces
- Nullable reference types
- Primary constructors
- Expression-bodied members
- Pattern matching improvements
- Async/await best practices

---

## 🔒 Production-Readiness Foundations

Although currently development-focused, the architecture is designed for:

- Logging integration (Serilog planned)
- Structured error handling middleware
- DTO separation layer
- Validation pipeline (FluentValidation planned)
- Authentication & role-based authorization
- Containerization (Docker-ready)
- Swap-ready relational DB provider (SQL Server / PostgreSQL)

---

## 📈 Future Roadmap

Planned enhancements:

- Authentication (JWT + Roles)
- Global exception middleware
- Structured logging
- DTO + AutoMapper separation
- Pagination & filtering
- Health checks
- OpenTelemetry tracing
- Docker containerization
- Frontend client (Blazor or React)
- Event-driven or microservices exploration

---

## 🎯 Purpose

This project serves as:

- A real-world backend template
- A C# mastery progression platform
- A CI-integrated architecture reference
- A portfolio-ready demonstration of backend engineering capability

It is intentionally structured to scale into enterprise-grade systems.

---

## 🧠 Engineering Philosophy

The goal is not just to “make it work.” The goal is to:

- Make it maintainable
- Make it testable
- Make it scalable
- Make it production-friendly
- Make it future-proof

---

## 📄 License

MIT

---

## 👨‍💻 Author

Craig Richards
Backend Developer | .NET Engineer