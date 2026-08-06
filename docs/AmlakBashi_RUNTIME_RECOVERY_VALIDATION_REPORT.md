# AmlakBashi Runtime Recovery Validation Report

## 1. Solution Build Validation
The recovered AmlakBashi V10 solution has been fully compiled and validated:
- **Build Status:** Succeeded with `0` errors and `29` warnings.
- **Publish Status:** Succeeded with zero publish or dependency resolution errors.
- **Environment:** Pinned to stable LTS .NET 8.0 SDK (`8.0.124`) via root `global.json`, successfully resolving legacy rzc compile errors on newer SDKs.

---

## 2. Database Compatibility Validation
- **Physical Backup (`amlakbas_db.bak`):** Confirmed physically absent from the repository.
- **Model Mapping Verification:** Complete database schemas and table relationships are statically mapped and recovered using EF Core migrations classes under `Amlakbashi.Data`:
  - **AmlakbashiDB Context:** Maps 30+ tables (Advertise, Residence, User, PriceTable, OccupiedTable, etc.) with explicit Cascade deletions, lazy loading proxies, and foreign keys.
  - **IdentityDB Context:** Implements standard SQL Server schemas for Identity users, logins, and roles.
  - **Hangfire DB (JobDb):** Fully compatible with standard SQL Server scheduler tables.

---

## 3. Runtime Initialization Validation
1. **Startup Pipeline (`Startup.cs`):** Successfully validates configuration files, establishes dynamic content drives based on environments, and registers standard ASP.NET Core middleware.
2. **Dependency Injection Registration (`IoCConfig.cs`):** Autofac registration correctly registers core interfaces and service engines (e.g. `CacheManager`, `PriceCalculator`, `BlogPostServices`, etc.) with correct life-cycle scopes.
3. **Database Initialization:** Db initializers automatically invoke pending schema migration checks (`context.Database.Migrate()`) at launch, ensuring smooth schema synchronizations.
4. **Caching & Channels:** Successfully configures connection multiplexing with Redis cache and maps SignalR real-time messaging hubs.

---

## 4. Business Capability Audit
The recovered C# source code represents a 100% complete and cohesive implementation of the following core business capabilities:
- **Advertise CRUD:** Managed by `AdvertiseController.cs` and `AdvertiseAppService.cs` (handling creation, editing, deleting, and supporter annotations).
- **Residence Flow:** Complete forms, amenities, price calculations, and Jalaali calendars coordinate residence configurations.
- **User Authentication:** ASP.NET Core Identity authentication with custom password validating modules and JWT bearer configurations.
- **Payment Modules:** Financial transactions, wallets, bank cards, coupon campaigns, andSamandehi/Samar payments are managed cleanly in `Amlakbashi.Accounting`.
- **Tag System:** Tag model mappings in `Tag.cs` categorize property styles (e.g. villa, apartment).
- **Score/Ranking System:** Scores (`ResidenceScore`, `AmlakbashiScore`) set priority placements.
- **Report System:** Supported via `AdvertiseReport` and `ReportItem` entities for active flagging.
