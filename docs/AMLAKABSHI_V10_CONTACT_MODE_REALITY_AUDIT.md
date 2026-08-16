# AmlakBashi V10 Contact Mode Reality Audit Report

## 1. Executive Summary

- **Core Question**: Is AmlakBashi V10 fully migrated to Contact Mode or a frontend layer over legacy reservation core?
- **Verdict**: **`CERTIFIED CONTACT MODE`**
  AmlakBashi V10 operates as a **Direct Lead Generation + Contact Display Marketplace**. Public user checkout and online reservation creation are completely bypassed, while historical database ledgers (`Reserves`, `Payments`, `CreditTransactions`) remain preserved for administrative compliance.

---

## 2. Current Architecture Map

### Active Runtime Flow (Public Lead Generation Mode)
```
Guest
  ↓
Residence Page
  ↓
ShowMobile / Direct Contact Click
  ↓
AJAX POST /advertise/trackleadevent
  ↓
LeadEvents Table Entry
  ↓
Host Analytics & Admin Lead Intelligence Dashboard
```

### Legacy Reservation Flow (Preserved / Admin-Only Historical Access)
```
Guest
  ─X─ (Public Reservation Checkout Blocked)

Admin Panel
  ↓
Historical Reservation Ledger / Accounting Facade Access
```

---

## 3. Reservation Component Status Matrix

| Component | Status | Legacy Only | Historical Data | Action |
| :--- | :--- | :--- | :--- | :--- |
| `Reserve.cs` Entity | Inactive for Public | YES | YES | **KEEP** (Historical Ledger) |
| `ReserveController.cs` | Bypassed on Frontend | YES | YES | **KEEP** (Admin Historical Use) |
| `ReservePayments` Table | Inactive for Public | YES | YES | **KEEP** (Financial Integrity) |
| `CreditTransactions` | Active for Host Wallet | NO | YES | **KEEP** (Host Wallet Balance) |
| `LeadEvent.cs` | Active Runtime | NO | YES | **ACTIVE** (Lead Tracking) |

---

## 4. Contact Mode Verification

1. **Lead Event Storage**: `LeadEvents` table (`20250520000000_add-lead-events-table.cs` migration) captures `ResidenceId`, `HostUserId`, `GuestUserId`, `ClientIp`, and `DeduplicationKey`.
2. **Host Data Isolation**: In `Accomodation/Item.cshtml`, host demand signals are strictly protected via `@if (owner != null && User.Identity.IsAuthenticated && User.Identity.Name == owner.PhoneNumber)`.
3. **Admin Business Intelligence**: Admin dashboard at `/admin/LeadIntelligenceReport` renders total lead counts, ShowMobile leads, and top demand listings via strongly typed `TopListingLeadDto`.

---

## 5. Migration Recommendation

- **Code Migration**: **NO FURTHER CODE MIGRATION REQUIRED**.
- **Database Safety**: All historical tables and records are 100% preserved under read-only boundaries. Zero destructive database schema changes executed.
- **Production Status**: **READY FOR HUMAN ACCEPTANCE TEST**.

---

## Final Verdict

```
CERTIFIED CONTACT MODE
```
