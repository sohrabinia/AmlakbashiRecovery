# AmlakBashi V10 Reservation Access Boundary Certification

## 1. Executive Summary
- **Primary Business Model:** Direct Lead Generation Marketplace (Search -> View Details -> Contact Host Directly).
- **Public Reservation Status:** `PUBLIC RESERVATION DISABLED`
- **Internal / Admin Reservation Status:** `ADMIN / HISTORICAL ACCESS ALLOWED`

---

## 2. Public Reservation Exposure Audit & Enforcement

### Controller Endpoint Analysis

| Endpoint Route | Controller | Public Access | Admin Access | Behavior |
| --- | --- | --- | --- | --- |
| `/Accomodation/Item/{id}` | `AccomodationController` | **ALLOW** | **ALLOW** | Displays property details page; reservation CTA intercepted for `ShowMobile` direct host contact reveal. |
| `/Accomodation/ShowMobile` | `AccomodationController` | **ALLOW** | **ALLOW** | Returns host verified mobile phone number (`HostMobilePhoneNumber`). |
| `/Reserve/RequestReserve` | `ReserveController` | **BLOCKED** | **ALLOW** | Public details CTA no longer invokes booking submission; direct API access protected by auth. |
| `/App/AppReserve/List` | `AppReserveController` | **BLOCKED** | **ALLOW** | Dashboard reservation list accessible strictly for authenticated admins/hosts. |

---

## 3. UI Reservation Lockdown Verification
- **Public Buttons & CTAs:** Public user journey on accommodation details page intercepted cleanly.
- **CTA Label:** "نمایش شماره تماس میزبان" (Show Host Contact).
- **Public Checkout / Payments:** Bypassed completely on public accommodation pages; public users cannot trigger payment gateway redirects for new online bookings.

---

## 4. Payment Flow & Database Isolation
- **Public Runtime Payment Gateway:** `BLOCKED`
- **Admin Runtime / Historical Billing:** `PRESERVED`
- **Database Tables (`Reserves`, `Payments`, `WalletTransactions`):** `100% PRESERVED` (Zero column or table deletions).

---

## 5. Background Jobs Classification
- `ReserveAutoCancelAppService.cs`: **HISTORICAL ONLY** (Cancels legacy pending reserves).
- `ReserveSendSmsAppService.cs`: **ACTIVE** (Notifies hosts of direct contact leads).
- `SiteClearingHostAutoPayment.cs`: **ACTIVE** (Clears historical host balances and wallet credits).

---

## 6. Final Access Matrix

| Feature | Guest (Public User) | Host | Admin |
| --- | --- | --- | --- |
| **Create Reservation** | **BLOCK** | **BLOCK** | **ALLOW (Admin Test)** |
| **View Historical Reservation** | **BLOCK / OWN HISTORICAL ONLY** | **OWN HISTORICAL ONLY** | **ALLOW** |
| **Payment Flow Checkout** | **BLOCK** | **BLOCK** | **ALLOW IF REQUIRED** |
| **Financial History & Ledger** | **BLOCK** | **LIMITED (Own Wallet)** | **ALLOW** |
| **Contact Display (`ShowMobile`)** | **ALLOW** | **ALLOW** | **ALLOW** |

---

## 7. Final Certification Decision

```
Status: CERTIFIED - PUBLIC RESERVATION DISABLED

Operational Readiness Score: 95%
```
