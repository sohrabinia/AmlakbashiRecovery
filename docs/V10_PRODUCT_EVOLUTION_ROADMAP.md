# AmlakBashi V10 Product Evolution Roadmap

## 1. Current Product State
- **Baseline Git Tag:** `v10-production-baseline`
- **Core Marketplace:** Operational property search, dynamic category filtering, Persian SEO routes, accommodation CRUD, scoring algorithms.
- **Direct Lead Generation:** Host mobile phone reveal (`ShowMobile`) active on accommodation details pages (`wwwroot/js/app/advertise/item.js`).
- **Data Protection Guarantee:** All historical reservation tables (`Reserves`, `Payments`, `WalletTransactions`) and accounting ledgers (`Amlakbashi.Accounting`) preserved.

---

## 2. Business Model Opportunities (Post-Go-Live)
1. **Host Monetization & Featured Listings:** Introduce paid listing placement options (P0/P1) to boost host visibility without requiring booking commissions.
2. **SEO Expansion:** Expand Persian city/region landing pages and structured schema markup (JSON-LD) for better organic search ranking.
3. **Lead Analytics Dashboard:** Provide hosts with real-time conversion metrics (number of contact reveals, calls received, guest interest signals).

---

## 3. Priority Matrix

| Priority | Feature / Feature Group | Business Impact | Technical Effort |
| --- | --- | --- | --- |
| **P0** | **Lead Event Tracking & Analytics** | High | Low (Log contact clicks in database) |
| **P1** | **Featured Accommodation Placement (Monetization)** | High | Medium (Add priority rank field to `Advertises`) |
| **P1** | **Persian SEO Schema Markup Expansion** | Medium | Low (Add JSON-LD to `_Master.cshtml`) |
| **P2** | **Host Lead Notification SMS Alerting** | High | Low (Integrate with existing `ReserveSendSmsAppService`) |
| **P2** | **Containerized CI/CD & Docker Setup** | Medium | Medium (Add `Dockerfile` and GitHub Workflows) |

---

## 4. Recommended Execution Plan

### Next 30 Days (Immediate Post-Release Optimization)
- Log and track `ShowMobile` lead generation events in SQL Server database for host conversion reporting.
- Expand Persian JSON-LD structured data for accommodation and city pages.

### Next 90 Days (Monetization & Host Features)
- Launch paid accommodation promotion plans for hosts.
- Add lead analytics summary widgets to the host dashboard (`/App/Dashboard`).

### Next 6 Months (Platform Modernization)
- Introduce Docker containerization and automated CI/CD deployment pipelines.
- Explore Phase 2 AI-assisted listing content optimization tools.

---

## Final Status

```
Status: READY FOR DEVELOPMENT
Baseline: v10-production-baseline
```
