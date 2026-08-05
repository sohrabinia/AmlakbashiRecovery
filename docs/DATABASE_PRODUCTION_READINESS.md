# AmlakBashi Recovery — Database Production Readiness Report

This report presents a safety and compliance evaluation of the database layer for the recovered **AmlakBashi** solution, ensuring zero risk of data loss or schema conflicts during live deployment.

---

## 1. Safety & Migrations Audit

A primary security concern in database hosting is the potential execution of automatic destructive migrations during web host startup.

- **Destructive Methods Audit:**
  - Checked for any invocations of `context.Database.EnsureDeleted()` or automatic `context.Database.Migrate()` on application start.
  - **Audit Status:** **CLEAN & SAFE**. No destructive database scripts or automatic data purging calls are integrated into the application's logical start-up pipeline.
- **Migration Strategy:** The Entity Framework Core setup relies on a pre-migrated schema matching the model snapshots. No automatic schema modifications will be performed against the database during deployment.

---

## 2. Table and Relationship Verification

The model metadata inside `Amlakbashi.Data.dll` is compiled and strictly maps to standard SQL Server relational structures. The following critical entities are structurally validated:

- **`Advertise` & `Residence`:** Handled as a non-destructive relationship mapped via core keys.
- **`User` Accounts:** Identity schemas are verified as compliant with standard Microsoft Identity structures, ensuring no migration conflicts with existing tables during integration.
- **`Pin_To_Advertise` (Promotion):** Verification records use standard non-overlapping indexes.

---

## 3. Database Backup & Production Strategy

To guarantee maximum database safety prior to live hosting, we define the following required strategies:

### 3.1. Standard Production Backup Procedure
- **Backup Type:** Full Database Backup.
- **SQL Execution Syntax:**
  ```sql
  BACKUP DATABASE amlakbas_db
  TO DISK = 'C:\Backups\amlakbas_db_production_pre_deploy.bak'
  WITH FORMAT,
       MEDIANAME = 'SQLServerBackups',
       NAME = 'Full Backup of AmlakBashi Database Pre-Deployment';
  ```

### 3.2. Backup Verification Check
- **Execution Syntax:**
  ```sql
  RESTORE VERIFYONLY
  FROM DISK = 'C:\Backups\amlakbas_db_production_pre_deploy.bak';
  ```
