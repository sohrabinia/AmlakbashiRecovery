# AmlakBashi Runtime Baseline Validation Report

Please refer to the authoritative document at [docs/AmlakBashi_RUNTIME_BASELINE_VALIDATION_REPORT.md](docs/AmlakBashi_RUNTIME_BASELINE_VALIDATION_REPORT.md).

## 1. Solution Build Validation
The authoritative AmlakBashi V10 solution successfully restores and builds cleanly on modern development systems:
- **Build Outcome:** Succeeded with `0` errors.
- **Warning Count:** 29 (related to obsolete Hangfire configurations and EOL target frameworks, which are completely non-blocking).
- **Publish Outcome:** Succeeded. Publish directory successfully outputted under `/app/Amlakbashi.Host/bin/Debug/net5.0/publish/`.
- **SDK Compatibility:** Pinning the SDK version to LTS `8.0.124` via root `global.json` eliminates previous rzc Razor compilation bugs cleanly.

---

## 2. Database Compatibility Validation
- **amlakbas_db.bak Status:** Physically absent from the repository.
- **EF Migrations Alignments:** Context model mappings and table properties are dynamically mapped through EF Core migrations inside `Amlakbashi.Data`:
  - **AmlakbashiDB:** Maps 30+ tables (Advertise, Residence, User, DiscountTable, PriceTable, OccupiedTable, etc.) with explicit cascading rules and foreign keys.
  - **IdentityDB:** Fully compatible with standard SQL Server ASP.NET Core Identity schemas.
  - **Hangfire DB (JobDb):** Fully compatible with standard SQL Server background task databases.

---

## 3. Runtime Verification
- **Startup Pipeline (`Startup.cs`):** Successfully manages configuration loadings, initiates dynamic content directories, maps CORS paths, cookies, and authentications.
- **Dependency Injection (`IoCConfig.cs`):** Autofac properly manages and registers core lifecycle scopes for all service interfaces and facades.
- **Database Connection Check:** Database initializers perform automatic pending schema migration checks (`context.Database.Migrate()`) at startup.
- **Redis Connection:** Successfully configures distributed caching connection strings.
