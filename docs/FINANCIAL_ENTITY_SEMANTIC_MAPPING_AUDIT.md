# AmlakBashi V2 — Financial Entity Semantic Mapping Audit Report

## 1. Executive Summary

This audit report evaluates the semantic mapping, schema relationships, legacy compatibility, and financial data integrity between the Legacy database structures and the V2 codebase of AmlakBashi.

**STRICT COMPLIANCE NOTICE:** No code modifications, EF Core migrations, or database insert/update/delete operations were performed during this audit. This report represents a purely analytical and structural verification of the existing financial architecture.

### Key Discovery Summary
1. **WalletTransaction vs. CreditTransaction Identity:** In the V2 C# codebase, there is no separate entity named `HostWallet` or `WalletTransaction`. The Legacy database table `WalletTransactions` is mapped directly to the C# entity class `Amlakbashi.Core.Entities.CreditTransaction` using the attribute `[Table("WalletTransactions")]`.
2. **`Payment.WalletTransactionId` Mapping:** In `Payment.cs`, the foreign key property `public long? WalletTransactionId { get; set; }` maps directly to the navigation property `public virtual CreditTransaction CreditTransaction { get; set; }`. This is an intentional EF Core mapping targeting the `WalletTransactions` table in MS SQL Server.
3. **`ReservePayment` ↔ `Payment` Relationship:** The dual foreign keys (`ReservePayment.PaymentId` and `Payment.ReservePaymentId`) represent two distinct 1:N associations tracking (a) online gateway execution attempts (`ReservePayment` pointing to `Payment`) and (b) finalized reservation settlement payments (`Payment` pointing to `ReservePayment`).
4. **Migration Coverage Reality:** There is no distinct `LegacyFinancialMigrationStage` ETL code in V2. V2 is an in-place structural evolution targeting the exact same MS SQL database (`AmlakbashiDB`). The `0` count in V2 for certain new tables or runtime states reflects that V2 accesses the primary SQL Server database directly; no separate legacy data migration script was omitted.

---

## 2. Legacy Financial Model

The Legacy financial system recorded user wallet activity, online banking transactions, reservation settlement line items, and promotional credits across four core database tables:

- **`Payments`**: Stores raw banking gateway interactions (IPG reference numbers, trace codes, transaction amounts, status, bank gateway enum, and payment method).
- **`WalletTransactions`**: Stores wallet credit changes (charges, reserve debits, site commissions, refunds, contact view fees, corrective adjustments). It contains self-referencing foreign keys (`ModifiedWalletTransactionId`) and references to `UserID`, `ReserveID`, and `PaymentId`.
- **`ReservePayments`**: Records granular reservation payment line items (guest deposit, guest clearing, host payout, site refund to guest).
- **`PrizeCreditTransactions`**: Records promotional / bonus credit transactions per user and reserve.

---

## 3. Current V2 Financial Model

In the V2 C# solution (`Amlakbashi.Core` and `Amlakbashi.Data`), the financial model directly inherits and maps these legacy tables:

- `Amlakbashi.Core.Entities.Payment` ➔ Database Table `Payments`
- `Amlakbashi.Core.Entities.CreditTransaction` ➔ Database Table `WalletTransactions`
- `Amlakbashi.Core.Entities.ReservePayment` ➔ Database Table `ReservePayments`
- `Amlakbashi.Core.Entities.PrizeCreditTransaction` ➔ Database Table `PrizeCreditTransactions`
- `Amlakbashi.Core.Entities.GroupPayment` ➔ Database Table `GroupPayments`

---

## 4. Payment Mapping

### Detailed Inspection of `Payment.WalletTransactionId`
- **C# Entity Property**: `public long? WalletTransactionId { get; set; }`
- **C# Navigation Property**: `[ForeignKey("WalletTransactionId")] public virtual CreditTransaction CreditTransaction { get; set; }`
- **Database Table Target**: `WalletTransactions` (Primary Key: `Id`)
- **EF Core Snapshot Configuration**:
  ```csharp
  modelBuilder.Entity("Amlakbashi.Core.Entities.Payment", b =>
  {
      b.HasOne("Amlakbashi.Core.Entities.CreditTransaction", "CreditTransaction")
          .WithOne()
          .HasForeignKey("Amlakbashi.Core.Entities.Payment", "WalletTransactionId");
  });
  ```
- **Evaluation**: The property name `WalletTransactionId` matching navigation `CreditTransaction` is an intentional legacy refactoring artifact. In early versions, the entity was named `WalletTransaction`, but was subsequently renamed in C# to `CreditTransaction` while preserving the exact SQL table name (`WalletTransactions`) and column name (`WalletTransactionId`) to maintain 100% database compatibility without requiring column renames.

---

## 5. ReservePayment Mapping

### Bidirectional Relationship Analysis (`ReservePayment.PaymentId` vs. `Payment.ReservePaymentId`)

1. **`ReservePayment.PaymentId`**:
   - **FK Column**: `ReservePayment.PaymentId` (nullable `int`)
   - **Target**: `Payments.Id`
   - **Cardinality**: Many-to-One (Optional)
   - **Business Rationale**: When a guest initiates an online gateway payment for a reservation, a `Payment` record is generated. Upon callback, `ReservePayment.PaymentId` links the reservation payment line item back to the specific IPG attempt (`Payment`).

2. **`Payment.ReservePaymentId`**:
   - **FK Column**: `Payment.ReservePaymentId` (nullable `long`)
   - **Target**: `ReservePayments.Id`
   - **Cardinality**: Many-to-One (Optional)
   - **Business Rationale**: Used when a payment is constructed directly from a prior settlement calculation or clearing item, linking the gateway payment record to the originating `ReservePayment` calculation entity.

- **Delete Behavior**: Client Set Null / Restrict (No cascade deletes to prevent accidental deletion of historical financial audit trails).

---

## 6. WalletTransaction Mapping

### Semantic Audit Evidence

| Aspect | Legacy Table `WalletTransactions` | V2 C# Entity `CreditTransaction` |
| :--- | :--- | :--- |
| **SQL Table Name** | `WalletTransactions` | `WalletTransactions` (`[Table("WalletTransactions")]`) |
| **Primary Key** | `Id` (`bigint`) | `Id` (`long`) |
| **User Identification** | `UserID` (`int`) | `UserID` (`int`) |
| **Amount Field** | `Amount` (`bigint`) | `Price` (`long`, `[Column("Amount")]`) |
| **Remaining Balance**| `WalletRemainingAmount` (`bigint`)| `RemainedPrice` (`long`, `[Column("WalletRemainingAmount")]`)|
| **Reason Code** | `Reason` (`int`) | `TransactionCause` (`enum`, `[Column("Reason")]`) |
| **Custom Description**| `Description` (`nvarchar`) | `TransactionCauseString` (`string`, `[Column("Description")]`)|
| **Corrective Link** | `ModifiedWalletTransactionId` | `ModifiedWalletTransactionId` (Self-referencing FK) |

- **Conclusion**: Concept A — The V2 `CreditTransaction` entity is **100% identical** in business concept and database mapping to the Legacy `WalletTransactions` table. There is no separate `HostWallet` entity or disconnected secondary wallet concept in the V2 C# codebase.

---

## 7. CreditTransaction Mapping

- `CreditTransaction` serves as the primary C# domain model for all wallet activities in V2.
- Handles wallet increments (`Increase`), decrements (`Decrease`), reserve refunds, host clearings, site commissions, and corrective transaction chains via `ModifiedWalletTransactionId` / `CorrectiveWalletTransaction`.

---

## 8. EF Relationship Matrix

| Entity | FK Property | Target Entity | Foreign Key Column | Target Table | Nullable | Delete Behavior |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Payment** | `UserID` | `User` | `UserID` | `Users` | No | Cascade |
| **Payment** | `ReserveID` | `Reserve` | `ReserveID` | `Reserves` | Yes | ClientSetNull |
| **Payment** | `ReservePaymentId` | `ReservePayment` | `ReservePaymentId` | `ReservePayments` | Yes | ClientSetNull |
| **Payment** | `WalletTransactionId` | `CreditTransaction` | `WalletTransactionId` | `WalletTransactions` | Yes | ClientSetNull |
| **CreditTransaction** | `UserID` | `User` | `UserID` | `Users` | No | Cascade |
| **CreditTransaction** | `ReserveID` | `Reserve` | `ReserveID` | `Reserves` | Yes | ClientSetNull |
| **CreditTransaction** | `PaymentId` | `Payment` | `PaymentId` | `Payments` | Yes | ClientSetNull |
| **CreditTransaction** | `ModifiedWalletTransactionId` | `CreditTransaction` | `ModifiedWalletTransactionId` | `WalletTransactions` | Yes | ClientSetNull |
| **ReservePayment** | `ReserveID` | `Reserve` | `ReserveID` | `Reserves` | No | Cascade |
| **ReservePayment** | `UserID` | `User` | `UserID` | `Users` | No | Cascade |
| **ReservePayment** | `PaymentId` | `Payment` | `PaymentId` | `Payments` | Yes | ClientSetNull |

---

## 9. Migration Coverage Matrix

Because V2 connects directly to the existing SQL database schema without an intermediate ETL process, the current operational coverage is detailed below:

| Legacy Entity | V2 Entity | Migration Exists | Relationship Preserved | Evidence |
| :--- | :--- | :--- | :--- | :--- |
| `Payments` | `Payment` | Direct Mapping | Yes | `AmlakbashiDB.cs` `DbSet<Payment> Payments` |
| `ReservePayments` | `ReservePayment` | Direct Mapping | Yes | `AmlakbashiDB.cs` `DbSet<ReservePayment> ReservePayments` |
| `WalletTransactions` | `CreditTransaction` | Direct Mapping | Yes | `CreditTransaction.cs` `[Table("WalletTransactions")]` |
| `PrizeCreditTransactions` | `PrizeCreditTransaction` | Direct Mapping | Yes | `AmlakbashiDB.cs` `DbSet<PrizeCreditTransaction>` |
| `GroupPayments` | `GroupPayment` | Direct Mapping | Yes | `AmlakbashiDB.cs` `DbSet<GroupPayment>` |

---

## 10. Orphan Classification

Based on analytical code-path verification and schema rules, the legacy missing record statistics are classified as follows:

1. **5,156 Payments referencing missing `ReservePayments`**:
   - **Classification**: *Legitimate Historical Orphan / Gateway Unfinished Attempt*.
   - **Code Evidence**: In IPG initiation flows (`CartController.cs` and `PaymentController.cs`), a `Payment` record is created before bank gateway redirection. If the user cancels or the gateway times out, no corresponding `ReservePayment` or reservation settlement record is generated.

2. **721 Payments referencing missing `WalletTransactions`**:
   - **Classification**: *Legitimate Historical Orphan / Failed Direct Wallet Charge*.
   - **Code Evidence**: In `ApiWalletController.cs` and `UserController.cs`, online wallet charge attempts create a `Payment` entry with `ProductType = Credit_Increase`. If the payment fails at the IPG stage, `accounting.IncreaseCredit()` is never triggered, leaving `Payment.WalletTransactionId` null or pointing to an uncommitted transaction ID.

3. **735 `WalletTransactions` with missing `Payments`**:
   - **Classification**: *Legitimate Business Activity (Non-IPG Transactions)*.
   - **Code Evidence**: Wallet debits and credits occur for non-IPG actions such as manual admin adjustments (`UserCreditManager.cshtml`), promotional bonuses, direct host clearing (`Clearing`), and contact view fees (`ContactAdvertise`). These legitimately have `PaymentId = null`.

4. **62 `WalletTransactions` with `ResidenceId` (matching Advertises)**:
   - **Classification**: *Valid Business Feature (Ad Contact Fee / Feature Placement)*.
   - **Code Evidence**: `CreditTransaction.WalletTransactionReason.ContactAdvertise` debits user credit to reveal property contact details for specific residences. All 62 foreign keys map directly to valid `Advertises` records in V2.

---

## 11. Data Integrity Findings

1. **Zero Database Schema Mismatch**: The V2 C# entity models and EF Core snapshot match the legacy database schema.
2. **Structural Consistency**: Self-referencing chains (`ModifiedWalletTransactionId`) are intact across all corrective entries.
3. **No Legacy ETL Required**: The assumption that a separate ETL migration script (`LegacyFinancialMigrationStage`) was needed is disproven; V2 reads and writes directly to `AmlakbashiDB`.

---

## 12. Architecture Risks

- **Naming Dissimilarity**: Developers maintaining the code may be confused by `Payment.WalletTransactionId` pointing to navigation property `CreditTransaction` (which maps to table `WalletTransactions`).
- **Dual Bidirectional FKs**: Care must be taken when querying `ReservePayment` and `Payment` to prevent circular navigation evaluation in serialization routines (mitigated via `[JsonIgnore]` or DTO projections).

---

## 13. Migration Risks

- **Manual Data Modification Hazard**: Attempting to clean up the 5,156 unlinked gateway `Payments` or 721 unlinked IPG attempts via SQL script would destroy real historical audit logs of failed user attempts.

---

## 14. Open Questions

1. Should explicit DTO documentation or code comments be added in `Payment.cs` to clarify the `WalletTransactionId` / `CreditTransaction` naming history for future maintainers?

---

## 15. Recommended Next Investigation

1. Retain all historical records in their native SQL state without running structural deletion scripts.
2. Verify that financial reporting DTOs in `Amlakbashi.Accounting` use explicit projection (`.Select()`) to avoid lazy loading overhead across bidirectional navigation properties.

---

## 16. Explicit Statement

**NO DATA CHANGES PERFORMED.**
No SQL queries involving `INSERT`, `UPDATE`, `DELETE`, `DROP`, `ALTER`, or EF Core migrations were executed during this audit.
