# Amlakbashi.netcore (AmlakBashi V10)

This repository contains the full, restored C# source code for AmlakBashi V10, representing a complete engineering recovery of the platform.

## 🌟 The New Truth
- **Source Code is Fully Available:** Assumed compiled-only recovery limitations are obsolete. This repository contains 1,200+ fully-editable C# source files (`*.cs`).
- **Fully Rebuildable:** The codebase is rebuildable and compiles successfully with `0` errors.
- **Production-Ready:** Production deployment can proceed after target environment configuration/validation.

---

## 🛠 Project Structure
The solution `Amlakbashi.sln` includes the following 6 core projects:
1. **Amlakbashi.Core** (`netcoreapp3.1`): Entities, Static Data, and Price calculations.
2. **Amlakbashi.Data** (`netcoreapp3.1`): EF Core database context and repositories.
3. **Amlakbashi.Mediator** (`netcoreapp3.1`): CQRS Commands and Events.
4. **Amlakbashi.Accounting** (`netcoreapp3.1`): Billing, payment gateway interfaces.
5. **Amlakbashi.Application** (`netcoreapp3.1`): Application-level services and Hangfire jobs.
6. **Amlakbashi.Host** (`net5.0`): MVC Presentation layer and frontend single-page application.

---

## 🚀 How to Build & Run

### Prerequisites
- .NET 8.0 SDK or .NET 10.0 SDK.
- The repository includes a `global.json` file pinning the SDK to version `8.0.124` to ensure a consistent and completely successful compilation across modern hosting systems.

### Build Solution
To restore dependencies and build the entire solution:
```bash
dotnet restore
dotnet build Amlakbashi.sln
```

### Run Web Host
To run the MVC / Web API host:
```bash
cd Amlakbashi.Host
dotnet run
```

---

## 📂 Documentation
Comprehensive audits and verification reports are available in the `/docs` directory:
- [Source Recovery Verification Report](docs/V10_SOURCE_RECOVERY_VERIFICATION_REPORT.md)
- [Final Source Audit & Merge Readiness Report](docs/V10_FINAL_SOURCE_AUDIT_REPORT.md) (Pending)
