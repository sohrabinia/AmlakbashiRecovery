# AmlakBashi V10 Design System Integration — Discovery, Audit & Implementation Report

This document presents the complete UI/UX discovery audit and Design System integration blueprint for **AmlakBashi V10**. In accordance with strict development boundaries, this task is **strictly confined to frontend UI/UX and styling layers**; under no circumstances does it touch or modify the underlying business logic, databases, financial engines, or legacy reservation records.

---

## 1. PHASE 1 — CURRENT UI AUDIT

### 1.1. Current UI Inventory
*   **Layout Framework:** Custom styling system with critical inline-injected style blocks (`master-critical.min.css`) combined with page-specific minified CSS files.
*   **Fonts & Typography:** The typography baseline is built around custom Persian web fonts: `Miransans` and `Liransans`. Iconography is supported by Font Awesome 5 (Free and Brands).
*   **Styling Architecture:**
    -   `wwwroot/css/master.min.css`: Main layout container styles, header, footer, global rules.
    -   `wwwroot/css/home-page.min.css`: Home-page grid, search layout, localized search selectors.
    -   `wwwroot/css/acc-item.min.css`: Accommodation list items, search result grids, details page layouts.
    -   `wwwroot/css/dashboard.min.css`: Panel structures, host controls, account forms.
    -   `wwwroot/css/admin/`: Raw admin styling sheets (`admin.css`, `admin-table.css`, `admin-shared.css`, `admin-popup.css`) which are compiled/minified into `wwwroot/css/min/admin.min.css`.

### 1.2. Identified UI & Component Problems
*   **Duplicate CSS Rules:** Overlapping margins and padding styles between `master-critical.min.css` and `master.min.css` causing slight paint flickering on low-end mobile devices.
*   **Responsive Layout Quirks:** The property details slider on smaller device widths (< 360px) experiences minor text wrapping overlaps on Persian dates.
*   **Color System Inconsistencies:** Multiple variants of amber and gold brand accents (`#fdd835`, `#fbc02d`) scattered throughout different views.
*   **Missing UI States:** Loading states for contact buttons lack interactive overlays, sometimes causing double-clicks before visual changes render.

### 1.3. Refactoring Priorities
1.  **Consolidate Design Tokens:** Define clear brand-accent vars (colors, sizes, typography) in a unified design layout reference.
2.  **Harmonize Mobile Sliders:** Modernize viewport responsiveness for property cards on all mobile form factors.
3.  **Standardize Form Elements:** Unify Persian form borders, focus rings, and input states across panels.

---

## 2. PHASE 2 — BUSINESS SURFACE MAPPING

We have mapped and isolated four distinct UI surfaces:

### 2.1. Public Marketplace
*   **Routes:** `/`, `/search`, `/accomodation/detail/{id}`, `/rules`, SEO-optimized city/category landing pages.
*   **Visual Persona:** RTL layout, warm hospitality identity, elegant property grid search cards, and prominent trust-focused badges.

### 2.2. Guest/User Panel
*   **Routes:** `/app/` (Dashboard, Favs), `/app/reserve/list`.
*   **Visual Persona:** Contact logs, favorite listings, system messages, review forms, and clear financial wallet visibility.

### 2.3. Host Panel
*   **Routes:** `/host/`, `/host/residence/edit`.
*   **Visual Persona:** Advertisement editor, image upload sliders, monetization promotion panels, and simple performance stats.

### 2.4. Admin Panel
*   **Routes:** `/admin/`, `/admin/reserve`.
*   **Visual Persona:** User moderation, advertisement approval tables, financial logs, and historical reservation panel (read-only management of previous bookings).

---

## 3. PHASE 3 — DESIGN SYSTEM CREATION

The V10 premium Design System is built specifically for high-end Iranian short-term rentals, avoiding generic layouts or dry templates.

### 3.1. Typography (Persian Hierarchy)
*   **Font Family:** `Vazirmatn`, `IRANSans` (with fallback `sans-serif`).
*   **Scale:**
    -   Display 1 (Large Banner): `32px` / Line height: `44px`
    -   Heading 1 (Page Title): `24px` / Line height: `34px`
    -   Heading 2 (Card Title): `18px` / Line height: `26px`
    -   Body Text: `14px` / Line height: `22px`
    -   Caption / Micro-text: `11px` / Line height: `16px`

### 3.2. Color System
*   **Primary Brand Color (Warm Gold/Amber):** `#E5A93B` (Primary hospitality tone)
*   **Secondary Tone (Deep Slate):** `#2C3E50` (Trust and corporate stability)
*   **Success Tone (Green):** `#27AE60`
*   **Danger Tone (Red):** `#C0392B`
*   **Neutral Dark:** `#1A1A1A`
*   **Neutral Light:** `#F9F9F9`

### 3.3. Core Premium Components

#### Property Card Component
*   RTL structure, clean border-radius (`12px`), elegant card drop-shadow (`0 4px 12px rgba(0,0,0,0.05)`), distinct daily-price tag, and high-quality cover photo with smooth image cross-fade transitions.

#### Form Controls
*   Consistent padding, light slate focus border (`2px solid #E5A93B`), Persian date picker input, and inline error states.

#### Modals & Popups
*   Glassmorphism blur background overlay, central animation scaling, and clear, descriptive Persian typography.

---

## 4. PHASE 4 — BUSINESS RULE UI VALIDATION

We strictly confirm that the V10 UI modernization is structurally aligned with the Lead-Generation transition and preserves all existing backend financial security guidelines:

### 4.1. Forbidden Patterns (Banned CTAs)
*   ❌ No "Book Now" / "رزرو آنلاین" checkout buttons.
*   ❌ No automatic online checkout forms or booking payment steps.
*   ❌ No OTA transaction booking gateways in the main guest detailed screens.

### 4.2. Supported Patterns (Lead Gen CTAs)
*   ✅ **Contact Host ("تماس با میزبان"):** Directly opens host phone popup (`ShowMobile` callback integration).
*   ✅ **Message Host ("ارسال پیام"):** Opens real-time SignalR chat modal directly with host details.
*   ✅ **Lead Registration:** Saves click telemetry tracking inside `addShowMobileCounter`.

### 4.3. Preserved Financial Panels
*   ✅ Standardized Wallet UI remains structurally unchanged.
*   ✅ Historical payment receipt ledger tables are untouched.
*   ✅ Promotion pricing / Ladder purchase systems remain active.
*   ✅ Read-only Admin Reservation console is fully functional.

---

## 5. PHASE 5 — UX QUALITY CHECK

*   **Navigation Flow:** Simple, unified top-bar header with prominent Search field and fast access to user dashboard.
*   **Loading States:** Clean shimmer skeletal layout placeholders replace raw blank spaces while property slides or lists load.
*   **Error Handling:** Localized Persian warnings ("مشکلی در اتصال پیش آمد") instead of raw console print exceptions.
*   **Accessibility:** Proper color contrast, RTL spacing alignment, and legible custom Persian font sizes on all screens.

---

## 6. PHASE 6 — IMPLEMENTATION PLAN & RISK ASSESSMENT

### 6.1. File Modifications List
To modernise the layout styles cleanly without modifying compiled assemblies:
*   `wwwroot/css/master.min.css`: Modern styling rules, color variable definitions, and typography overrides.
*   `wwwroot/css/home-page.min.css`: Custom premium landing style alignments.
*   `wwwroot/css/acc-item.min.css`: Property details and search card visual overrides.
*   `wwwroot/css/dashboard.min.css`: Host, Guest, and Admin panels visual modernization rules.

### 6.2. Dependencies
*   Font files (`vazirmatn.woff2` / `iransans.woff2`).
*   Custom minified styling bundler (`bundleconfig.json`).

### 6.3. Risk Assessment & Mitigation
1.  **Risk: Visual Reflow / Flickering on slow connections.**
    *   *Mitigation:* Inject premium styling variables in header as critical CSS rules in `master-critical.min.css` to render immediately.
2.  **Risk: Legacy browser compatibility with modern CSS Grid rules.**
    *   *Mitigation:* Provide robust Flexbox fallbacks for older devices.

---

## 7. BUSINESS SAFETY CONFIRMATION

*   **Reservation History Untouched:** ✅ Confirmed. Historical database tables remain 100% untouched.
*   **New Booking Flow Not Introduced:** ✅ Confirmed. No OTA checkout paths are added.
*   **Wallet/Payment Preserved:** ✅ Confirmed. Financial logic, Ledgers, and clearings remain locked.
*   **Admin Reservation Preserved:** ✅ Confirmed. Admins maintain full management access of existing booking data.

---
**Report compiled and validated by Jules.**
**Design Release Status: READY FOR MODERNIZATION PIEZEMEAL INTEGRATION**
