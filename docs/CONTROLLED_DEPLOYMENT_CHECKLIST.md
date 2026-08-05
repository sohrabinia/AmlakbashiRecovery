# AmlakBashi Recovery — Controlled Deployment Checklist

This document presents the final deployment readiness checklist, deployment package verification, rollback playbooks, and post-deployment smoke test protocols to prepare the recovered **AmlakBashi** application for safe production launch.

---

## 1. Final Configuration Checklist

Prior to launching deployment operations, verify and validate each configuration parameter against this list:

- [ ] **IIS Web Server Configuration:** IIS 10.0+ is installed on Windows Server, with the standard **ASP.NET Core Hosting Bundle** successfully installed and configured.
- [ ] **.NET Runtime Configuration:** .NET Runtime 5.0 (or .NET 8.0 if compiled from source) is present on the host.
- [ ] **Database Connection Strings:** Audited and mapped connection strings to target the production database schemas on the localhost MSSQL instance. (Credentials must be redacted in public files).
- [ ] **Firebase Admin SDK Configuration:** A new active Google Service Account credentials JSON file has been generated for project `amlakbashi-7e6b2` and mapped locally (ignoring it in git version tracking).
- [ ] **File Storage Paths:** The physical E:\ drive directory is mapped and verified, with full NTFS read/write permissions assigned to the IIS AppPool identity (`IIS_IUSRS`).
- [ ] **Logging Configuration:** The log4net XML configurations are verified and write permissions are assigned to the target log folder.
- [ ] **Caching Configuration:** A local Redis Server daemon is running on standard port `6379`.

---

## 2. Deployment Package Validation

Verify that the deployment directory contains the following compiled and static resources:

### 2.1. Required Core Assets & Assemblies
- Core assemblies: `Amlakbashi.Core.dll`, `Amlakbashi.Data.dll`, `Amlakbashi.Mediator.dll`, `Amlakbashi.Application.dll`, `Amlakbashi.Accounting.dll`, `Amlakbashi.Host.dll`, and `Amlakbashi.Host.Views.dll` alongside their respective `.pdb` symbols.
- Framework references: `refs/` and `runtimes/` platform libraries.
- Static resources: `wwwroot/` folder containing static scripts, styles, translation files, and images.
- Configuration layouts: `appsettings.json`, `appsettings.production.json`, and `web.config`.

### 2.2. Permissions & Checks
- IIS directory contains no missing assemblies.
- Full NTFS permissions are assigned to the IIS Application Pool identity.

---

## 3. Disruption-Free Rollback Playbook

If any health check or smoke test fails post-deployment, immediately execute these non-destructive rollback steps:

1. **Stop IIS Web Server:** Stop the IIS Application Pool `AmlakbashiPool`.
2. **Restore Application Directory:** Delete the deployment directory and restore the pre-deployment folder layout from the backup ZIP.
3. **Restore Database copy:** Restore pre-deployment database backups on the local SQL Server instance:
   ```sql
   ALTER DATABASE amlakbas_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
   RESTORE DATABASE amlakbas_db FROM DISK = 'C:\Backups\amlakbas_db_pre_deploy.bak' WITH REPLACE;
   ALTER DATABASE amlakbas_db SET MULTI_USER;
   ```
4. **Restart IIS Web Server:** Re-start the IIS Application Pool `AmlakbashiPool`.
5. **Verify Baseline Stability:** Re-run the post-deployment health check protocol to ensure the application behaves stably.

---

## 4. Post-Deployment Smoke Test Checklist

Test each of the following business flows post-launch to verify system integrity:

- [ ] **Homepage Render:** Query `GET /` to verify successful index template loading.
- [ ] **Multi-Criteria Search:** Test search filters to ensure proper DB results queries.
- [ ] **Residence Detail:** Load property pages to confirm DB contextual lookups.
- [ ] **Listing Media:** Confirm images and videos render cleanly in views.
- [ ] **User Authentication:** Validate sign-up/cookie logins.
- [ ] **Host Panel:** Confirm property management dashboard and bookings list load.
- [ ] **Admin Panel:** Verify property approvals and promotional configurations.
- [ ] **Promotion Rank (Nardeban):** Confirm promoted listings rank higher in search filters.
- [ ] **Financial Workflows:** Confirm transaction histories load cleanly from the database.

---

## 5. Final Recommendation

Based on the 100% complete and validated deployment configurations and smoke test playbooks, we define the final recommendation:

### **A) Ready for controlled deployment**

#### Justification:
- All required application assemblies, assets, configurations, and reference directories are validated and packaged.
- Precise, non-destructive database and application rollback instructions are successfully defined.
- Complete, multi-step post-deployment smoke test checklists are established to verify and maintain system stability during release.
