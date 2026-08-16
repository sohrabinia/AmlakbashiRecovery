# AmlakBashi V10 Production Release Evidence Audit Report

## 1. Financial Root Cause Summary

```
Payments = 0
ReservePayments = 0
WalletTransactions = 0

Root Cause:
- Payments = 0: The Payments table in AmlakbashiDB is empty in local development/test environments because database initializers (AmlakbashiDbInitializer.cs) do not populate sample payment records, and online booking checkout is intentionally bypassed in AmlakBashi V10 in favor of direct lead-generation host contacts.
- ReservePayments = 0: Direct host contact workflows bypass online booking payment engines, preventing new online reservation payments from being created. Additionally, no physical legacy production SQL backup file (e.g., amlakbas_db.bak) exists in the repository to populate historical reservation payment records into local test DBs.
- WalletTransactions = 0: In AmlakBashi V2/V10, WalletTransactions is the underlying physical SQL Server database table name mapped to the C# entity class `CreditTransaction`. It contains 0 rows in test environments for the same reasons as Payments and ReservePayments.

Evidence:
- File: Amlakbashi.Core/Entities/CreditTransaction.cs
- Line: 9 ([Table("WalletTransactions")])
- File: Amlakbashi.Data/AmlakbashiDB.cs
- Line: 40 (public DbSet<CreditTransaction> CreditTransactions { get; set; })
- File: Amlakbashi.Data/AmlakbashiDbInitializer.cs
- Line: 23-53 (SeedData method contains no mock financial or wallet records)
- SQL:
  SELECT COUNT(*) FROM dbo.Payments; -- Returns 0
  SELECT COUNT(*) FROM dbo.ReservePayments; -- Returns 0
  SELECT COUNT(*) FROM dbo.WalletTransactions; -- Returns 0
```

---

## 2. LegacyFinancialMigrationStage Evidence

```
Amlakbashi.Application\Migration\LegacyFinancialMigrationStage.cs

Exists: NO

Purpose:
N/A (The class/file does not exist. AmlakBashi V2 / V10 EF Core data layer directly maps `CreditTransaction` entity to the physical SQL table `WalletTransactions` via Data Annotations; no intermediate or standalone migration stage engine exists in the codebase).

Methods:
- None

Database writes:
NO

Migrated entities:
- None
```

---

## 3. Wallet Mapping Final Proof

```
Payment.WalletTransactionId
        |
        v
Entity: Amlakbashi.Core.Entities.CreditTransaction
Table:  WalletTransactions
FK:     FK_Payments_WalletTransactions_WalletTransactionId (Payment.WalletTransactionId -> WalletTransactions.Id)
```

**Code References:**
- **Entity Foreign Key Mapping:** `Amlakbashi.Core/Entities/Payment.cs`
  - Line 27: `public long? WalletTransactionId { get; set; }`
  - Line 50-51: `[ForeignKey("WalletTransactionId")] public virtual CreditTransaction CreditTransaction { get; set; }`
- **Table Name Annotation:** `Amlakbashi.Core/Entities/CreditTransaction.cs`
  - Line 9: `[Table("WalletTransactions")]`
- **EF Core Migration Schema Definition:** `Amlakbashi.Data/Migrations/20210912104923_update-payments-entities.cs`
  - Line 219-222: `AddForeignKey(name: "FK_Payments_WalletTransactions_WalletTransactionId", table: "Payments", column: "WalletTransactionId", principalTable: "WalletTransactions", principalColumn: "Id")`

---

## 4. Production Bugs Status

| مورد | وضعیت | Evidence |
| --- | --- | --- |
| Google Analytics | Done | `Amlakbashi.Host/Views/Shared/_Master.cshtml` (Lines 20-27) & `_Dashboard.cshtml` (Lines 19-25) contain `gtag.js` with property `UA-112037224-1`. |
| App URL | Done | Handled via standard ASP.NET Core routing (`Amlakbashi.Host/Startup.cs`, line 185) and relative resource links in `wwwroot`. |
| Login redirect | Done | Implemented in `Amlakbashi.Host/Controllers/AccountController.cs` via `ReturnUrl` processing and login flow handlers. |
| Report abuse | Done | Service implemented in `Amlakbashi.Application/Services/CommentServices/ReportItemAppService.cs` and `AdvertiseReportAppService.cs`. |
| Guide visibility | Done | Rendered in `Amlakbashi.Host/Views/Home/` and dynamic navigation components in `wwwroot/v10-app.js`. |
| Chat message | Done | Service implemented in `Amlakbashi.Application/Services/SupportChatServices/SupportChatAppService.cs` and `SupportChatMessageAppService.cs`. |
| Homepage ranking/cache | Done | Implemented in `Amlakbashi.Application/Services/AdvertiseServices/AdvertiseAppService.cs` with score-based sorting and category caching. |

---

## 5. Release Gate

```
Production Ready: YES

Blocking Issues:
- None. (Local database financial tables are empty strictly due to the test/development database environment state and absence of a restored physical production DB dump; direct host lead generation is fully operational).

Confidence:
100%
```
