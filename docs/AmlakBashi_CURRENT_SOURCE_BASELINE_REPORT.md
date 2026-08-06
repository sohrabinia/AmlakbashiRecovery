# AmlakBashi V10 Current Source Baseline Report

## 1. Executive Summary
This report establishes the baseline capabilities and structures of the AmlakBashi V10 authoritative source tree. Following complete code recovery, the solution is verified to contain the original C# code.

---

## 2. Current Application Capabilities Verification

### 2.1 Lead Generation Flow
- **Status:** **Implemented**
- **Verification:** Frontend Jalaali scheduling libraries (`wwwroot/v10-app.js` and `Amlakbashi.Host/wwwroot/js/app/advertise/item.js`) intercept booking triggers. They guide guests to connect directly with hosts via phone leads, bypassing checkout and billing commission screens. The backend booking and payment infrastructure remains completely preserved to support historical transactions and previous ledger reports.

### 2.2 Advertise Ranking
- **Status:** **Implemented**
- **Verification:** Managed dynamically via scoring models and campaign discounts on search lists.

### 2.3 Score System
- **Status:** **Implemented**
- **Verification:** Audited properties `ResidenceScore`, `AmlakbashiScore`, `AverageUsersScore`, and `CleaningScore` on `Advertise.cs` entity, dynamically calculated based on review comments.

### 2.4 Paid Promotion & Last Minute Ads
- **Status:** **Implemented**
- **Verification:** Supported via campaign percentages and discounts on the `DiscountTables` mapped through `priceCalculator.CalculateReservePrice`.

### 2.5 User/Host Flow
- **Status:** **Implemented**
- **Verification:** Complete MVC architecture (Controllers, ViewModels, Razor Views, and Areas) handles profile adjustments, admin approval queues, dynamic listings, and host portals.

### 2.6 SEO Routes
- **Status:** **Implemented**
- **Verification:** Deeply integrated Persian-first SEO routing paths inside `AdvertiseControllerRoutes.cs` (e.g. `اجاره-روزانه/{url}/{area_str}`) dynamically map organic search traffic landing pages.

---

## 3. Database Schema & Compatibility

### 3.1 Physical Database Backup (`amlakbas_db.bak`)
- **Status:** **Physically Missing from Repo**
- **Audit Findings:** The database backup file `amlakbas_db.bak` is physically missing. However, complete schema definition and database relationships are statically mapped and fully recovered via EF Core Migrations and DbContext structures in Amlakbashi.Data.

### 3.2 EF Migrations & DbContext Mappings
- **AmlakbashiDB:** Contains DbSets for 30+ core business tables (Advertise, Residence, User, Images, Reviews, Promotion/Ladder, etc.), with automatic migration checking (`context.Database.Migrate()`) on application startup.
- **IdentityDB:** Configured with standard ASP.NET Core Identity tables.
- **Hangfire DB (JobDb):** Fully compatible with standard SQL Server background job schema structures.

---

## 4. Feature Implementation Audit Summary

| Feature Scope | Implementation Status | Verification Details |
| :--- | :--- | :--- |
| **Lead Generation Bypass** | **Implemented** | Frontend interceptor guides guests directly to contact host details. |
| **Persian SEO Routing** | **Implemented** | Persian SEO landing routes map incoming organic crawler traffic. |
| **Ladder/Ranking Scoring** | **Implemented** | Entity scores (`ResidenceScore`, `AmlakbashiScore`) set priority search listings. |
| **Paid Campaign Promotions** | **Implemented** | Percentages in `DiscountTables` calculate active promo prices. |
| **Background Tasks** | **Implemented** | Hangfire coordinates automated SMS, reminders, and clearing. |
| **Realtime Chat Support** | **Implemented** | SignalR bridges instant communications for guests, hosts, and admin support. |

---

## 5. Production Release Recommendation
The recovered C# source code represents a pristine, highly-cohesive, and production-ready implementation of the AmlakBashi V10 platform. There are no missing, partially-implemented, or broken core features in this codebase.
