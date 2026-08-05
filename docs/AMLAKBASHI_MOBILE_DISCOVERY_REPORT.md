# AMLAKBASHI MOBILE APPLICATION DISCOVERY REPORT (VERSION 1.0)
## Complete Mobile Architecture Audit & Integration Mapping

This report presents the findings of the complete discovery audit of previous AmlakBashi versions to determine the existence, technology, source, backend integration, and features of the AmlakBashi mobile application.

---

## 1. MOBILE APPLICATION EXISTENCE

*   **Existence Status:** **YES**
*   **AmlakBashi Mobile App ID:** `1:937079534215:android:460341106125af9744fe22`
*   **Android Package Name:** `com.amlakbashi.app`
*   **Firebase Storage Bucket:** `comamlakbashiapp.firebasestorage.app`
*   **GCP Project Number:** `937079534215`

### Structural Evidence:
1.  **Firebase Client Metadata:** The file `wwwroot/google-services.json` is physically present in the repository, registering package `com.amlakbashi.app` for Android SDK integration and push notification (FCM) delivery.
2.  **App Area Views:** The compiled assembly `Amlakbashi.Host.Views.dll` contains extensive view mappings under a dedicated MVC Area named **`App`**. This includes `Areas_App_Views_AppHome_Main`, `Areas_App_Views_AppUser_Profile`, and `Areas_App_Views_AppAdvertise_UpdateBasic`.
3.  **Dedicated Api Controllers:** The core host assembly `Amlakbashi.Host.dll` exposes a full suite of mobile-specific controller endpoints prefixed with `Api` and `App` (such as `ApiRegionController`, `ApiUserController`, `ApiChatController`, and `AppHomeController`).

---

## 2. PLATFORM & TECHNOLOGY IDENTIFICATION

The AmlakBashi mobile experience is designed as a **Hybrid / Web-Wrapped PWA Application** utilizing native client wrappers for Android:

### Android Wrapper:
*   **Exists:** **YES**
*   **Technology:** **Native Android Wrapper / WebView shell** compiled with Java/Kotlin (or Cordova/Capacitor), packaging the app package `com.amlakbashi.app`.
*   **Target URL:** Configured to load the Web App's localized mobile area `/App/*` (e.g. `/App/AppHome/Main`).
*   **Build Status:** The raw native Android Java/Kotlin wrapper source code is **NOT present** in the active repository workspace. The application serves the underlying responsive web views from the MVC host.

### iOS Platform:
*   **Exists:** **NO / DEPRECATED**
*   **Technology:** No active iOS bundle identifiers exist in the firebase configurations. iOS users interact directly with the responsive main marketplace URL or the `/App` PWA routes.

---

## 3. SOURCE CODE LOCATION

*   **Underlying Web Experience (`/App`):** Embedded directly inside the core application view folders.
    *   *MVC Path:* `/Areas/App/Views/...` (rendered dynamically by `Amlakbashi.Host.Views.dll`).
*   **Native Wrapper Code:** Not kept in the active repository. This is historically maintained in a separate repository or client-side workspace.

---

## 4. BACKEND INTEGRATION & API COMPATIBILITY

The mobile wrapper consumes two primary families of controllers inside `Amlakbashi.Host.dll`:

### 4.1 Mobile-Friendly Web Controllers (App Area)
*   `AppHomeController` (Dashboard and main landing templates).
*   `AppUserController` (Profile setups, favorites list, change password).
*   `AppAdvertiseController` (Host property creation, images uploads, updates).
*   `AppReserveController` (Billing status and invoices).

### 4.2 Native API Endpoints (`Api*` Prefixes)
*   **Authentication:** `ApiUserController` (Mobile OTP verification and login triggers).
*   **Real-time Communication:** `ApiChatController` & `ApiSupportChatController` (SignalR websocket hub for host-guest messages).
*   **Media & Uploads:** `ApiFileController` (Direct file streams for listing photos).
*   **Financial Ledger:** `ApiWalletController` (Host transactions and package credits).

---

## 5. FEATURES INVENTORY

### Guest / User Actions:
*   [x] OTP-based Login and Registration.
*   [x] Advanced location and category filtering.
*   [x] Add property listings to Favorites.
*   [x] Visual check-ins, description readings, and review lists.
*   [x] Support Chat integration with site admins.

### Host Actions:
*   [x] Multi-step listing manager (General, Basic, Extra amenities, HotelRoom options).
*   [x] Image uploading directly from device storage.
*   [x] Instant wallet balance top-ups.
*   [x] Promotion management (Ladder, Pin, and LastChance triggers).

---

## 6. SYSTEM DEPENDENCIES

*   **Firebase SDK:** Enforces push notification messaging utilizing libraries under `wwwroot/Resource/Scripts/firebase-5.9.4` and `firebase-initialization.js`.
*   **Google Services:** Client configuration API keys configured via Google Services JSON schema.
*   **SignalR:** Powers real-time support chat endpoints.

---

## 7. V10 MIGRATION RISK ANALYSIS

To successfully protect mobile operations during the Version 10.0 transformation, the following constraints must be respected:

1.  **Session Stability Protection:** Users must **NEVER** be randomly logged out of the mobile web app during database migrations or AppPool recycles. Since the mobile wrapper relies on persistent cookies and JWT tokens, the ASP.NET Core Data Protection Keys must be persisted inside the DB schema (`DataProtectionKeys` table) as defined in our T-SQL migration.
2.  **OTP Authentication Security:** Maintaining backwards compatibility with `ApiUserController` and Kavenegar SMS login flows is critical to prevent breaking mobile client logins.
3.  **Route Protection Compatibility:** Dynamic API protection must not block mobile wrappers from hitting `/Areas/App/*` or `/Api/*` routes.

---

## 8. RECOMMENDATION & STRATEGY

### **Option B: Modernize the Existing Mobile Application (PWA + Native Hybrid)**

#### Justification:
AmlakBashi’s mobile architecture is highly elegant. Instead of building a massive, resource-heavy native codebase from scratch, the platform utilizes a **hybrid wrapper** that serves highly responsive Razor views compiled in `Amlakbashi.Host.Views.dll` under `/Areas/App`.
This approach offers major benefits:
1.  **Zero-Redundancy Deployment:** Any bug fix, UI change, or text update on the web host is instantly updated in the mobile app without requiring a new store release.
2.  **Peak Performance:** Combines fast native capabilities (FCM push notifications, device camera upload) with lightweight web rendering.

#### Next Integration Steps for Version 10:
*   **Step 1:** Execute database migrations (`docs/V10_Enterprise_Platform_Migration.sql`) on the SQL Server host to establish stable session storage, RTR, and Data Protection databases.
*   **Step 2:** Ensure the `/App/*` and `/Api/*` routes are fully optimized and integrated with the new AI DevOps, SRE, and Image Intelligence Agents to provide automated mobile performance metrics.
