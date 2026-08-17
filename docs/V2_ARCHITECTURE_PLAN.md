# AMLAKBASHI V2 ARCHITECTURE PLAN
## Phase 5: Clean Architecture Foundation & Service Separation

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** Senior .NET Migration Architect
**Architecture Model:** Clean Architecture / Domain-Driven Design (DDD)
**Status:** `DESIGNED & PREPARED`

---

## 1. Architectural Vision & Layer Boundaries

The AmlakBashi V2 architecture transitions the recovered monolith into a highly scalable, decoupled, clean architecture solution while strictly maintaining 100% backward compatibility with all Persian SEO routes, existing database schemas, and direct host contact business logic.

```
┌─────────────────────────────────────────────────────────────┐
│                    Amlakbashi.Host (API)                    │
│   ASP.NET Core Controllers / Razor Views / OpenAPI Gateway   │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                 Amlakbashi.Mediator / CQRS                  │
│       Command & Query Handlers / Pipeline Behaviors         │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                   Amlakbashi.Application                    │
│     Application Services / DTOs / Interfaces / Handlers     │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────┬───────────────────┐
│             Amlakbashi.Data             │Amlakbashi.Accounting│
│  EF Core DbContext / Repositories / Db  │ Accounting Facade │
└──────────────────────────────┬──────────┴───────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                    Amlakbashi.Core                          │
│     Domain Entities / Aggregates / Interfaces / Value Objects│
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Layer Responsibilities & API Boundaries

### 2.1 Domain Layer (`Amlakbashi.Core`)
- **Responsibilities:** Pure domain entities (`Advertise`, `User`, `LeadEvent`, `CreditTransaction`, `Region`), domain events, value objects, and repository interfaces.
- **Rules:** Zero external dependencies on ASP.NET Core, EF Core, or web libraries.

### 2.2 Persistence & Accounting Layers (`Amlakbashi.Data` & `Amlakbashi.Accounting`)
- **Responsibilities:** EF Core `AmlakbashiDB` context, SQL Server mapping configurations, database migrations (`20250520000000_add-lead-events-table.cs`), and financial facade (`IAccountingFacade`).
- **Safety Boundary:** Direct mapping to existing tables (`[WalletTransactions]`, `[Advertises]`, `[Users]`) with zero schema destruction.

### 2.3 Application & Mediator Layer (`Amlakbashi.Application` & `Amlakbashi.Mediator`)
- **Responsibilities:** CQRS commands and queries, business service interfaces (`IAdvertiseAppService`, `IUserAppService`), lead tracking orchestration, and background job handlers (`Hangfire`).

### 2.4 Presentation Layer (`Amlakbashi.Host`)
- **Responsibilities:** Web endpoints, REST APIs, Persian Razor views, static assets (`wwwroot/v10-app.js`), and auth middleware.

---

## 3. SEO Route & Persian Slug Preservation

- **URL Preservation Guarantee:** All Persian URLs (e.g. `/اجاره-ویلا-رامسر`, `/اجاره-سوئیت-تهران`) are decoded and dispatched via `AdvertiseController.AdvertisePage` and `CategoryUrlLocalization` without breaking legacy indexed pages.
- **Metadata Consistency:** Canonical links, OpenGraph tags, and Schema.org JSON-LD microdata generated dynamically for every accommodation item.
