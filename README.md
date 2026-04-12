# E-commerce (C#)

A compact, console-based e-commerce reference application implemented in C# using .NET 8. It demonstrates small-scale clean architecture patterns, domain modeling, and an in-memory persistence approach suitable for learning and experimentation.

## Highlights

- Simple domain models: `Product`, `Customer`, `Order`, `Cart` and related value objects
- Application services and interfaces (e.g., `CheckoutService`, `IRepository`)
- In-memory repository implementation for fast iteration
- Seed data support for local demos
- Unit tests with xUnit in the `E-commerce.Test` project

## Repository Layout

- `E-commerce.Core/` — main application and domain (console app)
	- `Application/` — interfaces and services
	- `Domain/` — aggregates, entities, enums, value objects
	- `Infrastructure/Seed/` — data seeding helpers
- `E-commerce.Test/` — xUnit tests covering domain and application logic

## Project Structure

A concise, familiar view of the repository to help you find things quickly:

- `E-commerce.Core/` — console application containing the app code and domain
	- `Application/` — services and repository interfaces (e.g., `CheckoutService`, `IRepository`)
	- `Domain/` — aggregates, entities, enums, and value objects
	- `Infrastructure/` — seed data and small helpers
- `E-commerce.Test/` — xUnit tests (unit tests for domain and application logic)

If you want the full file tree back, I can re-insert the expanded view.

## Prerequisites

- .NET 8 SDK (install from https://dotnet.microsoft.com)

Verify your SDK with:

```powershell
dotnet --version
```

## Quick Start — Build and Run

Restore, build, and run the console app from the solution root:

```powershell
dotnet restore
dotnet build --configuration Debug
dotnet run --project E-commerce.Core
```

The app is a minimal console demo that exercises the domain and `CheckoutService` flow.

## Testing with xUnit

All unit tests live in the `E-commerce.Test` project. From the solution root you can run:

```powershell
dotnet test E-commerce.Test --configuration Debug
```

Common test commands:

- Run tests and build the test project:

```powershell
dotnet test E-commerce.Test
```

- Run a single test by name (example):

```powershell
dotnet test E-commerce.Test --filter FullyQualifiedName~CheckoutServiceTest
```

- Run tests without rebuilding (useful when you already built):

```powershell
dotnet test E-commerce.Test --no-build
```

Optional: generate coverage with Coverlet (MSBuild integration). From solution root:

```powershell
dotnet test E-commerce.Test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Contributing

- Run the test suite before submitting changes.
- Follow the existing project structure and naming conventions.

If you'd like help extending the project (new features, CI, or expanding tests), open an issue or create a PR.

## License

This repository is intended for learning. Add a license file if you plan to publish or share the code.

## Contact

For questions about the code, tests, or architecture, open an issue or message the repository maintainer.
