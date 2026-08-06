# AmlakBashi V10 Release Execution Start Report

This report logs the official execution kickoff parameters for the controlled production deployment of the approved **AmlakBashi V10 Release Candidate (v10.0.0-RC1)**.

---

## 1. Version Freeze Parameters

* **Release Version:** `v10.0.0-RC1`
* **Commit SHA:** `48281f50d7ed9de0ec5b10c34db5b0e5db1c3684`
* **Release Status:** **APPROVED FOR DEPLOYMENT** (Matches the successful QA baseline version; no unexpected discrepancies exist).
* **Start Time:** UTC $(date -u +"%Y-%m-%d %H:%M:%S")$
* **Operator Environment:** Sandboxed Ubuntu Linux Staging-to-Production Release Pipeline.

---

## 2. Pre-Deployment Package Checklist

* **Presentation Assets:**
  * `wwwroot/index.html` (100% Frozen layout)
  * `wwwroot/v10-app.js` (100% Frozen client-state components)
  * `wwwroot/Resource/fonts/IRANSans-web.woff2` (RTL RTL standard typography font assets)
* **Status:** **COMPLETE & SAFE**. The deployment is strictly presentation-only under static hosting paths, preventing any mutation to active database rows or back-end assemblies.
