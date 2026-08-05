# AMLAKBASHI V10 — COMPLETE LEGACY UI DISCOVERY & FIGMA HANDOFF DOCUMENT

This document compiles a complete, detailed inventory of every single user-visible interface element currently existing in **AmlakBashi V10**. It maps out public pages, login states, dashboards, modals, shared components, and user flows based on the source code extracted from the production assembly `Amlakbashi.Host.Views.dll` and controllers in `Amlakbashi.Host.dll`.

**Target Audience:** Figma UI/UX Design Team
**Scope:** Discovery & documentation only. Features or routes with no user-visible UI (database-only logic, internal backend modules, developer-only endpoints) are strictly excluded.

---

## SECTION 1 — PUBLIC WEBSITE INVENTORY

These are the public pages accessible to anonymous search users and guests prior to authentication.

### 1. Homepage
*   **Screen Name:** Homepage
*   **Route:** `/` or `/Home/Index`
*   **User Role:** Anonymous User / Public Guest
*   **Purpose:** Initial landing, search initialization, and featured property listings.
*   **Current Behavior:**
    *   Renders a massive hero background header containing the primary search component.
    *   Showcases property categories (e.g., Villas, Suites, Apartments, Cottages).
    *   Features a horizontal slider of "Last Minute Deals" (Last Chance Accommodation).
    *   Includes a featured blog and news section near the bottom.
*   **Components Used:**
    *   `Views_Shared__Layout` & `Views_Shared__Master`
    *   `Views_Shared__HomePageSearch`
    *   `Views_Shared__LastChanceAccommodation`
    *   `Views_Shared__HomePageBlogNews`
    *   `Views_Shared__Footer`
*   **Data Source:** `CategoryController`, `PostController` (last-minute listings), and `BlogPostController`.
*   **Redesign Notes:** Convert into a modern, high-converting homepage with responsive visual categories, high-contrast typography, and intuitive Persian typography (IRANSans/Vazir).

---

### 2. Search & Categories Filter Experience
*   **Screen Name:** Search Results Page / Dynamic Category Page
*   **Route:** `/Category/Item` or `/Post/Search`
*   **User Role:** Anonymous User / Public Guest
*   **Purpose:** Search, filtering, and listing categorized properties.
*   **Current Behavior:**
    *   Displays a left or top sidebar filter control depending on screen width.
    *   Includes price-range selection sliders, capacity counters, region selectors, and property types.
    *   Renders list-cards for matching properties with title, price, images, scoring stars, and "Last Minute" tag.
*   **Components Used:**
    *   `Views_Category_Item`
    *   `Views_Shared__AdvertiseFilter`
    *   `Views_Shared__AdvertiseListItems`
    *   `Views_Shared__AdvertiseRegionSelector`
    *   `Views_Shared__StarRating`
*   **Data Source:** `CategoryController` pulling search indexes from the database mapped to `Advertise` tables.
*   **Redesign Notes:** Build a sticky sidebar filter. The design should support immediate filter state visualization, tag-based filter removals, and cleaner layout grids for property cards.

---

### 3. Property Detail Page (Accommodation Item)
*   **Screen Name:** Property Detail Page
*   **Route:** `/Accomodation/Item/{id}`
*   **User Role:** Anonymous User / Guest
*   **Purpose:** Full details on a single property listing.
*   **Current Behavior:**
    *   Large media gallery with image lightboxes or sliders.
    *   Displays general information: Capacity, rooms, land area, bed count, rules, and host contact details.
    *   Features a Persian date picker for checking vacancy.
    *   **Crucial current business logic:** Displays a "Contact Host" direct button or triggers support chat popup. Instant reserve or physical online checkout is bypassed; users view direct contact or check details statically.
*   **Components Used:**
    *   `Views_Accomodation_Item`
    *   `Views_Accomodation_AccPagePartials__AccPageAddress`
    *   `Views_Accomodation_AccPagePartials__AccPageAmenities`
    *   `Views_Accomodation_AccPagePartials__AccPageBasicInfo`
    *   `Views_Accomodation_AccPagePartials__AccPageComments`
    *   `Views_Accomodation_AccPagePartials__AccPagePrice`
    *   `Views_Accomodation_AccPagePartials__AccPageRules`
*   **Data Source:** `AccomodationController` returning the `AccommodationItemDTO`.
*   **Redesign Notes:** High priority detail layouts. Needs clear visual distinction for primary actions (call/chat host), well-structured grid icons for amenities, elegant reviews layout, and sticky pricing bar on mobile scroll.

---

## SECTION 2 — AUTHENTICATION EXPERIENCE

Unified user sign-in, mobile verification, and onboarding.

### 1. Unified Auth Modal / Page
*   **Screen Name:** Login & Register Popup / Mobile Onboarding Screen
*   **Route:** `/User/MobileLogin` or triggered via the `Views_Shared__LoginPopup` modal.
*   **User Role:** Guest / Host
*   **Purpose:** Multi-step authentication flow (via mobile SMS OTP or email password).
*   **Current Behavior:**
    1.  *Step 1 (Identifier):* Asks for mobile phone number or email (with international flag selector).
    2.  *Step 2 (OTP Verification):* If registering or logging via code, displays a code key-up input with a resend button and countdown timer.
    3.  *Step 3 (Password Screen):* Option to enter a static password instead.
    4.  *Step 4 (Onboarding / Register):* Asks for Name, Last Name, and optional referral/presenter code if the user is new.
*   **Components Used:**
    *   `Views_Shared__LoginPopup`
    *   `Views_Shared__resend_form_login`
    *   `Views_User_MobileLogin`
*   **Data Source:** `UserController` invoking authentication services.
*   **Redesign Notes:** Create a unified, step-by-step card format that handles transition states smoothly with neat micro-animations. Eliminate raw text overlays and enhance inline validation errors.

---

## SECTION 3 — GUEST / USER PANEL

The dashboard and user-panel views seen by registered regular guests (Regular User, `Type = 0`).

### 1. Guest Dashboard Main Hub
*   **Screen Name:** Regular Dashboard
*   **Route:** `/Post/dashboard`
*   **User Role:** Registered Guest
*   **Purpose:** Simple launcher to navigate guest account actions.
*   **Current Behavior:**
    *   Renders cards leading to profile details, favorite lists, wallet credit balance, and reservation lists.
*   **Components Used:**
    *   `Views_Post_dashboard`
    *   `Views_Shared__DashboardAccount`
*   **Data Source:** `PostController` returning user-scoped menu options.

---

### 2. Guest Profile & Password Manager
*   **Screen Name:** My Profile Manager / Change Password
*   **Route:** `/User/ProfileManager` and `/User/ChangePassword`
*   **User Role:** Guest / Host
*   **Purpose:** View and update account demographics and account password.
*   **Current Behavior:**
    *   Forms for entering First Name, Last Name, National Code, Email, Gender, and password parameters.
*   **Components Used:**
    *   `Views_User_ProfileManager`
    *   `Views_User_ChangePassword`
*   **Data Source:** `UserController` mapping to standard Identity tables.

---

### 3. Favorites Page
*   **Screen Name:** My Saved Listings
*   **Route:** `/Post/FavoriteManager`
*   **User Role:** Guest
*   **Purpose:** List of bookmarked/starred properties.
*   **Current Behavior:**
    *   Responsive list showing standard property card designs with an option to remove from favorites.
*   **Components Used:**
    *   `Views_Post_FavoriteManager`
*   **Data Source:** Mapped to user's favorite table bindings.

---

### 4. My Wallet Balance & Transactions
*   **Screen Name:** Wallet Manager
*   **Route:** `/User/UserCreditManager`
*   **User Role:** Guest / Host
*   **Purpose:** Displays virtual credits, wallet balance, and previous credit transactions.
*   **Current Behavior:**
    *   Renders dynamic current balance in Tomans.
    *   Lists historic ledger entries showing transaction type (increase, decrease, promotion, payout).
*   **Components Used:**
    *   `Views_User_UserCreditManager`
    *   `Views_Wallet__UserWalletInfo`
*   **Data Source:** `WalletController` returning host/guest ledger details.

---

### 5. My Reservations List
*   **Screen Name:** Guest Reservation History
*   **Route:** `/Reserve/ReserveItemManager?selecttype=1`
*   **User Role:** Guest
*   **Purpose:** Displays list of requested, accepted, or historic property bookings.
*   **Current Behavior:**
    *   Table/card layout highlighting booking status (Pending approval, Paid, Expired, Canceled).
*   **Components Used:**
    *   `Views_Reserve__ReserveList`
    *   `Views_Reserve__ReserveItem`
*   **Data Source:** `ReserveController` returning reservation objects.

---

## SECTION 4 — HOST PANEL

Specifically used by property managers and hosts (`Type = 1` or higher).

### 1. Host Dashboard Main Hub
*   **Screen Name:** Host Dashboard Launcher
*   **Route:** `/Post/dashboard`
*   **User Role:** Host
*   **Purpose:** Central hub for property owners.
*   **Current Behavior:**
    *   Launches My Listings (`/post/personal`), Profile, Add New Listing (`/accomodation/accbasicform`), Reservation Invoices (`/reserve/invoice`), Wallet balance, and My Bookings.
*   **Components Used:**
    *   `Views_Post_dashboard`
    *   `Views_Shared__DashboardAccount`

---

### 2. My Listings Page
*   **Screen Name:** Personal Listings Manager
*   **Route:** `/Post/Personal`
*   **User Role:** Host
*   **Purpose:** List and manage properties owned by the current host.
*   **Current Behavior:**
    *   Row or grid elements containing property thumb, title, verification state (Approved, Suspended, Awaiting Admin approval), and action items: Edit, Pin/Promotion, Set Price/Calendar.
*   **Components Used:**
    *   `Views_Shared__AccSetOccupied`
    *   `Views_Shared__AccSetPrice`
    *   `Views_Shared__AccSetMinNorouzReserve`
*   **Data Source:** `PostController` returning properties owned by user.

---

### 3. Add & Edit Listing Wizards (Multiphase Forms)
*   **Screen Name:** Add New Property / Edit Listing
*   **Route:** `/Accomodation/AccBasicForm` or `/Accomodation/AccComplexForm` or `/Accomodation/AccExtraForm` or `/Accomodation/AccGeneralForm` or `/Accomodation/AccHotelForm`
*   **User Role:** Host
*   **Purpose:** Comprehensive forms to publish properties. Divided into multiple specialized screens depending on property type.
*   **Current Behavior:**
    *   Asks for address, rules, amenities, room layout, prices, land area, capacities, and license codes.
*   **Components Used:**
    *   `Views_Accomodation_AddEditFormInputs__AccAddressInput`
    *   `Views_Accomodation_AddEditFormInputs__AccAmenitiesInput`
    *   `Views_Accomodation_AddEditFormInputs__AccBedInput`
    *   `Views_Accomodation_AddEditFormInputs__AccCapacityInput`
    *   `Views_Accomodation_AddEditFormInputs__AccPhotoInput`
    *   `Views_Accomodation_AddEditFormInputs__AccPriceInput`
    *   `Views_Accomodation_AddEditFormInputs__AccRulesInput`
*   **Redesign Notes:** Figma should optimize these complex multiphase wizard steps. Reduce screen clutter, use visual icons for amenities, and add drag-and-drop media upload indicators.

---

### 4. Promotion, Pin, and Ladder (Nardeban) Screen
*   **Screen Name:** Paid Promotions Hub
*   **Route:** Inline on listings page via `/Views/Shared/_PinAccommodation` modal.
*   **User Role:** Host
*   **Purpose:** Pay virtual or gateway credits to Pin, Ladder (Nardeban), or flag a listing as a Last Chance Deal.
*   **Current Behavior:**
    *   Modal popup requesting selection of promotion duration and fee details. Deducts from host wallet.
*   **Components Used:**
    *   `Views_Shared__PinAccommodation`
*   **Redesign Notes:** Package this into a beautiful upsell modal.

---

### 5. Invoices & Saman / Pasargad Payments
*   **Screen Name:** Host Financial Statement / Payment Receipt
*   **Route:** `/Reserve/Invoice` or `/Cart/TransactionResult`
*   **User Role:** Host / Guest
*   **Purpose:** Financial checkout success/failure screens.
*   **Current Behavior:**
    *   Success checkmarks or warning symbols detailing gateway transactions, card numbers, transaction IDs, and ledger balances.
*   **Components Used:**
    *   `Views_Cart_TransactionResult`
    *   `Views_Cart_Index`

---

## SECTION 5 — ADMIN PANEL

Accessible only to users with high clearance administrative roles. Consists of a sidebar layout with major modules.

### 1. Admin Statistic & Overview Dashboard
*   **Screen Name:** Admin Main Portal
*   **Route:** `/Admin/Home` and `/Admin/AdminStatistic`
*   **User Role:** System Admin
*   **Purpose:** Management launcher and visual chart analytics.
*   **Current Behavior:**
    *   Displays summary dashboard cards leading to Admin property lists, User accounts management, and Reservation details.
    *   Renders charts displaying listing growths, payments, and registered users.
*   **Components Used:**
    *   `Views_Shared__MasterAdmin`
    *   `Views_Admin_Home`
    *   `Views_Admin_AdvertiseChart`
    *   `Views_Admin_PaymentChart`

---

### 2. User & Roles Account Management
*   **Screen Name:** Admin Account Directory
*   **Route:** `/User/Index` and `/Role/Index`
*   **User Role:** Admin
*   **Purpose:** Moderating and assigning permission roles to users.
*   **Current Behavior:**
    *   Comprehensive filterable user records grid with action buttons: Edit password, Edit phone number, Change roles, and Adjust virtual balance.
*   **Components Used:**
    *   `Views_User_Index`
    *   `Views_User_Edit`
    *   `Views_Role_Index`

---

### 3. Listings Moderation & Verification Portal
*   **Screen Name:** Property Review Panel
*   **Route:** `/Advertise/NewIndex` and `/Accomodation/AdminStatusForm`
*   **User Role:** Admin
*   **Purpose:** Moderating property submissions before publishing.
*   **Current Behavior:**
    *   List of submitted properties marked "Awaiting Approval".
    *   Admin reviews rules, photos, videos, amenities, and edits elements inline.
    *   Launches status modal to Approve, Suspend, or Reject with optional host notification.
*   **Components Used:**
    *   `Views_Advertise_NewIndex`
    *   `Views_Accomodation_AdminStatusForm`

---

### 4. Content Intelligence & Blog Management Hub
*   **Screen Name:** Admin Blog & News Portal
*   **Route:** `/BlogPost/Index` and `/Post/Index`
*   **User Role:** Admin
*   **Purpose:** Complete editor for static SEO content pages, news links, and blogs.
*   **Current Behavior:**
    *   Interactive WYSIWYG editor form to write blog articles, specify keywords, set meta descriptions, and associate tags.
*   **Components Used:**
    *   `Views_BlogPost_Index`
    *   `Views_BlogPost_AddEdit`
    *   `Views_Post_Index`

---

### 5. Support Chat Inbox
*   **Screen Name:** Support Agent Chatroom
*   **Route:** `/SupportChat/Index`
*   **User Role:** Admin / Support Agent
*   **Purpose:** Real-time customer chat desk.
*   **Current Behavior:**
    *   Left sidebar detailing active user chats. Right pane displaying message bubble threads with instant text replies.
*   **Components Used:**
    *   `Views_SupportChat_Index`
    *   `Views_SupportChat__ChatItemList`
    *   `Views_SupportChat__SupportItemList`

---

### 6. Accounting & Payouts Engine
*   **Screen Name:** Financial Ledgers & Cashouts
*   **Route:** `/Cart/Admin` and `/Wallet/Index`
*   **User Role:** Admin / Accountant
*   **Purpose:** Settling payouts and tracking system financials.
*   **Current Behavior:**
    *   Grid highlighting Host balances, payout Sheba codes, and a manual payout confirmation trigger.
*   **Components Used:**
    *   `Views_Cart_Admin`
    *   `Views_Wallet_Index`

---

### 7. Reservations Manager
*   **Screen Name:** Historic Reservation Manager
*   **Route:** `/Reserve/NewIndex` or `/Reserve/Admin`
*   **User Role:** Admin
*   **Purpose:** Archive tools for managing bookings.
*   **Current Behavior:**
    *   Grid detailing historic reservations, refund status, and host settlement values.
*   **Components Used:**
    *   `Views_Reserve_Admin`
    *   `Views_Reserve__ReserveAdminDetails`

---

## SECTION 6 — MOBILE APPLICATION UI

The hybrid webview views tailored for mobile screens (`com.amlakbashi.app`).

### 1. Mobile App Main Home
*   **Screen Name:** Mobile Home / Search Splash
*   **Route:** `/Areas/App/Views/AppHome/Main`
*   **User Role:** Mobile Guest
*   **Purpose:** Native feel category selection and prominent search inputs.
*   **Components Used:**
    *   `Areas_App_Views_AppHome_Main`
    *   `Areas_App_Views_AppHome__HomePageSearch`
    *   `Areas_App_Views_Shared__AppLayout`

---

### 2. Mobile Search Results Card Layout
*   **Screen Name:** Mobile Listing Grid
*   **Route:** `/Areas/App/Views/AppCategory/Item`
*   **User Role:** Mobile Guest
*   **Purpose:** Compact, double-column or single-row property list optimized for touch interaction.
*   **Components Used:**
    *   `Areas_App_Views_AppCategory_Item`
    *   `Areas_App_Views_AppCategory__AdvertiseListItems`

---

### 3. Mobile Host / Guest Unified App Dashboard
*   **Screen Name:** App Dashboard Hub
*   **Route:** `/Areas/App/Views/AppHome/Dashboard`
*   **User Role:** Authenticated Mobile User
*   **Purpose:** Clean app drawer navigation for Profile, Saved, Wallet, and Bookings.
*   **Components Used:**
    *   `Areas_App_Views_AppHome_Dashboard`
    *   `Areas_App_Views_Shared__AppDashboardLayout`

---

## SECTION 7 — SHARED COMPONENT INVENTORY

Reusable component cards and layouts to compile as Figma UI Library components.

### 1. Structure & Layout templates
*   `Views_Shared__Master` (Main public responsive shell)
*   `Views_Shared__MasterAdmin` (Admin layout framework)
*   `Areas_App_Views_Shared__AppLayout` (Mobile app webview wrapper)

### 2. Core Forms, Controls, and Inputs
*   `Views_Shared__DatePicker` (Persian date selector)
*   `Views_Shared__numberInput` (Capacity and numeric spinbox helper)
*   `Views_Shared__RadioButtonForm` (Clean option buttons)
*   `Views_Accomodation_AddEditFormInputs__AccAmenitiesInput` (Checkbox matrix for property amenities)

### 3. Popups & Modals
*   `Views_Shared__LoginPopup` (SMS / Email auth gateway dialog)
*   `Views_Shared__SupportChatPopup` (Real-time user chat bubble)
*   `Views_Shared__MessagePopup` (Global notification/alert banners)
*   `Views_Shared__PinAccommodation` (Upsell promotion modal)

### 4. Interactive Feedback Cards
*   `Views_Shared__StarRating` (Review stars generator)
*   `Views_Shared__LastChanceAccommodation` (Flash sales carousel widget)
*   `Views_Shared__BlogNewsItem` (Standard card for blog/news lists)

---

## SECTION 8 — USER FLOWS

Detailed map of screen-by-screen navigation loops across user profiles.

### Flow A: Guest Discovery & Support Loop
```
[Public Homepage / Search Portal]
               │
               ▼ (Enter destination / Select category)
[Search Results & Filters Page]
               │
               ▼ (Click on Property Card)
[Property Detail Screen]
               │
               ▼ (Click "Contact Host")
[Trigger SMS Login Popup (If unauthenticated)] ──► [Onboarding Profile Input]
               │
               ▼ (Authenticated)
[Reveal Direct Phone Number / Launch Support Realtime Chat popup]
```

---

### Flow B: Host Property Listing & Promotion Loop
```
[Login Screen / App Splash]
               │
               ▼
[Host Dashboard Hub]
               │
               ▼ (Click "Add Listing")
[Wizard Phase 1: General Category Select]
               │
               ▼
[Wizard Phase 2: AccBasicForm (Address, Location, Maps)]
               │
               ▼
[Wizard Phase 3: AccAmenitiesForm (Grid of features)]
               │
               ▼
[Wizard Phase 4: AccCapacityForm & Bed Layout]
               │
               ▼
[Wizard Phase 5: AccPhotoInput & Description Details]
               │
               ▼ (Submit Listing)
[Listings Manager (Shows "Awaiting Approval" state)]
               │
               ▼ (Click "Promote Listing")
[Launch Pin/Ladder Promotion Modal] ──► [Select Credit Settlement] ──► [Listing Promoted]
```

---

### Flow C: Admin Content Intelligence & Moderation Loop
```
[Admin Authentication Page]
               │
               ▼
[Admin Statistic & Home Portal]
               │
               ├─────────────────────────────────────────┐
               ▼                                         ▼
[Manage submitted listings]                    [Blog & Content intelligence Hub]
               │                                         │
               ▼ (Review items)                          ▼
[Launch Listing Status Edit Form]              [WYSIWYG BlogPost AddEdit Form]
               │                                         │
               ▼ (Approve / Reject)                      ▼
[Publish Listing live & Alert Host]            [Publish Static SEO Category / Article]
```

---

## SECTION 9 — SUMMARY OF DISCOVERED USER VIEWS
*   **Total Identified User-Visible Views:** Over **300 distinct interfaces** and partial controllers compile-checked.
*   **Target UX Improvement Areas:**
    1.  *Authentication:* Move the multiple inline OTP/Email passwords into a elegant single animated step interface.
    2.  *Listing forms:* The wizard currently spans up to 5 distinct forms. These should be combined into a modern single-page-app step layout in Figma.
    3.  *Property cards:* Improve grid card alignments, badges, and high-DPI image support.
