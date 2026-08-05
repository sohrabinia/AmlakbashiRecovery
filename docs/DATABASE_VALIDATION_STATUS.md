# AmlakBashi Recovery — Database Validation Status Report

This report presents an objective evaluation of the database backup and structure status for the `AmlakbashiRecovery` solution, based on physical evidence scans and static context disassembly.

---

## 1. Database Backup File Verification

- **Target Filename:** `amlakbas_db.bak`
- **Existence Status:** **NOT FOUND**
- **File Size:** 0 bytes (the file is physically missing from the cloned repository and workspace).
- **Physical Search Results:**
  - Standard directory find: `0 results`
  - System-wide search: `0 results`

**Conclusion:** The database backup file `amlakbas_db.bak` is not tracked or stored in the GitHub repository. It was not supplied as part of the cloned recovery archive.

---

## 2. Restore Compatibility & Requirements

Since the backup file is missing, actual database restoration cannot be completed. However, based on the compiled data access assemblies, the expected database restore environment parameters are defined below:

- **Database Engine:** Microsoft SQL Server (suggested: version 2016 or higher).
- **Restore Target Database Names:**
  1. `amlakbas_db` (Core business database context)
  2. `Amlakbashi_jdb` (Background jobs context, e.g. Hangfire/Jobs)
  3. `Amlakbashi.Identity` (Authentication and membership database context)
- **Host System Compatibility:** Windows SQL Server or SQL Server running on Linux (via Docker/Kubernetes). Note that unprivileged nested Docker sandboxes cannot run MSSQL containers due to overlayfs whiteout restrictions.

---

## 3. Core Database Entities & Table Mappings (Static Audit)

By performing a static disassembly and model metadata analysis on `Amlakbashi.Data.dll`, we successfully mapped the Entity Framework Core DbContext schemas. Below are the verified core business structures and tables mapping to SQL Server:

### 3.1. User & Authentication Tables
- **Context:** `Amlakbashi.Data.Identity.IdentityDB`
- **Tables / Entities:**
  - `AspNetUsers` / `User` (primary identity accounts)
  - `AspNetRoles` (role-based access levels: Admin, Manager, User)
  - `AspNetUserClaims`, `AspNetUserRoles` (permission mapping tables)

### 3.2. Real Estate Listing & Category Tables
- **Context:** `Amlakbashi.Data.AmlakbashiDB`
- **Tables / Entities:**
  - `Advertise` / `advertiseDbSet` (the primary listing table containing coordinates, prices, registration dates, titles, and descriptions)
  - `Residence` / `residenceDbSet` (contains accommodation features, room counts, bathroom counts, and pool specs)
  - `Category` / `categoryDbSet` (listing categories, e.g. Apartments, Villas, Suites)
  - `Region` / `RegionRepository` (geographic coverage and neighborhood mapping)

### 3.3. Images & Media Mappings
- **Tables / Entities:**
  - `AdvertiseImage` (paths to uploaded listing pictures stored under `wwwroot/content/`)
  - `GeneralData.VideosDirectoryDrive` (external drive reference for physical media, historically defaulting to `E:/videos`)

### 3.4. Reviews & Rating Structures
- **Tables / Entities:**
  - `AdvertiseScore` / `AverageUsersScore` (tracks rating parameters, e.g. Cleanliness, Host behavior, Accuracy)
  - `SubmitAdvertiseScore` (user rating submissions)

### 3.5. Promotion & Ladder Mappings
- **Tables / Entities:**
  - `Pin_To_Advertise` (tracks pinned/laddered advertisements)
  - `WalletTransaction` (wallet billing records for promotions)
  - `PinnedDateTime` (stamps promotion date for ranking)
  - `LastChanceExpireAt` (tracks "Last Minute" deal promotions)

---

## 4. DB Status & Known Blockers

- **Database Restoration Status:** **BLOCKED**
- **Core Blockers:**
  1. **Missing Backup File:** The physical database backup binary (`amlakbas_db.bak`) is entirely missing from the repository, making dynamic restoration and validation impossible.
  2. **Sandbox Environment Limitation:** Lightweight unprivileged sandboxes do not support running SQL Server services locally.
- **Recommended Remediation:** The production team must supply the original `amlakbas_db.bak` file, restore it natively on a dedicated Windows or Linux host running SQL Server, and verify connection string validity.
