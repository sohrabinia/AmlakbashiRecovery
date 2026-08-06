# AmlakBashi V10 Production Deployment Runbook

This runbook describes the exact step-by-step execution sequence required to transition the **AmlakBashi** platform from V9.0/Release Candidate to V10.0 Production.

---

## 1. Phase 1 — Pre-Deployment & Preparation

### 1.1. Maintenance Decision & Window
* **Window Recommendation:** Deploy during a low-traffic window, typically between **2:00 AM and 5:00 AM Iran Time**.
* **Maintenance Page Setup:** To prevent active user operations from raising database state discrepancies, configure a standard "System under Maintenance" page in IIS.

### 1.2. Verification Checklist
- [ ] Confirm that both full database and application static directory backups are complete and verified (`docs/V10_PRE_RELEASE_BACKUP_PROCEDURE.md`).
- [ ] Ensure the deployment package (`docs/V10_RELEASE_PACKAGE_VERIFICATION.md`) is transferred and available locally on the hosting server.
- [ ] Ensure Redis server is active and accessible.

---

## 2. Phase 2 — Deployment Execution Sequence

Execute the following steps in order to deploy the V10 presentation layer:

### Step 2.1 — Stop Active IIS Application Pools
To avoid file-locking exceptions during replacement, stop the active website application pools:
```powershell
Stop-WebAppPool -Name "AmlakbashiPool"
```

### Step 2.2 — Replace Static Frontend Assets
Replace the legacy presentation assets under the active IIS directory (`C:\inetpub\wwwroot\amlakbashi\wwwroot\`) with the verified V10 Release Candidate files:
1. Copy `index.html` to the target website root folder.
2. Copy `v10-app.js` to the target website root folder.
3. Ensure premium Persian RTL fonts (`IRANSans-web.woff2`) are located in `wwwroot/Resource/fonts/`.

```powershell
# Copy files via PowerShell (Update target path according to physical IIS settings)
Copy-Item -Path ".\wwwroot\index.html" -Destination "C:\inetpub\wwwroot\amlakbashi\wwwroot\index.html" -Force
Copy-Item -Path ".\wwwroot\v10-app.js" -Destination "C:\inetpub\wwwroot\amlakbashi\wwwroot\v10-app.js" -Force
```

### Step 2.3 — Update Application Configurations (If Required)
Confirm that `appsettings.production.json` is configured correctly, checking SQL connection strings (`AmlakbashiDB`, `IdentityDB`, and `JobDb`) and storage paths for media elements:
```json
{
  "ConnectionStrings": {
    "AmlakbashiDB": "Server=.;Database=amlakbas_db;Trusted_Connection=True;User Id=sa;Password=Yq2KtJs7z9LxAfnB;MultipleActiveResultSets=true;",
    ...
  }
}
```

### Step 2.4 — Apply NTFS Folder Permissions
Assign Read/Write permissions on the physical IIS directory to the Application Pool Identity:
```powershell
icacls "C:\inetpub\wwwroot\amlakbashi\wwwroot" /grant "IIS_IUSRS:(OI)(CI)F" /T
```

### Step 2.5 — Restart hosting services
Restart the Application Pool to apply the new assemblies and configurations:
```powershell
Start-WebAppPool -Name "AmlakbashiPool"
```

---

## 3. Phase 3 — Post-Deployment Verification

### 3.1. Startup & Log Health Check
- Query the main domain `GET /` and ensure a `200 OK` status response.
- Review active logs in the application directory to verify no initialization or DB connection exceptions exist.

### 3.2. CDN Cache Purge
If using dynamic DNS or CDN layers (such as ArvanCloud or Cloudflare), trigger a cache purge for `/v10-app.js` and `/index.html` to ensure users instantly load the new modernized interface.

### 3.3. Execute Post-Deployment Smoke Tests
Execute the comprehensive smoke-testing protocol outlined in `docs/V10_POST_RELEASE_SMOKE_TEST_CHECKLIST.md`.
