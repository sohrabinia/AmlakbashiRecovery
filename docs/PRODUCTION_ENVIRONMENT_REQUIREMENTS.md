# AmlakBashi Recovery — Production Environment Requirements

This document outlines the detailed system configurations, runtime packages, databases, services, and directory specifications required to host and run the recovered **AmlakBashi** application in a live production environment.

---

## 1. Application Runtime Specification

- **Target Framework:** `.NET 5.0` (Out of support. To deploy as-is, the **ASP.NET Core Runtime 5.0 Hosting Bundle** must be installed on the host. Alternatively, the reconstructed source code can target `.NET 8.0` for native compatibility).
- **Web Server:** IIS (Internet Information Services) version 10.0+ on Windows Server 2016/2019/2022. IIS must be configured with **InProcess hosting** for Kestrel.

---

## 2. Infrastructure & Middleware Requirements

### 2.1. SQL Server Database Engine
- **Requirement:** Microsoft SQL Server 2016, 2019, or 2022.
- **Capacity:** At least 10 GB of storage for listing schemas, media references, logs, and authentication tables.
- **Target Schemas (3 Contexts):**
  1. `amlakbas_db` (Core business data)
  2. `Amlakbashi_jdb` (Background job registrations)
  3. `Amlakbashi.Identity` (Authentication and membership tables)

### 2.2. Redis Server (Distributed Cache)
- **Requirement:** Redis Server version 6.0 or higher.
- **Connection Parameters:** Localhost (`127.0.0.1`) on Port `6379`.
- **Roles:** Active distributed cache, state caching, session management, and transaction queues.

---

## 3. Directory and File Storage Requirements

- **Media Storage Path:**
  - **Windows IIS Default:** Physical absolute drive directory mapped under `E:/videos` (governed by `GeneralData.VideosDirectoryDrive` in settings).
  - **Static Asset Path:** `/wwwroot/content/` (for listing pictures and thumbnail resources).
  - **Permission:** The IIS Application Pool identity (`IIS_IUSRS`) must have full **Read, Write, and Modify** NTFS permissions on the target directory.

---

## 4. External Services & APIs

- **Firebase Admin SDK:**
  - **Service Role:** FCM push messaging and notification delivery.
  - **Credentials:** Requires an active `project_id: amlakbashi-7e6b2` Google Service Account private key JSON file mapped in the application layout.
- **Kavenegar SMS Gateway:**
  - **Service Role:** User sign-up verification codes, Booking vouchers, and transactional notifications.
  - **Credentials:** Registered active Kavenegar API key.
