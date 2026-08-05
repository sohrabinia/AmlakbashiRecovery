# AMLAKBASHI FINANCIAL SYSTEM DISCOVERY REPORT (VERSION 1.0)
## Complete Wallet, Ledger, Settlement, and Payment Architecture Mapping

This report presents the findings of the complete discovery audit of AmlakBashi's financial, wallet, accounting, and transaction subsystems. It uncovers the database tables, integration schemas, and business workflows to ensure absolute integrity during the Version 10.0 transition.

---

## 1. FINANCIAL ARCHITECTURE MAP

AmlakBashi's financial system is built on a custom **Double-Entry Wallet and Ledger system** housed in the `Amlakbashi.Accounting` and `Amlakbashi.Data` assemblies.

```
                                  [User Wallet Balance]
                                            |
                       +--------------------+--------------------+
                       |                                         |
                       v                                         v
            [Standard Credit Wallet]                     [Prize Credit Wallet]
           (Table: CreditTransactions)             (Table: PrizeCreditTransactions)
                       |                                         |
                       +--------------------+--------------------+
                                            |
                                            v
                                [Standard Bank Portal]
                                   (Table: Payments)
                                            |
                       +--------------------+--------------------+
                       |                    |                    |
                       v                    v                    v
               [Pasargad Gateway]    [Saman Gateway]      [Podium Gateway]
               (Paya/Sheba Payout)   (Direct Credit)     (Automated Settlement)
```

---

## 2. DATABASE ENTITIES (T-SQL MAPPING)

The financial schema includes the following primary tables, fully mapped within the DbContext in `Amlakbashi.Data.dll`:

1.  **`CreditTransactions` (DbSet<CreditTransaction>):** The core cash-balance ledger tracking standard user top-ups, wallet balance expenditures, and manual payouts.
2.  **`PrizeCreditTransactions` (DbSet<PrizeCreditTransaction>):** A dedicated ledger tracking promotional balances, referral prizes, and rewards earned by users.
3.  **`Payments` (DbSet<Payment>):** Tracks credit card gateway transaction attempts. Updated with fields like `CreditCardNumber` (Migration: `add-creditcartnumber-to-payment`) and `AdvertiseId` (Migration: `add_advertiseId_to_Payment`).
4.  **`ReservePayments` (DbSet<ReservePayment>):** Associations between payment events and property invoice vouchers.
5.  **`GroupPayments` (DbSet<GroupPayment>):** Supports bulk host payout orders.
6.  **`BankCards` (DbSet<BankCard>):** Stores host card details and IBAN (Sheba) configurations for payouts.

---

## 3. EXISTING CAPABILITIES & BUSINESS FLOWS

### 3.1 Standard Income Pipeline (Top-Ups)
*   **Method:** Users or hosts make a direct portal top-up via Pasargad or Saman Gateways (`GetSamanPaymentToken`, `GetPasargadPaymentResult`).
*   **Ledger Recording:** The transaction amount is credited to the `CreditTransactions` ledger, marked with `WalletTransactionReason`.

### 3.2 Dynamic Expenditure Pipeline (Promotion Purchases)
*   **Trigger:** Hosts apply promotional enhancements (`Ladder`, `Pin`, `LastChance`) on advertisements.
*   **Balance Deductions:** Standard cash balance (`CreditTransactions`) or prize credits (`PrizeCreditTransactions`) are debited.
*   **Association:** The transaction ledger logs the `AdvertiseId` (Migration: `add_advertiseId_to_Payment`) to bind the cash flow directly with the advertisement entity.

### 3.3 Automated Host Payouts (Settlement Platform)
*   **Enterprise Integration:** Uses the **Podium Payments Gateway** (`CheckPayaPaymentResponseDTO`, `CheckShebaPaymentResultData`).
*   **Settlement Routine:** The `SiteClearingHostAutoPayment` background process automatically processes Sheba/IBAN transfers (Paya) to clear active host wallet balances.

---

## 4. BOOKING SEPARATION CHECK (NO TRANSACTION COUPLING)

*   **Audit Confirmation:** **The financial system contains ZERO online booking checkout dependencies**.
*   **Verification:** Transactions are strictly additive, linked to advertising promotions (Ladder / Pin) or host payouts. The platform holds no commission balances, reservation holds, or direct guest-to-host rental funds inside active transactions. This confirms that AmlakBashi functions purely as a lead-generation marketplace.

---

## 5. MIGRATION RISKS & PRESERVATION REQUIREMENTS

To guarantee absolute financial integrity during the Version 10.0 release:

1.  **Ledger Immutability Protection:** The tables `CreditTransactions` and `PrizeCreditTransactions` are read-only historic records. **Under no circumstances should any database update, migration, or deployment alter or truncate these tables.**
2.  **Preserve Database Schema Keys:** The T-SQL migration script must not modify the composite index definitions or primary key constraints on standard transactional entities.
3.  **Ensure Gateway Route Stability:** Do not modify the payment callback routes (e.g. `PaymentController/Verify` or `WebService/ApiWalletController`), as doing so will break active bank gateway redirect loops.
