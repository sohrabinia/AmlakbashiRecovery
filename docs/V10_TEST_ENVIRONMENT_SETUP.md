# AmlakBashi V10 Test Environment Setup Guide

This document defines the staging and local test environment setup parameters for testing the new **AmlakBashi V10 Release**, ensuring zero disruption to production resources.

---

## 1. Test Environment Architecture

Testing is executed in a sandboxed, isolated environment mimicking production:
- **Local Host:** served via a lightweight web server (Python http.server on port 8000) mapped to `/wwwroot`.
- **Operating System:** Ubuntu Linux with configured `.NET Core` legacy compatibility runtimes.
- **Library Compatibility:** Requires loading legacy `libssl1.1` and `libcrypto.so.1.1` libraries into `LD_LIBRARY_PATH`.

---

## 2. Configuration Settings

### Database Connections (Staging Baseline):
The connection string inside development configurations is mapped to target the isolated test copy of the database:
```json
"ConnectionStrings": {
  "AmlakbashiDB": "Server=localhost;Database=amlakbas_recovery_test;User Id=sa;Password=YourHardenedPassword;",
  "JobDb": "Server=localhost;Database=JobDb_test;User Id=sa;Password=YourHardenedPassword;",
  "IdentityDB": "Server=localhost;Database=IdentityDB_test;User Id=sa;Password=YourHardenedPassword;"
}
```

### Video Storage Overrides:
To prevent directory errors on Linux environments:
- `ASPNETCORE_ENVIRONMENT` must be set to `Development`.
- The physical videos directory is mapped to: `/app/wwwroot/content/videos`.

---

## 3. Deployment Checklist for QA

1. Set environment variables:
   ```bash
   export ASPNETCORE_ENVIRONMENT=Development
   export LD_LIBRARY_PATH=/usr/local/lib:$LD_LIBRARY_PATH
   ```
2. Start the local server under `wwwroot/`:
   ```bash
   python3 -m http.server 8000 --directory /app/wwwroot
   ```
3. Run automated verification suite to confirm UI performance:
   ```bash
   python3 /home/jules/verification/verify_v10.py
   ```
