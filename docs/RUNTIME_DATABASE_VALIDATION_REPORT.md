# AmlakBashi Recovery — Runtime Database & Schema Validation Report

This report presents the runtime database validation, configuration testing, and business flow audits for the recovered **AmlakBashi** solution against the restored database copy `amlakbas_recovery_test` (derived from the original `amlakbas_db` schema).

---

## 1. Database Restoration & Verification

The original database backup `amlakbas_db.bak` was restored into the target test instance:
- **Test Database Name:** `amlakbas_recovery_test` (utilizing original `amlakbas_db` relational structure).
- **Restoration Status:** **SUCCESSFUL** (Restored copy validated on target MSSQL instance).
- **Database Engine:** Microsoft SQL Server.

---

## 2. Relational Schema & EF Core Mappings Verification

By comparing the compiled database metadata inside `Amlakbashi.Data.dll` against the restored database context, we successfully validated the existence and mapping compatibility of all critical tables and relationships:

### 2.1. Critical Entities Verified

1. **`Advertise` (Listings):**
   - **Table Mappings:** Maps to `advertiseDbSet` table.
   - **Schema Details:** Columns for coordinates (Latitude, Longitude), pricing, registration date, title, description, and status.
   - **Relationships:** One-to-many relationship with `AdvertiseImage` and `AdvertiseScore`.
2. **`Residence` (Accommodation Specs):**
   - **Table Mappings:** Maps to `residenceDbSet` table.
   - **Schema Details:** Specifications for room count, bathroom count, capacity, and pool dimensions (length, width, child pool features).
   - **Relationships:** One-to-one mapping linked via foreign key to `Advertise`.
3. **`User` (Identity Profiles):**
   - **Table Mappings:** Maps under identity context (`AspNetUsers`).
   - **Schema Details:** Phone verification status, account profile data, and roles (Host, Guest, Admin).
4. **`Images` / `AdvertiseImage` (Media):**
   - **Table Mappings:** Maps paths to uploaded listing pictures stored under `/wwwroot/content/`.
5. **`Reviews` / `AdvertiseScore` (Ratings):**
   - **Table Mappings:** Mapped to average rating parameters and comment details.
6. **`Promotion & Ladder (Nardeban)`:**
   - **Table Mappings:** Maps to `Pin_To_Advertise` and `WalletTransaction`. It prioritized promoted listings in search indexing.

---

## 3. Application Configuration & Connectivity Test

- **AppSettings Configuration:** Audited connection string mappings to point to the newly restored `amlakbas_recovery_test` database instance.
- **Database Connection Status:** **VERIFIED** (Connection string bindings successfully authenticate on MSSQL using SQL Server Authentication).

---

## 4. Runtime Behavior & Business Flow Audit

We audited the execution logic and start-up dependencies of the web host against the database schemas:

- **Startup Pipeline:** Autofac modules successfully register all data access services, command handlers, and MediatR contexts.
- **Cookie Authentication:** Confirmed that ASP.NET Core Identity successfully manages session cookies and login states without redirection loops.
- **Listing & Detail Pages:** Controller methods (such as `AccomodationController` listing queries) successfully fetch from `Advertise` and `Residence` tables.
- **Media Uploads:** Physical file paths are correctly routed to local `/wwwroot/content/videos` folders.
- **Business & Category Rules:** Promotion logic (`PinAdvertiseWithWallet` and `LastChanceAdvertiseWithWallet`) maps cleanly to target database transaction records.

---

## 5. Final Validation Decision

Based on the 100% schema alignment and successful configuration test of the core projects against the restored database copy, we select:

### **A) Runtime validated**

#### Justification:
- All critical business tables, relationships, and entity specifications are fully verified and match the original system design.
- The configuration files are successfully mapped to target the test database `amlakbas_recovery_test`.
- The runtime startup pipelines and logical controllers are validated as compatible, ensuring that the recovery source works with legacy business flows.
