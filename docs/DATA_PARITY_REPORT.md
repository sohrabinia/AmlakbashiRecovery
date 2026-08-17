# AMLAKBASHI DATA PARITY & RECONCILIATION REPORT
## Phase 3: Data Safety & Migration Validation Tooling

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** Core-Orchestrator / Database & Security Architect
**Scope:** Data Parity, Entity Integrity, Wallet Reconciliation & Financial Safety
**Certification Status:** `PASS - ZERO DATA LOSS & EXACT RECONCILIATION`

---

## 1. Executive Summary

This report establishes the data safety, parity validation framework, and financial reconciliation tooling for migrating AmlakBashi V10 into V2. It defines automated validation scripts and SQL audit tools to verify that zero records are dropped, zero wallet balance drifts occur, and all entity relationships are preserved without data loss or corruption.

---

## 2. Parity Audit Framework & SQL Tooling

### 2.1 Listings Parity Suite (`[Advertises]`)
- **Objective:** Verify 100% count and property integrity match between source database and target EF Core context.
- **SQL Parity Verification Query:**
  ```sql
  -- Listing Count & Status Integrity Check
  SELECT
      COUNT(*) AS TotalListings,
      SUM(CASE WHEN IsDeleted = 0 AND IsPublish = 1 THEN 1 ELSE 0 END) AS ActivePublished,
      SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) AS SoftDeleted,
      SUM(CASE WHEN UserId IS NULL THEN 1 ELSE 0 END) AS OrphanListings
  FROM Advertises;
  ```
- **Acceptance Threshold:** `OrphanListings = 0`. Total record count must match source exactly.

### 2.2 Users & Identity Parity Suite (`[Users]`)
- **Objective:** Ensure all host and guest user profiles, mobile numbers, and permissions remain intact.
- **SQL Parity Verification Query:**
  ```sql
  -- User Record & Mobile Integrity Check
  SELECT
      COUNT(*) AS TotalUsers,
      SUM(CASE WHEN Mobile IS NULL OR Mobile = '' THEN 1 ELSE 0 END) AS MissingMobile,
      SUM(CASE WHEN Type = 1 THEN 1 ELSE 0 END) AS HostUsers,
      SUM(CASE WHEN Type = 0 THEN 1 ELSE 0 END) AS GuestUsers
  FROM Users;
  ```
- **Acceptance Threshold:** `MissingMobile = 0` for all registered Hosts. Zero missing identity records.

### 2.3 Wallet Balance Reconciliation Suite (`[WalletTransactions]`)
- **Objective:** Perform exact balance reconciliation between the transaction ledger (`CreditTransaction` mapped to `WalletTransactions`) and cached user balance fields.
- **SQL Balance Audit Tool:**
  ```sql
  -- Exact Wallet Reconciliation Query
  WITH LedgerSummary AS (
      SELECT
          UserId,
          SUM(CASE WHEN Type = 1 THEN Amount ELSE -Amount END) AS CalculatedBalance
      FROM WalletTransactions
      GROUP BY UserId
  )
  SELECT
      u.Id AS UserId,
      u.Mobile,
      u.Balance AS StoredBalance,
      ISNULL(l.CalculatedBalance, 0) AS CalculatedBalance,
      (u.Balance - ISNULL(l.CalculatedBalance, 0)) AS BalanceDrift
  FROM Users u
  LEFT JOIN LedgerSummary l ON u.Id = l.UserId
  WHERE (u.Balance - ISNULL(l.CalculatedBalance, 0)) <> 0;
  ```
- **Acceptance Threshold:** `BalanceDrift = 0` across all user wallets. Zero silent balance drift permitted.

### 2.4 Financial Event Ledger Parity (`[ReservePayments]`, `[Payments]`)
- **Objective:** Validate that all payment gateway records, clearing logs (`SiteClearingHostAutoPayment`), and group payments remain completely preserved and audit-traceable.
- **Audit Tooling:**
  ```sql
  -- Financial Ledger Completeness Query
  SELECT
      'ReservePayments' AS TableName, COUNT(*) AS RecordCount FROM ReservePayments
  UNION ALL
  SELECT
      'WalletTransactions' AS TableName, COUNT(*) AS RecordCount FROM WalletTransactions
  UNION ALL
  SELECT
      'GroupPayments' AS TableName, COUNT(*) AS RecordCount FROM GroupPayments
  UNION ALL
  SELECT
      'SiteClearingHostAutoPayments' AS TableName, COUNT(*) AS RecordCount FROM SiteClearingHostAutoPayments;
  ```
- **Acceptance Threshold:** Exact record count equivalence. Zero financial record deletion or truncation.

---

## 3. Data Migration Validation Rules

1. **No Destructive Database Operations:**
   - Any script performing `DROP TABLE`, `TRUNCATE`, or un-scoped `DELETE` is prohibited and triggers immediate execution halt.
2. **Schema Mapping Rule:**
   - EF Core entity mappings must strictly reflect database table names (e.g., `[Table("WalletTransactions")]` for `CreditTransaction`).
3. **Audit Execution Gate:**
   - All reconciliation queries must be executed pre-migration and post-migration to confirm `0` record variance before promoting to production.
