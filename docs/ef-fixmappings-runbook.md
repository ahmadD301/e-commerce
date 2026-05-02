# EF Core FixMappings runbook (what was done)

Date: 2026-05-02

## Goal
Before generating migrations, ensure the EF Core model is valid (design-time DbContext can be created). Then generate a migration (`FixMappings`), update the database, restart the API, and verify `POST /api/auth/login`.

## Commands run
> These are the exact commands used (PowerShell).

```powershell
# (Requested) Validate model
# NOTE: This command is not available in EF Tools 10.0.7 (see below)
dotnet ef dbcontext validate --project Ecommerce.Infrastructure --startup-project Ecommerce.Api

# Actual validation used (loads the model / creates DbContext at design-time)
dotnet ef dbcontext info --project Ecommerce.Infrastructure --startup-project Ecommerce.Api

# Generate migration
dotnet ef migrations add FixMappings --project Ecommerce.Infrastructure --startup-project Ecommerce.Api

# Apply migrations
dotnet ef database update --project Ecommerce.Infrastructure --startup-project Ecommerce.Api

# Run API
dotnet run --project Ecommerce.Api --launch-profile http

# Test login
Invoke-RestMethod -Method Post -Uri 'http://localhost:5120/api/auth/login' -ContentType 'application/json' -Body '{"email":"john.doe@email.com"}'
```

## What broke, and how it was fixed

### 1) `dotnet ef dbcontext validate` is not a valid command
- Symptom: `Unrecognized command or argument 'validate'`.
- Cause: EF Core tools 10.0.7 do not include `dbcontext validate`.
- Fix/Approach: Use `dotnet ef dbcontext info` as the practical “validation” step: if the model/mappings are broken, `dbcontext info` fails before any migration is generated.

### 2) Startup project missing EF design-time package
- Symptom (from `dbcontext info`):
  - `Your startup project 'Ecommerce.Api' doesn't reference Microsoft.EntityFrameworkCore.Design`.
- Fix:
  - Added `Microsoft.EntityFrameworkCore.Design` to the startup project so `dotnet ef ... --startup-project Ecommerce.Api` works.
  - File changed: `Ecommerce.Api/Ecommerce.Api.csproj`.

### 3) EF model error: `Product` constructor could not be bound
- Symptom (from `dbcontext info`):
  - `No suitable constructor was found for entity type 'Product'... Cannot bind 'price' in 'Product(ProductId id, string name, Money price, int stock)'`
- Cause:
  - EF Core cannot bind constructor parameters that correspond to an owned type (`Money price`).
- Fix:
  - Added an EF-only parameterless constructor and made `Id` settable for materialization.
  - File changed: `E-commerce.Core/Domain/Aggregates/Product.cs`.

### 4) Money `decimal` precision warnings
- Symptom: EF warnings about `Money.Amount` having no configured store type/precision and risking silent truncation.
- Fix:
  - Added `.HasPrecision(18, 2)` for each owned `Money.Amount` mapping.
  - File changed: `Ecommerce.Infrastructure/Data/AppDbContext.cs`.

### 5) Database update failed: missing connection string
- Symptom (from `dotnet ef database update`):
  - `The ConnectionString property has not been initialized.`
- Cause:
  - `builder.Configuration.GetConnectionString("DefaultConnection")` returned null because `ConnectionStrings:DefaultConnection` wasn’t defined.
- Fix:
  - Added `ConnectionStrings:DefaultConnection` to `Ecommerce.Api/appsettings.json` and `Ecommerce.Api/appsettings.Development.json`.

### 6) LocalDB not installed, switched to local SQL Server instance
- Symptom when using LocalDB connection string:
  - `Unable to locate a Local Database Runtime installation`.
- Fix:
  - Detected `MSSQLSERVER` service running and switched connection string to `Server=localhost;...;Trusted_Connection=True;TrustServerCertificate=True`.

### 7) Existing DB had tables but no migrations history
- Symptom (on `EcommerceDb`):
  - `There is already an object named 'Orders' in the database.`
- Cause:
  - The database already contained schema objects, but EF migration history didn’t match, so EF tried to create tables that already existed.
- Fix (non-destructive):
  - Pointed `DefaultConnection` to a fresh database name (`EcommerceDb_FixMappings`) to apply migrations cleanly.
  - This avoids dropping/altering an unknown existing DB.

### 8) Login endpoint returned 500 due to short JWT signing key
- Symptom (from `POST /api/auth/login`):
  - `IDX10720 ... key size must be greater than: '256' bits`
- Fix:
  - Set a dev-only JWT key >= 32 bytes in `Ecommerce.Api/appsettings.Development.json`.

## Extra: Make login test meaningful (seed sample data)
- Observation: `/api/auth/login` checks whether a customer exists by email; a fresh DB would otherwise return `401 Unauthorized`.
- Fix:
  - In Development only, run `dbContext.Database.MigrateAsync()` and seed sample customers/products **only when empty**.
  - File changed: `Ecommerce.Api/Program.cs`.

## Verification result
- `dotnet ef dbcontext info --project Ecommerce.Infrastructure --startup-project Ecommerce.Api` succeeded.
- Migration created: `FixMappings`.
- `dotnet ef database update` succeeded against `EcommerceDb_FixMappings`.
- `POST http://localhost:5120/api/auth/login` with `{ "email": "john.doe@email.com" }` returned `200 OK` with a JWT token.

## Files changed
- `Ecommerce.Api/Ecommerce.Api.csproj`
- `Ecommerce.Api/appsettings.json`
- `Ecommerce.Api/appsettings.Development.json`
- `Ecommerce.Api/Program.cs`
- `E-commerce.Core/Domain/Aggregates/Product.cs`
- `Ecommerce.Infrastructure/Data/AppDbContext.cs`

## Notes / follow-ups
- The NU190x vulnerability warnings are build-time advisories; they don’t block migrations, but should be addressed separately.
- If you *must* apply migrations to the existing `EcommerceDb`, we should first inspect its current schema + whether it was created by EF (presence/contents of `__EFMigrationsHistory`) and decide between:
  - mapping existing DB to EF migrations history (careful/manual), or
  - generating a baseline migration, or
  - a controlled rebuild.
