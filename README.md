# AmlakBashi — Recovered Core Engine & AI Intelligence Platform (V9.0)

Welcome to the unified core codebase of **AmlakBashi**, a premier localized real estate and accommodation marketplace with over 12 years of operational heritage.

This repository represents the fully audited and clean recovered core solution compiled for **.NET 5.0** alongside the blueprint for the **AmlakBashi AI Autonomous Intelligence Platform (Version 9.0)**.

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

## 2. PRODUCTION DOCUMENTATION & AUDITS

All validation findings, security metrics, SRE configurations, and implementation parameters are thoroughly documented in:

*   **[`docs/AMLAKBASHI_V9_0_COMPLETION_AUDIT_REPORT.md`](docs/AMLAKBASHI_V9_0_COMPLETION_AUDIT_REPORT.md)**: Exhaustive implementation status report and detailed verification metrics.
*   **[`docs/AI_AUTONOMOUS_INTELLIGENCE_PLATFORM.md`](docs/AI_AUTONOMOUS_INTELLIGENCE_PLATFORM.md)**: Core specifications and stubs for all 18 AI agents, Google API connectors, and HITL governance.
*   **[`docs/V9.0_AI_Platform_Migration.sql`](docs/V9.0_AI_Platform_Migration.sql)**: Complete, ready-to-run T-SQL database migration script.
*   **[`docs/FINAL_REPOSITORY_CLEANUP_REPORT.md`](docs/FINAL_REPOSITORY_CLEANUP_REPORT.md)**: Details on C# file cleanup, repository size analysis, and key security hygiene boundaries.
*   **[`docs/REPOSITORY_SECURITY_AUDIT.md`](docs/REPOSITORY_SECURITY_AUDIT.md)**: Proof of deactivation/revocation of legacy service account keys and connection safety.
*   **[`RECOVERY_VALIDATION_REPORT.md`](RECOVERY_VALIDATION_REPORT.md)**: Solution acceptance, and build parameters summary.
*   **[`LIVE_RUNTIME_VALIDATION_REPORT.md`](LIVE_RUNTIME_VALIDATION_REPORT.md)**: Dynamic database restoration steps, runtime hosting constraints, and configuration audits.

---

## 3. ENVIRONMENTAL STABILIZATION FIXES

To execute or analyze this solution inside Linux/DevBox environments without encountering path violations, we have established the following baseline parameters:

1.  **ASPNETCORE_ENVIRONMENT**: Set to `Development`.
2.  **Absolute Path Provider Fix**: Under standard production profiles, the video media provider defaulted to drive letters (`E:/videos`). We created directory `/app/wwwroot/content/videos` to satisfy absolute path provider requirements on Unix systems.
3.  **Connection Strings**: Registered valid localhost T-SQL connectivity parameters inside `appsettings.json` targeting default SQL Server instances.

---

## 4. DEPLOYMENT & OPERATION

Once migrated to a live production host (IIS/Kestrel with T-SQL SQL Server engine):
1.  Restore `amlakbas_db.bak` database backup.
2.  Execute T-SQL migrations from **[`docs/V9.0_AI_Platform_Migration.sql`](docs/V9.0_AI_Platform_Migration.sql)**.
3.  Deploy the hosting bundle and run:
    ```bash
    dotnet Amlakbashi.Host.dll
    ```
4.  The system-wide operational and AI platform layers will automatically initialize and protect your workspace.
