
using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.UserContact;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.PrizeCreditTransaction;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReservePayment;
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
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using Amlakbashi.Accounting.BankingContext;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.DTOs.WalletDTOs;
using Amlakbashi.Core.Common.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
using Amlakbashi.Core.Common.StaticData;
using System.Threading.Tasks;
using Amlakbashi.Core.DTOs;

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
        private readonly IBankingOperator bankingOperator;
        private readonly UserManager<AppUser> userManager;
        private readonly ILog logger;
        private readonly IWebHostEnvironment env;
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
            IBankingOperator bankingOperator,
            UserManager<AppUser> userManager,
            ILog logger,
            IWebHostEnvironment env)
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
            this.bankingOperator = bankingOperator;
            this.userManager = userManager;
            this.logger = logger;
            this.env = env;
        }

        // ReservePayment Functions
        public void DeleteReservePayment(long id)
        {
            reservePaymentService.Delete(id);
        }

        public IList<ReservePayment> FilterReservePayment(long reservePaymentId,
            long reserveId, long advertiseId, int userId, int operatorId,
            int paymentType, int paymentMethod, long transactionId)
        {
            return reservePaymentService.Filter(reservePaymentId, reserveId, advertiseId,
                userId, operatorId, paymentType, paymentMethod, transactionId);
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
            if (reservePayments.Any(w => w.PaymentType == (int)ReservePaymentType.SiteClearingToHost))
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
            var payablePrice = PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice, guest_payed, reserve.CouponPrice, reserve.PrizePrice);
            if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteDepositeToHost))
            {
                payablePrice -= reserve.ReservePayments.FirstOrDefault(f => f.PaymentType == (int)ReservePaymentType.SiteDepositeToHost).Price;
            }
            if (payablePrice <= 0)
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

        public void FilterCreditTransactions(CreditTransactionIndexDTO dto)
        {
            creditTransactionService.Filter(dto);
        }

        public IList<CreditTransaction> GetCreditListByUserId(int userId)
        {
            return creditTransactionService.GetListByUserId(userId);
        }

        public PagedList<CreditTransaction> GetUserWalletTransactions(int userId, int page, int pageItemCount)
        {
            return creditTransactionService.GetListByUserId(userId).ToPagedList(page, pageItemCount);
        }

        public CreditTransaction GetCanselInstantReserveCreditTransaction(int userId, int tranCause, long id)
        {
            return creditTransactionService.GetCanselInstantReserve(userId, tranCause, id);
        }

        public CreditTransaction FindCreditTransaction(long id)
        {
            return creditTransactionService.Find(id);
        }

        public long IncreaseCredit(int userId, long amount, long transactionId, long reserveId,
            out long currentCredit, CreditTransaction.WalletTransactionReason transactionCause,
            string transactionCauseString = null, int? paymentId = null,
            int doerUserId = 0, ActionSourceEnum actionSource = ActionSourceEnum.Undefined)
        {
            return InsertCreditTransaction(userId, amount, transactionId, reserveId,
                transactionCause, out currentCredit, transactionCauseString, paymentId, doerUserId, actionSource);
        }

        public long DecreaseCredit(int userId, long amount, long transactionId, long reserveId,
            out long currentCredit, CreditTransaction.WalletTransactionReason transactionCause,
            string transactionCouseString = null, int? paymentId = null,
            int doerUserId = 0, ActionSourceEnum actionSource = ActionSourceEnum.Undefined)
        {
            return InsertCreditTransaction(userId, -amount, transactionId, reserveId,
                transactionCause, out currentCredit, transactionCouseString, paymentId, doerUserId, actionSource);
        }

        private long InsertCreditTransaction(int userId, long amount, long transactionId,
            long reserveId, CreditTransaction.WalletTransactionReason transactionCause, out long currentCredit,
            string transactionCauseString = null, int? paymentId = null, int doerUserId = 0,
            ActionSourceEnum actionSource = ActionSourceEnum.Undefined)
        {
            var user = repository.FindUser(userId);
            currentCredit = user.WalletAmount + amount;
            var newCreditTransaction = new CreditTransaction()
            {
                UserID = userId,
                BankTransactionID = transactionId,
                ReserveID = reserveId < 1 ? (long?)null : reserveId,
                Date = DateTime.Now,
                Price = amount,
                RemainedPrice = currentCredit,
                TransactionCause = transactionCause,
                TransactionCauseString = transactionCauseString,
                Type = amount > 0 ? CreditTransaction.WalletTransactionType.Increase : CreditTransaction.WalletTransactionType.Decrease,
                PaymentId = paymentId < 1 ? null : paymentId
                //AdvertiseContactID = contactId
            };
            creditTransactionService.Insert(newCreditTransaction);
            mediator.Publish(new CreditTransactionUpdateEvent(userId, currentCredit, actionSource, doerUserId));
            return newCreditTransaction.Id;
        }

        public CreditTransaction EditCreditTransaction(CreditTransaction editedCreditTransaction, int operatorId,
            ActionSourceEnum actionSource = ActionSourceEnum.AdminPanel)
        {
            var creditTransaction = creditTransactionService.Find(editedCreditTransaction.Id);
            if (creditTransaction.Price != editedCreditTransaction.Price)
            {
                var diffPrice = editedCreditTransaction.Price - creditTransaction.Price;
                var user = repository.FindUser(creditTransaction.UserID);
                var newCredit = user.WalletAmount + diffPrice;
                var newCreditTransaction = new CreditTransaction()
                {
                    UserID = creditTransaction.UserID,
                    Date = DateTime.Now,
                    Price = diffPrice,
                    RemainedPrice = newCredit,
                    TransactionCause = CreditTransaction.WalletTransactionReason.Corrective,
                    Type = diffPrice > 0 ? CreditTransaction.WalletTransactionType.Increase : CreditTransaction.WalletTransactionType.Decrease,
                    ModifiedWalletTransactionId = creditTransaction.Id
                };
                creditTransactionService.Insert(newCreditTransaction);
                mediator.Publish(new CreditTransactionUpdateEvent(user.Id, newCredit, actionSource, operatorId));
            }
            return creditTransactionService.Update(editedCreditTransaction);
        }

        // PrizeCreditTransaction Functions

        public long IncreasePrizeCredit(int userId, long amount,
            PrizeTransactionType type, long reserveId,
            string customTitle, int doerUserId,
            ActionLog.ActionSourceEnum actionSource)
        {
            var user = repository.FindUser(userId);
            var newPrizeCredit = user.GiftWalletAmount + amount;
            var prizeCreditId = prizeCreditTransactionService.Insert(userId, amount, newPrizeCredit, type, reserveId, customTitle);
            mediator.Publish(new PrizeCreditUpdateEvent(user.Id, actionSource, doerUserId));
            return prizeCreditId;
        }

        public long DecreasePrizeCredit(int userId, long amount, PrizeTransactionType type,
            long reserveId, string customTitle, int doerUserId, ActionLog.ActionSourceEnum actionSource)
        {
            var user = repository.FindUser(userId);
            var newPrizeCredit = user.GiftWalletAmount - amount;
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
            var newPrizeCredit = user.GiftWalletAmount + reserve.PrizePrice;
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
            var identityUser = userManager.FindByNameAsync(user.PhoneNumber).Result;
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
                UserMainMobile = user.GetNoticesPhoneNumber(),
                UserAppNotificationToken = user.AppNotificationToken,
                UserEmail = identityUser.Email,
                EmailConfirmed = identityUser.EmailConfirmed,
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
            var guestIdentityUser = userManager.FindByNameAsync(guestUser.PhoneNumber).Result;
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
                    UserMainMobile = guestUser.GetNoticesPhoneNumber(),
                    UserAppNotificationToken = guestUser.AppNotificationToken,
                    UserEmail = guestIdentityUser.Email,
                    EmailConfirmed = guestIdentityUser.EmailConfirmed,
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
            if (user.GiftWalletAmount > 0)
            {
                var maxAmount = reserve.TotalPrice / 2;
                amount = Math.Min(user.GiftWalletAmount, maxAmount);
            }
            if (amount < 1)
            {
                return;
            }

            var newPrizeCredit = user.GiftWalletAmount + reserve.PrizePrice;
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
        public IList<Payment> FilterPayments(long refid, int status, int uid,
            long reserveId, DateTime fromDate, DateTime toDate, BankEnum bank, int type)
        {
            return paymentService.Filter(refid, status, uid, reserveId, fromDate, toDate, bank, type);
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

        public async Task<CheckPaymentDTO> CheckPaymentResult(int paymentId)
        {
            var payment = paymentService.Find(paymentId);
            if (payment.Type == Payment.PaymentType.Expenditure)
            {
                return null;
            }
            var response = await paymentOperator.GetPasargadPaymentResult(payment.Id, payment.CreateDate);
            response.PaymentId = payment.Id.ToString();
            if ((response.Result && payment.Status == Payment.PaymentStatus.NotPaid) ||
                (response.Result == false && payment.Status == Payment.PaymentStatus.Paid))
            {
                response.MustEdit = true;
            }
            if (payment.ReserveID != null)
            {
                response.ReserveId = payment.ReserveID.Value;
                response.ShowDoReserve = response.Result && (payment.Reserve.Status == ReserveStatus.WaitForReserve ||
                    payment.Reserve.Status == ReserveStatus.CanceledBySystem) ? true : false;
            }
            return response;
        }

        public async Task<bool> EditPaymentByReinquiry(int paymentId)
        {
            var payment = paymentService.Find(paymentId);
            if (payment.Type == Payment.PaymentType.Expenditure)
            {
                return false;
            }
            var result = await paymentOperator.GetPasargadPaymentResult(payment.Id, payment.CreateDate);
            if (result.Result == true && payment.Status == Payment.PaymentStatus.NotPaid)
            {
                payment.TransactionId = result.TransactionReferenceId;
                payment.ReferenceNumber = Convert.ToInt64(result.ReferenceNumber);
                payment.TraceNumber = result.TraceNumber;
                payment.PayDate = DateTime.Parse(result.TransactionDate);
                payment.Status = Payment.PaymentStatus.Paid;
                paymentService.Update(payment);
                return true;
            }
            else if (result.Result == false && payment.Status == Payment.PaymentStatus.Paid)
            {
                payment.TransactionId = null;
                payment.ReferenceNumber = 0;
                payment.TraceNumber = null;
                payment.PayDate = null;
                payment.Status = Payment.PaymentStatus.NotPaid;
                paymentService.Update(payment);
                return true;
            }
            return false;
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
                //if (item.ExcludeGroupPayment)
                //{
                //    excludingPayments.Add(item);
                //    continue;
                //}
                var bankCard = repository.FindBankCardByUserId(item.HostUserID);
                if (bankCard == null || string.IsNullOrEmpty(bankCard.BankCardNumber) /*|| item.PaymentHasError*/)
                {
                    paymentsWithError.Add(item);
                    continue;
                }
                todayPayments.Add(item);
            }
            return reserves;
        }

        public GuestPayResult GuestPayReserve(int userId, long reserveId,
            int payReserveType, out long paymentId, int doerUserId,
            ActionSourceEnum actionSource, bool useCoupon, bool usePrize, long couponId)
        {
            var payType = (ReservePaymentType)payReserveType;
            switch (payType)
            {
                case ReservePaymentType.GuestDeposite:
                case ReservePaymentType.GuestClearing:
                    var reserve = repository.FindReserve(reserveId);
                    if (reserve.Status == ReserveStatus.CanceledBySystem)
                    {
                        paymentId = 0;
                        return GuestPayResult.IncorrectPaymentType;
                    }
                    bool alreadyPayed;
                    long price;
                    paymentId = (long)PayGuest(userId, reserveId, payType, out alreadyPayed,
                        out price, ReservePaymentMethod.EPay, doerUserId, actionSource, useCoupon, usePrize, couponId);
                    if (alreadyPayed)
                    {
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
                    paymentId = 0;
                    return GuestPayResult.UnhandledPaymentType;
                default:
                    paymentId = 0;
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
                    var reserve = repository.FindReserve(reserveId);
                    if (reserve.Status == ReserveStatus.CanceledBySystem)
                    {
                        paymentId = 0;
                        return GuestPayResult.IncorrectPaymentType;
                    }
                    bool alreadyPayed;
                    long price;
                    paymentId = PayGuest(userId, reserveId, payType, out alreadyPayed,
                        out price, ReservePaymentMethod.AmlakbashiCredit, doerUserId,
                        actionSource, useCoupon, usePrize, couponId);
                    if (alreadyPayed)
                    {
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
                    objPay.CreateDate = DateTime.Now;
                    objPay.ProductType = payment_string;
                    objPay.Amount = price * 10;
                    objPay.UserID = doerUserId;
                    return paymentService.Insert(objPay);
                case ReservePaymentMethod.AmlakbashiCredit:
                    long currentCredit;
                    return DecreaseCredit(userId, price, 0, reserveId, out currentCredit, CreditTransaction.WalletTransactionReason.SitePortion);
                default:
                    return 0;
            }

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
                        Select(p => (long?)p.Amount).Sum() ?? 0;
                }
                else
                {
                    payment_count = paymentService.Filter(1, fromDate, toDate).Count();
                    payment_amount = paymentService.Filter(1, fromDate, toDate).
                        Where(w => w.Amount > 0).
                        Select(p => (long?)p.Amount).Sum() ?? 0;
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
                prizeAvailable = GetReservePrizeAvailable(reserve.TotalPrice, user.GiftWalletAmount);
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
                    objPay.CreateDate = DateTime.Now;
                    objPay.ProductType = payment_string;
                    objPay.Amount = price_to_pay * 10;
                    objPay.UserID = userId;
                    return paymentService.Insert(objPay);
                case ReservePaymentMethod.AmlakbashiCredit:
                    if (user.WalletAmount < price_to_pay)
                    {
                        return 0;
                    }
                    long currentCredit;
                    var creditTransactionId = DecreaseCredit(reserve.UserID, price_to_pay, 0, reserveId, out currentCredit, CreditTransaction.WalletTransactionReason.Reserve);
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

        // Epay

        public EpayDTO GeneratePaymentData(BankEnum bank, int paymentId, string redirectUrl = null)
        {
            var payment = paymentService.Find(paymentId);
            EpayDTO epay = new EpayDTO();
            if (payment == null || payment.Status == Payment.PaymentStatus.Paid)
            {
                epay.HasError = true;
                epay.ErrorMessage = "اطلاعات پرداخت صحیح نمی باشد";
                return epay;
            }

            switch (bank)
            {
                case BankEnum.Saman:
                    var samanRedirectUrl = GeneralData.WebsiteUrl + "/Cart/RegisterSamanEpay";
                    if (string.IsNullOrEmpty(redirectUrl) == false)
                    {
                        samanRedirectUrl += "?RedirectUrl=" + redirectUrl;
                    }
                    var request = new SamanRequestTokenDTO()
                    {
                        ResNum = payment.Id.ToString(),
                        Amount = payment.Amount,
                        RedirectUrl = samanRedirectUrl,
                        CellNumber = payment.User.PhoneNumber
                    };
                    epay = paymentOperator.GetSamanPaymentToken(request).Result;
                    break;
                case BankEnum.Pasargad:
                    var pasargadRedirectUrl = GeneralData.WebsiteUrl + "/Cart/RegisterPasargadEpay";
                    if (string.IsNullOrEmpty(redirectUrl) == false)
                    {
                        pasargadRedirectUrl += "?RedirectUrl=" + redirectUrl;
                    }
                    epay = paymentOperator.GetPasargadPaymentData(bank, paymentId,
                        payment.Amount, pasargadRedirectUrl);
                    break;
                default:
                    epay.HasError = true;
                    epay.ErrorMessage = "درگاه بانکی صحیح نمی باشد";
                    break;
            }

            payment.Bank = bank;
            payment.CreateDate = epay.Date;
            UpdatePayment(payment);

            return epay;
        }

        public bool RegisterPasargadEpay(PasargadEpayResponseDTO response, out string msg, out string referenceNumber)
        {
            if (string.IsNullOrEmpty(response.tref) == false && response.iN > 0 && response.iD > DateTime.Now.Date)
            {
                string paymentResult = string.Empty;
                var checkPaymentResult = paymentOperator.GetPasargadPaymentResult(response.tref, out paymentResult);
                if (checkPaymentResult.Result)
                {
                    referenceNumber = checkPaymentResult.ReferenceNumber;
                    var payment = paymentService.Find(response.iN);
                    var verifyPaymentResult = paymentOperator.VerifyPasargadPayment(BankEnum.Pasargad,
                        paymentResult, payment.Id, payment.Amount);
                    if (verifyPaymentResult)
                    {
                        DateTime transactionDate = DateTime.Parse(checkPaymentResult.TransactionDate);
                        return GiveProduct(payment.Id, payment.UserID, checkPaymentResult.ReferenceNumber,
                            checkPaymentResult.TransactionReferenceId, checkPaymentResult.TraceNumber, transactionDate,
                            out msg, response.ActionSource);
                    }
                }
            }
            msg = "عملیات پرداخت با خطا مواجه شد";
            referenceNumber = null;
            return false;
        }

        public bool RegisterSamanEpay(SamanEpayResponseDTO response, out string msg)
        {
            var payment = paymentService.Find(response.ResNum);
            if (payment != null && payment.Status == Payment.PaymentStatus.NotPaid
                && response.Status == SamanEpayResponseDTO.StatusEnum.OK
                && string.IsNullOrEmpty(response.RefNum) == false
                && paymentService.CheckTransactionId(response.RefNum, BankEnum.Saman) == false)
            {
                var verifyEpayResult = paymentOperator.VerifySamanEpay(response.RefNum).Result;
                double verfiyEpayAmout = double.Parse(verifyEpayResult);
                if (verfiyEpayAmout == payment.Amount)
                {
                    return GiveProduct(response.ResNum, payment.UserID, response.RRN,
                            response.RefNum, response.TraceNo, DateTime.Now,
                            out msg, response.ActionSource, response.SecurePan);
                }
            }
            msg = "عملیات پرداخت با خطا مواجه شد";
            return false;
        }

        public bool TestFinalizePayment(int pid, int userId, out string msg)
        {
            Random random = new Random();
            var randomRefId = random.Next(100000000, 999999999);
            var randomTransactionId = random.Next(100000000, 999999999).ToString();
            var randomTraceNumber = random.Next(100000000, 999999999);
            var transactionDate = DateTime.Now;
            return GiveProduct(pid, userId, randomRefId.ToString(), randomTransactionId, randomTraceNumber.ToString(), transactionDate, out msg, ActionSourceEnum.Background);
        }

        private bool GiveProduct(int paymentId, int userId, string referenceNumber,
            string transactionReferenceId, string traceNumber, DateTime transactionDate, out string msg,
            ActionSourceEnum actionSource, string creditCardNumber = null)
        {
            var payment = paymentService.Find(paymentId);
            if (payment.Status == Payment.PaymentStatus.Paid)
            {
                msg = "این تراکنش تکراری می باشد";
                return false;
            }
            payment.ReferenceNumber = long.Parse(referenceNumber);
            payment.TransactionId = transactionReferenceId;
            payment.TraceNumber = traceNumber;
            payment.Status = Payment.PaymentStatus.Paid;
            payment.PayDate = transactionDate;
            if (string.IsNullOrEmpty(creditCardNumber) == false && creditCardNumber.Length <= 16)
            {
                payment.CreditCardNumber = creditCardNumber;
            }
            var user = repository.FindUser(userId);
            var identityUser = userManager.FindByNameAsync(user.PhoneNumber).Result;
            var contact = new UserContactDTO()
            {
                UserMainMobile = user.GetNoticesPhoneNumber(),
                UserAppNotificationToken = user.AppNotificationToken,
                UserEmail = identityUser.Email,
                EmailConfirmed = identityUser.EmailConfirmed,
                UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                UserNotificationToken = user.NotificationToken,
                Type = UserContactType.payment,
                TransactionId = payment.ReferenceNumber.ToString()
            };
            mediator.Enqueue(new SendMessageCommand(contact));
            var price = payment.Amount / 10;
            msg = $"پرداخت شما با موفقیت انجام شد. شماره تراکنش پرداخت شما {payment.ReferenceNumber} می باشد.";
            long currentCredit;
            if (payment.ProductType.Contains("Reserve"))
            {
                mediator.Send(new FinalizeReserveCommand(payment.ReserveID == null ? 0 : (long)payment.ReserveID,
                    payment.ReferenceNumber, price, ReservePaymentMethod.EPay, actionSource, payment.UserID, -1, payment.CouponID,
                    payment.PrizePrice, true));
                if (payment.ReserveID != null)
                {
                    msg = $"{msg} شماره تماس میزبان: {payment.Reserve.HostUser.GetNormalizedNoticesPhoneNumber()}";
                }
            }
            else if (payment.ProductType == CreditTransaction.WalletTransactionTypeForPayment.Credit_Increase.ToString())
            {
                payment.WalletTransactionId = IncreaseCredit(payment.UserID, price, payment.ReferenceNumber, 0, out currentCredit,
                    CreditTransaction.WalletTransactionReason.Charge, null, paymentId, payment.UserID, actionSource);
            }
            else if (payment.ProductType == CreditTransaction.WalletTransactionTypeForPayment.Credit_Inc_Then_Res.ToString())
            {
                payment.WalletTransactionId = IncreaseCredit(payment.UserID, price, payment.ReferenceNumber, 0, out currentCredit,
                    CreditTransaction.WalletTransactionReason.Charge, null, paymentId, payment.UserID, actionSource);
                var creditTransactionId = DecreaseCredit(payment.UserID, payment.ReservePrice, 0, payment.ReserveID == null ? 0 : (long)payment.ReserveID,
                    out currentCredit, CreditTransaction.WalletTransactionReason.Reserve, null, null, payment.UserID, actionSource);
                if (creditTransactionId > 0)
                {
                    mediator.Send(new FinalizeReserveCommand(payment.ReserveID == null ? 0 : (long)payment.ReserveID,
                        creditTransactionId, payment.ReservePrice, ReservePaymentMethod.AmlakbashiCredit,
                        actionSource, payment.UserID, -1, payment.CouponID, payment.PrizePrice, true));
                }
                if (payment.ReserveID != null)
                {
                    msg = $"{msg} شماره تماس میزبان: {payment.Reserve.HostUser.GetNormalizedNoticesPhoneNumber()}";
                }
            }
            paymentService.Update(payment);
            return true;
        }

        // Podium Services

        public ShebaVerificationResultDTO VerifySheba(string sheba)
        {
            return bankingOperator.ShebaVerification(sheba);
        }

        public ShebaPaymentResultDTO SiteClearingHostAutoPayment(long reserveId, int operatorId)
        {
            var reserve = repository.FindReserve(reserveId);
            if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePayment.ReservePaymentType.SiteClearingToHost))
            {
                return new ShebaPaymentResultDTO()
                {
                    HasError = true,
                    ErrorMessage = "این رزرو قبلا تسویه شده است. برای ثبت، ابتدا تسویه قبلی را بیعانه کنید"
                };
            }

            var hostUser = reserve.HostUser;
            var hostBankCard = repository.FindBankCardByUserId(hostUser.Id);
            if (hostBankCard.ShabaStatus == (int)BankCard.BankCardStatusEnum.NotVerified)
            {
                return new ShebaPaymentResultDTO()
                {
                    HasError = true,
                    ErrorMessage = "شماره شبای کاربر تایید نشده است"
                };
            }

            ReservePaymentType reservePaymentType;
            if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.GuestClearing))
            {
                reservePaymentType = ReservePaymentType.SiteClearingToHost;
            }
            else if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.GuestDeposite))
            {
                reservePaymentType = ReservePaymentType.SiteDepositeToHost;
            }
            else
            {
                return new ShebaPaymentResultDTO()
                {
                    HasError = true,
                    ErrorMessage = "مهمان پرداختی برای این رزرو انجام نداده است"
                };
            }

            var guestPaidAmount = GetReservePaidAmount(reserveId, StatusStringType.Guest);
            var payablePrice = PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice, guestPaidAmount,
                reserve.CouponPrice, reserve.PrizePrice);

            long clearedDepositeAmount = 0;
            if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteDepositeToHost))
            {
                clearedDepositeAmount = reserve.ReservePayments.FirstOrDefault(f => f.PaymentType == (int)ReservePaymentType.SiteDepositeToHost).Price;
                payablePrice -= clearedDepositeAmount;
            }

            var payment = new Payment()
            {
                UserID = hostUser.Id,
                CreateDate = DateTime.Now,
                Amount = payablePrice * 10,
                Bank = BankEnum.Pasargad,
                Method = Payment.PaymentMethod.Podium,
                Type = Payment.PaymentType.Expenditure,
                Status = Payment.PaymentStatus.NotPaid,
                ReserveID = reserveId,
                TransactionId = BankUtility.GeneratePodiumTransactionId()
            };
            paymentService.Insert(payment);

            ShebaPaymentRequestDTO payDTO = new ShebaPaymentRequestDTO()
            {
                DestSheba = "IR" + hostBankCard.ShabaNumber,
                DestFirstName = hostBankCard.FName,
                DestLastName = hostBankCard.LName,
                PaymentId = payment.Id,
                Timestamp = payment.CreateDate,
                Amount = payablePrice * 10,
                CentralBankTransferDetailType = CentralBankTransferEnum.CCPA,
                TransactionId = payment.TransactionId,
                SourceComment = "تسویه رزرو",
                DestComment = "تسویه رزرو"
            };

            ShebaPaymentResultDTO result;
            if (env.IsDevelopment())
            {
                result = new ShebaPaymentResultDTO()
                {
                    HasError = false,
                    Message = "پرداخت با موفقیت انجام شد",
                    ErrorMessage = "پرداخت انجام نشد",
                    PayablePrice = payablePrice * 10,
                    TraceNumber = "123456789"
                };
            }
            else
            {
                result = bankingOperator.ShebaPayment(payDTO);
            }

            if (result.HasError == false)
            {
                var reservePayment = new ReservePayment()
                {
                    PaymentType = (int)reservePaymentType,
                    ReserveID = reserveId,
                    UserID = operatorId,
                    Price = payablePrice,
                    PaymentMethod = (int)ReservePayment.ReservePaymentMethod.Podium,
                    CreateDate = DateTime.Now,
                    OperatorID = operatorId,
                    TransactionID = long.Parse(result.TraceNumber),
                    PaymentId = payment.Id
                };
                reservePaymentService.Insert(reservePayment);

                payment.PayDate = DateTime.Now;
                payment.TraceNumber = result.TraceNumber;
                payment.ReservePaymentId = reservePayment.Id;
                payment.Status = Payment.PaymentStatus.Paid;
                paymentService.Update(payment);

                result.UserId = hostUser.Id;
                result.PayablePrice = payablePrice;
                result.AdvertiseId = reserve.AdvertiseID;
            }
            return result;
        }

        public ShebaPaymentResultDTO WalletClearingAutoPayment(int userId, int operatorId)
        {
            var user = repository.FindUser(userId);
            var hostBankCard = repository.FindBankCardByUserId(user.Id);

            if (hostBankCard.ShabaStatus == (int)BankCard.BankCardStatusEnum.NotVerified)
            {
                return new ShebaPaymentResultDTO()
                {
                    HasError = true,
                    ErrorMessage = "شماره شبای کاربر تایید نشده است"
                };
            }

            var payment = new Payment()
            {
                UserID = user.Id,
                CreateDate = DateTime.Now,
                Amount = user.WalletAmount * 10,
                Bank = BankEnum.Pasargad,
                Method = Payment.PaymentMethod.Podium,
                Type = Payment.PaymentType.Expenditure,
                Status = Payment.PaymentStatus.NotPaid,
                TransactionId = BankUtility.GeneratePodiumTransactionId()
            };
            paymentService.Insert(payment);

            ShebaPaymentRequestDTO payDTO = new ShebaPaymentRequestDTO()
            {
                DestSheba = "IR" + hostBankCard.ShabaNumber,
                DestFirstName = hostBankCard.FName,
                DestLastName = hostBankCard.LName,
                PaymentId = payment.Id,
                Timestamp = payment.CreateDate,
                Amount = user.WalletAmount * 10,
                CentralBankTransferDetailType = CentralBankTransferEnum.CCPA,
                TransactionId = payment.TransactionId,
                SourceComment = "تسویه کیف پول",
                DestComment = "تسویه کیف پول"
            };

            ShebaPaymentResultDTO result;
            if (env.IsDevelopment())
            {
                result = new ShebaPaymentResultDTO()
                {
                    HasError = false,
                    Message = "پرداخت با موفقیت انجام شد",
                    ErrorMessage = "پرداخت انجام نشد",
                    PayablePrice = user.WalletAmount,
                    TraceNumber = "123456789"
                };
            }
            else
            {
                result = bankingOperator.ShebaPayment(payDTO);
            }

            if (result.HasError == false)
            {
                result.PayablePrice = user.WalletAmount;
                long newCredit;
                var transactionCause = "تسویه کیف پول";
                var creditTransactionId = DecreaseCredit(user.Id, user.WalletAmount, long.Parse(result.TraceNumber), 0, out newCredit,
                    CreditTransaction.WalletTransactionReason.Other, transactionCause, payment.Id, operatorId, ActionSourceEnum.AdminPanel);
                mediator.Publish(new CreditTransactionUpdateEvent(user.Id, newCredit, ActionSourceEnum.AdminPanel, operatorId));
                payment.PayDate = DateTime.Now;
                payment.TraceNumber = result.TraceNumber;
                payment.WalletTransactionId = creditTransactionId;
                payment.Status = Payment.PaymentStatus.Paid;
                paymentService.Update(payment);
            }
            return result;
        }

        public CheckShebaPaymentResultDTO CheckShebaPaymentStatus(long paymentId)
        {
            var payment = paymentService.Find((int)paymentId);
            var result = bankingOperator.CheckShebaPaymentStatus(payment.TransactionId, payment.TraceNumber);
            result.PaymentStatus = payment.Status;
            result.PaymentId = payment.Id;
            return result;
        }

        public bool RegisterNotPaidPayment(long paymentId, int operatorId)
        {
            var payment = paymentService.Find((int)paymentId);
            if (payment.Type == Payment.PaymentType.Income || payment.Status == Payment.PaymentStatus.Paid)
            {
                return false;
            }
            string date = payment.CreateDate.ToString("yyyy/MM/dd");
            var result = bankingOperator.CheckShebaPaymentStatus(payment.TransactionId, payment.TraceNumber);
            if (result.HasError)
            {
                return false;
            }
            else
            {
                if (payment.ReserveID != null)
                {
                    var reserve = repository.FindReserve((long)payment.ReserveID);
                    ReservePaymentType reservePaymentType;
                    if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.GuestClearing))
                    {
                        reservePaymentType = ReservePaymentType.SiteClearingToHost;
                    }
                    else
                    {
                        reservePaymentType = ReservePaymentType.SiteDepositeToHost;
                    }
                    var reservePayment = new ReservePayment()
                    {
                        PaymentType = (int)reservePaymentType,
                        ReserveID = (long)reserve.Id,
                        UserID = operatorId,
                        Price = payment.Amount / 10,
                        PaymentMethod = (int)ReservePayment.ReservePaymentMethod.Podium,
                        CreateDate = DateTime.Now,
                        OperatorID = operatorId,
                        TransactionID = long.Parse(result.RefrenceNumber),
                        PaymentId = payment.Id
                    };
                    reservePaymentService.Insert(reservePayment);
                    payment.PayDate = DateTime.Now;
                    payment.TraceNumber = result.RefrenceNumber;
                    payment.ReservePaymentId = reservePayment.Id;
                    payment.Status = Payment.PaymentStatus.Paid;
                    paymentService.Update(payment);
                }
                else
                {
                    var user = repository.FindUser(payment.UserID);
                    long newCredit;
                    var transactionCause = "تسویه کیف پول";
                    var creditTransactionId = DecreaseCredit(user.Id, user.WalletAmount, long.Parse(result.RefrenceNumber), 0, out newCredit,
                        CreditTransaction.WalletTransactionReason.Other, transactionCause, payment.Id, operatorId, ActionSourceEnum.AdminPanel);
                    mediator.Publish(new CreditTransactionUpdateEvent(user.Id, newCredit, ActionSourceEnum.AdminPanel, operatorId));
                    payment.PayDate = DateTime.Now;
                    payment.TraceNumber = result.RefrenceNumber;
                    payment.WalletTransactionId = creditTransactionId;
                    payment.Status = Payment.PaymentStatus.Paid;
                    paymentService.Update(payment);
                }
                return true;
            }
        }
    }
}
