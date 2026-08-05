# GitHub Recovery Inventory Report — AmlakBashi

This report presents a thorough, evidence-based inventory and structure discovery of the `AmlakbashiRecovery` repository cloned from GitHub. It establishes the current state of the recovered artifacts and defines the path forward for modernization.

---

## 1. Repository Status & Clone Analysis

- **Target Repository:** `sohrabinia/AmlakbashiRecovery` (main branch)
- **Analyzed Commit SHA:** `8b95b4124ea741ad75d6dd6e90bbe08bb32c6a39`
- **Branch Status:** Clean, fully synchronized with remote `main` branch.
- **Repository Size on Disk:** ~345 MB (Uncompressed workspace size including `.git` history of ~105 MB).
- **Clone Verification:** Successful. No packet loss or history issues encountered during acquisition.

---

## 2. File Inventory Summary (Source Code Scan)

We performed a deep search across the entire workspace (excluding hidden directories like `.git`) for files matching specific extensions. The exact counts are presented below:

| File Extension | Match Count | Description / Role |
| :--- | :--- | :--- |
| **`*.cs`** | **0** | C# raw source files are completely missing from the active workspace. |
| **`*.sln`** | **0** | Visual Studio solution files are completely missing from the active workspace. |
| **`*.csproj`** | **0** | MSBuild project definition files are completely missing from the active workspace. |
| **`*.dll`** | **432** | Compiled dynamic-link library files (includes core application assemblies, framework references, and third-party dependencies). |
| **`*.pdb`** | **11** | Debug symbols corresponding to compiled application assemblies. |
| **`*.exe`** | **1** | Main application host executable (`Amlakbashi.Host.exe`). |
| **`*.zip`** | **1** | Full offline backup and recovery archive (`Amlakbashi_Recovery.zip`). |
| **`*.json`** | **26** | Configuration and dependency manifests (includes `appsettings.json`, `appsettings.Development.json`, `appsettings.production.json`, `bundleconfig.json`, `libman.json`, `manifest.json`, and untracked `amlakbashi-7e6b2-firebase-adminsdk-h6gkp-0159f2aab7.json`). |
| **`*.config`** | **2** | IIS and logging configurations (`web.config` and `log4net.config`). |

---

## 3. Structural Discovery & Findings

### 3.1. Is Original C# Source Code Available?
**No.** There is absolutely no raw C# source code (`*.cs`) tracked or present in the current cloned workspace.

### 3.2. Is This a Compiled Recovery Archive?
**Yes.** This repository represents a fully-packaged **Compiled Recovery Archive**. It contains:
1. **Core Recovered Application Assemblies (7 major components):**
   - `Amlakbashi.Core.dll` & `Amlakbashi.Core.pdb`
   - `Amlakbashi.Data.dll` & `Amlakbashi.Data.pdb`
   - `Amlakbashi.Mediator.dll` & `Amlakbashi.Mediator.pdb`
   - `Amlakbashi.Application.dll` & `Amlakbashi.Application.pdb`
   - `Amlakbashi.Accounting.dll` & `Amlakbashi.Accounting.pdb`
   - `Amlakbashi.Host.dll` & `Amlakbashi.Host.pdb` & `Amlakbashi.Host.exe`
   - `Amlakbashi.Host.Views.dll` & `Amlakbashi.Host.Views.pdb`
2. **Framework & Dependency Assemblies:**
   - Standard MVC framework libraries under `refs/` directory.
   - Platform-specific runtimes under `runtimes/` directory.
   - External library dependencies (such as `AutoMapper.dll`, `Autofac.dll`, `Hangfire.Core.dll`, `ServiceStack.Redis.dll`, etc.) tracked in the root.
3. **Deployment Configurations & Assets:**
   - Full deployment layouts, static front-end assets (under `wwwroot/`), and translation resources (`fa/`, `ca/`, `de/`, etc.).

### 3.3. Are Project Files Available?
**No.** Visual Studio project files (`*.csproj`) and solution files (`*.sln`) are completely missing from the workspace.

### 3.4. Can the Solution Theoretically Be Rebuilt From Source?
**No.** In its current state, the solution cannot be built from source because there are no C# source files or MSBuild structure. The repository is suitable for binary-based deployment and execution, but direct source-code modifications are blocked.

---

## 4. Evidence-Based Next Step Recommendations

Based strictly on the physical evidence discovered during the audit, we evaluate the next phase:

### **Recommended Action: B) Decompilation recovery**

#### Direct Evidence Justification:
- Since we have the compiled application assemblies (`Amlakbashi.*.dll`) and their corresponding symbol files (`*.pdb`) but lack raw `.cs` and `.csproj` structures, a **Decompilation Recovery Phase** is the mandatory first step.
- By using `.NET` decompilers (e.g., `ICSharpCode.Decompiler`, `ILSpy`, or `dnSpy`), we can reconstruct:
  1. The complete C# class hierarchy, controller structures, and business logic from the compiled assemblies.
  2. The Visual Studio solution (`.sln`) and project files (`.csproj`) referencing the required .NET 5.0 target framework and dependencies.
- Once the C# source is fully reconstructed, the team can immediately proceed to **A) Build validation**, followed by **C) Database restoration** and **D) Architecture migration preparation** (e.g., migrating from retired .NET 5.0 to modern long-term support versions like .NET 8.0/10.0).
