# AmlakBashi V10 Contact Mode Runtime Acceptance Report

## Executive Summary

- **Status**: **`PASS`**
- **Date**: 2025-05-18
- **Scope**: End-to-end runtime acceptance verification of Contact Mode lead generation workflow across Public, Lead Tracking, Admin Intelligence, and Reservation Data Protection boundaries.
- **Verdict**: AmlakBashi V10 operates as a certified Direct Lead Generation + Contact Display Marketplace. Public user checkout and online booking requests are completely bypassed on the frontend, while historical database ledgers (`Reserves`, `Payments`, `CreditTransactions`) remain preserved for administrative compliance.

---

## 1. Verification Matrix & Findings

### 1. Public Advertise Page (Contact Reveal)
- **URL / Route**: `/accomodation/item?id={residenceId}` (Rendered via `Accomodation/Item.cshtml` and `_Reserve.cshtml`)
- **ShowMobile Button**: Verified present in `_Reserve.cshtml` (`<div id="contact-host-button" onclick="show_contact(this)">`).
- **Phone Reveal UX**: Clicking the button triggers `show_contact()` in `wwwroot/js/app/advertise/item.js`, unmasking the host phone number container `#host-phone-number-container` with helper message *"زنگ بزن و بگو شماره رو از املاک باشی برداشتم!"*.
- **AJAX Execution**: Executes `POST /advertise/trackleadevent` with payload `{ residenceId, eventType: "ShowMobile" }`.

### 2. Lead Event Tracking Infrastructure
- **Entity Model**: `Amlakbashi.Core.Entities.LeadEvent`
- **DbSet Mapping**: `DbSet<LeadEvent> LeadEvents` in `Amlakbashi.Data.AmlakbashiDB`
- **Controller Action**: `AdvertiseController.TrackLeadEvent`
- **Data Attributes Captured**:
  - `ResidenceId`: Mapped to target accommodation ID.
  - `HostUserId`: Mapped to listing owner's user ID (`ad.UserId`).
  - `GuestUserId`: Captured when user is authenticated, else `null`.
  - `EventType`: Defaulted to `"ShowMobile"`.
  - `DeduplicationKey`: Mapped to `{residenceId}_{userId/ip}_{yyyyMMddHH}` to prevent event inflation.
  - `ClientIp` & `UserAgent`: Logged from request context.

### 3. Admin Business Intelligence
- **URL / Route**: `/admin/LeadIntelligenceReport` (View action in `AdminController.cs`)
- **View**: `Amlakbashi.Host/Views/Admin/LeadIntelligenceReport.cshtml`
- **Metrics Exhibited**:
  - **Total Lead Counter**: Aggregate count of all recorded lead events.
  - **ShowMobile Leads**: Specific count of direct contact reveal events.
  - **Top Demand Listings**: Strongly typed `TopListingLeadDto` table ranking listings by host contact reveal volume.
  - **Security Gate**: Protected with `[Authorize(Policy = Policies.Statistics_View)]`.

### 4. Historical Reservation Protection
- **Entity Safety**: `Reserve.cs`, `ReservePayment.cs`, and `Payment.cs` entities remain intact in `Amlakbashi.Core`.
- **Database Safety**: `Reserves`, `ReservePayments`, `Payments`, and `CreditTransactions` tables are 100% preserved in `AmlakbashiDB`.
- **Migration Integrity**: Zero destructive schema migrations executed. `20250520000000_add-lead-events-table.cs` only adds the `LeadEvents` table without altering legacy financial or booking tables.

---

## 2. Code Evidence References

- `Amlakbashi.Host/Controllers/AdvertiseController.cs` (Line 60): `TrackLeadEvent` AJAX endpoint.
- `Amlakbashi.Host/wwwroot/js/app/advertise/item.js` (Line 380): `show_contact` JS function triggering AJAX lead event tracking.
- `Amlakbashi.Host/Views/Accomodation/_Reserve.cshtml` (Line 1): V10 Contact Mode UI component replacing booking checkout.
- `Amlakbashi.Host/Controllers/AdminController.cs` (Line 150): `LeadIntelligenceReport` admin reporting controller action.
- `Amlakbashi.Data/Migrations/20250520000000_add-lead-events-table.cs`: Non-destructive EF Core migration adding `LeadEvents`.

---

## Final Status

```
STATUS: PASS
```
