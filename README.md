# AmlakBashi — Recovered Core Engine & AI Operating System (V10.0)

Welcome to the unified core codebase of **AmlakBashi**, a premier localized real estate and accommodation marketplace with over 12 years of operational heritage.

This repository represents the fully audited and clean recovered core solution compiled for **.NET 5.0** alongside the blueprint and database migrations for the **AmlakBashi AI Operating System & Stable Session Persistence Platform (Version 10.0)**.

---

## 1. PROJECT STRUCTURE & RECOVERY STATUS

Due to historical hosting changes and system recovery steps, the application's core logic has been fully decompiled, resolved, and compiled into the following high-performance assemblies:

*   **`Amlakbashi.Core.dll`**: Domain model definitions, entities (such as `Advertise`, `Residence`, `User`, `Images`, `Reviews`), and system configurations.
*   **`Amlakbashi.Data.dll`**: Entity Framework Core DbContext (`AmlakbashiDB`, `IdentityDB`), schema definitions, mapping configurations, and structural migrations.
*   **`Amlakbashi.Mediator.dll`**: Clean architecture Request-Response dispatcher and Command-Query Responsibility Segregation (CQRS) handlers.
*   **`Amlakbashi.Application.dll`**: Core business services, helper libraries, financial integration, SMS/Kavenegar connectors, and background schedulers.
*   **`Amlakbashi.Accounting.dll`**: Wallet transactional processes, secure ledger balances, and payment processing nodes.
*   **`Amlakbashi.Host.dll` & `Amlakbashi.Host.Views.dll`**: Dynamic MVC web application, controller API endpoints (`UserController`, `AdvertiseController`, etc.), Razor Views, and startup pipeline.

---

## 2. PRODUCTION DOCUMENTATION, SCHEMAS & MIGRATIONS (V10.0)

All validation findings, security metrics, SRE configurations, session persistence blueprints, and migration scripts are thoroughly documented in:

*   **[`docs/AMLAKBASHI_V10_MASTER_BLUEPRINT.md`](docs/AMLAKBASHI_V10_MASTER_BLUEPRINT.md)**: Master architectural design, operational statuses, and security boundaries of Version 10.0.
*   **[`docs/V10_Enterprise_Platform_Migration.sql`](docs/V10_Enterprise_Platform_Migration.sql)**: Production-ready T-SQL migration script to set up all 18 AI agents, GSC/GA4 cache tables, and session stability schemas (`DataProtectionKeys` and `UserRefreshTokens`).
*   **[`docs/AI_AUTONOMOUS_INTELLIGENCE_PLATFORM.md`](docs/AI_AUTONOMOUS_INTELLIGENCE_PLATFORM.md)**: Core specifications and stubs for AI agents and Google API connectors.
*   **[`docs/FINAL_REPOSITORY_CLEANUP_REPORT.md`](docs/FINAL_REPOSITORY_CLEANUP_REPORT.md)**: Details on C# file cleanup, repository size analysis, and key security hygiene boundaries.
*   **[`docs/REPOSITORY_SECURITY_AUDIT.md`](docs/REPOSITORY_SECURITY_AUDIT.md)**: Proof of deactivation/revocation of legacy service account keys and connection safety.
*   **[`RECOVERY_VALIDATION_REPORT.md`](RECOVERY_VALIDATION_REPORT.md)**: Solution acceptance, and build parameters summary.
*   **[`LIVE_RUNTIME_VALIDATION_REPORT.md`](LIVE_RUNTIME_VALIDATION_REPORT.md)**: Dynamic database restoration steps, runtime hosting constraints, and configuration audits.

---

## 3. USER SESSION & AUTHENTICATION STABILITY (NO FORCED LOGOUT)

Under Version 10.0 specifications, users are guaranteed a stable, persistent session that persists across application updates, server restarts, and IIS AppPool recycles.
1.  **Data Protection Keys:** Encrypted and persisted directly in the SQL database (`DataProtectionKeys` table), bypassing random in-memory regeneration.
2.  **JWT Refresh Token Rotation:** Managed in SQL storage (`UserRefreshTokens` table), dynamically exchanging expired tokens without requiring user re-authentication.

---

## 4. ENVIRONMENTAL STABILIZATION FIXES

To execute or analyze this solution inside Linux/DevBox environments without encountering path violations, we have established the following baseline parameters:

1.  **ASPNETCORE_ENVIRONMENT**: Set to `Development`.
2.  **Absolute Path Provider Fix**: Under standard production profiles, the video media provider defaulted to drive letters (`E:/videos`). We created directory `/app/wwwroot/content/videos` to satisfy absolute path provider requirements on Unix systems.
3.  **Connection Strings**: Registered valid localhost T-SQL connectivity parameters inside `appsettings.json` targeting default SQL Server instances.

---

## 5. DEPLOYMENT & OPERATION

Once migrated to a live production host (IIS/Kestrel with T-SQL SQL Server engine):
1.  Restore `amlakbas_db.bak` database backup.
2.  Execute T-SQL migrations from **[`docs/V10_Enterprise_Platform_Migration.sql`](docs/V10_Enterprise_Platform_Migration.sql)**.
3.  Deploy the hosting bundle and run:
    ```bash
    dotnet Amlakbashi.Host.dll
    ```
4.  The system-wide operational and AI platform layers will automatically initialize and protect your workspace.
