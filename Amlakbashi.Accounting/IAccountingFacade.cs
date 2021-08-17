using Amlakbashi.Accounting.PaymentContext;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs.PaymentStatisticsDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.ActionLog;
using static Amlakbashi.Core.Entities.PrizeCreditTransaction;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReservePayment;
using static Amlakbashi.Core.Entities.User;

namespace Amlakbashi.Accounting
{
    public interface IAccountingFacade
    {
        // ReservePayment Functions
        ReservePayment FindReservePayment(long id);
        void DeleteReservePayment(long id);
        ReservePayment InsertReservePayment(ReservePayment reservePayment);
        ReservePayment InsertReservePayment(int user_id, long reserve_id, long transaction_id, long ref_id, ReservePaymentType type,
            long price, ReservePaymentMethod payment_method, int operator_id = 0, bool dontSave = false);
        void InsertReservePayment(IList<ReservePayment> reservePayments);
        void UpdateReservePayment(ReservePayment editedData);
        bool ReservePaymentExists(long transactionId, int paymentMethod, long id = 0);
        IList<ReservePayment> FilterReservePayment(long reservePaymentId, long reserveId, long advertiseId,
            int userId, int operatorId, int paymentType, long transactionId, int status);
        long GetReservePaidAmount(long reserveId, Reserve.StatusStringType payType, long exceptPaymentId = -1);
        long GetReservePaidAmount(IList<ReservePayment> reserve_payments, Reserve.StatusStringType type_of_pay,
            long except_payment_id = -1);
        long GetReserveGuestPaidAmount(IEnumerable<ReservePayment> reservePayments);
        bool IsReservePaidCompletely(long reserveId);
        long GetReserveRemainedAmount(long reserveId);
        long GetReservePaymentPrice(long reserve_id, ReservePaymentType type, out DateTime date,
             out long transactionId, int targetUserID = 0);
        IList<ReservePayment> GetAllReservePayments();
        IQueryable<ReservePayment> GetAllReservePaymentsAsIQueriable();
        IQueryable<Payment> GetAllPaymentsAsIQueriable();
        bool ReserveShouldRefund(long reserveId, Reserve.ReserveStatus status, out bool refundDone);
        bool ReserveCanClear(long reserveId);
        // DiscountCoupon Functions
        DiscountCoupon FindDiscountCoupon(long id);
        DiscountCoupon FindDiscountCoupon(int userId, DiscountCoupon.DiscountCouponType type);
        DiscountCoupon InsertDiscountCoupon(int userId, DiscountCoupon.DiscountCouponType type,
            int percent, int presentorUserID = 0);
        void UseDiscountCouponForReserve(long couponId, long reserveId);
        long CalculateDiscountCouponPrice(int couponPercent, long couponCalculationPrice);
        DiscountCoupon GetMostValuableDiscountCouponIfAny(int userId);

        // CreditTransaction Functions
        IList<CreditTransaction> GetCreditListByUserId(int userId);
        CreditTransaction GetCanselInstantReserveCreditTransaction(int userId, int tranCause, long id);
        CreditTransaction FindCreditTransaction(long id);
        long IncreaseCredit(int userId, long amount, long transactionId,
            long reserveId, CreditTransactionCause transactionCause,
            out long currentCredit, string transactionCauseString = null,
            int doerUserId = 0, ActionSourceEnum actionSource = ActionSourceEnum.Undefined);
        long DecreaseCredit(int userId, long amount, long transactionId,
            long reserveId, out long currentCredit, CreditTransactionCause transactionCause,
            string transactionCouseString = null, long contactId = 0, int doerUserId = 0,
            ActionLog.ActionSourceEnum actionSource = ActionLog.ActionSourceEnum.Undefined);

        // PrizeCreditTransaction Functions
        long IncreasePrizeCredit(int userId, long amount, PrizeTransactionType type,
            long reserveId, string customTitle, int doerUserId, ActionLog.ActionSourceEnum actionSource);
        long DecreasePrizeCredit(int userId, long amount, PrizeTransactionType type,
            long reserveId, string customTitle, int doerUserId, ActionLog.ActionSourceEnum actionSource);
        void RefundPrizeCreditIfAny(long reserveId);
        void GivePresentorPrizeIfAny(long reserveId, ActionLog.ActionSourceEnum actionSource, int doerUserId);
        long GetReservePrizeAvailable(long reserveTotalPrice, long userPrizeCredit);
        void GiveAppreciateDiscountIfDeserve(long reserveId, ActionLog.ActionSourceEnum actionSource, int doerUserId);
        void UsePrizeCreditForReserve(long reserveId, int doerUserId, ActionLog.ActionSourceEnum actionSource);

        // Cart Functions
        IList<Cart> FilterCarts(int status = -1, int uid = -1, long refid = -1);

        // Payment Functions
        IList<Payment> FilterPayments(long refid, int status, int uid, DateTime fromDate, DateTime toDate);
        IList<Payment> GetPaymentRange(DateTime fromDate, DateTime toDate, int status, IList<int> userIds = null,
            bool byTotalPrice = false);
        int GetPaymentTriesCount(long reserveId, out string lastTryDateStr);
        Payment FindPayment(long id);
        void InsertPayment(Payment newPayment);
        void UpdatePayment(Payment editedPayment);
        CheckPaymentDTO CheckPaymentResult(int paymentId);

        // GroupPayment Functions
        IList<GroupPayment> FilterGroupPayment(int status);
        GroupPayment FindGroupPayment(int id);
        void InsertGroupPayment(GroupPayment newGroupPayment);
        void UpdateGroupPaymentDownloadCount(int id, int downloadCount);
        void UpdateGroupPaymentStatus(int id, GroupPayment.PaymentStatus status);
        IEnumerable<Reserve> GetGroupPaymentReserves(out List<Reserve> todayPayments,
            out List<Reserve> paymentsWithError, out List<Reserve> excludingPayments);
        void RefundCouponIfAny(long reserveId);
        void ScheduleSendMessageGroupPayment(UserContactDTO contactDTO, int delay);

        // Common
        long PayAmlakbashiPortion(long reserveId, ReservePaymentType payType,
            out bool alreadyPaid, out long price, ReservePaymentMethod paymentMethod, int userId, int doerUserId);

        bool FinalizePayment(BanksEnum bank, int pid, int userId, DateTime date,
            string tref, out string paymentResult, out string msg,
            out bool invalidInput, ActionSourceEnum actionSource, int doerUserId);

        bool TestFinalizePayment(int pid, int userId, out string msg);

        Dictionary<string, object> GeneratePaymentData(BanksEnum bank, int pid, string redirectAddress);
        GuestPayResult GuestPayReserve(int userId, long reserveId,
            int payReserveType, out long payment_id, int doerUserId,
            ActionSourceEnum actionSource, bool useCoupon, bool usePrize, long couponId);

        GuestPayResult GuestPayReserveWithCredit(int userId, long reserveId,
            int payReserveType, out long paymentId, int doerUserId,
            ActionSourceEnum actionSource, bool useCoupon, bool usePrize, long couponId);
        void GenerateReserveFinanceChart(int year, int month,
            out PaymentChartDTO TotalReservePriceChart,
            out PaymentChartDTO SitePortionChart,
            out PaymentChartDTO HostCreditorChart);
        PaymentChartDTO GeneratePaymentChart(int year, int month, bool extra_filter = false, List<int> user_list = null);

        // Podium Services
        ShebaVerificationResultDTO VerifySheba(string sheba);
        ShebaPaymentResultDTO SiteClearingHostAutoPayment(long reserveId, int operatorId);
        CheckShebaPaymentResultDTO CheckShebaPaymentStatus(long reservePaymentId);
    }
}
