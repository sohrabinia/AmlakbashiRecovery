# AmlakBashi V10 Production Access Status Report

This document assesses deployment capability on production servers and identifies remaining infrastructure and credential blockers.

---

## 1. Production Access Capability Assessment

An evaluation of system access and control permissions within this sandboxed development workspace has been conducted:

### 1.1. Application Server (IIS / Windows Server Host)
* **Live Server Access (SSH/FTP):** **UNAVAILABLE** (No direct remote terminal or file-transfer capabilities to the hosting server).
* **IIS Manager Access:** **UNAVAILABLE** (No administrative controls over the live IIS Application Pool `AmlakbashiPool` or physical site path configurations).
* **Deployment Directory Permissions:** **UNAVAILABLE** (Read/Write capabilities are strictly isolated to this development container workspace; no network visibility into production server partitions).

### 1.2. Database Server (Microsoft SQL Server)
* **SQL Server Instance Connection:** **UNAVAILABLE** (Live database connection strings mapped to target server resources are isolated and unreachable from the sandbox environment; only staging/recovery local mocks could be run).
* **Backup/Restore Capability:** **UNAVAILABLE** (Direct permission to invoke `RESTORE DATABASE` or `BACKUP DATABASE` commands on the live SQL Server instance is missing).
* **Database Backup File Access:** **UNAVAILABLE** (The production database backup `amlakbas_db.bak` is physically absent from the development workspace, restricting direct offline restorations).

### 1.3. Configuration & Third-Party Integrations
* **Production Configurations:** Redacted and frozen in `appsettings.production.json`.
* **API Credentials & SMS Panels:** Live payment gateway secrets, SMS provider credentials, and Google API keys are isolated from the development pipeline to maintain security boundaries.

---

## 2. Classification Status

Based on the access assessment above, the deployment readiness status is classified as:

### **B) Deployment ready but waiting for production access**

#### Justification:
The V10 frontend presentation layer is fully stable, structurally isolated from affecting legacy compiled assemblies or active schemas, and completely packaged. However, physical server and database deployment execution cannot be performed directly from this development workspace due to the lack of production server network routes and administrative credentials.

---

## 3. Deployment Blocker Resolution Requirements

To enable successful on-site deployment, the IT administrators or system operations team must provide or configure the following:
1. **Host Server Administrative Privileges:** Remote desktop (RDP) or secure SSH access to the Windows Server/IIS 10.0 hosting platform.
2. **MSSQL Database Access:** High-privilege SQL logins (such as `sa` or equivalent with database backup/restore privileges) for target production databases.
3. **Operational Environment Secrets:** Verified API keys for active Persian SMS gateways (e.g., Kavenegar) and safe configuration profiles inside the production web directory.
