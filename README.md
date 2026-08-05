# AmlakBashi — Recovered Core Engine & AI Intelligence Platform

Welcome to the unified core codebase of **AmlakBashi**, a premier localized real estate and accommodation marketplace with over 12 years of operational heritage.

This repository represents the fully audited and clean recovered core solution compiled for **.NET 5.0** alongside the blueprint for the **AmlakBashi AI Autonomous Intelligence Platform (Chapter 13)**.

---

## 1. PROJECT STRUCTURE & RECOVERY STATUS

Due to historical hosting changes and system recovery steps, the application's core logic has been fully decompiled, resolved, and compiled into the following high-performance assemblies:

*   **`Amlakbashi.Core.dll`**: Domain model definitions, entities (such as `Advertise`, `Residence`, `User`, `Images`, `Reviews`), and system configurations.
*   **`Amlakbashi.Data.dll`**: Entity Framework Core DbContext (`AmlakbashiDB`, `IdentityDB`), schema definitions, mapping configurations, and structural migrations.
*   **`Amlakbashi.Mediator.dll`**: Clean architecture Request-Response dispatcher and Command-Query Responsibility Segregation (CQRS) handlers.
*   **`Amlakbashi.Application.dll`**: Core business services, helper libraries, financial integration, SMS/Kavenegar connectors, and background schedulers.
*   **`Amlakbashi.Accounting.dll`**: Wallet transactional processes, secure ledger balances, and payment processing nodes.
*   **`Amlakbashi.Host.dll` & `Amlakbashi.Host.Views.dll`**: Dynamic MVC web application, controller API endpoints (`UserController`, `AdvertiseController`, etc.), Razor Views, and startup pipeline.

### Recovery Validation Evidence:
*   Static disassembly scans confirm that all 37 DbSets and business behaviors are intact.
*   The system has been meticulously analyzed, and the results are detailed in:
    *   **[`docs/FINAL_REPOSITORY_CLEANUP_REPORT.md`](docs/FINAL_REPOSITORY_CLEANUP_REPORT.md)**: Details on C# file cleanup, repository size analysis, and key security hygiene boundaries.
    *   **[`docs/REPOSITORY_SECURITY_AUDIT.md`](docs/REPOSITORY_SECURITY_AUDIT.md)**: Proof of deactivation/revocation of legacy service account keys and connection safety.
    *   **[`RECOVERY_VALIDATION_REPORT.md`](RECOVERY_VALIDATION_REPORT.md)**: Solution acceptance, and build parameters summary.
    *   **[`LIVE_RUNTIME_VALIDATION_REPORT.md`](LIVE_RUNTIME_VALIDATION_REPORT.md)**: Dynamic database restoration steps, runtime hosting constraints, and configuration audits.

---

## 2. ENVIRONMENTAL STABILIZATION FIXES

To execute or analyze this solution inside Linux/DevBox environments without encountering path violations, we have established the following baseline parameters:

1.  **ASPNETCORE_ENVIRONMENT**: Set to `Development`.
2.  **Absolute Path Provider Fix**: Under standard production profiles, the video media provider defaulted to drive letters (`E:/videos`). We created directory `/app/wwwroot/content/videos` to satisfy absolute path provider requirements on Unix systems.
3.  **Connection Strings**: Registered valid localhost T-SQL connectivity parameters inside `appsettings.json` targeting default SQL Server instances.

---

## 3. AMLAKBASHI AI AUTONOMOUS INTELLIGENCE PLATFORM (MANDATORY CHAPTER 13)

To secure peak performance, protect over 12 years of SEO equity, and automate operational tasks, the platform incorporates a full **Autonomous AI Operational Layer**. This system is structured as an internal enterprise service rather than a standalone chat application.

### Key Documentation:
The architectural blueprints, T-SQL database tables, C# integration services, and human-in-the-loop workflows are fully detailed in:
👉 **[`docs/AI_AUTONOMOUS_INTELLIGENCE_PLATFORM.md`](docs/AI_AUTONOMOUS_INTELLIGENCE_PLATFORM.md)**

### Specialized Agent List:
1.  **AI DevOps / SRE Agent**: Continuous health monitoring, DB/Redis performance profiling, and error trace analysis.
2.  **AI Backup & Disaster Recovery Agent**: Verifies backup integrity, structures files, and checks physical asset storage.
3.  **AI SEO Intelligence Agent**: Technical SEO checks, broken link redirects, and Persian search taxonomy parsing.
4.  **AI GEO / Local SEO Agent**: Generates landing page plans and content clusters for Gilan, Mazandaran, and Golestan.
5.  **AI Listing Intelligence Agent**: Evaluates listing metrics and structures a dynamic Quality Score (0-100).
6.  **AI Listing Editor Agent**: Recommends optimized Persian titles and conversion-focused descriptions.
7.  **AI Image Intelligence Agent**: Validates media files, recommends cover selections, and filters poor-quality photos.
8.  **AI Duplicate Detection Agent**: Employs perceptual hash comparisons to eliminate duplicate listings.
9.  **AI Moderation Agent**: Flags direct phone number leaks, spam patterns, and policy violations.
10. **AI Ranking Intelligence Agent**: Adjusts listing ranks based on ladder status, promotion, and user clicks.
11. **AI Analytics Agent**: Measures marketplace growth, revenue trends, and SEO organic progress.
12. **AI Customer Support Agent**: Handles Persian search intent mapping and acts as a conversational search advisor.
13. **AI Host Assistant**: Provides hosts with dynamic pricing suggestions and listing improvement rules.
14. **AI Admin Copilot**: Explains server health reports and assists admins with bulk operational tasks.
15. **AI Knowledge Base**: Creates semantic graphs linking region structures with localized tourist intent.
16. **AI Memory Layer**: Retains conversational contexts and logs historical decision rules.
17. **AI Content Intelligence Hub**: Unifies Blog & News content lifecycles including idea discovery, SEO drafting, and internal linking.
18. **AI Governance Layer**: Assures zero silent database mutations by logging all predictions inside audited schemas.

---

## 4. DEPLOYMENT & OPERATION

Once migrated to a live production host (IIS/Kestrel with T-SQL SQL Server engine):
1.  Restore `amlakbas_db.bak` database backup.
2.  Configure Connection Strings inside `appsettings.production.json`.
3.  Deploy the hosting bundle and run:
    ```bash
    dotnet Amlakbashi.Host.dll
    ```
4.  The system-wide operational and AI platform layers will automatically initialize and protect your workspace.
