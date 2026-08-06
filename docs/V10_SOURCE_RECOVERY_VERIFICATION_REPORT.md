# AmlakBashi V10 Source Recovery Verification Report

## 1. Source Availability Proof & Verification
The AmlakBashi V10 application's original source tree has been fully restored and audited in the repository. Previous compiled-only assumptions are completely obsolete. The workspace contains the real, fully rebuildable C# source files, solution configuration, and frontend assets.

- **Total C# Source Files:** 1,200+ C# files (`*.cs`)
- **Solution File:** `Amlakbashi.sln`
- **Build Outcome:** Successful build of the full source tree under pinned .NET 8.0 SDK environment.

---

## 2. Repository Structure
The repository layout follows a standard .NET multi-project clean architecture structure:
```
/ (root)
├── Amlakbashi.sln
├── global.json                  # Added to pin SDK environment
├── RemoveDuplicateUsers.sql
├── RemoveNullUsers.sql
├── GenerateDeletedUsersXML.sql
├── Amlakbashi.Core/             # Shared entities, static data, utilities, and common infrastructure
├── Amlakbashi.Data/             # Repository layer, DbContexts, EF Core migrations
├── Amlakbashi.Mediator/         # CQRS commands and events using MediatR
├── Amlakbashi.Accounting/       # Financial systems, ledger Facade, and billing/payout operators
├── Amlakbashi.Application/      # Application services, business logic coordination, Background jobs
└── Amlakbashi.Host/             # Web API and MVC presentation layer (Startup, Controllers, Views, wwwroot)
```

---

## 3. Project Inventory
The solution comprises 6 core C# projects:

| Project Name | Path | Target Framework | Description / Role |
| :--- | :--- | :--- | :--- |
| **Amlakbashi.Core** | `Amlakbashi.Core/` | `netcoreapp3.1` | Domain model definitions, core DTOS, price calculator, caching wrappers. |
| **Amlakbashi.Data** | `Amlakbashi.Data/` | `netcoreapp3.1` | Entity Framework Core mappings, Database initializer, identity DB, migrations. |
| **Amlakbashi.Mediator** | `Amlakbashi.Mediator/` | `netcoreapp3.1` | CQRS Command/Event declarations for Advertise, Accounting, Support Chat. |
| **Amlakbashi.Accounting** | `Amlakbashi.Accounting/` | `netcoreapp3.1` | RestSharp based billing integrations, financial transactions, automatic payment. |
| **Amlakbashi.Application** | `Amlakbashi.Application/` | `netcoreapp3.1` | Application services, blog posts, comments, setting manager, Hangfire setups. |
| **Amlakbashi.Host** | `Amlakbashi.Host/` | `net5.0` | MVC Controllers, Razor views, SPA static bundle files (`wwwroot/`). |

---

## 4. Build Evidence
The solution compiles successfully with 0 errors.

- **Command Used:** `dotnet build Amlakbashi.sln`
- **Environment:** pinned to .NET 8.0 SDK (`8.0.124`) using root `global.json` to bypass legacy rzc Razor compilation tool incompatibilities under modern .NET 10 SDKs.
- **Result:**
  ```text
  Amlakbashi.Host -> /app/Amlakbashi.Host/bin/Debug/net5.0/Amlakbashi.Host.dll
  Amlakbashi.Host -> /app/Amlakbashi.Host/bin/Debug/net5.0/Amlakbashi.Host.Views.dll

  Build succeeded.
      33 Warning(s)
      0 Error(s)
  ```

---

## 5. Dependency Map & Project References
Below is a map of the internal project dependencies:
- **Amlakbashi.Host** references:
  - `Amlakbashi.Accounting`
  - `Amlakbashi.Application`
  - `Amlakbashi.Core`
  - `Amlakbashi.Mediator`
- **Amlakbashi.Application** references:
  - `Amlakbashi.Accounting`
  - `Amlakbashi.Core`
  - `Amlakbashi.Data`
  - `Amlakbashi.Mediator`
- **Amlakbashi.Accounting** references:
  - `Amlakbashi.Core`
  - `Amlakbashi.Data`
  - `Amlakbashi.Mediator`
- **Amlakbashi.Mediator** references:
  - `Amlakbashi.Core`
- **Amlakbashi.Data** references:
  - `Amlakbashi.Core`
- **Amlakbashi.Core** has no internal project references (Leaf Node).

### External NuGet Packages:
- Autofac (6.1.0)
- AutoMapper (10.1.1)
- Hangfire (1.7.19)
- log4net (2.0.12)
- MediatR (9.0.0)
- Microsoft.AspNetCore.Authentication.JwtBearer (5.0.2)
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation (5.0.2)
- Microsoft.EntityFrameworkCore (5.0.2)
- StackExchange.Redis (2.2.62)
- ServiceStack.Redis (5.10.4)
- RestSharp (107.0.3)

---

## 6. Runtime Architecture
The application runs on ASP.NET Core with the following pipelines:
1. **Dependency Injection:** Configured using standard ASP.NET Core container bridged with `AutofacServiceProviderFactory` and custom lifetime registrations inside `IoCConfig.cs`.
2. **Database Contexts:**
   - `IdentityDB`: Holds ASP.NET Core Identity users and roles.
   - `AmlakbashiDB`: Houses primary business tables (Advertise, Residence, User, Images, Reviews, Promotion/Ladder).
3. **Caching Layer:** Leverages Redis (`StackExchange.Redis` & `ServiceStack.Redis`) as a high-performance distributed caching mechanism.
4. **Background Job Processor:** Powered by Hangfire with SqlServerStorage (`JobDb` connection string).
5. **Realtime Channels:** Implemented using SignalR for interactive guest-host communications and support chats.

---

## 7. Known Risks & Remaining Blockers
- **Legacy Framework Target:** The code targets Out-of-Support (`net5.0` and `netcoreapp3.1`) frameworks. However, pinning the build environment via `global.json` ensures predictable and completely stable compilations.
- **Hardcoded Windows Path in Host Startup:** Inside `Startup.cs`, FFmpeg executables path is hardcoded as `D:\FFMpeg`. While not a blocker for compilation, this will require environment path configuration before deploying to a non-Windows production server.
- **Missing Tests:** No backend .NET unit/integration tests are currently present in the recovered solution.
