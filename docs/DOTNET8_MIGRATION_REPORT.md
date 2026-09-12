# AMLAKBASHI .NET 8 MIGRATION REPORT
## Phase 2: Modernization & Upgrade Preparation Strategy

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** Senior .NET Migration Architect
**Migration Target:** .NET 8.0 LTS (ASP.NET Core 8.0, EF Core 8.0)
**Status:** `PREPARED & CERTIFIED`

---

## 1. Executive Migration Objective

This document outlines the execution blueprint for migrating the recovered AmlakBashi V10 solution from `net5.0` to `.NET 8.0 LTS`. The strategy guarantees zero business logic drift, 100% preservation of Persian SEO URLs, absolute financial precision across all accounting tables, and zero disruption to the Contact Mode lead generation engine.

---

## 2. Project Target Framework Mapping

| Project Name | Current Target Framework | Target Framework | Key Dependencies Upgrade |
| :--- | :--- | :--- | :--- |
| `Amlakbashi.Core` | `net5.0` | `net8.0` | `AutoMapper` 12.x+, `Newtonsoft.Json` / `System.Text.Json` |
| `Amlakbashi.Data` | `net5.0` | `net8.0` | `Microsoft.EntityFrameworkCore.SqlServer` 8.0.x |
| `Amlakbashi.Accounting` | `net5.0` | `net8.0` | `RestSharp` 110.x, EF Core 8.0 |
| `Amlakbashi.Application` | `net5.0` | `net8.0` | `Hangfire.Core` 1.8.x |
| `Amlakbashi.Mediator` | `net5.0` | `net8.0` | `MediatR` 12.x |
| `Amlakbashi.Host` | `net5.0` | `net8.0` | ASP.NET Core 8.0, Serilog 3.x, HealthChecks |

---

## 3. Legacy Package Modernization Strategy

### 3.1 Replacing `log4net`
- **Current State:** `log4net` 2.0.12 injected directly into controllers (`ILog logger`) and services.
- **Modernization Plan:** Replace with `Microsoft.Extensions.Logging.ILogger<T>` and `Serilog.AspNetCore`.
- **Adapter / Compatibility Strategy:** Introduce a light `ILog` adapter wrapper during intermediate migration steps so existing `logger.Error(...)` calls remain operational without breaking changes.
- **Log Sink Configuration:** Structured JSON logging targeting console, rolling file logs, and OpenTelemetry / Elastic APM compatibility.

### 3.2 Replacing `X.PagedList`
- **Current State:** `X.PagedList` and `X.PagedList.Mvc.Core` used for pagination across search, admin lists, and user dashboards.
- **Modernization Plan:** Implement strongly typed `PagedList<T>` / `PaginatedList<T>` EF Core extensions utilizing native `IQueryable<T>.Skip()` and `TakeAsync()` calls.
- **Persist DTO Contract:** Preserve `IPagedList<T>` properties (`PageNumber`, `PageSize`, `TotalItemCount`, `PageCount`, `HasNextPage`, `HasPreviousPage`) so Razor views (`_PagedListPager`) continue to render accurately without UI breakage.

### 3.3 Dependency Upgrades & Vulnerability Remediation
- **AutoMapper:** Update mapping profiles to AutoMapper 12+, removing obsolete method calls.
- **RestSharp:** Update gateway clients (Saman, Pasargad, Podium, SMS providers) to RestSharp 110+ async syntax (`ExecuteAsync`).
- **ELMAH Legacy Replacement:** Transition from legacy ELMAH (`elmah.corelibrary`) to native ASP.NET Core `UseExceptionHandler`, `UseDeveloperExceptionPage`, and Serilog error logging.

---

## 4. Business & Financial Logic Preservation Rules

1. **Persian SEO Routing:**
   - All Persian route parameters, category URL encoders (`CategoryUrlLocalization`), and `[Route]` annotations in controllers must remain unchanged.
2. **Financial Precision:**
   - All decimal currency fields in `CreditTransaction`, `ReservePayment`, `Payment`, and `SiteClearingHostAutoPayment` must retain `decimal(18, 2)` or `decimal(18, 0)` SQL precision.
3. **Contact Mode Bypass:**
   - Online guest booking routes must remain blocked from public navigation; host contact reveal (`ShowMobile`) and `TrackLeadEvent` telemetry remain the primary public lead flow.

---

## 5. Verification & Rollback Strategy

- **Build Verification Gate:** `dotnet build Amlakbashi.sln` must compile with 0 errors.
- **Runtime Smoke Test:** Verify `/` homepage, `/شمال` regional search, `/accomodation/item/{id}` detail page, and `/advertise/trackleadevent` AJAX response.
- **Rollback Safety:** If EF Core 8 migration encounters runtime provider issues, baseline net5.0 assemblies remain accessible on git tag `v10-production-baseline`.
