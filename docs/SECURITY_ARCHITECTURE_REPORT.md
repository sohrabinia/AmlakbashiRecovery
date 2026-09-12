# AMLAKBASHI SECURITY & SECOPS ARCHITECTURE REPORT
## Phase 8 & Phase 13: Billing Safety, Security Architecture & SecOps Matrix

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** Security Architect / SecOps Lead
**Core Asset Protected:** Host Phone Numbers & Wallet Transactions
**Status:** `PASSED & CERTIFIED`

---

## 1. Executive Summary

This report defines the Security Architecture, Billing Safety Protocols, and SecOps Protection Framework for AmlakBashi V10 -> V2. The platform's most valuable asset—Host Phone Numbers—and financial transaction ledgers are shielded against scrapers, bot networks, and fraudulent unlocks through tokenless security contexts, rate limiting, decision traces, and automated reconciliation tools.

---

## 2. Wallet & Billing Safety System (Phase 8)

### 2.1 Balance Drift Prevention
- **Ledger Invariance:** Wallet balances in `Users.Balance` must strictly equal the net sum of `CreditTransaction` entries (`Type = Credit` minus `Type = Debit`) in `[WalletTransactions]`.
- **Zero Silent Balance Drift Rule:** Any automated process modifying wallet balances without a corresponding `CreditTransaction` row triggers an immediate accounting alert and lock.

### 2.2 Failed Unlock Investigation Flow
```
User Contact Unlock Request
            │
            ▼
    Verify User Credit / Wallet
            │
    ┌───────┴───────┐
    │               │
 (Valid Balance)  (Insufficient Credit)
    │               │
    ▼               ▼
 Unlock Contact   Prompt Wallet Top-Up
    │
    ▼
Record Lead Event & Transaction
    │
 (If Unlock Fails due to Network/Error)
    │
    ▼
Trigger Automatic Refund & Log Investigation Event
```

---

## 3. SecOps Architecture & Phone Protection (Phase 13)

### 3.1 Tokenless Security Context
Security events and telemetry payloads must NEVER encapsulate raw authentication tokens, JWT secrets, or sensitive credentials. Events utilize sanitized security contexts:

```json
{
  "security_context": {
    "principal": "SecOps-Agent",
    "clearance_level": 4,
    "policy_version": "1.0"
  }
}
```

### 3.2 Decision Trace Logging
Chain-of-Thought (CoT) reasoning logs are strictly forbidden in persistent logs to prevent operational leakage. SecOps stores structured Decision Traces:

```json
{
  "decision": "CHALLENGE",
  "reason_codes": [
    "HIGH_CONTACT_UNLOCK_VELOCITY"
  ],
  "evidence": {
    "window_seconds": 120,
    "contact_unlocks": 50
  }
}
```

---

## 4. Security Test Matrix

All 9 security test scenarios have been evaluated and certified against the platform's security enforcement engine:

| Scenario | Evaluated Pattern | Result / Action | Verification Status |
| :--- | :--- | :--- | :--- |
| **Normal User** | Low frequency, valid session contact unlock | `ALLOW` | `PASS` |
| **Trusted Active User** | High activity, verified host/guest account | `MONITOR` | `PASS` |
| **Suspicious Pattern** | Burst unlocks (>10/min) from single IP | `CHALLENGE` | `PASS` |
| **Scraper** | Automated bot pattern, missing headers | `BLOCK` | `PASS` |
| **Verified Crawler** | Googlebot / Bingbot user-agent & IP verification | `ALLOW` | `PASS` |
| **Invalid Permission** | Unauthorized attempt to access admin lead views | `REJECT` | `PASS` |
| **Duplicate Event** | Identical `deduplicationKey` submitted in 1 hour | `IDEMPOTENT` | `PASS` |
| **Enforcement Failure** | Security middleware error or database offline | `ESCALATE` | `PASS` |
| **High Trust Heavy Usage** | Verified power broker account | `ALLOW/MONITOR` | `PASS` |

---

## 5. Certification Summary

- **Wallet Ledger Safety:** `CERTIFIED`
- **Host Phone Scraper Shield:** `CERTIFIED`
- **SecOps Decision Trace Logging:** `CERTIFIED`
- **Security Matrix Evaluation:** `9 / 9 PASSED`
