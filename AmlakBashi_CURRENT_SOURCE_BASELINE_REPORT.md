# AmlakBashi V10 Current Source Baseline Report

Please refer to the authoritative document at [docs/AmlakBashi_CURRENT_SOURCE_BASELINE_REPORT.md](docs/AmlakBashi_CURRENT_SOURCE_BASELINE_REPORT.md).

## Executive Summary
This report establishes the baseline capabilities and structures of the AmlakBashi V10 authoritative source tree. Following complete code recovery, the solution is verified to contain the original C# code.

---

## 1. Current Application Capabilities Verification
1. **Lead Generation Flow (Implemented):** Frontend Jalaali scheduling libraries (`wwwroot/v10-app.js` and `Amlakbashi.Host/wwwroot/js/app/advertise/item.js`) intercept booking triggers. They guide guests to connect directly with hosts via phone leads, bypassing checkout and billing commission screens. The backend booking and payment infrastructure remains completely preserved to support historical transactions and previous ledger reports.
2. **Advertise Ranking (Implemented):** Managed dynamically via scoring models and campaign discounts on search lists.
3. **Score System (Implemented):** Audited properties `ResidenceScore`, `AmlakbashiScore`, `AverageUsersScore`, and `CleaningScore` on `Advertise.cs` entity, dynamically calculated based on review comments.
4. **Paid Promotion & Last Minute Ads (Implemented):** Supported via campaign percentages and discounts on the `DiscountTables` mapped through `priceCalculator.CalculateReservePrice`.
5. **User/Host Flow (Implemented):** Complete MVC architecture (Controllers, ViewModels, Razor Views, and Areas) handles profile adjustments, admin approval queues, dynamic listings, and host portals.
6. **SEO Routes (Implemented):** Deeply integrated Persian-first SEO routing paths inside `AdvertiseControllerRoutes.cs` (e.g. `اجاره-روزانه/{url}/{area_str}`) dynamically map organic search traffic landing pages.

---

## 2. Database Schema & Compatibility
- **Physical Database Backup (`amlakbas_db.bak`):** Physically missing. Complete schema definition and database relationships are statically mapped and fully recovered via EF Core Migrations and DbContext structures in Amlakbashi.Data.
- **AmlakbashiDB:** Contains DbSets for 30+ core business tables (Advertise, Residence, User, Images, Reviews, Promotion/Ladder, etc.), with automatic migration checking (`context.Database.Migrate()`) on application startup.
- **IdentityDB:** Configured with standard ASP.NET Core Identity tables.
- **Hangfire DB (JobDb):** Fully compatible with standard SQL Server background job schema structures.
