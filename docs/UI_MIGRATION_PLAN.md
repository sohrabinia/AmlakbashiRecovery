# AMLAKBASHI UI MIGRATION PLAN
## Phase 6: Frontend Modernization Foundation & RTL Design System

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** Frontend Architect / UX Lead
**Design Paradigm:** Persian RTL First, Mobile-First, Tailwind CSS + IRANSans Typography
**Status:** `DESIGNED & PREPARED`

---

## 1. Executive Modernization Vision

The AmlakBashi UI modernization replaces legacy jQuery/Bootstrap templates with an interactive, responsive, Persian RTL-first Single Page Application (SPA) hybrid architecture embedded in `wwwroot`. Modernization strictly preserves server-rendered SEO markup for search engines while providing a fast, mobile-app-like experience for users.

---

## 2. Design System Standards & Brand Tokens

### 2.1 Brand Palette
- **Primary Green:** `#0F5132` (Trust, Hospitality)
- **Secondary Cream:** `#F5EFE6` (Warm Background)
- **Accent Gold:** `#C59D5F` (Featured / VIP Listings)
- **Neutral Dark:** `#111827` (Persian Text Readability)
- **Neutral Light:** `#F9FAFB` (Card Backgrounds)

### 2.2 Persian Typography
- **Primary Font Family:** `IRANSans` / `Miransans` / `Liransans` (Local WebFonts in `wwwroot/content/fonts/`)
- **Direction:** `dir="rtl"` hardcoded on HTML root.

---

## 3. UI Modernization Architecture (`wwwroot/v10-app.js`)

### 3.1 Component Architecture
```
wwwroot/
  ├── index.html (SPA Host Shell)
  ├── v10-app.js (Reactive Component Orchestrator)
  ├── js/
  │   └── app/
  │       └── advertise/
  │           └── item.js (Item Detail & ShowMobile Telemetry)
  └── css/
      └── tailwind.css (Compiled RTL Design Tokens)
```

### 3.2 Key Portals & Layouts
1. **Public Marketplace Portal:**
   - Search Header with Iranian Province/City auto-suggest.
   - Property Card Grid with instant price formatting (IRR / Toman).
   - Item Details Page with Host Contact Reveal (`ShowMobile` modal/button).
2. **Host Growth Panel:**
   - Property Creation Wizard (Multi-step step wizard, BBQ checkbox, amenities, photo uploader).
   - Lead Performance Dashboard (Contact reveal counts, view analytics).
3. **Admin Control Portal:**
   - Approval Queue, Listing Moderation, Support Chat, and Financial Ledger View.

---

## 4. Safety & SEO Boundaries

- **Legacy Page Co-existence:** Legacy Razor views remain functional until modern SPA endpoints complete full visual regression verification.
- **SEO Preservation:** HTML shell includes pre-rendered meta tags, Schema.org JSON-LD structured data, and SSR fallbacks for web crawlers.
