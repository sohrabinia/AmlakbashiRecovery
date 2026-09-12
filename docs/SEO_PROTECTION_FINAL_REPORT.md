# AMLAKBASHI SEO PROTECTION FINAL REPORT
## Phase 7: SEO Protection System & URL Preservation Strategy

**Target Branch:** `feature/v10-production-candidate`
**Execution Context:** SEO Preservation Engineer / Web Architect
**Market Domain:** Iranian Accommodation Marketplace (Persian Search Engines)
**Status:** `PROTECTED & CERTIFIED`

---

## 1. Executive Summary

AmlakBashi possesses over 12 years of domain authority and indexed Persian SEO rankings across major search engines (Google, Barez, etc.). This report establishes the complete SEO Protection System to ensure zero loss of search engine rankings, organic traffic, or indexed Persian slug URLs during the V10 to V2 transition.

---

## 2. Core Protection Mechanics

### 2.1 Persian Slug & Route Preservation Matrix

| Legacy URL Pattern | Target Persian Route | Handling Mechanics | Status |
| :--- | :--- | :--- | :--- |
| `/اجاره-ویلا-{city}` | `/اجاره-ویلا-{city}` | Direct Route Dispatch in `AdvertiseController` | `PRESERVED` |
| `/شمال` | `/شمال` | Category Route for North Iranian accommodations | `PRESERVED` |
| `/accomodation/item/{id}` | `/accomodation/item/{id}` | Accommodation Detail Page | `PRESERVED` |
| Trailing Slash URLs | `/path` (no trailing slash) | 301 Permanent Redirect (`HtmlUtility.EncodeUrlForRedirect`) | `PRESERVED` |

### 2.2 Category URL Localization (`CategoryUrlLocalization.cs`)
- Code Component: `Amlakbashi.Core.Infrastructure.LocalizationHelpers.CategoryUrlLocalization`
- Functionality: Converts internal category IDs, dynamic regions, and province names into SEO-optimized Persian URLs (`CategoryToUrl(category)`).
- Safety Rule: No modification permitted to URL generator algorithms or slug formatting logic.

---

## 3. Structured Data Strategy (JSON-LD)

To maximize search snippet visibility and rich result rankings, every item detail page dynamically embeds Schema.org JSON-LD structured data:

```json
{
  "@context": "https://schema.org",
  "@type": "LodgingBusiness",
  "name": "ویلای ۳ خوابه استخردار رامسر",
  "description": "اقامتگاه ویلایی دارای استخر آبگرم در رامسر",
  "url": "https://amlakbashi.com/accomodation/item/12345",
  "telephone": "0912XXXXXXX",
  "address": {
    "@type": "PostalAddress",
    "addressLocality": "رامسر",
    "addressRegion": "مازندران",
    "addressCountry": "IR"
  },
  "geo": {
    "@type": "GeoCoordinates",
    "latitude": 36.9012,
    "longitude": 50.6543
  }
}
```

---

## 4. Redirect & Canonical Enforcement Rules

1. **301 Permanent Redirect Rule:**
   - Any legacy path variation, AMP URL, or query parameter re-ordering must respond with HTTP status `301 Moved Permanently`.
2. **Canonical URL Tags:**
   - Head tag explicitly sets `<link rel="canonical" href="https://amlakbashi.com/...">` matching the primary Persian slug.
3. **HTTP 404 Protection:**
   - Soft 404s are prohibited. Invalid property IDs return explicit HTTP status 404 using `/views/errors/http404.cshtml`.
