# SmartInventory API

> A production-oriented ASP.NET Core 9 Web API demonstrating clean architecture, EF Core with SQLite, automated testing, CI/CD, and modern C# 13 patterns. Includes a React/TypeScript frontend with dashboard, product listing, pagination, and filtering.

---

## Overview

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

## Architecture

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

## Design Principles

This project follows:

- SOLID principles
- Separation of Concerns
- Dependency Injection
- Test-driven friendly structure
- Explicit migration-based schema evolution
- Async-first design
- Frontend error handling and null-safe React Components

---

## Technology Stack

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

- Fully seeded database with **150 products across 15 categories**  
- `FakeProductService` updated with:
  - GetTopAsync(int count) for top product summary in dashboard
  - GetPagedAsync for pagination, search, and category filtering
- Products and Categories include a Description field
- Paginated API endpoints support:
  - Pagination (page, pageSize)
  - Category filtering (categoryId)
  - Search by product name (search)


## Frontend Features

- React + TypeScript frontend (`smart-inventory-frontend`)  
- Dashboard displays:
  - Top 5 products summary using backend GetTopAsync 
  - Inventory by category list
  - Full paginated **ProductsPage** table  
- ProductsPage supports:
  - Pagination controls  
  - Search by product name  
  - Category filtering 
- Frontend handles empty states and avoids runtime errors when API returns no data

## CI (Continuous Integration)

- GitHub Actions on Windows
- Backend: dotnet restore/build/test
- Frontend: npm install/lint/build
- Ensures code quality and full-stack compilation on each push/PR

---

## Database Strategy

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

## Testing Strategy

The project uses xUnit for unit tests, combined with EF Core InMemory provider to isolate tests from the production database. Service-level tests ensure that business logic works as expected, including creation, updating, deletion, pagination, and filtering.

The project uses:

- xUnit
- EF Core InMemory provider
- Service-level unit tests

Testing Strategy (Expanded)

- **CRUD Tests** (CreateAsync, GetByIdAsync, UpdateAsync, DeleteAsync)

  - Ensure products can be created, read, updated, and deleted correctly.

  - Includes edge cases such as:

  - Creating a product with negative quantity throws an exception.

  - Deleting a non-existent product throws KeyNotFoundException.

  - Rationale: Guarantees business logic integrity and prevents regression.

- GetAllAsync

  - Validates that all products are retrieved.

  - Uses AsNoTracking to ensure the query returns lightweight, detached entities.

  - Rationale: Confirms the service layer returns the correct set of products, independent of EF Core change tracking.

- **Pagination Tests** (GetPagedAsync)

  - Validates correct page size, total count, and filtered results by:

  - Page number and page size

  - Category filter

  - Search term filter

  - Combination of category + search term

  - **Zero-padding Note:** Product names in tests are zero-padded (Product01, Product02, …) to ensure deterministic lexicographical sorting.

  - Without padding, "Product10" would come before "Product2" when ordered by string.

  - This ensures pagination tests always return consistent results.

  - **Rationale:** Guarantees predictable pagination and filtering, which is critical for frontend UI components.

- **Top Products Test** (GetTopAsync)

  - Returns products with highest quantity for dashboard summaries.

  - Validates ordering and ensures the count matches request or total products if fewer.

  - **Rationale:** Ensures that summary metrics are accurate and correctly ordered.

- **FakeProductService Tests**

  - FakeProductService provides an in-memory, seedable collection without a database.

  - Supports all service methods (CreateAsync, GetByIdAsync, UpdateAsync, DeleteAsync, GetPagedAsync, GetTopAsync).

  - **Rationale:** Useful for quick unit testing, CI pipelines, or frontend simulations without needing a real database.

**Why Testing Matters in This Project**

  - **Deterministic Results:** Using InMemoryDatabase and zero-padded names ensures consistent order and avoids flaky tests.

  - **Isolation:** Each test runs in a fresh context; no test depends on another.

  - **Coverage:** Critical business logic, edge cases, pagination, search, and dashboard metrics are all validated.

  - **Confidence in CI/CD:** Automated tests run in GitHub Actions, preventing regressions before deployment.

Tests mirror the source structure:

```
tests/SmartInventory.Tests/Services/ProductServiceTests.cs
tests/SmartInventory.Tests/Services/FakeProductServiceTests.cs
```

Run all tests:

```bash
dotnet test
```

CI automatically runs tests on push.

---
---
## Continuous Integration

GitHub Actions pipeline:

- Restores dependencies
- Builds solution
- Runs all tests

See `.github/workflows/ci.yml` for details.

---

## Running the API

```bash
dotnet run --project src/SmartInventory.Web
```

Swagger UI (local): https://localhost:5001/swagger

---

## Modern C# Features Demonstrated

- File-scoped namespaces
- Nullable reference types
- Primary constructors
- Expression-bodied members
- Pattern matching improvements
- Async/await best practices

---

## Production-Readiness Foundations

Although currently development-focused, the architecture is designed for:

- Logging integration (Serilog planned)
- Structured error handling middleware
- DTO separation layer
- Validation pipeline (FluentValidation planned)
- Authentication & role-based authorization
- Containerization (Docker-ready)
- Swap-ready relational DB provider (SQL Server / PostgreSQL)

---

## Future Roadmap

Planned enhancements:

- Authentication (JWT + Roles)
- Global exception middleware
- Structured logging
- DTO + AutoMapper separation
- Pagination & filtering → (Done)
- Health checks
- OpenTelemetry tracing
- Docker containerization
- Frontend client (Blazor or React) → (Done - Chose React)
- Event-driven or microservices exploration

---

## Purpose

This project serves as:

- A real-world backend template
- A C# mastery progression platform
- A CI-integrated architecture reference
- A portfolio-ready demonstration of backend engineering capability

It is intentionally structured to scale into enterprise-grade systems.

---

## Engineering Philosophy

The goal is not just to “make it work.” The goal is to:

- Make it maintainable
- Make it testable
- Make it scalable
- Make it production-friendly
- Make it future-proof

---

## License

MIT

---

## Author

Craig Richards
Backend Developer | .NET Engineer