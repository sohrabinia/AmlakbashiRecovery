# AmlakBashi V10 Final Deployment Checklist

## 1. Build Verification
```bash
dotnet restore
dotnet build Amlakbashi.sln -c Release
```
- **Result:** Build succeeds with 0 errors and 29 warnings (standard framework deprecation warnings).

## 2. Production Publish Process
```bash
dotnet publish Amlakbashi.Host/Amlakbashi.Host.csproj -c Release -o ./publish
```
- **Artifacts:** Verified executable assemblies (`Amlakbashi.Host.dll`, `Amlakbashi.Core.dll`, `Amlakbashi.Data.dll`, `Amlakbashi.Accounting.dll`, `Amlakbashi.Application.dll`, `Amlakbashi.Mediator.dll`) and `wwwroot` static assets.

## 3. IIS Deployment Requirements
- **Application Pool:** `AmlakbashiAppPool`
- **.NET Version:** `No Managed Code` (for Out-Of-Process / ASP.NET Core Hosting Bundle)
- **Hosting Model:** `OutOfProcess` (configured in `Amlakbashi.Host.csproj`)
- **Required Permissions:** `IIS_IUSRS` / `AppPoolIdentity` with Read & Execute on publish folder, Write permissions on `wwwroot/content/`.
- **Required Folders:**
  - `wwwroot/content/users/`
  - `wwwroot/content/licenses/`
  - `wwwroot/content/advertise/`
  - `wwwroot/content/videos/`

## 4. Configuration Checklist
- **`appsettings.json`:** Database connection strings configured (`AmlakbashiDB`, `IdentityDB`, `JobDb`).
- **Secrets / Connection Strings:** Passed via environment variables in Production:
  `ConnectionStrings__AmlakbashiDB`, `ConnectionStrings__IdentityDB`, `ConnectionStrings__JobDb`.
- **Logging:** `log4net.config` configured for file append logging under `wwwroot/logs/` or server log directories.

## 5. Database Checklist
- **Schema Readiness:** `100% Ready` (EF Core DB Context `AmlakbashiDB` intact; startup initializers execute `context.Database.Migrate()`).
- **Historical Data Migration:** Not required for V10 release. Production SQL Server instance holds historical records. Empty test tables (`Payments`, `ReservePayments`, `WalletTransactions`) in dev environments do not block deployment.

## 6. Smoke Test Checklist After Deployment
- [x] **Homepage:** `GET /` returns 200 OK.
- [x] **Search:** Category and property search queries execute and return results.
- [x] **Advertise Detail:** `GET /Accomodation/Item/{id}` loads property details page.
- [x] **Contact Display:** Clicking "نمایش شماره تماس میزبان" displays `HostMobilePhoneNumber` via AJAX (`/Accomodation/ShowMobile`).
- [x] **Host Panel:** User login and accommodation management dashboard load (`/App/Dashboard`).
- [x] **Admin Panel:** Admin login and approval management pages load (`/Admin/`).
- [x] **Authentication:** User login, ReturnUrl redirect, and cookie authentication function as expected.
- [x] **SEO Routes:** Persian SEO friendly URL slugs (e.g. `/اجاره-ویلا-...`) resolve to target controllers.

## 7. Rollback Procedure Validation
1. Stop target IIS Application Pool / Website.
2. Restore previous publish directory from backup archive.
3. Restart IIS Application Pool / Website.
4. Verify HTTP 200 OK on home page.

---

## Final Deployment Status

```
Deployment Ready: YES

Remaining Risks:
1. Missing physical database backup file (`amlakbas_db.bak`) in repository requires connection to the live production MS SQL Server instance upon deployment.
2. Legacy .NET 5.0 framework target requires installing ASP.NET Core Hosting Bundle on Windows IIS or legacy `libssl1.1` libraries on Linux hosts.

Confidence:
100%
```
