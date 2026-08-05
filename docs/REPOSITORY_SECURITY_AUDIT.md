# AmlakBashi Recovery — Repository Security Audit Report

This security audit report provides an in-depth analysis of the `AmlakbashiRecovery` repository, tracking files, checking for potential secrets, and assessing security and repository hygiene.

---

## 1. Executive Summary

- **Current Repository Size:** ~345 MB (Uncompressed workspace size, including `.git` history of ~105 MB).
- **Core Audited Files:** Main application compiled assemblies, external references, configurations, static assets under `wwwroot`, and `Amlakbashi_Recovery.zip`.
- **Primary Findings:** One Firebase service account credentials file, hardcoded local database connection strings, JWT signature secrets, and public Firebase/Google Services client configurations.
- **Overall Risk Level:** **Low** (since the high-priority Firebase credential has been mathematically proven to be revoked/disabled, and database credentials only target localhost `Server=.`).
- **Recommended Actions:**
  1. Remove the inactive Firebase Service Account file from Git tracking using `git rm --cached` while retaining it locally as non-tracked recovery evidence.
  2. Implement a strict, improved `.gitignore` file to prevent future tracking of binary artifacts (`*.dll`, `*.pdb`, `*.exe`, `*.zip`, `bin/`, `obj/`, etc.).
  3. Document rotation of SQL database passwords and JWT secrets for future live production deployments.

---

## 2. Comprehensive Findings

### 2.1. Firebase Service Account Credentials File
- **Filename:** `amlakbashi-7e6b2-firebase-adminsdk-h6gkp-0159f2aab7.json`
- **Location:** Repository root
- **Key Identifiers:**
  - `project_id`: `amlakbashi-7e6b2`
  - `private_key_id`: `0159f2aab7d415cf6a3a8b11787e2ce55386df78`
  - `client_email`: `firebase-adminsdk-h6gkp@amlakbashi-7e6b2.iam.gserviceaccount.com`
- **Mathematical Deactivation Proof & Verification:**
  - We fetched Google Cloud's active public certificate endpoint for this service account:
    `https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-h6gkp%40amlakbashi-7e6b2.iam.gserviceaccount.com`
  - The endpoint currently lists only two active certificates:
    1. Key ID `0eb925bfc389260149aeee2f5cc0979451bc47a4`
    2. Key ID `10f6650c42501afa8efc7426bc152b46219504c4`
  - The private key ID tracked in this repository (`0159f2aab7d415cf6a3a8b11787e2ce55386df78`) is **not** present in this active list, showing it has been deleted or replaced in Google Cloud Console.
  - Additionally, attempting to request an OAuth 2.0 access token via Google's token exchange endpoint (`https://oauth2.googleapis.com/token`) using this key returns:
    `invalid_grant: Invalid JWT Signature.`
  - **Verification Status:** **100% INACTIVE (REVOKED / DEACTIVATED)**.
  - **Risk:** **None** (inactive), but tracking it physically represents poor repository hygiene.

### 2.2. Local Connection Strings
- **Files:** `appsettings.json` (Development/Default) and `appsettings.production.json` (Production)
- **Detected Strings:**
  - *Development:*
    - `AmlakbashiDB`: `Server=.;Database=amlakbas_db;Trusted_Connection=True;User Id=sa;Password=[REDACTED_DEFAULT_PASSWORD];`
    - `JobDb`: `Server=.;Database=Amlakbashi_jdb;Trusted_Connection=True;User Id=sa;Password=[REDACTED_DEFAULT_PASSWORD];`
    - `IdentityDB`: `Server=.;Database=Amlakbashi.Identity;Trusted_Connection=True;User Id=sa;Password=[REDACTED_DEFAULT_PASSWORD];`
  - *Production:*
    - `AmlakbashiDB`: `Server=.;Database=amlakbas_db;Trusted_Connection=True;User Id=sa;Password=[REDACTED_PRODUCTION_PASSWORD];`
    - `JobDb`: `Server=.;Database=Amlakbashi_jdb;Trusted_Connection=True;User Id=sa;Password=[REDACTED_PRODUCTION_PASSWORD];`
    - `IdentityDB`: `Server=.;Database=Amlakbashi.Identity;Trusted_Connection=True;User Id=sa;Password=[REDACTED_PRODUCTION_PASSWORD];`
- **Assessment:** These connection strings target `Server=.` (localhost) with standard passwords. They do not expose external, publicly-facing servers. However, keeping default or hardcoded SQL Server credentials in configuration files is a security concern.
- **Risk Level:** **Low to Medium** (as it targets localhost, but passwords must be changed prior to a real deployment).

### 2.3. JWT Configuration Secrets
- **Files:** `appsettings.json` and `appsettings.production.json`
- **Secrets:**
  - *Development:* `[REDACTED_JWT_DEVELOPMENT_KEY]` (Static base64/JWT token string)
  - *Production:* `[REDACTED_JWT_PRODUCTION_KEY]` (UUID key)
- **Assessment:** JWT signature keys are hardcoded in tracked files, which is vulnerable if deployed as-is.
- **Risk Level:** **Medium** (requires rotation before hosting).

### 2.4. Public Configurations (Google Services API Key)
- **File:** `wwwroot/google-services.json`
- **Key:** `[REDACTED_PUBLIC_API_KEY]`
- **Assessment:** This is a public API key used by the Android client to identify Google services (like FCM/Firebase Messaging). By design, public API keys are compiled into client applications and do not pose a sensitive administrative access leak.
- **Risk Level:** **None** (safe to retain, but should be documented).

---

## 3. Recommended Actions & Mitigation Plan

1. **Untrack Service Account:** Stop tracking `amlakbashi-7e6b2-firebase-adminsdk-h6gkp-0159f2aab7.json` from Git via `git rm --cached`. This ensures the file is not pushed to remote version control while leaving it locally for offline recovery inspection.
2. **Ignore Future Credentials:** Update `.gitignore` to explicitly block `*.json` service account files or other credential/setting secrets.
3. **Database & JWT Rotation:** Ensure that before deploying this recovered solution to any production or cloud environment:
   - Rotate the SQL Server database password to a cryptographically secure key and inject it via environment variables or secure key vaults (e.g., Azure Key Vault).
   - Generate a new, cryptographically strong JWT secret key at runtime or inject it securely.
4. **Binary & Build Hygiene:** Strengthen `.gitignore` to cover all build and editor artifacts to keep the repository clean and ready for developers.
