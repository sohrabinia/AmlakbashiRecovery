# AmlakBashi V10 Final System Truth Report

## 1. Current Architecture Diagram
```
[ Guest / Public Users ] <---> [ Amlakbashi.Host (MVC + SPA) ]
                                      |
                                      +---> [ Amlakbashi.Application (Services / MediatR) ]
                                      |            |
                                      |            +---> [ Amlakbashi.Data (EF Core / AmlakbashiDB) ]
                                      |            +---> [ Amlakbashi.Mediator (CQRS Handlers) ]
                                      |
                                      +---> [ Amlakbashi.Accounting (Ledgers & Banking Engines) ]
                                                   |
                                                   +---> [ External Payment Gateways / Pasargad / Saman / Podium ]
```

## 2. Existing Production Capabilities
- **Core Marketplace:** Property search, dynamic filtering, Persian SEO routes, accommodation CRUD, photo uploads, tag assignments, score calculation.
- **Direct Lead Generation:** Host mobile reveal (`ShowMobile`), direct guest-to-host contact.
- **Admin Panel:** Residence approvals, user management, category management, promotional banners.
- **Accounting & Ledgers:** Host balance calculations, site clearing engines (`SiteClearingHostAutoPayment`), bank card registration, credit transactions.
- **Content Platform:** Blog engine (`BlogPostAppService`), dynamic categories.

## 3. Missing Capabilities
- **Online Booking Checkout:** Intentionally bypassed in V10 frontend details page in favor of Direct Lead Generation.
- **AI Agents / LLM Integration:** Runtime C#/JS AI engines do not exist in the codebase; features are documented as post-release Phase 2 roadmap.

## 4. Legacy vs. New V10 Components
- **Legacy Components:** Online reservation state machine (`ReserveStateContext`, `WaitReserveState`), online booking payment checkout gateways.
- **V10 Components:** Intercepted lead generation contact reveal (`wwwroot/js/app/advertise/item.js`), interactive Persian RTL SPA (`wwwroot/v10-app.js`), score-based homepage ranking.

## 5. Business Model Reality
- **Business Model:** Direct Lead-Generation Marketplace (Search -> View Details -> Contact Host Directly).
- **Evidence:** Intercepted booking flow in `Amlakbashi.Host/wwwroot/js/app/advertise/item.js` (lines 195–220, commit `b0e570c`).
