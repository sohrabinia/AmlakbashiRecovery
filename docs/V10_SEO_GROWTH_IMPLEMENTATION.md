# AmlakBashi V10 SEO Growth Implementation

## Executive Summary
This document details the Persian SEO preservation and schema expansion implemented in AmlakBashi V10.

## Structured Data (JSON-LD) Schemas
1. **Residence Schema (`RentAction` / `Hotel`)**:
   - Location: `Amlakbashi.Host/Views/Accomodation/Item.cshtml`
   - Attributes: Address, Price, Landlord, Rating, Image, and Contact info.
2. **Breadcrumb Schema (`BreadcrumbList`)**:
   - Location: `Amlakbashi.Host/Views/Accomodation/Item.cshtml`
   - Hierarchy: Home ➔ Rent Category ➔ City Page ➔ Specific Residence Slug.
3. **LocalBusiness / RealEstateAgent Schema**:
   - Location: `Amlakbashi.Host/Views/Home/Index.cshtml`
   - Identity: Official organization details, phone number, and logo.

## Persian SEO URL Preservation
- All Persian slug formats (e.g. `/اجاره-ویلا-...`, `/s/{cityId}/{cityName}`) are fully preserved.
- Canonical link structures and OpenGraph metadata are maintained.
