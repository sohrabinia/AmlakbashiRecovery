# AmlakBashi V10 Pre-Release Production Backup Procedure

This document outlines the step-by-step backup instructions that on-site IT administrators must execute prior to initiating any deployment activities on the production servers.

---

## 1. Application Directory Backup

To safeguard existing website assets, legacy static resources, and media libraries before copying over V10 files, the current application directories must be archived.

### 1.1. Required Backup Assets
- The entire `wwwroot/` physical deployment directory in IIS.
- All configuration files (`appsettings.json`, `appsettings.production.json`, `web.config`, `log4net.config`).

### 1.2. Archiving Procedure (Windows/IIS Host)
1. Log in to the production Windows Server hosting environment with Administrative privileges.
2. Open PowerShell as Administrator.
3. Stop the active IIS Application Pool to release file locks:
   ```powershell
   Stop-WebAppPool -Name "AmlakbashiPool"
   ```
4. Compress and archive the target site folder:
   ```powershell
   Compress-Archive -Path "C:\inetpub\wwwroot\amlakbashi\*" -DestinationPath "C:\Backups\amlakbashi_v9_pre_deploy_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').zip" -Force
   ```
5. Confirm the backup file exists, size is greater than 0MB, and store it on an isolated backup volume.

---

## 2. Database Backup (Microsoft SQL Server)

To ensure zero risk of mutating or losing historical reservation logs, active listings, user roles, or financial ledger balances, a verified full database backup must be executed.

### 2.1. SQL Server Management Studio (SSMS) Method
1. Connect to the local MSSQL Server instance hosting the production databases.
2. Right-click on `amlakbas_db` ➔ **Tasks** ➔ **Back Up...**
3. Select **Backup type:** `Full`.
4. Add backup destination: `E:\backups\amlakbas_db_v10_pre.bak`.
5. Under **Options**, check **Verify backup when finished** and click **OK**.

### 2.2. Transact-SQL (T-SQL) Execution Command
Alternatively, run the following script in a high-privilege query window:

```sql
-- Create a full verified backup of the core database before V10 deployment
BACKUP DATABASE [amlakbas_db]
TO DISK = 'E:\backups\amlakbas_db_v10_pre.bak'
WITH FORMAT,
     MEDIANAME = 'V10PreBackup',
     NAME = 'Full Backup of amlakbas_db before V10 deployment',
     STATS = 10;
GO

-- Verify the integrity of the backup file
RESTORE VERIFYONLY
FROM DISK = 'E:\backups\amlakbas_db_v10_pre.bak';
GO
```

If multiple databases are used (such as Identity or Hangfire Jobs), repeat the backup procedure for each:
```sql
BACKUP DATABASE [Amlakbashi.Identity] TO DISK = 'E:\backups\Amlakbashi_Identity_v10_pre.bak' WITH FORMAT;
BACKUP DATABASE [Amlakbashi_jdb] TO DISK = 'E:\backups\Amlakbashi_jdb_v10_pre.bak' WITH FORMAT;
```

---

## 3. Configuration & State Backups

1. **Redis Cache State:** If dynamic sessions are managed in Redis, backup the Redis dump:
   - File: `/var/lib/redis/dump.rdb` or standard Windows Redis install directory dump.
2. **Registry Keys:** Ensure data protection keys (routed to database registry systems) are fully synced and backed up as part of the database procedure.
