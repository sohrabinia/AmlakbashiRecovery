
using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.PrizeCreditTransaction;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReservePayment;
using static Amlakbashi.Core.Entities.User;
using static Amlakbashi.Core.Entities.ActionLog;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using Amlakbashi.Accounting.PaymentContext;
using Amlakbashi.Core.DTOs.PaymentDTOs.PaymentStatisticsDTOs;
using System.Globalization;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Commands.AccountingCommands;
using Amlakbashi.Mediator.Events.AccountingEvents;
using log4net;
using Amlakbashi.Mediator.Commands.UserCommands;
using Microsoft.AspNetCore.Identity;
using Amlakbashi.Core.Identity.Entities;

namespace Amlakbashi.Accounting
{
    internal class AccountingFacade : IAccountingFacade
    {
        private readonly IReservePaymentAppService reservePaymentService;
        private readonly IDiscountCouponAppService discountCouponService;
        private readonly ICreditTransactionAppService creditTransactionService;
        private readonly IPrizeCreditTransactionAppService prizeCreditTransactionService;
        private readonly ICartAppService cartService;
        private readonly IPaymentAppService paymentService;
        private readonly IGroupPaymentAppService groupPaymentService;
        private readonly IAccountingRepository repository;
        private readonly IMediator mediator;
        private readonly IPaymentOperator paymentOperator;
        private readonly UserManager<AppUser> userManager;
        private readonly ILog logger;
        public AccountingFacade(IReservePaymentAppService reservePaymentService,
            IDiscountCouponAppService discountCouponService,
            ICreditTransactionAppService creditTransactionService,
            IPrizeCreditTransactionAppService prizeCreditTransactionService,
            ICartAppService cartService,
            IPaymentAppService paymentService,
            IGroupPaymentAppService groupPaymentService,
            IAccountingRepository repository,
            IMediator mediator,
            IPaymentOperator paymentOperator,
            UserManager<AppUser> userManager,
            ILog logger)
        {
            this.reservePaymentService = reservePaymentService;
            this.discountCouponService = discountCouponService;
            this.creditTransactionService = creditTransactionService;
            this.prizeCreditTransactionService = prizeCreditTransactionService;
            this.cartService = cartService;
            this.paymentService = paymentService;
            this.groupPaymentService = groupPaymentService;
            this.repository = repository;
            this.mediator = mediator;
            this.paymentOperator = paymentOperator;
            this.userManager = userManager;
            this.logger = logger;
        }

        // ReservePayment Functions
        public void DeleteReservePayment(long id)
        {
            reservePaymentService.Delete(id);
        }

        public IList<ReservePayment> FilterReservePayment(long reservePaymentId,
            long reserveId, long advertiseId, int userId, int operatorId,
            int paymentType, long transactionId, int status)
        {
            return reservePaymentService.Filter(reservePaymentId, reserveId, advertiseId,
                userId, operatorId, paymentType, transactionId, status);
        }

        public ReservePayment FindReservePayment(long id)
        {
            return reservePaymentService.Find(id);
        }

        public IList<ReservePayment> GetAllReservePayments()
        {
            return reservePaymentService.GetAll();
        }

        public IQueryable<ReservePayment> GetAllReservePaymentsAsIQueriable()
        {
            return reservePaymentService.GetAllAsIQueriable();
        }

        public IQueryable<Payment> GetAllPaymentsAsIQueriable()
        {
            return paymentService.GetAllAsIQueryable();
        }

        public long GetReservePaidAmount(long reserveId, Reserve.StatusStringType payType, long exceptPaymentId = -1)
        {
            return reservePaymentService.GetPaidAmount(reserveId, payType, exceptPaymentId);
        }

        public long GetReservePaidAmount(IList<ReservePayment> reserve_payments, Reserve.StatusStringType type_of_pay, long except_payment_id = -1)
        {
            return reservePaymentService.GetPaidAmount(reserve_payments, type_of_pay, except_payment_id);
        }

        public long GetReserveGuestPaidAmount(IEnumerable<ReservePayment> reservePayments)
        {
            return reservePaymentService.GetPaidAmount(reservePayments.ToList(), StatusStringType.Guest);
        }

        public bool IsReservePaidCompletely(long reserveId)
        {
            var reserve = repository.FindReserve(reserveId);
            var paidAmount = GetReservePaidAmount(reserveId, Reserve.StatusStringType.Guest);
            return paidAmount >= reserve.TotalPayablePrice;
        }

        public long GetReserveRemainedAmount(long reserveId)
        {
            var reserve = repository.FindReserve(reserveId);
            var paidAmount = GetReservePaidAmount(reserveId, Reserve.StatusStringType.Guest);
            return reserve.TotalPayablePrice - paidAmount;
        }

        public long GetReservePaymentPrice(long reserve_id,
            ReservePaymentType type, out DateTime date,
            out long transactionId, int targetUserID = 0)
        {
            return reservePaymentService.GetPaymentPrice(
                reserve_id, type, out date, out transactionId, targetUserID);
        }

        public bool ReserveCanClear(long reserveId)
        {
            var reserve = repository.FindReserve(reserveId);
            var reservePayments = reserve.ReservePayments;
            if (reservePayments.Any(w =>
                w.PaymentType == (int)ReservePaymentType.SiteClearingToHost))
            {
                return false;
            }
            if (reserve.Status != ReserveStatus.Reserved &&
                reserve.Status != ReserveStatus.Started &&
                reserve.Status != ReserveStatus.Completed &&
                reserve.Status != ReserveStatus.CashPay)
            {
                return false;
            }
            var guest_payed = GetReservePaidAmount(reserveId, StatusStringType.Guest);
            if (PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice, guest_payed, reserve.CouponPrice, reserve.PrizePrice) <= 0)
                return false;
            return true;
        }

        public ReservePayment InsertReservePayment(ReservePayment reservePayment)
        {
            return reservePaymentService.Insert(reservePayment);
        }

        public ReservePayment InsertReservePayment(int user_id, long reserve_id, long transaction_id,
            long ref_id, ReservePaymentType type, long price,
            ReservePaymentMethod payment_method, int operator_id = 0, bool dontSave = false)
        {
            if (type == ReservePaymentType.GuestDeposite)
            {
                var reserve = repository.FindReserve(reserve_id);
                var paidAmount = reservePaymentService.GetPaidAmount(reserve.ReservePayments.ToList(), Reserve.StatusStringType.Guest);
                if ((paidAmount + price) >= (reserve.TotalPrice - reserve.CouponPrice - reserve.PrizePrice))
                {
                    type = ReservePaymentType.GuestClearing;
                }
            }
            return reservePaymentService.Insert(user_id, reserve_id, transaction_id, ref_id, type,
                price, payment_method, operator_id, dontSave);
        }

        public void InsertReservePayment(IList<ReservePayment> reservePayments)
        {
            reservePaymentService.Insert(reservePayments);
        }

        public bool ReservePaymentExists(long transactionId, int paymentMethod, long id = 0)
        {
            return reservePaymentService.Exists(transactionId, paymentMethod, id);
        }

        public bool ReserveShouldRefund(long reserveId, Reserve.ReserveStatus status, out bool refundDone)
        {
            return reservePaymentService.ReserveShouldRefund(reserveId, status, out refundDone);
        }

        public void UpdateReservePayment(ReservePayment editedData)
        {
            reservePaymentService.Update(editedData);
        }

        // DiscountCoupon Functions

        public DiscountCoupon FindDiscountCoupon(long id)
        {
            return discountCouponService.Find(id);
        }

        public DiscountCoupon FindDiscountCoupon(int userId, DiscountCoupon.DiscountCouponType type)
        {
            return discountCouponService.Find(userId, type);
        }

        public DiscountCoupon InsertDiscountCoupon(int userId, DiscountCoupon.DiscountCouponType type,
            int percent, int presentorUserID = 0)
        {
            return discountCouponService.Insert(userId, type, percent, presentorUserID);
        }

        public void UseDiscountCouponForReserve(long couponId, long reserveId)
        {
            var coupon = discountCouponService.Find(couponId);
            var reserve = repository.FindReserve(reserveId);
            discountCouponService.UpdateCouponUsing(couponId, reserveId, DiscountCoupon.StatusEnum.Used);
            var couponPrice = discountCouponService.CalculateCouponPrice(coupon.Percent, reserve.CouponCalculationPrice);
            mediator.Publish(new SetReserveCouponEvent(reserveId, couponId, couponPrice));
        }

        public void RefundCouponIfAny(long reserveId)
        {
            var reserve = repository.FindReserve(reserveId);
            if (reserve.CouponID < 1)
            {
                return;
            }
            discountCouponService.UpdateCouponUsing(reserve.CouponID, reserve.Id, DiscountCoupon.StatusEnum.NotUsed);
            mediator.Publish(new SetReserveCouponEvent(reserveId, 0, 0));
        }

        public long CalculateDiscountCouponPrice(int couponPercent, long couponCalculationPrice)
        {
            return discountCouponService.CalculateCouponPrice(couponPercent, couponCalculationPrice);
        }

        public DiscountCoupon GetMostValuableDiscountCouponIfAny(int userId)
        {
            return discountCouponService.GetMostValuableCouponIfAny(userId);
        }

        // CreditTransaction Functions
        public IList<CreditTransaction> GetCreditListByUserId(int userId)
        {
            return creditTransactionService.GetListByUserId(userId);
        }

        public CreditTransaction GetCanselInstantReserveCreditTransaction(int userId, int tranCause, long id)
        {
            return creditTransactionService.GetCanselInstantReserve(userId, tranCause, id);
        }

        public CreditTransaction FindCreditTransaction(long id)
        {
            return creditTransactionService.Find(id);
        }

        public long IncreaseCredit(int userId, long amount, long transactionId,
            long reserveId, CreditTransactionCause transactionCause,
            out long currentCredit, string transactionCauseString = null,
            int doerUserId = 0, ActionSourceEnum actionSource = ActionSourceEnum.Undefined)
        {
            return UpdateCreditTransaction(userId, amount, transactionId, reserveId,
                transactionCause, out currentCredit, transactionCauseString, 0, doerUserId, actionSource);
        }

        public long DecreaseCredit(int userId, long amount, long transactionId,
            long reserveId, out long currentCredit, CreditTransactionCause transactionCause,
            string transactionCouseString = null, long contactId = 0, int doerUserId = 0,
            ActionLog.ActionSourceEnum actionSource = ActionLog.ActionSourceEnum.Undefined)
        {
            return UpdateCreditTransaction(userId, -amount, transactionId, reserveId,
                transactionCause, out currentCredit, transactionCouseString, contactId, doerUserId, actionSource);
        }

        private long UpdateCreditTransaction(int userId, long amount, long transactionId,
            long reserveId, CreditTransactionCause transactionCause, out long currentCredit,
            string transactionCauseString = null, long contactId = 0, int doerUserId = 0,
            ActionSourceEnum actionSource = ActionLog.ActionSourceEnum.Undefined)
        {
            var user = repository.FindUser(userId);
            currentCredit = user.Credit + amount;
            var newCreditTransaction = new CreditTransaction()
            {
                UserID = userId,
                BankTransactionID = transactionId,
                ReserveID = reserveId < 1 ? (long?)null : reserveId,
                Date = DateTime.Now,
                Price = amount,
                RemainedPrice = currentCredit,
                TransactionCause = (int)transactionCause,
                TransactionCauseString = transactionCauseString,
                AdvertiseContactID = contactId
            };
            creditTransactionService.Insert(newCreditTransaction);
            mediator.Publish(new CreditTransactionUpdateEvent(userId, currentCredit,
                actionSource, doerUserId));

            return newCreditTransaction.Id;
        }


        // PrizeCreditTransaction Functions
        public long IncreasePrizeCredit(int userId, long amount,
            PrizeTransactionType type, long reserveId,
            string customTitle, int doerUserId,
            ActionLog.ActionSourceEnum actionSource)
        {
            var user = repository.FindUser(userId);
            var newPrizeCredit = user.PrizeCredit + amount;
            var prizeCreditId = prizeCreditTransactionService.Insert(userId, amount, newPrizeCredit, type, reserveId, customTitle);
            mediator.Publish(new PrizeCreditUpdateEvent(user.Id, actionSource, doerUserId));
            return prizeCreditId;
        }

        public long DecreasePrizeCredit(int userId, long amount, PrizeTransactionType type,
            long reserveId, string customTitle, int doerUserId, ActionLog.ActionSourceEnum actionSource)
        {
            var user = repository.FindUser(userId);
            var newPrizeCredit = user.PrizeCredit - amount;
            var prizeCreditId = prizeCreditTransactionService.Insert(userId, amount, newPrizeCredit, type, reserveId, customTitle);
            mediator.Publish(new PrizeCreditUpdateEvent(user.Id, actionSource, doerUserId));
            return prizeCreditId;
        }

        public void RefundPrizeCreditIfAny(long reserveId)
        {
            var reserve = repository.FindReserve(reserveId);
            if (reserve.PrizePrice < 1)
            {
                return;
            }
            var user = repository.FindUser(reserve.UserID);
            var newPrizeCredit = user.PrizeCredit + reserve.PrizePrice;
            prizeCreditTransactionService.Insert(reserve.UserID,
                reserve.PrizePrice, newPrizeCredit,
                PrizeTransactionType.IncreaseRefund, reserveId);
            mediator.Publish(new SetReservePrizeCreditEvent(reserveId, 0, 0));
            mediator.Publish(new PrizeCreditUpdateEvent(user.Id, ActionLog.ActionSourceEnum.Undefined, 0));
        }

        public void GivePresentorPrizeIfAny(long reserveId,
            ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = repository.FindReserve(reserveId);
            var user = repository.FindUser(reserve.UserID);
            var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
            if (user.PresentorUserID < 1 ||
                user.PresentorPrizeGiven)
            {
                return;
            }
            var prizeAmount = (long)(reserve.CouponCalculationPrice * 0.05f);
            prizeCreditTransactionService.Increase(user.PresentorUserID,
                prizeAmount, PrizeTransactionType.IncreasePresent, reserveId, null,
                doerUserId, actionSource);
            mediator.Publish(new PresentorPrizeGivenEvent(user.Id, actionSource, doerUserId));
            var contact = new UserContactDTO()
            {
                UserMainMobile = user.MainMobile,
                UserAppNotificationToken = user.AppNotificationToken,
                UserEmail = identityUser.Email,
                UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                UserNotificationToken = user.NotificationToken,
                Type = UserContactType.PrizeCharge,
                Extra1 = string.Format("{0:n0}", prizeAmount)
            };
            mediator.Enqueue(new SendMessageCommand(contact));
        }

        public void GiveAppreciateDiscountIfDeserve(long reserveId,
            ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            var reserve = repository.FindReserve(reserveId);
            var guestUser = reserve.GuestUser;
            var guestIdentityUser = userManager.FindByNameAsync(guestUser.MainMobile).Result;
            if (guestUser.RecieveAppreciateDiscount)
            {
                return;
            }
            if (guestUser.Reserves.Count(x => x.Status == Reserve.ReserveStatus.Completed) > 2)
            {
                mediator.Publish(new AppreciateDiscountGivenEvent(
                    guestUser.Id, actionSource, doerUserId));
                discountCouponService.Insert(guestUser.Id, DiscountCoupon.DiscountCouponType.Appreciate, 5);
                var contact = new UserContactDTO()
                {
                    UserMainMobile = guestUser.MainMobile,
                    UserAppNotificationToken = guestUser.AppNotificationToken,
                    UserEmail = guestIdentityUser.Email,
                    UserFcmAppNotificationToken = guestUser.FcmAppNotificationToken,
                    UserNotificationToken = guestUser.NotificationToken,
                    Type = UserContactType.CouponAppreciate,
                    Extra1 = "5%"
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
        }

        public long GetReservePrizeAvailable(long reserveTotalPrice, long userPrizeCredit)
        {
            if (userPrizeCredit < 1)
            {
                return 0;
            }
            var maxAmount = reserveTotalPrice / 2;
            return Math.Min(userPrizeCredit, maxAmount);
        }

        public void UsePrizeCreditForReserve(long reserveId, int doerUserId, ActionLog.ActionSourceEnum actionSource)
        {
            var reserve = repository.FindReserve(reserveId);
            var user = repository.FindUser(reserve.UserID);
            long amount = 0;
            if (user.PrizeCredit > 0)
            {
                var maxAmount = reserve.TotalPrice / 2;
                amount = Math.Min(user.PrizeCredit, maxAmount);
            }
            if (amount < 1)
            {
                return;
            }

            var newPrizeCredit = user.PrizeCredit + reserve.PrizePrice;
            var prizeCreditId = prizeCreditTransactionService.Insert(reserve.UserID, reserve.PrizePrice, newPrizeCredit,
                PrizeCreditTransaction.PrizeTransactionType.IncreaseRefund, reserveId);

            mediator.Publish(new SetReservePrizeCreditEvent(reserveId, amount, prizeCreditId));
            mediator.Publish(new PrizeCreditUpdateEvent(user.Id, actionSource, doerUserId));
        }

        // Cart Functions
        public IList<Cart> FilterCarts(int status = -1, int uid = -1, long refid = -1)
        {
            return cartService.Filter(status, uid, refid);
        }

        // Payment Functions
        public IList<Payment> FilterPayments(long refid, int status, int uid, DateTime fromDate, DateTime toDate)
        {
            return paymentService.Filter(refid, status, uid, fromDate, toDate);
        }

        public IList<Payment> GetPaymentRange(DateTime fromDate, DateTime toDate, int status, IList<int> userIds = null,
            bool byTotalPrice = false)
        {
            return paymentService.GetRange(fromDate, toDate, status, userIds);
        }

        public int GetPaymentTriesCount(long reserveId, out string lastTryDateStr)
        {
            return paymentService.GetPaymentTriesCount(reserveId, out lastTryDateStr);
        }

        public Payment FindPayment(long id)
        {
            return paymentService.Find((int)id);
        }

        public void InsertPayment(Payment newPayment)
        {
            paymentService.Insert(newPayment);
        }

        public void UpdatePayment(Payment editedPayment)
        {
            paymentService.Update(editedPayment);
        }

        // GroupPayment Functions
        public IList<GroupPayment> FilterGroupPayment(int status)
        {
            return groupPaymentService.Filter(status);
        }

        public GroupPayment FindGroupPayment(int id)
        {
            return groupPaymentService.Find(id);
        }

        public void InsertGroupPayment(GroupPayment newGroupPayment)
        {
            groupPaymentService.Insert(newGroupPayment);
        }

        public void UpdateGroupPaymentDownloadCount(int id, int downloadCount)
        {
            groupPaymentService.UpdateDownloadCount(id, downloadCount);
        }

        public void UpdateGroupPaymentStatus(int id, GroupPayment.PaymentStatus status)
        {
            groupPaymentService.UpdateStatus(id, status);
        }

        public IEnumerable<Reserve> GetGroupPaymentReserves(out List<Reserve> todayPayments,
            out List<Reserve> paymentsWithError, out List<Reserve> excludingPayments)
        {
            var minDate = DateTime.Now.AddDays(-30).Date;
            var reserves = repository.GetReservesThatHaveReservePayment();
            reserves = reserves.Where(x => x.Status == Reserve.ReserveStatus.Reserved ||
                x.Status == Reserve.ReserveStatus.Started ||
                x.Status == Reserve.ReserveStatus.Completed ||
                x.Status == Reserve.ReserveStatus.CashPay);
            reserves = reserves.Where(x => x.EndDate >= minDate);

            var matched_ids = new List<long>();
            foreach (var item in reserves)
            {
                if (item.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteClearingToHost))
                {
                    continue;
                }
                else
                {
                    var guest_payed = reservePaymentService.GetPaidAmount(item.ReservePayments.ToList(), Reserve.StatusStringType.Guest);
                    if (PriceUtility.CalculateHostPayablePrice(item.TotalPrice, guest_payed, item.CouponPrice, item.PrizePrice) <= 0)
                    {
                        continue;
                    }
                }
                var hostPayablePrice = PriceUtility.CalculateHostPayablePrice(
                    item.TotalPrice, reservePaymentService.GetPaidAmount(item.ReservePayments.ToList(),
                        StatusStringType.Guest), item.CouponPrice, item.PrizePrice);
                if (hostPayablePrice <= 500000 &&
                    DateTimeUtility.GetSiteClearingDate(item.StartDate, item.EndDate) <= DateTime.Now)
                {
                    if (!groupPaymentService.ExistReserveId(item.Id, GroupPayment.PaymentStatus.ReadyToPay))
                    {
                        matched_ids.Add(item.Id);
                    }
                }
            }

            reserves = reserves.Where(x => matched_ids.Contains(x.Id));
            todayPayments = new List<Reserve>();
            paymentsWithError = new List<Reserve>();
            excludingPayments = new List<Reserve>();
            foreach (var item in reserves)
            {
                if (item.ExcludeGroupPayment)
                {
                    excludingPayments.Add(item);
                    continue;
                }
                var advertise = item.Advertise;
                var hostUser = repository.FindUser(advertise.UserID);
                var bankCard = repository.FindBankCardByUserId(hostUser.Id);
                if (bankCard == null || string.IsNullOrEmpty(bankCard.BankCardNumber) ||
                    item.PaymentHasError)
                {
                    paymentsWithError.Add(item);
                    continue;
                }
                todayPayments.Add(item);
            }
            return reserves;
        }

        public GuestPayResult GuestPayReserve(int userId, long reserveId,
            int payReserveType, out long payment_id, int doerUserId,
            ActionSourceEnum actionSource, bool useCoupon, bool usePrize, long couponId)
        {
            var payType = (ReservePaymentType)payReserveType;
            switch (payType)
            {
                case ReservePaymentType.GuestDeposite:
                case ReservePaymentType.GuestClearing:
                    bool already_payed;
                    long price;
                    payment_id = (long)PayGuest(userId, reserveId, payType, out already_payed,
                        out price, ReservePaymentMethod.EPay, doerUserId, actionSource, useCoupon, usePrize, couponId);
                    if (already_payed)
                    {
                        var reserve = repository.FindReserve(reserveId);
                        if (reserve.Status == ReserveStatus.WaitForReserve)
                        {
                            mediator.Send(new SetReserveStatusCommand(reserveId,
                                ReserveStatus.Reserved, true, actionSource,
                                doerUserId));
                        }
                        else if (reserve.Status == ReserveStatus.Reserved)
                        {
                            DateTime canStartTime;
                            if (reserve.CanReserveStarted(out canStartTime))
                            {
                                mediator.Send(new SetReserveStatusCommand(reserveId,
                                ReserveStatus.Started, true, actionSource,
                                doerUserId));
                            }
                        }
                        return GuestPayResult.AlreadyPaid;
                    }
                    else
                    {
                        return GuestPayResult.ReadyToPay;
                    }
                case ReservePaymentType.SiteDepositeToHost:
                case ReservePaymentType.SiteClearingToHost:
                case ReservePaymentType.SiteRefundToGuest:
                    payment_id = 0;
                    return GuestPayResult.UnhandledPaymentType;
                default:
                    payment_id = 0;
                    return GuestPayResult.IncorrectPaymentType;
            }
        }

        public GuestPayResult GuestPayReserveWithCredit(int userId, long reserveId,
            int payReserveType, out long paymentId, int doerUserId,
            ActionSourceEnum actionSource, bool useCoupon, bool usePrize, long couponId)
        {
            var payType = (ReservePaymentType)payReserveType;
            switch (payType)
            {
                case ReservePaymentType.GuestDeposite:
                case ReservePaymentType.GuestClearing:
                    bool already_payed;
                    long price;
                    paymentId = PayGuest(userId, reserveId, payType, out already_payed,
                        out price, ReservePaymentMethod.AmlakbashiCredit, doerUserId,
                        actionSource, useCoupon, usePrize, couponId);
                    if (already_payed)
                    {
                        var reserve = repository.FindReserve(reserveId);
                        if (reserve.Status == ReserveStatus.WaitForReserve)
                        {
                            mediator.Send(new SetReserveStatusCommand(reserveId,
                                ReserveStatus.Reserved, true, actionSource,
                                doerUserId));
                        }
                        else if (reserve.Status == ReserveStatus.Reserved)
                        {
                            DateTime canStartTime;
                            if (reserve.CanReserveStarted(out canStartTime))
                            {
                                mediator.Send(new SetReserveStatusCommand(reserveId,
                                ReserveStatus.Started, true, actionSource,
                                doerUserId));
                            }
                        }
                        return GuestPayResult.AlreadyPaid;
                    }
                    else
                    {
                        if (paymentId < 1)
                        {
                            return GuestPayResult.NotEnoughCredit;
                        }
                        mediator.Send(new FinalizeReserveCommand(reserveId, paymentId, price,
                            ReservePaymentMethod.AmlakbashiCredit, actionSource, doerUserId,
                            userId, 0, 0, true));
                        return GuestPayResult.Paid;
                    }
                case ReservePaymentType.SiteDepositeToHost:
                case ReservePaymentType.SiteClearingToHost:
                case ReservePaymentType.SiteRefundToGuest:
                    paymentId = 0;
                    return GuestPayResult.UnhandledPaymentType;
                default:
                    paymentId = 0;
                    return GuestPayResult.IncorrectPaymentType;
            }
        }

        public void ScheduleSendMessageGroupPayment(UserContactDTO contactDTO, int delay)
        {
            mediator.Schedule(new ScheduleSendMessageGroupPaymentCommand(contactDTO), new TimeSpan(0, 0, delay));
        }

        // Common

        public long PayAmlakbashiPortion(long reserveId, ReservePaymentType payType,
            out bool alreadyPaid, out long price, ReservePaymentMethod paymentMethod, int userId, int doerUserId)
        {
            price = 0;
            var reserve = repository.FindReserve(reserveId);
            var payment_group_string = PaymentLocalization.GetPaymentDatabaseGroupString(payType);
            var payment_string = ReservePayment.GetPaymentDatabaseString(payType);
            var paidPrice = GetReservePaidAmount(reserveId, Reserve.StatusStringType.Guest);
            var priceToPay = payType == ReservePaymentType.GuestDeposite ?
                (reserve.DepositPrice - reserve.CouponPrice - reserve.PrizePrice) :
                (reserve.TotalPrice - reserve.CouponPrice - reserve.PrizePrice);
            alreadyPaid = paidPrice >= priceToPay;
            if (alreadyPaid)
                return 0;

            price = ((long)(reserve.TotalPrice * 0.1f)) - paidPrice - reserve.CouponPrice - reserve.PrizePrice;
            switch (paymentMethod)
            {
                case ReservePaymentMethod.EPay:
                    var objPay = new Payment();
                    objPay.ReserveID = (int)reserveId;
                    objPay.Date = DateTime.Now;
                    objPay.ProductType = payment_string;
                    objPay.TotalPrice = price * 10;
                    objPay.UserID = doerUserId;
                    return paymentService.Insert(objPay);
                case ReservePaymentMethod.AmlakbashiCredit:
                    long currentCredit;
                    return DecreaseCredit(userId, price, 0, reserveId, out currentCredit, User.CreditTransactionCause.SitePortion);
                default:
                    return 0;
            }

        }

        public bool FinalizePayment(BanksEnum bank, int pid, int userId, DateTime date,
            string tref, out string paymentResult, out string msg,
            out bool invalidInput, ActionSourceEnum actionSource, int doerUserId)
        {
            try
            {
                msg = "";
                if (!string.IsNullOrEmpty(tref) && pid > 0 && date > DateTime.Now.Date)
                {
                    if (paymentOperator.ReadPaymentResult(bank, tref, out paymentResult))
                    {
                        var objpay = paymentService.Find(pid);
                        string referenceNumber;
                        long transactionReferenceID;
                        var verified = paymentOperator.VerifyPayment(bank,
                            paymentResult, objpay.Id, objpay.TotalPrice,
                            out referenceNumber, out transactionReferenceID);
                        if (verified)
                        {
                            if (GiveProduct(pid, userId, referenceNumber,
                                transactionReferenceID, out msg, actionSource,
                                doerUserId))
                            {
                                invalidInput = false;
                                return true;
                            }
                        }
                        else
                        {
                            msg = "خطایی در هنگام پرداخت رخ داده . لطفا دوباره امتحان کنید .";
                        }
                    }
                }
                else
                {
                    msg = "خطایی در هنگام پرداخت رخ داده . لطفا دوباره امتحان کنید .";
                    invalidInput = true;
                }
                invalidInput = false;
                paymentResult = null;
                return false;
            }
            catch (Exception exc)
            {
                logger.Error("AccountingFacade.FinalizePayment", exc);
                msg = "خطایی در هنگام پرداخت رخ داده . لطفا دوباره امتحان کنید .";
                invalidInput = false;
                paymentResult = null;
                return false;
            }
        }

        public bool TestFinalizePayment(int pid, int userId, out string msg)
        {
            Random random = new Random();
            var randomRefId = random.Next(100000000, 999999999);
            var randomTransactionId = random.Next(100000000, 999999999);
            return GiveProduct(pid, userId, randomRefId.ToString(), randomTransactionId, out msg, ActionSourceEnum.Background, userId);
        }

        public Dictionary<string, object> GeneratePaymentData(BanksEnum bank, int pid, string redirectAddress)
        {
            var payment = paymentService.Find(pid);
            string sign;
            DateTime invoiceDate;
            var result = paymentOperator.GeneratePaymentData(bank, pid,
                payment.TotalPrice, redirectAddress, out sign, out invoiceDate);
            payment.Authority = sign;
            payment.BankId = (int)bank;
            payment.Date = invoiceDate;
            UpdatePayment(payment);
            return result;
        }

        public void GenerateReserveFinanceChart(int year, int month,
            out PaymentChartDTO TotalReservePriceChart,
            out PaymentChartDTO SitePortionChart,
            out PaymentChartDTO HostCreditorChart)
        {
            var objPersianCalendar = new PersianCalendar();
            var month_days = objPersianCalendar.GetDaysInMonth(year, month);
            var fromDate = DateTimeUtility.ConvertDate(string.Format("{0}/{1}/{2}", year, month, 1));
            var toDate = DateTimeUtility.ConvertDate(string.Format("{0}/{1}/{2}", year, month, month_days));

            IQueryable<Reserve> allReserves = repository.GetAllReserves();
            toDate = fromDate.AddDays(1);
            allReserves = allReserves.Where(p =>
                p.Status >= ReserveStatus.Reserved &&
                p.Status <= ReserveStatus.Completed);
            TotalReservePriceChart = new PaymentChartDTO();
            TotalReservePriceChart.AmountList = new List<long>();
            TotalReservePriceChart.CountList = new List<long>();
            SitePortionChart = new PaymentChartDTO();
            SitePortionChart.AmountList = new List<long>();
            SitePortionChart.CountList = new List<long>();
            HostCreditorChart = new PaymentChartDTO();
            HostCreditorChart.AmountList = new List<long>();
            HostCreditorChart.CountList = new List<long>();
            for (int i = 0; i < month_days; i++)
            {
                IQueryable<Reserve> filtered_reserves = allReserves.Where(p =>
                    p.CreateDate >= fromDate && p.CreateDate <= toDate);
                long hostCreditor = 0;
                long payToHostPrice = 0;
                long totalPrice = 0;
                long sitePortion = 0;
                int paymentCount = 0;
                foreach (var reserve in filtered_reserves)
                {
                    var reservePayments = reserve.ReservePayments;
                    var guest_payed_price = GetReserveGuestPaidAmount(reservePayments);
                    hostCreditor += PriceUtility.CalculateHostPayablePrice(
                        reserve.TotalPrice, guest_payed_price, reserve.CouponPrice,
                        reserve.PrizePrice);
                    var payToHostPayments = reservePayments.Where(w => w.PaymentType == 3/*(int)ReservePaymentType.SiteClearingToHost*/);
                    payToHostPrice += payToHostPayments.Any() ?
                        payToHostPayments.Sum(s => s.Price) : 0;
                    totalPrice += reserve.TotalPrice;
                    sitePortion += (long)reserve.TotalPrice / (long)10f;
                    paymentCount++;
                }
                hostCreditor = hostCreditor - payToHostPrice;
                TotalReservePriceChart.CountList.Add(paymentCount);
                SitePortionChart.CountList.Add(paymentCount);
                HostCreditorChart.CountList.Add(paymentCount);
                TotalReservePriceChart.AmountList.Add(totalPrice);
                SitePortionChart.AmountList.Add(sitePortion);
                HostCreditorChart.AmountList.Add(hostCreditor);

                fromDate = toDate;
                toDate = fromDate.AddDays(1);
            }
        }

        public PaymentChartDTO GeneratePaymentChart(int year, int month,
            bool extra_filter = false, List<int> user_list = null)
        {
            var objPersianCalendar = new PersianCalendar();
            var month_days = objPersianCalendar.GetDaysInMonth(year, month);
            var fromDate = DateTimeUtility.ConvertDate(string.Format("{0}/{1}/{2}", year, month, 1));
            var toDate = DateTimeUtility.ConvertDate(string.Format("{0}/{1}/{2}", year, month, month_days));

            List<long> CountMonthValue = new List<long>();
            List<long> AmountMonthValue = new List<long>();

            toDate = fromDate.AddDays(1);
            for (int i = 0; i < month_days; i++)
            {
                int payment_count = 0;
                long payment_amount = 0;
                if (extra_filter)
                {
                    payment_count = paymentService.Filter(1, fromDate, toDate).
                        Count(w => user_list.Contains(w.UserID));
                    payment_amount = paymentService.Filter(1, fromDate, toDate).
                        Where(w => user_list.Contains(w.UserID)).
                        Select(p => (long?)p.TotalPrice).Sum() ?? 0;
                }
                else
                {
                    payment_count = paymentService.Filter(1, fromDate, toDate).Count();
                    payment_amount = paymentService.Filter(1, fromDate, toDate).
                        Where(w => w.TotalPrice > 0).
                        Select(p => (long?)p.TotalPrice).Sum() ?? 0;
                }
                CountMonthValue.Add(payment_count);
                AmountMonthValue.Add(payment_amount);

                fromDate = toDate;
                toDate = fromDate.AddDays(1);
            }
            PaymentChartDTO chart = new PaymentChartDTO();
            chart.AmountList = AmountMonthValue;
            chart.CountList = CountMonthValue;
            return chart;
        }

        private bool GiveProduct(int pid, int user_id, string referenceNumber,
            long transactionReferenceID, out string msg,
            ActionSourceEnum actionSource, int doerUserId)
        {
            try
            {
                var objpay = paymentService.Find(pid);
                objpay.RefID = long.Parse(referenceNumber);
                objpay.Authority = transactionReferenceID.ToString();
                objpay.Status = 1;
                objpay.PayDate = DateTime.Now;
                paymentService.Update(objpay);
                var user = repository.FindUser(user_id);
                var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
                var contact = new UserContactDTO()
                {
                    UserMainMobile = user.MainMobile,
                    UserAppNotificationToken = user.AppNotificationToken,
                    UserEmail = identityUser.Email,
                    UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                    UserNotificationToken = user.NotificationToken,
                    Type = UserContactType.payment,
                    TransactionId = objpay.RefID.ToString()
                };
                mediator.Enqueue(new SendMessageCommand(contact));
                var price = objpay.TotalPrice / 10;
                msg = " پرداخت شما با موفقیت انجام شد . شماره تراکنش پرداخت شما " + objpay.RefID.ToString() + "می باشد .";
                long currentCredit;
                if (objpay.ProductType.Contains("Reserve"))
                {
                    mediator.Send(new FinalizeReserveCommand(objpay.ReserveID == null ? 0 : (long)objpay.ReserveID,
                        objpay.RefID, price, ReservePaymentMethod.EPay,
                        actionSource, doerUserId, -1, objpay.CouponID,
                        objpay.PrizePrice, true));
                    try
                    {
                        var reserve = repository.FindReserve(objpay.ReserveID == null ? 0 : (long)objpay.ReserveID);
                        var hostUser = repository.FindUser(reserve.Advertise.UserID);
                        msg = " پرداخت شما با موفقیت انجام شد . شماره تراکنش پرداخت شما " + objpay.RefID.ToString() + "می باشد . شماره تماس میزبان: " + PhoneUtility.InternationalNumberToLocal(hostUser.Mobile);
                    }
                    catch { }
                }
                else if (objpay.ProductType == CreditTransactionType.Credit_Increase.ToString())
                {
                    IncreaseCredit(objpay.UserID, price, objpay.RefID, 0, CreditTransactionCause.Charge, out currentCredit, null, doerUserId, actionSource);
                }
                else if (objpay.ProductType == CreditTransactionType.Credit_Inc_Then_Res.ToString())
                {
                    IncreaseCredit(objpay.UserID, price, objpay.RefID, 0, CreditTransactionCause.Charge, out currentCredit, null, doerUserId, actionSource);
                    var creditTransactionId = DecreaseCredit(objpay.UserID, objpay.ReservePrice, 0, objpay.ReserveID == null ? 0 : (long)objpay.ReserveID, out currentCredit, CreditTransactionCause.Reserve, null, 0, doerUserId, actionSource);
                    if (creditTransactionId > 0)
                    {
                        mediator.Send(new FinalizeReserveCommand(objpay.ReserveID == null ? 0 : (long)objpay.ReserveID,
                            creditTransactionId, objpay.ReservePrice, ReservePaymentMethod.AmlakbashiCredit,
                            actionSource, doerUserId, -1, objpay.CouponID,
                            objpay.PrizePrice, true));
                    }
                    try
                    {
                        var reserve = repository.FindReserve(objpay.ReserveID == null ? 0 : (long)objpay.ReserveID);
                        var hostUser = repository.FindUser(reserve.Advertise.UserID);
                        msg = " پرداخت شما با موفقیت انجام شد . شماره تراکنش پرداخت شما " + objpay.RefID.ToString() + "می باشد . شماره تماس میزبان: " + PhoneUtility.InternationalNumberToLocal(hostUser.Mobile);
                    }
                    catch { }
                }
                return true;
            }
            catch (Exception exc)
            {
                msg = exc.Message;
                //PostDepend.AddError(" خطا در پرداخت از طرف ما  " + pid.ToString());
                return false;
            }
        }

        private long PayGuest(int userId, long reserveId, ReservePaymentType payType,
            out bool alreadyPaid, out long price, ReservePaymentMethod paymentMethod,
            int doerUseId, ActionSourceEnum actionSource, bool useCoupon, bool usePrize, long couponId)
        {
            price = 0;
            var reserve = repository.FindReserve(reserveId);
            var user = repository.FindUser(userId);
            var payment_group_string = PaymentLocalization.GetPaymentDatabaseGroupString(payType);
            var payment_string = ReservePayment.GetPaymentDatabaseString(payType);

            var paidAmount = reservePaymentService.GetPaidAmount(reserveId, StatusStringType.Guest);
            long couponAvailable = 0, prizeAvailable = 0;
            DiscountCoupon coupon = null;
            if (couponId > 0)
            {
                coupon = discountCouponService.Find(couponId);
                couponAvailable = coupon != null && coupon.UserID == userId ? CalculateDiscountCouponPrice(coupon.Percent, reserve.CouponCalculationPrice) : 0;
            }
            else if (useCoupon)
            {
                coupon = discountCouponService.GetMostValuableCouponIfAny(reserve.UserID);
                couponAvailable = coupon != null ? discountCouponService.CalculateCouponPrice(coupon.Percent, reserve.CouponCalculationPrice) : 0;
            }
            else if (usePrize)
            {
                prizeAvailable = GetReservePrizeAvailable(reserve.TotalPrice, user.PrizeCredit);
            }

            var price_to_pay = (payType == ReservePaymentType.GuestDeposite ?
                reserve.DepositPrice :
                reserve.TotalPrice)
                - couponAvailable - prizeAvailable
                - reserve.CouponPrice - reserve.PrizePrice;

            alreadyPaid = paidAmount >= price_to_pay;

            if (alreadyPaid)
                return 0;

            price_to_pay -= paidAmount;
            price = price_to_pay;

            switch (paymentMethod)
            {
                case ReservePaymentMethod.EPay:
                    var objPay = new Payment();
                    objPay.ReserveID = (int)reserveId;
                    objPay.CouponID = coupon == null ? 0 : coupon.Id;
                    objPay.PrizePrice = prizeAvailable;
                    objPay.Date = DateTime.Now;
                    objPay.ProductType = payment_string;
                    objPay.TotalPrice = price_to_pay * 10;
                    objPay.UserID = userId;
                    return paymentService.Insert(objPay);
                case ReservePaymentMethod.AmlakbashiCredit:
                    if (user.Credit < price_to_pay)
                    {
                        return 0;
                    }
                    long currentCredit;
                    var creditTransactionId = DecreaseCredit(reserve.UserID, price_to_pay, 0, reserveId, out currentCredit, User.CreditTransactionCause.Reserve);
                    if (creditTransactionId > 0)
                    {
                        if (coupon != null)
                        {
                            UseDiscountCouponForReserve(coupon.Id, reserveId);
                        }
                        if (prizeAvailable > 0)
                        {
                            UsePrizeCreditForReserve(reserveId, doerUseId, actionSource);
                        }
                    }
                    return creditTransactionId;
                default:
                    return 0;
            }
        }
    }
}
