# Multi-Tenant SaaS Subscription & Billing API

A Clean Architecture ASP.NET Core Web API for managing multi-tenant SaaS subscriptions and billing — trial-to-paid lifecycle, usage-based proration, and role-scoped access per tenant.

![CI](https://github.com/Othdu/SaaSBillingApi/actions/workflows/ci.yml/badge.svg)

## Status

🚧 In progress — built incrementally, one feature per commit.

## Architecture

```
src/
  SaaSBillingApi.Domain          # Entities, state machine, proration logic — zero dependencies
  SaaSBillingApi.Application     # Interfaces, use cases, DTOs — depends on Domain only
  SaaSBillingApi.Infrastructure  # EF Core, repositories, external services — depends on Application
  SaaSBillingApi.Api             # Controllers, middleware, DI wiring — depends on Infrastructure + Application
tests/
  SaaSBillingApi.UnitTests       # xUnit tests for Domain + Application logic
```

Dependency direction flows inward: `Api → Infrastructure → Application → Domain`. Domain has no knowledge of EF Core, ASP.NET Core, or any external concern.

## Key Features

- **Multi-tenancy** via EF Core global query filters — every query is automatically scoped to the authenticated tenant.
- **Subscription state machine**: `Trial → Active → PastDue → Cancelled`, enforced in the Domain layer.
- **Proration** as a pure Domain service — no EF Core or infrastructure dependency, fully unit-testable.
- **Repository + Unit of Work** pattern over EF Core.
- **JWT authentication** with `SuperAdmin` / `TenantAdmin` / `TenantUser` roles.
- **Daily billing background job** via `IHostedService`.
- **Structured logging** with Serilog, tagged with tenant context.
- **xUnit tests** targeting the state machine and proration logic.

## Tech Stack

- ASP.NET Core 9 Web API
- Entity Framework Core 9 + SQL Server
- JWT Bearer Authentication
- Serilog
- xUnit

## Setup

_(to be filled in once the API is runnable — connection strings, migrations, running locally)_

## API Documentation

_(Swagger link / example requests — to be added)_

## Testing

```bash
dotnet test
```

## CI

Every push and PR to `main` runs a GitHub Actions workflow that restores, builds, and runs the full test suite.
