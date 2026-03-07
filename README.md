# E-commerce (C#)

A simple console-based e-commerce project built with C# and .NET 8.

## Features

- Product and customer domain models
- Cart and order management
- Checkout service flow
- In-memory repository abstraction
- Data seeding support

## Project Structure

- `Application/` - DTOs, interfaces, services, validators
- `Domain/` - aggregates, entities, enums, value objects
- `Infrastructure/` - persistence, payments, seed data
- `Program.cs` - application entry point
- `E-commerce.sln` - solution file

## Requirements

- .NET 8 SDK

## Run the Project

```bash
dotnet restore
dotnet build
dotnet run
```

## Notes

This project is intended for learning and practicing clean architecture concepts in a small e-commerce domain.
