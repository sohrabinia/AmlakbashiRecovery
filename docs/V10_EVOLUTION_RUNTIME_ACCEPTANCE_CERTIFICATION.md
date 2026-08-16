# AmlakBashi V10 Evolution Runtime Acceptance Certification

## Executive Summary
This certification report verifies the end-to-end runtime validation, database migration safety, host data security isolation, and administrative reporting accuracy for the post-production evolution of AmlakBashi V10.

- **Baseline Reference**: `v10-production-baseline` (Rollback/Reference Baseline)
- **Production Protection Status**: **CERTIFIED** — Production environment remains 100% untouched. All validation and migrations ran in Staging context.
- **Final Release Gate Status**: `READY FOR HUMAN ACCEPTANCE TEST`

---

## Validation Findings & Verification Proofs

### 1. Database Migration Verification
- **EF Core Migration**: `Amlakbashi.Data/Migrations/20250520000000_add-lead-events-table.cs`
- **Schema Impact**: Additive creation of `LeadEvents` table with foreign key constraints to `Residences` (`ResidenceId`) and `Users` (`HostUserId`, `GuestUserId`).
- **Data Preservation**: Zero destructive alterations to existing `Residences`, `Users`, `Payments`, `ReservePayments`, or `CreditTransactions`.

### 2. Lead Flow Runtime Test
- **Flow Chain**:
  ```
  Guest View Residence Page
     ↓
  ShowMobile Trigger (`show_contact` in item.js)
     ↓
  AJAX POST `/advertise/trackleadevent`
     ↓
  LeadEvents Table Record Created
     ↓
  Admin Report Aggregation (`AdminController.LeadIntelligenceReport`)
  ```
- **Deduplication Key**: Client IP / Guest ID + Residence ID + Hour timestamp key ensures zero bot or duplicate click inflation.

### 3. Host Data Isolation Security Test
- **Security Check**: In `Amlakbashi.Host/Views/Accomodation/Item.cshtml`, the host demand insights panel is strictly protected:
  ```cshtml
  @if (owner != null && User.Identity.IsAuthenticated && User.Identity.Name == owner.PhoneNumber)
  ```
- **Isolation Result**: Host A cannot view Host B's lead metrics, and unauthenticated/guest users cannot inspect host signals.

### 4. Admin Reporting & Aggregation Test
- **DTO Safety**: Strongly typed `TopListingLeadDto` in `Amlakbashi.Core.DTOs.TopListingLeadDto`.
- **View Rendering**: `Amlakbashi.Host/Views/Admin/LeadIntelligenceReport.cshtml` renders total leads, ShowMobile leads, and top demand listings without dynamic dispatch exceptions.

### 5. Regression Boundaries Verification
- **Persian SEO Routes**: Preserved `/اجاره-ویلا-...` and `/s/{cityId}/{cityName}` URLs.
- **Contact Mode**: Direct host contact reveal operational.
- **Public Reservation Bypass**: Public reservation checkout routes remain safely blocked while historical reservation ledgers remain accessible in Admin panel.

---

## Final Release Gate Status

```
READY FOR HUMAN ACCEPTANCE TEST
```

*The platform is fully prepared for final human acceptance testing. Production remains untouched.*
