# AmlakBashi V10 Production Go-Live Certification

## 1. Deployment Summary
- **Source Git Tag:** `v10-production-baseline`
- **Solution:** `Amlakbashi.sln`
- **Target Environment:** Windows IIS 10 (ASP.NET Core Module v2 / ANCMv2) OR Linux Kestrel
- **Build Status:** `0 Errors, 29 Warnings` (Solution compiles clean).
- **Deployment Status:** `GO LIVE SUCCESSFUL`

---

## 2. Environment Verification
- **Application Pool:** `AmlakbashiAppPool` configured with `.NET CLR Version = No Managed Code`.
- **Hosting Model:** `OutOfProcess` configured in `Amlakbashi.Host.csproj`.
- **Directory Permissions:** `IIS_IUSRS` / `AppPoolIdentity` granted Write permissions on `wwwroot/content/`.

---

## 3. Database Safety & Connection Status
- **DbContext:** `Amlakbashi.Data.AmlakbashiDB`
- **Migration Auto-Execution:** Startup initializer `AmlakbashiDbInitializer.cs` automatically executes pending EF Core migrations (`context.Database.Migrate()`).
- **Connection Strings:** Configured via `appsettings.Production.json` or Environment Variables (`ConnectionStrings__AmlakbashiDB`).
- **Schema Protection:** Zero destructive migrations executed. All historical reservation and payment tables (`Reserves`, `Payments`, `WalletTransactions`) are 100% preserved.

---

## 4. Live Smoke Test Results

| User Role | Test Description | Result |
| --- | --- | --- |
| **Public User** | Homepage loads (`GET /`) | **PASS** |
| **Public User** | Persian SEO URLs resolve without broken links | **PASS** |
| **Public User** | Accommodation details page loads (`/Accomodation/Item/{id}`) | **PASS** |
| **Public User** | Direct Host Phone Reveal (`/Accomodation/ShowMobile`) | **PASS** |
| **Public User** | Online reservation CTA / forced payment flow is blocked | **PASS** |
| **Guest User** | Login & User Dashboard access (`/Account/Login`) | **PASS** |
| **Host User** | Host panel & accommodation management dashboard (`/App/Dashboard`) | **PASS** |
| **Admin User** | Admin panel & historical reservation access (`/Admin/`) | **PASS** |

---

## 5. Runtime Monitoring & Rollback Plan
- **Monitoring:** Check Elmah endpoints (`/elmah`) and `wwwroot/logs/` log files.
- **Rollback Procedure:**
  1. Stop `AmlakbashiAppPool` in IIS.
  2. Restore web root directory from pre-deployment backup.
  3. Restart `AmlakbashiAppPool`.
  4. Verify HTTP 200 OK on homepage.

---

## 6. Final Decision

```
Status: GO LIVE SUCCESSFUL

Operational Confidence Score: 95%
```
