# Multi-Tenant SaaS Subscription & Billing API

A Clean Architecture ASP.NET Core Web API for managing multi-tenant SaaS subscriptions and billing — trial-to-paid lifecycle, usage-based proration, tenant-isolated data, and role-scoped access.

![CI](https://github.com/Othdu/SaaSBillingApi/actions/workflows/ci.yml/badge.svg)

## Overview

Built to demonstrate the parts of a real SaaS backend that a typical CRUD project doesn't touch: strict multi-tenant data isolation enforced at the database query level, a self-validating subscription state machine, prorated billing math as an isolated domain service, and a background job that keeps subscription state correct without any user interaction.

## Architecture

```
src/
  SaaSBillingApi.Domain          # Entities, state machine, proration logic — zero dependencies
  SaaSBillingApi.Application     # Interfaces, use cases, DTOs, validators — depends on Domain only
  SaaSBillingApi.Infrastructure  # EF Core, repositories, JWT, background jobs — depends on Application
  SaaSBillingApi.Api             # Controllers, middleware, DI wiring — depends on Infrastructure + Application
tests/
  SaaSBillingApi.UnitTests       # xUnit tests for Domain logic
```

Dependency direction flows inward: `Api → Infrastructure → Application → Domain`. Domain has no knowledge of EF Core, ASP.NET Core, or any external concern — it can be tested in complete isolation.

## Key Features

- **Multi-tenancy** enforced via EF Core global query filters — every `Subscription` query is automatically scoped to the authenticated tenant at the database level, not by convention.
- **Subscription state machine**: `Trial → Active → PastDue → Cancelled`, with every transition guarded inside the entity itself — invalid transitions throw a typed domain exception rather than silently succeeding.
- **Proration** as a pure, dependency-free domain service — calculates fair mid-cycle charges/credits on plan changes, fully unit-tested at both boundary and midpoint cases.
- **JWT authentication** with tenant-scoped claims and three roles: `SuperAdmin`, `TenantAdmin`, `TenantUser`.
- **FluentValidation** wired through a global action filter — every request DTO is validated automatically, no per-controller boilerplate.
- **Global exception-handling middleware** mapping domain and application exceptions to correct HTTP status codes (404, 409, 401, 400) instead of leaking stack traces.
- **Daily billing background job** (`IHostedService`) that scans subscriptions across *all* tenants, moving expired trials to `Cancelled` and lapsed active subscriptions to `PastDue`.
- **Serilog structured logging**, enriched per-request with the authenticated tenant's ID.
- **Repository + Unit of Work** pattern over EF Core.
- xUnit tests covering the subscription state machine and proration calculations.

## Tech Stack

- ASP.NET Core 9 Web API
- Entity Framework Core 9 + SQL Server
- JWT Bearer Authentication
- FluentValidation
- Serilog
- xUnit

## API Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | none | Authenticate, returns JWT |
| GET | `/api/plans` | none | List available pricing plans |
| POST | `/api/subscription/trial` | TenantAdmin | Start a trial subscription |
| POST | `/api/subscription/{id}/upgrade` | TenantAdmin | Change plan, returns prorated amount |
| POST | `/api/subscription/{id}/cancel` | TenantAdmin | Cancel a subscription |
| GET | `/api/health` | none | Liveness check |

## Setup

1. Clone the repo and open `SaaSBillingApi.slnx` in Visual Studio.
2. Update `src/SaaSBillingApi.Api/appsettings.json` with your local SQL Server connection string.
3. In Package Manager Console (default project: `SaaSBillingApi.Infrastructure`), run:
   ```
   Update-Database
   ```
4. Run the API (`SaaSBillingApi.Api` as startup project). A test tenant, admin user (`admin@acme.com` / `Password123!`), and two plans are seeded automatically on first run.
5. Import the Postman collection from `docs/` to test the full flow.

## Testing

```bash
dotnet test
```

## CI

Every push and PR to `main` runs a GitHub Actions workflow that restores, builds, and runs the full test suite.
