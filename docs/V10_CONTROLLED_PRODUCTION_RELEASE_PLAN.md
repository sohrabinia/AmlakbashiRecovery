# AmlakBashi V10 Controlled Production Release Plan

This document establishes the official controlled production release and rollback playbook for the approved **AmlakBashi V10 Release Candidate**.

---

## 1. Release Candidate Metadata

* **Version Identifier:** `v10.0.0-RC1`
* **Frozen Commit SHA:** `48281f50d7ed9de0ec5b10c34db5b0e5db1c3684`
* **Build Status:** **SUCCESSFUL** (All responsive RTL layouts, reusable components, and portal controllers render cleanly without runtime exceptions).

---

## 2. Deployment Playbook

This deployment represents a safe, presentation-only frontend upgrade within the static `wwwroot/` folder.

### Step 1: Backup Requirements
Before copying new files, backup the current active production directory:
```bash
tar -czf /var/backups/amlakbashi_v9_backup_$(date +%F).tar.gz /app/wwwroot
```

### Step 2: Copying New Front-End Assets
Extract the new V10 files directly into the production `wwwroot/` folder:
```bash
cp wwwroot/index.html /app/wwwroot/index.html
cp wwwroot/v10-app.js /app/wwwroot/v10-app.js
cp -r wwwroot/Resource/fonts/* /app/wwwroot/Resource/fonts/
```

### Step 3: Server & Service Restarts
Restart the Web/Kestrel web server to clear cache directories:
```bash
sudo systemctl restart kestrel-amlakbashi.service
```

### Step 4: Configuration Verification
Verify that the `appsettings.production.json` file remains correct, ensuring standard database connection strings (`AmlakbashiDB`, `JobDb`, `IdentityDB`) are untouched.

---

## 3. Rollback Playbook

If staging tests or post-smoke validation detects critical exceptions, execute this rollback process immediately.

### Action A — Restoring Static Frontend Files
Remove V10 files and extract the pre-release backup:
```bash
rm -f /app/wwwroot/index.html /app/wwwroot/v10-app.js
tar -xzf /var/backups/amlakbashi_v9_backup_*.tar.gz -C /
```

### Action B — Database Rollback Strategy
Since V10 introduces **zero** database migrations, EF core updates, or schema changes, DB rollback is unnecessary. However, if emergency DB restoration is required:
* Standard T-SQL transaction rollback commands can be safely executed.
* Traditional full-backups can be restored without affecting the V10 frontend.

### Action C — Clearing Caches & Restarts
Restart the web host to complete the restoration:
```bash
sudo systemctl restart kestrel-amlakbashi.service
```

---

## 4. Post-Release Smoke Test Suite

Verify that all major components render correctly following deployment:

| Test Target | Check Description | Expected Result |
| :--- | :--- | :--- |
| **Homepage** | Direct access to Kestrel on port 80/443 | Proper loading of slider, RTL structures, and Iranian headers |
| **Search & Filters** | Filter accommodation by price/amenity | Instantly filters the state with correct Rial indicators |
| **Listing / Details** | Navigating to `/Advertise/Detail/{id}` | Correct gallery, descriptions, reviews, and related items render |
| **Host Contact** | Click "Show Mobile" / "Contact Host" | Modal launches displaying WhatsApp link template (Direct Lead-Gen) |
| **Authentication** | Click login buttons on Header | High-fidelity interactive auth modals slide in smoothly |
| **Host Dashboard** | Toggle role selection to "Host Panel" | Dynamic metric cards, wizard forms, and promotion sections render |
| **Admin Panel** | Toggle role selection to "Admin Panel" | Listing approval grids and user moderation workflows function |
| **SEO Patterns** | Verify URL parameters | Existing Persian routing schema and dynamic metadata remain untouched |

---

## 5. Monitoring & SRE Checklist

Following launch, monitor the following metrics for 24-hours:
1. **Application Exceptions:** Audit syslog/Kestrel log files for startup exceptions or uncaught errors.
2. **Database Integrity:** Track active SQL connections, connection timeouts, or database transaction locks.
3. **Performance Metrics:** Review TTFB, paint times, and responsive visual load times across mobile devices.
4. **User Authentication:** Validate that JWT refreshes and cookie session renewals are persistent.
5. **Asset Loading:** Ensure images from `/content/videos` are loaded without 404 response errors.

---

## 6. Summary & Release Decision

### Final Release Decision:
**[A) Ready for controlled production release]**

### Justification:
The V10 frontend presentation layer has successfully completed and passed all pre-production tests, and is structurally decoupled from modifying compiled assemblies, business models, or database tables. It represents a zero-risk, high-impact aesthetic upgrade that conforms strictly to lead-generation guidelines. It is fully approved for immediate controlled production release.
