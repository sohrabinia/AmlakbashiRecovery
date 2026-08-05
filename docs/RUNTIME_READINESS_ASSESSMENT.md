# AmlakBashi Recovery — Runtime Readiness Assessment

This report provides a comprehensive audit of the configuration files, connection strings, services, and system dependencies required to host, configure, and execute the recovered `Amlakbashi` web application.

---

## 1. AppSettings & Environment Files Audit

The cloned repository contains three primary environment configuration JSON files:

1. **`appsettings.json` (Core default configuration):**
   - Configures the logging loglevels, allowed hosts, Redis host/port settings, JWT token parameters, and base database connection strings.
2. **`appsettings.Development.json` (Development overrides):**
   - Contains minimal development-specific overrides.
3. **`appsettings.production.json` (Production overrides):**
   - Overrides default connection strings to target production database configurations and defines the production JWT secret UUID.

---

## 2. Infrastructure & Service Requirements

To run the application, the target host machine (development or hosting server) must fulfill the following infrastructure requirements:

### 2.1. SQL Server Database Engine
- **Requirement:** Microsoft SQL Server instance (2016, 2019, or 2022).
- **Target Connection Mappings:**
  - **AmlakbashiDB:** Targets the core business schema `Database=amlakbas_db` using SQL Server Authentication on localhost.
  - **JobDb:** Targets the background jobs registry schema `Database=Amlakbashi_jdb`.
  - **IdentityDB:** Targets the user and authentication schema `Database=Amlakbashi.Identity`.
- **Status:** **BLOCKED** (No SQL Server database is currently restored, and the required database backup `amlakbas_db.bak` is missing from the repository).

### 2.2. Redis Distributed Cache
- **Requirement:** Redis Server instance.
- **Default Parameters:** `Server: localhost`, `Port: 6379`.
- **Role:** Distributed caching, real-time transient data management, and session state persistence.
- **Status:** Requires local Redis daemon initialization before application launch.

### 2.3. .NET 5.0 Hosting Runtime
- **Requirement:** .NET 5.0 Runtime (Hosting Bundle for IIS or runtime binaries for Kestrel).
- **Platform Boundaries:**
  - .NET 5.0 is retired. On modern Linux host systems (e.g., Ubuntu 24.04), glibc and OpenSSL library mismatches prevent the direct co-installation of .NET 5.0 with newer .NET SDKs.
  - Recommended host is a **Windows Server running IIS** with .NET 5.0 Hosting Bundle, or a legacy Docker container using custom base images.

---

## 3. External Services & Critical Integrations

### 3.1. Firebase Admin SDK
- **Integration Status:** Embedded in the `Amlakbashi.Host` startup registry.
- **Credentials:** Uses `amlakbashi-7e6b2-firebase-adminsdk-h6gkp-0159f2aab7.json`.
- **Validation Audit:** **CRITICAL BLOCKER**. We programmatically proved that the private key `0159f2aab7` has been **revoked/deactivated in Google Cloud Console** (token requests return `invalid_grant: Invalid JWT Signature`).
- **Impact:** FCM Push Notifications, Firebase Storage, and administrative operations will fail dynamically during runtime until a new active service account credential is generated and mapped.

### 3.2. Kavenegar SMS Gateway
- **Integration Status:** Present in compiled logic (`Kavenegar.Core.dll`).
- **Requirement:** Active SMS API keys must be registered and configured in settings.

---

## 4. File Storage & Directory Configuration

- **Drive Parameter:** `GeneralData.VideosDirectoryDrive`
- **Default Production Setting:** `E:/videos`
- **Linux Execution Exception:** On Linux, any attempt to initialize drive letters (`E:/`) throws a `PhysicalFileProvider` absolute path exception:
  ```text
  The path must be absolute. (Parameter 'root')
  ```
- **Workaround:** Force `ASPNETCORE_ENVIRONMENT=Development` and pre-create the local media directory at `/app/wwwroot/content/videos` to satisfy path parameters under Linux environments.

---

## 5. Summary of Runtime Readiness

- **Runtime Readiness Status:** **NOT READY / BLOCKED**
- **Actionable Remediation List:**
  1. **Supply Backup:** Provide the `amlakbas_db.bak` file and restore it on a local SQL Server.
  2. **Regenerate Firebase Credentials:** Generate a new active Service Account JSON file in Google Cloud Console for project `amlakbashi-7e6b2`, rename it, and place it locally (ensuring it remains ignored by the updated `.gitignore`).
  3. **Establish Redis Host:** Ensure Redis Server is running on port 6379.
  4. **Target Windows hosting:** Deploy the application to a Windows IIS host running .NET 5.0 Hosting Bundle to prevent Linux path absolute issues and runtime glibc conflicts.
