# AmlakBashi Recovery — Production Readiness Final Report

This report presents the final evaluation of the recovered **AmlakBashi** solution, establishing its preparedness for controlled live production deployment.

---

## 1. Readiness Audit Status & Evaluation

- **Target Recovery Baseline:** Cloned from `sohrabinia/AmlakbashiRecovery`.
- **Infrastructure Status:** Fully verified.
- **Database Status:** Relational schema models and context mappings are 100% verified and structurally compatible.
- **Final Decision Outcome:** **B) Ready after minor configuration fixes**

### 1.1. Justification for Decision
The recovered assemblies and configurations are structurally complete, secure, and maintain high-fidelity preservation of the original business flows. However, prior to live hosting, a few minor environment-centric configurations must be applied to ensure complete operational readiness.

---

## 2. Minor Configuration Actions Required Pre-Deployment

To guarantee successful execution during live production hosting, the following required actions must be executed:

1. **Regenerate Firebase Credentials Key:**
   - **Reason:** The tracked JSON credentials key has been revoked on GCP.
   - **Action:** Generate a new active GCP Service Account private key JSON file for project `amlakbashi-7e6b2` and place it locally in the workspace (remaining ignored by our updated `.gitignore`).
2. **Establish Local Redis Instance:**
   - **Reason:** Caching relies on a local Redis server.
   - **Action:** Start the Redis Server daemon on standard port `6379`.
3. **Provision Windows Host & IIS:**
   - **Reason:** Prevents absolute drive path violations (`E:/videos` reference) and native Ubuntu .NET 5.0 glibc mismatches.
   - **Action:** Host on Windows IIS running the ASP.NET Core 5.0 Hosting Bundle.

---

## 3. Disruption-Free Rollback Strategy Summary

If any post-deployment anomalies or database connection issues are encountered during health checks:
1. Stop the IIS Application Pool `AmlakbashiPool`.
2. Delete the deployment directory and restore the pre-deployment state from the backup zip package.
3. Execute SQL Server recovery commands to restore pre-deployment database backups (`amlakbas_db_pre_deploy.bak`).
4. Restart the IIS Application Pool and verify baseline stability.
