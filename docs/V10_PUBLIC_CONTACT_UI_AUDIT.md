# AmlakBashi V10 Public UI Contact Mode Audit Report

## 1. Executive Summary

- **Audit Date**: 2025-05-18
- **Objective**: Audit public accommodation detail views (`Item.cshtml`, `_Reserve.cshtml`, `item.js`) to identify and eliminate legacy reservation CTAs ("تاریخ شروع و پایان سفر را انتخاب کنید", "درخواست رزرو") on the public frontend while preserving historical reservation data and admin management features.

---

## 2. Identified UI Elements & File Paths

### 1. `Amlakbashi.Host/Views/Accomodation/Item.cshtml`
- **Current Behavior**: Renders the listing header, photo gallery, details, host info, and includes `_Reserve.cshtml` for the sidebar booking widget and sticky mobile footer bar.
- **Required Changes**:
  - Remove sticky mobile reservation button triggers that prompt for dates/booking.
  - Render a clean, direct "تماس با میزبان / نمایش شماره" (Contact Owner) CTA button in both desktop sidebar and mobile sticky bottom bar.

### 2. `Amlakbashi.Host/Views/Accomodation/_Reserve.cshtml`
- **Current Behavior**: Renders the booking sidebar container with date picker inputs, guest count selectors, price calculation labels, and "درخواست رزرو" buttons.
- **Required Changes**:
  - Remove date picker triggers, guest count dropdowns, and reserve buttons from the public view.
  - Render a focused Contact Box containing `#contact-host-button`, `#host-phone-number-container`, and call prompt *"زنگ بزن و بگو شماره رو از املاک باشی برداشتم!"*.

### 3. `Amlakbashi.Host/wwwroot/js/app/advertise/item.js`
- **Current Behavior**: Contains legacy reservation functions (`checkReserve`, `showDatePicker`, `updateReservePrice`) alongside Contact Mode function `show_contact(elem, id)`.
- **Required Changes**:
  - Ensure `show_contact(elem, id)` performs AJAX POST to `/advertise/trackleadevent`, receives host phone number, unmasks phone text `#host-phone-text`, and ensures `#host-phone-number-container` and parent `.advertise-page__reserve-container` are fully expanded and visible on mobile and desktop.

---

## 3. Risk Assessment

| Risk Area | Severity | Mitigation Strategy |
| :--- | :--- | :--- |
| **Historical Data Loss** | High | **Do NOT delete** backend `ReserveController.cs`, `Reserve.cs`, or `Payments` DB tables. Only alter public Razor views. |
| **Admin Panel Impact** | High | Ensure admin reservation views under `Views/Admin/` and support tools remain 100% operational. |
| **Phone Number Security** | Medium | Host phone numbers are NOT embedded in raw HTML source; they are dynamically fetched via AJAX upon user interaction. |
