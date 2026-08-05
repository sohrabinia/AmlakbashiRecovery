# AMLAKBASHI V10 MOBILE COMPATIBILITY ANALYSIS REPORT
## Complete API Mapping, Session Persistence & Integration Strategy

This report performs a comprehensive compatibility analysis and API dependency mapping of the discovered AmlakBashi Android Hybrid Application (`com.amlakbashi.app`) against the Version 10.0 Enterprise specifications.

---

## 1. MOBILE API DEPENDENCY MAP

The Android Hybrid WebView application interacts with the .NET backend via two layers: the hybrid Web views served from the `/Areas/App` routes and native JSON API endpoints hosted under the `Amlakbashi.Host.Controllers.WebService` namespace.

The complete physical map of active mobile API endpoints extracted from the `Amlakbashi.Host.dll` assembly metadata is detailed below:

| Controller Name | Endpoint Route | Action Method / Purpose | Current Status |
| :--- | :--- | :--- | :--- |
| **ApiAccountController** | `/api/account/login` | `LoginOrRegister` / Initial phone trigger | **100% Extracted** |
| | `/api/account/login/verify` | `LoginVerify` / OTP confirmation | **100% Extracted** |
| | `/api/account/login/resendcode`| `ResendVerifyCode` / OTP resend | **100% Extracted** |
| | `/api/account/refreshtoken` | `RefreshToken` / Dynamic JWT rotation | **100% Extracted** |
| | `/api/account/panel` | `ChangePanel` / User profile view toggles | **100% Extracted** |
| **ApiUserController** | `/api/user/Profile` | Get/Set User Profile details | **100% Extracted** |
| | `/api/user/phonenumber/update` | `UpdateMainPhoneNumber` / Update requests| **100% Extracted** |
| | `/api/user/phonenumber/verify` | `VerifyMainPhoneNumber` / Verifies change | **100% Extracted** |
| | `/api/user/host/{id:int}` | Retrieves specific Host profile records | **100% Extracted** |
| **ApiResidenceController**| `/api/residence/create` | Initiates new accommodation creations | **100% Extracted** |
| | `/api/residence/update/basic/{id}`| `UpdateBasicInfo` (Title, rooms, rules) | **100% Extracted** |
| | `/api/residence/update/general/{id}`| `UpdateGeneralInfo` (Location, specs) | **100% Extracted** |
| | `/api/residence/update/final/{id}`| `UpdateFinalInfo` (Completion step) | **100% Extracted** |
| | `/api/residence/calendar/{id}` | `UpdateCalendarData` (Host avail. dates)| **100% Extracted** |
| | `/api/residence/favorite/{id}` | Toggles guest accommodation favorites | **100% Extracted** |
| **ApiFileController** | `/api/file/user` | `UpdateUserProfileImage` upload stream | **100% Extracted** |
| | `/api/file/advertise/{advId}/{fId}`| `AddAdvertiseImage` upload stream | **100% Extracted** |
| | `/api/file/advertise` | `DeleteAdvertiseImage` remove stream | **100% Extracted** |
| **ApiReserveController** | `/api/reserve/invoice/{id}` | `Submit` / Displays billing statements | **100% Extracted** |
| | `/api/reserve/discount` | `Start` / Applies promo coupon codes | **100% Extracted** |
| | `/api/reserve/cancel` | `Cancel` / Handles cancellations | **100% Extracted** |

---

## 2. AUTHENTICATION & SESSION PERSISTENCE COMPATIBILITY

To enforce the **No Forced Logout** requirement in Version 10.0, the mobile authentication architecture utilizes:

1.  **JWT Bearer Scheme Integration:** Native API endpoints utilize standard JWT tokens passed in authorization headers (`Bearer {token}`).
2.  **No Forced Logouts on Recycle:** Because cryptographic keys are now persisted inside the SQL database via `DataProtectionKeys`, recycles on the Kestrel / IIS web app pool will **not** invalidate the tokens used by the mobile WebView clients.
3.  **Refresh Token Rotation (RTR):** Consumes `/api/account/refreshtoken` to dynamically fetch fresh JWT access tokens using the rotating tokens stored in the `UserRefreshTokens` table. This ensures seamless user sessions.

---

## 3. MOBILE SPECIFIC CONFIGURATIONS & COMPATIBILITY

*   **Push Notifications (FCM):** Google Services integration remains active. The client SDK utilizes `wwwroot/google-services.json` to handle message payloads.
*   **Real-time Communication:** Consumes the `ApiChatController` and `ApiSupportChatController` SignalR websocket hubs to deliver messaging features.

---

## 4. V10 MIGRATION RISK ANALYSIS

*   **Endpoint Integrity Risk:** Restructuring route layouts or namespace maps in Version 10.0 will immediately break the mobile wrapper. **All `/api/*` endpoint paths must be preserved exactly as mapped in Section 1**.
*   **iOS Presence Verification:** No physical trace, source directory, or bundle ID exists for a native iOS application. **Only Android native configuration (`com.amlakbashi.app`) is active.** iOS users must continue utilizing the responsive web routes directly.
*   **SMS Gateway Reliability:** Mobile login relies entirely on OTP. The OTP SMS send API (`login/resendcode`) must maintain direct compatibility with the Kavenegar SMS dispatch configuration.

---

## 5. MOBILE STRATEGY RECOMMENDATION

### **Option B: Modernize the Responsive Hybrid WebView Wrapper**

#### Comparative Assessment:
*   **Option A (Keep Wrapper as-is):** Lowest cost, but misses opportunities to monitor mobile performance and track latency spikes.
*   **Option C (Rebuild in Flutter/React Native):** Extremely high cost and risk. Requires rewriting over 140 recovered view templates, setting up multi-platform authentication, and risking user drops during store updates.
*   **Option B (Recommended - Modernize Wrapper):** The most secure and cost-effective approach. Keeps the lightweight native wrapper (`com.amlakbashi.app`), but enhances the backend integration.

#### Strategic Advantages:
1.  **Instant Upgrades:** Any backend performance improvements, layout updates, or AI features (like the Listing Editor’s automated description generation) are immediately active in the wrapper without requiring user App Store updates.
2.  **DevOps & SRE Analytics Integration:** The new AI DevOps Agent can directly monitor latency, success ratios, and crash metrics on `/api/*` endpoints to protect the mobile user experience.
3.  **Low Development Cost & Zero Data Risk:** Preserves 12 years of user adaptation and completely avoids data migration risks.
