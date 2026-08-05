# AmlakBashi Recovery — Production Configuration Audit Report

This report presents a thorough, evidence-based audit of the default and production configurations tracked in the recovered codebase, highlighting environmental dependencies and security considerations.

---

## 1. AppSettings & Connectivity Audit

The application contains standard connection settings mapped under `appsettings.json` and `appsettings.production.json`:

### 1.1. Connection Strings (Redacted)
- **AmlakbashiDB:** `Server=.;Database=amlakbas_db;Trusted_Connection=True;User Id=sa;Password=[REDACTED_PRODUCTION_PASSWORD];MultipleActiveResultSets=true;`
- **JobDb:** `Server=.;Database=Amlakbashi_jdb;Trusted_Connection=True;User Id=sa;Password=[REDACTED_PRODUCTION_PASSWORD];`
- **IdentityDB:** `Server=.;Database=Amlakbashi.Identity;Trusted_Connection=True;User Id=sa;Password=[REDACTED_PRODUCTION_PASSWORD];MultipleActiveResultSets=true;`

**Audit Finding:** The connection strings target `Server=.` (localhost) with standard SQL Server Authentication. This requires SQL Server to be running on the same host where IIS is deployed. Connection strings are structurally valid.

---

## 2. Token & Security Key Configurations

- **JWT Configuration Secret:**
  - Audited security signature UUID configurations mapped in the JWT settings.
  - **Security Assessment:** Keys are statically declared in configuration files. To enforce live production security, we highly recommend rotating these keys using environment variable injections (e.g. `ASPNETCORE_JWTCONFIG__SECRET`) during IIS hosting.

---

## 3. Environment & Local Infrastructure Dependencies

### 3.1. Local Redis Configuration
- **Caching Host:** `Server: localhost`, `Port: 6379`.
- **Dependency:** Requires an active Redis Server instance running on port 6379 on the local host.

### 3.2. Web Hosting Parameters (`web.config`)
- Audited the `web.config` layout, which correctly declares the `AspNetCoreModuleV2` with `hostingModel="inprocess"` targeting `Amlakbashi.Host.exe`.
- **Status:** Valid and ready for standard IIS hosting.

### 3.3. Hardcoded File Storage Drive
- **Drive Setting:** `GeneralData.VideosDirectoryDrive`
- **Default Value:** `E:/videos`
- **Security Assessment:** This absolute path requires the hosting Windows Server to contain an active physical or mapped `E:\` partition. If deployed on Linux, Kestrel will immediately crash with a `PhysicalFileProvider` exception (which can be bypassed by setting `ASPNETCORE_ENVIRONMENT=Development` and pre-creating `/app/wwwroot/content/videos`).
