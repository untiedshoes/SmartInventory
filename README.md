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
├── smart-inventory-frontend/ → React frontend
│  ├── src/
│  ├── api/
│  └── pages/
│
├── src/
│  ├── SmartInventory.Web → API Layer
│  │   ├── Controllers
│  │   ├── DI wiring
│  │   └── Middleware (Swagger, error handling)
│  │
│  ├── SmartInventory.Services → Application / Business Logic
│  │   ├── ProductService (EF Core)
│  │   └── FakeProductService (In-memory, DTOs)
│  │
│  ├── SmartInventory.Data → Persistence
│  │   ├── InventoryDbContext
│  │   └── Migrations
│  │
│  └── SmartInventory.Core → Domain Entities & Contracts
│      ├── Entities (Product, Category)
│      ├── Interfaces (IProductService)
│      └── DTOs
│
├── tests/
│   └── SmartInventory.Tests → Unit Tests (mirrors src structure)
│       ├── Services
│       └── Controllers
│
└── .github/workflows/
    └── ci.yml → CI pipeline (build backend, run tests, lint/build frontend)
```

### Layer Responsibilities

| Layer | Responsibility |
|-------|----------------|
| Core | Domain entities, DTOs, and interfaces (contracts) |
| Data | Persistence logic, EF Core DbContext, migrations |
| Services | Business rules and application logic; includes `ProductService` (EF Core) and `FakeProductService` (in-memory + DTOs for dev/frontend simulation) |
| Web | API layer: Controllers, DI wiring, middleware, Swagger; maps DTOs to/from entities; supports dev-mode DTO APIs via `FakeProductService` |
| Tests | Unit testing with isolated in-memory services and databases; validates CRUD, pagination, filtering, and top-N metrics |

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
- `FakeProductService` for testing and dev-mode API simulation:
  - Implements IProductService without EF Core
  - Provides deterministic in-memory products and categories
  - Supports CRUD, pagination, filtering, and top-N summaries
  - DTO-aware methods for frontend simulation:
    - `GetAllDtosAsync()`
    - `GetTopDtosAsync(int count)`
    - `GetPagedDtosAsync(page, pageSize, categoryId?, search?)`
    - `GetByIdDtoAsync(Guid id)`
- Paginated API endpoints support:
  - Pagination (`page`, `pageSize`)
  - Category filtering (`categoryId`)
  - Search by product name (`search`)
- Product and Category entities include `Description` field


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
  - Provides in-memory, seedable product and category collections
  - Supports all IProductService methods
  - Unit tests validate:
    - CRUD operations
    - Pagination and filtering
    - Top-N product summaries
  - **Rationale:** Enables deterministic unit tests, CI-friendly validation, and frontend development without a real database

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

## Development Guidelines (Adding New Endpoints)

## Adding New API Endpoints with DTOs

When introducing a new API endpoint (CRUD or query) for products, categories, or other entities, follow this checklist to ensure consistency, testability, and dev-mode support:

### 1. Define the DTOs
- Create `CreateXDto`, `UpdateXDto`, and `XDto` as needed.
- Include only the fields required by the frontend.
- Keep DTOs separate from entities to enforce encapsulation.

### 2. Update Services
- **Production service** (`ProductService` or relevant):
  - Implement the endpoint logic using EF Core.
  - Return entities or mapped DTOs if needed.
- **Dev-mode / Fake service** (`FakeProductService`):
  - Implement in-memory version returning DTOs for frontend simulation.
  - Use deterministic seeding for predictable results.
  - Include async methods like `GetAllDtosAsync`, `GetByIdDtoAsync`, `GetPagedDtosAsync`, `GetTopDtosAsync`.

### 3. Update Controllers
- Inject both production `IProductService` and optional `FakeProductService`.
- Use `_fakeService` in dev mode for DTO return values.
- Map entities to DTOs via AutoMapper in production mode.
- Handle null / empty states gracefully.

### 4. Add Unit Tests
- Add tests in `SmartInventory.Tests/Services`:
  - Verify **CRUD** operations on the fake service.
  - Test **pagination, filtering, and search**.
  - Test **Top-N or summary endpoints** if applicable.
- Use deterministic data (zero-padded names, seeded quantities) for predictable ordering.
- Test both normal and edge cases (e.g., missing entity, empty search results).

### 5. Frontend Considerations
- Update the API client (React/TypeScript) to call the new endpoint.
- Validate response DTOs match the frontend expectations.
- Handle empty states or errors safely.

### 6. Documentation
- Update `README.md`:
  - Include endpoint summary, parameters, and expected DTO.
  - Add tests or dev-mode notes if applicable.

### Quick Reference
| Step | Action |
|------|--------|
| 1 | Define DTOs |
| 2 | Implement in Production service |
| 3 | Implement in Fake service (dev-mode DTOs) |
| 4 | Add Controller endpoints (use dev-mode DTOs if `_fakeService != null`) |
| 5 | Add Unit Tests for all behaviors |
| 6 | Update Frontend and Documentation |

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