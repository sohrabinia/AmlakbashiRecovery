# AMLAKBASHI DEPLOYMENT GUIDE & PLATFORM EVOLUTION
## Phase 9 - 12: Host Platform, Search, CRM & DevOps Foundation

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** DevOps Engineer / Systems Architect
**Runtime Platform:** .NET 8 / Docker / Linux & Windows IIS (OutOfProcess)
**Status:** `READY FOR DEPLOYMENT`

---

## 1. Executive Summary

This document serves as the Deployment Guide and Systems Foundation manual for AmlakBashi V10 -> V2. It specifies host growth improvements, AI listing assist mechanisms, semantic search vector designs, idempotent CRM SMS notifications, and containerized DevOps deployment playbooks.

---

## 2. Host Platform & Additive AI Assistant (Phase 9)

### 2.1 Preserved Workflows
- **Listing Creation Wizard:** Multi-step registration (`/accomodation/accbasicform`) remains fully operational.
- **Admin Approval Workflow:** Moderation queue (`NewIndex`, `NotVerify`, rejection reason logging) remains strictly enforced.

### 2.2 Additive AI Property Description Assistant
- **Mode:** Optional, non-blocking input assist tool.
- **Input Example:**
  `"ویلای سه خواب استخردار نزدیک دریا"`
- **Structured Output Generation:**
  - `Title`: "ویلا ۳ خوابه استخردار نزدیک ساحل"
  - `Rooms`: 3
  - `Amenities`: `["Pool", "Parking", "SeaView"]`
  - `Description`: "اقامتگاه ویلایی دارای ۳ اتاق خواب، استخر سرپوشیده آبگرم..."
- **Rule:** AI assistance is strictly additive. Manual entry and editing workflows are never blocked or gated by AI.

---

## 3. Search Intelligence & Semantic Embedding Strategy (Phase 10)

- **Co-existence Architecture:** Current SQL Server text/location filtering (`AdvertisePage`, `RegionCity` filters) remains the primary query engine.
- **Vector Search Design:** Additive pgvector / Qdrant embedding layer for natural language search queries (e.g., "ویلا مناسب جاده چالوس با حیاط بزرگ").

---

## 4. CRM Automation & SMS Notification Architecture (Phase 11)

- **Abstraction Layer:** `ISmsProviderService` interface wrapping Kavenegar / Magfa providers.
- **Host Engagement Flow:** Automated SMS alerts upon new contact reveal events or admin approval status updates.
- **Operational Requirements:**
  - **Idempotent:** Deduplication via unique event ID.
  - **Rate Limited:** Max 3 SMS per user per 10 minutes.
  - **Auditable:** Logged in `[SmsLog]` table with provider delivery IDs.

---

## 5. DevOps & Containerized Deployment Guide (Phase 12)

### 5.1 Docker Configuration (`Dockerfile`)
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Amlakbashi.sln", "./"]
COPY ["Amlakbashi.Host/Amlakbashi.Host.csproj", "Amlakbashi.Host/"]
COPY ["Amlakbashi.Core/Amlakbashi.Core.csproj", "Amlakbashi.Core/"]
COPY ["Amlakbashi.Data/Amlakbashi.Data.csproj", "Amlakbashi.Data/"]
COPY ["Amlakbashi.Accounting/Amlakbashi.Accounting.csproj", "Amlakbashi.Accounting/"]
COPY ["Amlakbashi.Application/Amlakbashi.Application.csproj", "Amlakbashi.Application/"]
COPY ["Amlakbashi.Mediator/Amlakbashi.Mediator.csproj", "Amlakbashi.Mediator/"]
RUN dotnet restore "Amlakbashi.sln"
COPY . .
RUN dotnet build "Amlakbashi.sln" -c Release -o /app/build
RUN dotnet publish "Amlakbashi.Host/Amlakbashi.Host.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Amlakbashi.Host.dll"]
```

### 5.2 IIS OutOfProcess Deployment
1. Set IIS App Pool to `.NET CLR Version: No Managed Code`.
2. Configure `web.config` for `hostingModel="OutOfProcess"`.
3. Set environment variable `ASPNETCORE_ENVIRONMENT=Production`.
4. Ensure SQL Server connection strings (`AmlakbashiDB`, `JobDb`, `IdentityDB`) target production instances.

### 5.3 Rollback Procedure
If deployment verification fails:
1. Re-bind IIS website to previous physical path `/inetpub/amlakbashi_v10_baseline`.
2. Verify database connection string integrity.
3. Restart IIS App Pool `AmlakBashiPool`.
