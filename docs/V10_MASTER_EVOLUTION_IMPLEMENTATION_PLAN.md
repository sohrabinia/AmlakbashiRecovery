# AmlakBashi V10 Master Evolution Implementation Plan

## Executive Overview
This implementation plan governs the post-production evolution of AmlakBashi V10 from a certified Contact Display Marketplace into a scalable Lead Intelligence Marketplace, Host Growth Platform, Monetization Engine, and SEO Growth Engine while establishing an automated Staging-first DevOps lifecycle and AI-assisted validation pipeline.

## Baseline Certification & Red Lines
- **Baseline Git Tag**: `v10-production-baseline` (Rollback/Reference Baseline)
- **CRITICAL OPERATING RULE**: **PRODUCTION MUST NOT BE MODIFIED DURING THIS TASK**.
- All implementation and database schema changes run exclusively against isolated TEST/STAGING environments.
- **Business Protection (Immutable)**:
  - Existing Persian SEO URLs and category/city routing
  - Historical reservation database tables and financial ledgers (`CreditTransactions`, `Payments`, `WalletTransactions`)
  - Host listing data and core `Advertise` business logic
  - Production database schema stability

## Evolution Phases & Roadmap

### Phase 1: Baseline Lock & Repository Forensic Check
- Branch strategy: `v10-product-evolution` development branch.
- Baseline tag `v10-production-baseline` preserved untouched.

### Phase 2: Lead Intelligence Foundation & Host Growth
- `LeadEvent` entity, `LeadEventDto`, `LeadEvents` DbSet, and `TrackLeadEvent` API endpoint with client `item.js` tracking.
- Host demand signals and real-time contact lead analytics on listing items.

### Phase 3: Monetization & SEO Engines
- Nardeban (listing bump) and Featured Listings score preservation (`ResidenceScore`, `AmlakbashiScore`).
- JSON-LD Structured Data (`RentAction`/`Hotel`, `BreadcrumbList`, `RealEstateAgent`) across property detail and homepage views.

### Phase 4: Business Intelligence & Technical Hardening
- Admin reporting endpoints (`GetLeadIntelligenceStatistics`) and view (`LeadIntelligenceReport.cshtml`).
- System hardening across EF Core queries, Redis distributed cache, and Hangfire background jobs.

### Phase 5: Database Safety, Staging & Ollama AI Pipeline
- Staging environment isolation (separate IIS app pool, separate Staging DB).
- Local Ollama AI analysis framework for code diffs, test logs, and schema migrations.

### Phase 6: Complete Regression & Release Gate Validation
- `dotnet build` and `dotnet test` suite execution.
- Security boundary verification (Public user reservation blocked, admin historical reservation preserved).
- Final release gate status set to `READY FOR HUMAN ACCEPTANCE TEST`.
