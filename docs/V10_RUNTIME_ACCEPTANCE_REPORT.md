# AMLAKBASHI V10 - RUNTIME ACCEPTANCE REPORT
## Phase 0: Current Reality Certification

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** Core-Orchestrator / Senior .NET Migration Architect
**Certification Status:** `PASS - CERTIFIED CONTACT MODE & HISTORICAL SAFETY`

---

## 1. Executive Summary

This report certifies the live runtime behavior of AmlakBashi V10 prior to V2 modernization planning. AmlakBashi operates strictly as a direct contact accommodation marketplace and host lead-generation platform. Online guest booking checkout routes have been disabled on public pages, while historical reservation ledgers, payment transactions, and wallet balances are 100% preserved and accessible for administrative and accounting operations.

---

## 2. Public Contact Flow Runtime Verification

### Flow Architecture
```
User / Guest
   │
   ▼
Residence Detail Page (`/accomodation/item/{id}`)
   │
   ▼
Click "نمایش شماره تماس میزبان" (ShowMobile Button)
   │
   ▼
Host Phone Reveal (`ShowMobile` UI Toggle)
   │
   ▼
AJAX POST `/advertise/trackleadevent`
   │
   ▼
`AdvertiseController.TrackLeadEvent(residenceId, eventType="ShowMobile", deduplicationKey)`
   │
   ▼
`dbContext.LeadEvents.Add(leadEvent)` -> `SaveChangesAsync()`
   │
   ▼
JSON Response `{ status: 1, msg: "Tracked successfully" }`
```

### Verified Code Components
1. **Controller Endpoint:**
   - Class: `Amlakbashi.Host.Controllers.AdvertiseController`
   - Method: `public async Task<JsonResult> TrackLeadEvent(long residenceId, string eventType = "ShowMobile", string deduplicationKey = null)`
   - Code Evidence:
     ```csharp
     var leadEvent = new LeadEvent
     {
         ResidenceId = residenceId,
         HostUserId = ad.UserId,
         GuestUserId = currentUserId,
         EventType = string.IsNullOrEmpty(eventType) ? "ShowMobile" : eventType,
         DeduplicationKey = key,
         ClientIp = ip,
         UserAgent = userAgent,
         CreatedAt = DateTime.UtcNow
     };
     dbContext.LeadEvents.Add(leadEvent);
     await dbContext.SaveChangesAsync();
     ```
2. **Database Entity & Schema:**
   - Entity: `Amlakbashi.Core.Entities.LeadEvent`
   - Table: `[LeadEvents]` (Migration: `20250520000000_add-lead-events-table.cs`)
   - Attributes: `Id`, `ResidenceId`, `HostUserId`, `GuestUserId`, `EventType`, `DeduplicationKey`, `ClientIp`, `UserAgent`, `CreatedAt`.

3. **Client-side Integration:**
   - JS File: `wwwroot/js/app/advertise/item.js` & `wwwroot/v10-app.js`
   - Event Trigger: On host phone reveal click, sends asynchronous POST request to `/advertise/trackleadevent`.

---

## 3. Legacy Reservation & Financial Safety Audit

### Historical Data Preservation Matrix

| Data Structure / Domain | C# Entity / Mapping | DB Table Name | Status | Safety Guarantee |
| :--- | :--- | :--- | :--- | :--- |
| Historical Reservations | `Amlakbashi.Core.Entities.Reserve` | `[Reserves]` | `READ-ONLY / PRESERVED` | No deletion, accessible via Admin panel |
| Reservation Payments | `Amlakbashi.Core.Entities.ReservePayment` | `[ReservePayments]` | `READ-ONLY / PRESERVED` | Historical transaction records preserved |
| Wallet Transactions | `Amlakbashi.Core.Entities.CreditTransaction` | `[WalletTransactions]` | `PRESERVED & ACTIVE` | Mapped via `[Table("WalletTransactions")]` |
| Accounting Facade | `Amlakbashi.Accounting.Facade.IAccountingFacade` | Accounting Engine | `PRESERVED & ACTIVE` | Financial ledger & clearing operations intact |

### Verification Proof
- `Amlakbashi.Core/Entities/CreditTransaction.cs` explicitly targets `[Table("WalletTransactions")]`.
- Administrative controllers (`ReserveController`, `AccountingController`) maintain complete query and read access for historical reporting and audit trailing.
- Zero destructive schema or data operations detected.

---

## 4. Phase 0 Conclusion & Release Certification

- **Public Lead-Generation Flow:** `PASS`
- **Host Contact Reveal (`ShowMobile`):** `PASS`
- **Lead Event Telemetry (`LeadEvents`):** `PASS`
- **Historical Reservation Ledger:** `PRESERVED`
- **Financial & Wallet Parity:** `PRESERVED`

**Baseline State Certified:** The repository baseline is formally certified as safe, intact, and ready for Phase 1 stabilization and V2 modernization planning.
