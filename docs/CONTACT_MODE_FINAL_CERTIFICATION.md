# AMLAKBASHI CONTACT MARKETPLACE FINAL CERTIFICATION
## Phase 4: Contact Mode & Lead Generation Engine Verification

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** Core-Orchestrator / Product Lead / QA Lead
**Marketplace Mode:** `Direct Host Contact & Lead Generation Platform`
**Certification Status:** `CERTIFIED FINAL`

---

## 1. Executive Summary

This report certifies the final architecture and operational readiness of AmlakBashi as a Contact-First Short-Term Accommodation Marketplace. The platform completely bypasses guest online reservation checkout, eliminating fake reservation UX and forced payment steps. Guests interact with accommodation listings by directly revealing host phone numbers, generating auditable lead events tracked in real-time.

---

## 2. Public User Experience & Journey Verification

### 2.1 Contact-First User Journey
```
1. Search Accommodation
   ├── Persian Location Slugs (e.g. /اجاره-ویلا-رامسر)
   └── Instant Filter & Card View
        ↓
2. Residence Detail Page
   ├── High-Resolution Image Carousel
   ├── Property Specifications & Amenities
   └── Dynamic CTA: "نمایش شماره تماس میزبان" (Reveal Host Phone)
        ↓
3. Host Phone Reveal Action
   ├── AJAX Telemetry Request (`/advertise/trackleadevent`)
   ├── Phone Number Rendered: e.g. "0912XXXXXXX"
   └── Helper Prompt: "زنگ بزن و بگو شماره رو از املاک باشی برداشتم!"
```

### 2.2 Verification Checklist

| Aspect | Expected Behavior | Actual Behavior | Result |
| :--- | :--- | :--- | :--- |
| **Public Reservation CTA** | No online payment checkout CTA on item detail page | Reservation checkout bypassed; ShowMobile reveal button displayed | `PASS` |
| **Direct Host Contact** | Unlocks host phone number upon user click | Renders owner phone number immediately without forced payment | `PASS` |
| **Lead Tracking Telemetry** | Asynchronously records lead event in database | Calls `AdvertiseController.TrackLeadEvent` via AJAX | `PASS` |
| **De-duplication** | Prevents duplicate lead counts within same hourly window | Deduplication key (`{residenceId}_{user/IP}_{yyyyMMddHH}`) enforces uniqueness | `PASS` |

---

## 3. Database Architecture & Lead Analytics

### 3.1 `LeadEvents` Schema
- **Table Name:** `[LeadEvents]`
- **Entity Class:** `Amlakbashi.Core.Entities.LeadEvent`
- **Fields:**
  - `Id` (`bigint`, Primary Key)
  - `ResidenceId` (`bigint`, Foreign Key to `Advertises.Id`)
  - `HostUserId` (`int`, Foreign Key to `Users.Id`)
  - `GuestUserId` (`int?`, Nullable Foreign Key to `Users.Id` for authenticated guests)
  - `EventType` (`nvarchar(50)`, default `"ShowMobile"`)
  - `DeduplicationKey` (`nvarchar(200)`, Indexed Unique Key)
  - `ClientIp` (`nvarchar(50)`)
  - `UserAgent` (`nvarchar(500)`)
  - `CreatedAt` (`datetime2`, UTC Timestamp)

### 3.2 Lead Intelligence & Admin Analytics
- **Admin Lead Visibility:** Administrative dashboard provides real-time lead volume breakdown per property, host performance rankings, and unlock velocity analytics.
- **Advertise Performance:** Host dashboard exposes contact reveal statistics, allowing hosts to track listing lead conversion rates.

---

## 4. Phase 4 Release Certification

- **Public Contact Mode UX:** `CERTIFIED`
- **Lead Tracking Infrastructure:** `CERTIFIED`
- **Admin Analytics Integration:** `CERTIFIED`
- **Online Checkout Bypass:** `CERTIFIED`

The Contact Marketplace finalization is certified complete and operational.
