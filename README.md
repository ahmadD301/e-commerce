# E-commerce API (Clean Architecture / DDD sample)

A small e-commerce REST API built with ASP.NET Core and EF Core, showcasing a DDD-style domain model and a clean, layered project structure.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-API-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-ORM-512BD4?logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?logo=jsonwebtokens&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?logo=swagger&logoColor=black)
![Docker](https://img.shields.io/badge/Docker-Optional-2496ED?logo=docker&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-Tests-5B2C83)

## About the project

This repository is a compact “portfolio-sized” e-commerce backend that demonstrates:

- **Clean Architecture-ish layering**: API, Infrastructure, Core (domain + application services), and Tests.
- **DDD building blocks**: aggregates (`Customer`, `Order`, `Payment`, `Product`), entities (`Cart`, `CartItem`, `OrderItem`), and value objects (`Money`, `Quantity`, typed IDs).
- **REST API**: CRUD-style endpoints for products/customers, order operations, and a simple login token endpoint.
- **EF Core + SQL Server**: code-first schema via migrations and owned-type mapping for value objects.

## Architecture overview

The solution is split into 4 projects:

- **E-commerce.Core**
	- Owns: domain model (aggregates/entities/value objects), application interfaces, and domain/application services (e.g., checkout flow).
- **Ecommerce.Infrastructure**
	- Owns: EF Core `AppDbContext`, SQL Server mapping configuration, and repository implementations.
- **Ecommerce.Api**
	- Owns: HTTP surface (controllers), authentication configuration (JWT), Swagger/OpenAPI, DI wiring, and dev-time seeding.
- **E-commerce.Test**
	- Owns: xUnit unit tests for domain + application behaviors.

## Features

- REST API for products, customers, orders, and checkout
- JWT token issuance endpoint (`/api/auth/login`)
- EF Core mappings for DDD value objects (owned types + typed ID conversions)
- SQL Server persistence
- Swagger UI in Development
- Unit tests with xUnit + Moq + AutoFixture
- Docker-friendly option (use Docker for SQL Server if you prefer)

## API endpoints

Base URL (default): `http://localhost:5120`

> Auth is configured for JWT Bearer. **Note:** the current controllers do not enforce authorization attributes (no `[Authorize]`), so endpoints are currently callable without a token.

| Method | Route | Description | Auth required |
|---|---|---|---|
| POST | `/api/auth/login` | Issue a JWT for an existing customer email | No |
| POST | `/api/checkout` | Checkout a customer’s cart (creates an order + payment flow via service) | No |
| GET | `/api/customers` | List customers | No |
| GET | `/api/customers/{id}` | Get customer by id | No |
| POST | `/api/customers` | Create a customer | No |
| GET | `/api/products` | List products | No |
| GET | `/api/products/{id}` | Get product by id | No |
| POST | `/api/products` | Create a product | No |
| PATCH | `/api/products/{id}/stock` | Increase/decrease stock by a signed quantity | No |
| PATCH | `/api/products/{id}/active` | Activate/deactivate a product | No |
| GET | `/api/orders` | List orders | No |
| GET | `/api/orders/{id}` | Get order by id | No |
| GET | `/api/orders/by-customer/{customerId}` | List orders for a customer | No |
| POST | `/api/orders/{id}/cancel` | Cancel an order (domain rule enforced) | No |

### Swagger

When running in Development, Swagger UI is available at:

- `http://localhost:5120/swagger`

## Getting started

### Prerequisites

- **.NET SDK**: this repo targets `net10.0`.
- **SQL Server**: a local SQL Server instance (or a Docker container).
- Optional: **EF Core CLI** (`dotnet-ef`) for migration commands.

Verify .NET:

```powershell
dotnet --version
```

### Clone

```powershell
git clone https://github.com/ahmadD301/e-commerce.git
cd e-commerce
```

### Run locally (dotnet)

```powershell
dotnet restore
dotnet run --project Ecommerce.Api --launch-profile http
```

The API listens on `http://localhost:5120`.

In Development, the API applies migrations and seeds sample data once (if the database is empty).

### Run with Docker (SQL Server in a container)

This repo includes a ready-to-run Docker Compose setup (SQL Server + API).

### Run with Docker Compose (recommended)

From the repository root:

```powershell
docker compose up --build
```

Then open:

- API: `http://localhost:5120`
- Swagger UI: `http://localhost:5120/swagger`

The compose stack starts:

- `sqlserver` (SQL Server 2022)
- `api` (ASP.NET Core)

The API is configured via environment variables in `docker-compose.yml`:

- `ConnectionStrings__DefaultConnection` points to the `sqlserver` container
- `Jwt__Key` is set to a dev-only 32+ byte secret (required for HS256)

### Docker notes

- The default SA password in `docker-compose.yml` is `Your_password123` (change it for anything beyond local demos).
- Database schema is applied at startup in Development (migrations + seed-on-empty).

### Run SQL Server only (optional)

If you prefer to run the API on your host machine but keep SQL Server in Docker:

```powershell
docker compose up sqlserver
dotnet run --project Ecommerce.Api --launch-profile http
```

## Manual Docker (without Compose)

If you don’t want Compose, you can still run SQL Server manually:

```powershell
docker run --name ecommerce-sql \
	-e "ACCEPT_EULA=Y" \
	-e "MSSQL_SA_PASSWORD=Your_password123" \
	-p 1433:1433 \
	-d mcr.microsoft.com/mssql/server:2022-latest
```

Then update `ConnectionStrings:DefaultConnection` in `Ecommerce.Api/appsettings.Development.json`:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=localhost,1433;Database=EcommerceDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;MultipleActiveResultSets=true"
	}
}
```

Run the API:

```powershell
dotnet run --project Ecommerce.Api --launch-profile http
```

## Configuration

Configuration lives in `Ecommerce.Api/appsettings.json` and `Ecommerce.Api/appsettings.Development.json`.

### ConnectionStrings

- `ConnectionStrings:DefaultConnection`
	- SQL Server connection string used by EF Core (`UseSqlServer`).

### Jwt

- `Jwt:Key`
	- Secret used to sign tokens (HMAC SHA-256). Must be **at least 32 bytes** for HS256.
- `Jwt:Issuer`
	- Token issuer.
- `Jwt:Audience`
	- Token audience.
- `Jwt:ExpiresMinutes` (optional)
	- Defaults to `60` if not provided.

## Database / EF Core

Create a migration:

```powershell
dotnet ef migrations add <Name> --project Ecommerce.Infrastructure --startup-project Ecommerce.Api
```

Apply migrations:

```powershell
dotnet ef database update --project Ecommerce.Infrastructure --startup-project Ecommerce.Api
```

## Running tests

Run all tests:

```powershell
dotnet test
```

Or run only the test project:

```powershell
dotnet test E-commerce.Test
```

## Project structure

```text
.
├─ Ecommerce.Api/
│  ├─ Controllers/
│  ├─ appsettings.json
│  ├─ appsettings.Development.json
│  └─ Program.cs
├─ Ecommerce.Infrastructure/
│  ├─ Data/
│  │  └─ AppDbContext.cs
│  └─ Repositories/
├─ E-commerce.Core/
│  ├─ Application/
│  ├─ Domain/
│  └─ Infrastructure/
├─ E-commerce.Test/
├─ docs/
└─ README.md
```


