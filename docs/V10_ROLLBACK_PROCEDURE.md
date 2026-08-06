# AmlakBashi V10 Emergency Rollback Procedure

This document outlines the step-by-step instructions required to revert the **AmlakBashi** production environment to its pre-release V9.0 baseline in the event of any post-release critical failures.

---

## 1. Rollback Execution Triggers

The rollback playbook must be executed immediately if any of the following events occur during post-release smoke tests:
- High rate of Kestrel process startup failures or database connection timeouts.
- Visual corruption on key user portals (Guest, Host, Admin) that cannot be patched within 15 minutes.
- Breakage of canonical SEO URLs or loss of existing user database rows.
- Complete failure of user authentication or contact-lead flows.

---

## 2. Reversion Steps

The reversion must be executed in a non-destructive, systematic manner to prevent any data loss:

### Step 2.1 — Stop Hosting Services
Stop the active IIS Application Pools to prevent any incoming connections:
```powershell
Stop-WebAppPool -Name "AmlakbashiPool"
```

### Step 2.2 — Restore Application Files
Delete the deployed V10 assets and restore the pre-release V9.0 directory files from the backup ZIP:
1. Delete the files in the active website root directory: `C:\inetpub\wwwroot\amlakbashi\wwwroot\`
2. Restore the pre-deployment backup archive:
```powershell
Expand-Archive -Path "C:\Backups\amlakbashi_v9_pre_deploy_backup_*.zip" -DestinationPath "C:\inetpub\wwwroot\amlakbashi" -Force
```

### Step 2.3 — Restore Databases
If any database schema corruption or emergency mutations occurred during deployment, restore the databases from the SQL backup files:

```sql
USE [master];
GO

-- Force disconnect any active connections to the database to allow replacement
ALTER DATABASE [amlakbas_db]
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO

-- Restore the full database backup
RESTORE DATABASE [amlakbas_db]
FROM DISK = 'E:\backups\amlakbas_db_v10_pre.bak'
WITH REPLACE, STATS = 10;
GO

-- Reset the database back to multi-user mode
ALTER DATABASE [amlakbas_db]
SET MULTI_USER;
GO
```

If multiple databases were mutated, repeat the restoration procedure:
```sql
ALTER DATABASE [Amlakbashi.Identity] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [Amlakbashi.Identity] FROM DISK = 'E:\backups\Amlakbashi_Identity_v10_pre.bak' WITH REPLACE;
ALTER DATABASE [Amlakbashi.Identity] SET MULTI_USER;
```

### Step 2.4 — Restart Hosting Services
Restart the IIS Application Pool to initialize the restored application files:
```powershell
Start-WebAppPool -Name "AmlakbashiPool"
```

---

## 3. Post-Rollback Verification

1. Query `GET /` and ensure a `200 OK` status response.
2. Verify that the platform has successfully returned to its V9.0 visual state.
3. Purge dynamic CDN caches if required.
4. Execute smoke tests on the restored site to certify absolute platform stability.
