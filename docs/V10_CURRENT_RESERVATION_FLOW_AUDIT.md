# AmlakBashi V10 Current Reservation Flow Audit

## 1. Inventory & Classification of Reservation Components

| Component | Type | Path / File | Purpose | Classification |
| --- | --- | --- | --- | --- |
| **`ReserveController.cs`** | Controller | `Amlakbashi.Host/Controllers/ReserveController.cs` | MVC Controller handling guest reserve requests | **Internal only** (Disabled on frontend public details page; accessible for admin/history) |
| **`AppReserveController.cs`** | Controller | `Amlakbashi.Host/Areas/App/Controllers/AppReserveController.cs` | Host/Guest panel reserve management controller | **Keep** (Maintains host/guest historical reservation management dashboard) |
| **`ReserveAppService.cs`** | Application Service | `Amlakbashi.Application/Services/ReserveServices/ReserveAppService.cs` | CQRS Service for creating and updating reserves | **Internal only** (Preserved for historical ledgers and admin approvals) |
| **`SiteClearingHostAutoPayment.cs`** | Accounting Service | `Amlakbashi.Accounting/Services/SiteClearingHostAutoPayment.cs` | Payout engine settling host balances | **Keep** (Calculates historical host balances and clearing payouts) |
| **`Payment.cs`** | EF Core Entity | `Amlakbashi.Core/Entities/Payment.cs` | Bank transaction entity | **Keep** (Mandatory for schema integrity & historical banking records) |
| **`CreditTransaction.cs`** | EF Core Entity | `Amlakbashi.Core/Entities/CreditTransaction.cs` | Wallet transaction entity (`[Table("WalletTransactions")]`) | **Keep** (Mandatory for wallet ledgers and historical host balances) |
| **`ReserveSendSmsAppService.cs`** | Application Service | `Amlakbashi.Application/Services/ReserveServices/ReserveSendSmsAppService.cs` | SMS notification service | **Keep** (Handles host contact notifications) |
| **Public Reserve UI (`item.js`)** | JavaScript | `Amlakbashi.Host/wwwroot/js/app/advertise/item.js` | Accomodation details reservation handler | **Replace** (Intercepted to direct guest to `ShowMobile` host contact reveal) |

---

## 2. Summary
- All backend EF Core entities (`Reserve`, `Payment`, `CreditTransaction`) and C# services in `Amlakbashi.Application` and `Amlakbashi.Accounting` are 100% **preserved and kept** to guarantee database migration safety and historical ledger integrity.
- The public reservation checkout UI on the accommodation details page is **replaced** with the Direct Lead Generation flow (`ShowMobile` host contact reveal), successfully aligning public user experience with the V10 marketplace model.
