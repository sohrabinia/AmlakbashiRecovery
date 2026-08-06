# AmlakBashi V10 Post-Release Production Smoke Test Checklist

Following any deployment execution, the on-site quality assurance team must perform these functional checks to certify that the platform behaves stably and safely.

---

## 1. Public Discovery Workflows (Guest Portal)

Verify that the public user discovery flows operate seamlessly without exceptions:

- [ ] **Homepage Layout & RTL:** Load the primary domain (`GET /`) and confirm that standard Persian RTL alignment, brand colors, and elements load correctly.
- [ ] **Property Search:** Perform keyword, city, and category filter searches. Confirm that the search results render dynamically.
- [ ] **Property Detail Page:** Access an active listing detail page (e.g., `/Advertise/Detail/10`). Ensure that photos, prices, description, and dynamic maps are fully rendered from the database.
- [ ] **Contact Host Flow:** Click on the "Contact Host" button on a residence page. Confirm that the system opens a modal showing direct contact numbers (mobile, WhatsApp, email) and does *not* guide the user to an online payment or reservation checkout screen.

---

## 2. Authenticated User Flows (Member Portal)

Verify user session stability and identity controls:

- [ ] **User Authentication:** Trigger the login popup, enter credentials, and confirm successful cookie session generation.
- [ ] **Profile Modification:** Access the member dashboard, update user parameters (e.g., bio, Sheba bank details), and save. Verify that success notifications render cleanly.
- [ ] **Role Transition:** Confirm that changing roles between Guest, Host, and Admin adapts the dashboard interfaces instantly.

---

## 3. Residence & Advertise Management (Host Portal)

Verify active host controls and listing workflows:

- [ ] **Host Dashboard:** Confirm total listings, leads count, and payout graphs load and render with dynamic database metrics.
- [ ] **Advertisement Wizard:** Go through the 4-step wizard (Details, Images, Pricing, Review). Confirm that validation rules succeed and the BBQ checkbox behaves correctly.
- [ ] **Promotions & Laddering (Nardeban):** Click on "Nardeban" or "Last Chance" on an existing listing and confirm that the item's search ranking/priority updates.

---

## 4. Moderation & Back-office Controls (Admin Portal)

Verify platform governance and administrative capabilities:

- [ ] **Admin Dashboard:** Confirm that the administrative back-office loads, showing correct platform totals (listings, active users, and recent payout requests).
- [ ] **Listing Moderation:** Navigate to the moderation queue, inspect pending listings, and click "Approve" or "Reject". Verify that database records update instantly.
- [ ] **Transaction Ledger:** Access the transaction history and confirm that previous reservation logs, accounting ledger entries, and payout data render correctly.

---

## 5. SEO & Routing Integrity

Verify that search engine indexing configurations are fully preserved:

- [ ] **Persian URL Structures:** Verify that historical Persian URL routes like `/Advertise/Detail/{id}` are correctly resolved.
- [ ] **Static Meta Elements:** Confirm that standard HTML head headers (title, keywords, description, and meta tags) are correctly rendered by Kestrel to preserve Google Search Console crawls.
