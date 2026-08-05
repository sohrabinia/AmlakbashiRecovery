# AmlakBashi Recovery — Production Deployment Plan

This document presents a safe, non-destructive, and highly structured deployment plan and rollback playbook for launching the recovered **AmlakBashi** application in a controlled live production environment.

---

## 1. Step-by-Step Production Deployment Procedure

### Step 1: Pre-Deployment Database Backup
To guarantee zero risk of data loss, take full SQL Server backups of all active database contexts prior to launching any deployment operations:
- **T-SQL Backup Commands:**
  ```sql
  BACKUP DATABASE amlakbas_db TO DISK = 'C:\Backups\amlakbas_db_pre_deploy.bak' WITH FORMAT;
  BACKUP DATABASE Amlakbashi.Identity TO DISK = 'C:\Backups\Amlakbashi.Identity_pre_deploy.bak' WITH FORMAT;
  BACKUP DATABASE Amlakbashi_jdb TO DISK = 'C:\Backups\Amlakbashi_jdb_pre_deploy.bak' WITH FORMAT;
  ```

### Step 2: Live Application Directory Backup
- Compresses the current live physical files running under IIS to ensure a safe restore point:
  - **Path:** `C:\inetpub\wwwroot\Amlakbashi`
  - **Action:** Compress the directory into a secure ZIP file (e.g. `C:\Backups\Amlakbashi_Application_Pre_Deploy.zip`).

### Step 3: Deployment Execution (IIS File Overwrite)
1. Stop the target IIS Application Pool (`AmlakbashiPool`) to release file locks on active assemblies.
2. Copy the newly compiled assembly DLLs, configuration overrides (`appsettings.production.json`), and updated static resources (`wwwroot/`) into the physical IIS hosting folder.
3. Ensure that the untracked Firebase Service Account credentials JSON file is placed locally (and verified to remain ignored by the updated `.gitignore`).
4. Re-start the IIS Application Pool (`AmlakbashiPool`).

---

## 2. Post-Deployment Health Check Protocol

Once the deployment completes, verify application stability by running HTTP audits against key endpoints:

1. **Homepage Ping:** Query the root URL (`HTTP GET /`) to verify successful IIS and Kestrel initialization.
2. **Database Connectivity Audit:** Query the Category loading endpoint to ensure that model DbContexts are successfully retrieving relational records from the database context.
3. **Authentication Verification:** Test user sign-up/cookie logins to verify that encryption and member persistence behave correctly.
4. **Media Directory Presence:** Confirm that images and listing videos stored under `/wwwroot/content/videos` are successfully loading.

---

## 3. Disruption-Free Rollback Playbook

If a critical blocker or regression is detected during the post-deployment health check, immediately execute the rollback steps:

1. **Stop IIS pool:** Stop the IIS Application Pool `AmlakbashiPool`.
2. **Restore Application Files:** Delete the failed deployment directory and restore the pre-deployment folder layout from `Amlakbashi_Application_Pre_Deploy.zip`.
3. **Restore Database copy:** Restore the pre-deployment database backups on SQL Server:
   ```sql
   ALTER DATABASE amlakbas_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
   RESTORE DATABASE amlakbas_db FROM DISK = 'C:\Backups\amlakbas_db_pre_deploy.bak' WITH REPLACE;
   ALTER DATABASE amlakbas_db SET MULTI_USER;
   ```
4. **Restart IIS Pool:** Start the IIS Application Pool `AmlakbashiPool`.
5. **Verify Stability:** Re-run the Post-Deployment Health Check Protocol to ensure that the pre-deployment live state is fully active and stable.
