# AmlakBashi V10 Master Evolution Implementation Plan

## Executive Overview
This implementation plan governs the evolution of AmlakBashi V10 from a certified Contact Display Marketplace into a scalable Lead Intelligence Marketplace, Host Growth Platform, Monetization Platform, and SEO Growth Engine.

## Baseline Certification & Red Lines
- **Baseline Git Tag**: `v10-production-baseline`
- **Contact Mode Status**: Active (`ShowMobile` enabled, public reservation disabled, historical admin reservations preserved).
- **Business Protection (Immutable)**:
  - Existing Persian SEO URLs and category/city routing
  - Historical reservation database tables and financial ledgers (`CreditTransaction`, `Payment`, etc.)
  - Host listing data and core `Advertise` business logic
  - Production database schema stability

## Evolution Phases & Roadmap

### Phase 1: Lead Intelligence Foundation
- Introduce `LeadEvent` entity and `LeadEventDto` in `Amlakbashi.Core` for contact reveal tracking (`ShowMobile`), host/listing attribution, timestamping, and client deduplication keys.
- Map `LeadEvents` DbSet in `Amlakbashi.Data/AmlakbashiDB.cs`.
- Implement lead tracking endpoint in `Amlakbashi.Host/Controllers/AdvertiseController.cs`.

### Phase 2: Host Growth Platform
- Expose contact leads, listing demand metrics, and demand signals in `Amlakbashi.Host/Controllers/AccomodationController.cs` and `Amlakbashi.Host/Views/Accomodation/Item.cshtml`.
- Enable hosts to analyze listing demand and lead performance in real-time.

### Phase 3: Monetization Platform
- Enhance Nardeban (listing bump) and Featured Listings visibility logic in `Amlakbashi.Application/Services/AdvertiseServices/AdvertiseAppService.cs`.
- Document revenue systems and score preservation in `docs/V10_MONETIZATION_ARCHITECTURE.md`.

### Phase 4: SEO Growth Engine
- Embed JSON-LD Structured Data schemas (`Residence`, `LocalBusiness`, `Breadcrumb`) in `Amlakbashi.Host/Views/Accomodation/Item.cshtml` and `Amlakbashi.Host/Views/Home/Index.cshtml`.
- Author `docs/V10_SEO_GROWTH_IMPLEMENTATION.md`.

### Phase 5: Notification & Engagement Layer
- Integrate lead notification mechanisms (SMS alerts, dashboard notifications) across `Amlakbashi.Host/Controllers/AdvertiseController.cs` and `Amlakbashi.Application/Services/UserServices/UserAppService.cs`.

### Phase 6: Business Intelligence Layer
- Expand admin reporting in `Amlakbashi.Host/Controllers/Admin/AdminController.cs` and `Amlakbashi.Host/Views/Admin/AdminStatistic.cshtml` for lead volume, conversion trends, and top listing metrics.

### Phase 7: Technical Hardening
- Optimize EF Core query performance, background jobs, memory caching, and safety boundaries in `Amlakbashi.Data/AmlakbashiDB.cs` and `Amlakbashi.Host/Startup.cs`.
- Document technical findings in `docs/V10_TECHNICAL_HARDENING_REPORT.md`.

### Phase 8 & 9: Delivery Pipeline & Regression Validation
- Validate build integrity with `dotnet build Amlakbashi.sln` across Public, Guest, Host, and Admin portals.

### Phase 10: Final Certification
- Execute `dotnet build` and `dotnet test` suites.
- Author `docs/V10_MASTER_PRODUCT_EVOLUTION_CERTIFICATION.md`.

## Risk Management & Rollback Strategy
- **Rollback Point**: Git tag `v10-production-baseline`.
- **Database Safety**: All new lead structures are additive-only; zero destructive schema changes.
