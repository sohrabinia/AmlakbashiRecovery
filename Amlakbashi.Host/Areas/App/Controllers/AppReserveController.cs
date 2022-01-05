using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.ReserveDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Area.App.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Extensions;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/reserve/[action]")]
    public class AppReserveController : AppBaseController
    {
        private readonly IUserAccessor userAccessor;
        private readonly IReserveAppService reserveService;
        private readonly IReserveAutoCancelAppService reserveAutoCancelService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IChatAppService chatService;
        private readonly IAccountingFacade accounting;
        private readonly ILog logger;
        public AppReserveController(IUserAccessor userAccessor,
            IReserveAppService reserveService,
            IReserveAutoCancelAppService reserveAutoCancelService,
            IAdvertiseAppService advertiseService,
            IChatAppService chatService,
            IAccountingFacade accounting,
            ILog logger)
        {
            this.userAccessor = userAccessor;
            this.reserveService = reserveService;
            this.reserveAutoCancelService = reserveAutoCancelService;
            this.advertiseService = advertiseService;
            this.chatService = chatService;
            this.accounting = accounting;
            this.logger = logger;
        }

        [Authorize]
        public ActionResult List(string reserve_id = "", int status = -1, int category = -1,
            long initialPayId = 0, Reserve.ReserveManagerSelectType selectType = Reserve.ReserveManagerSelectType.All,
            string msg = "")
        {
            try
            {
                var currentUser = userAccessor.CurrentUser;
                if (reserve_id != null)
                    reserve_id = StringUtility.PersianNumberToEnglish(reserve_id);
                if (selectType == Reserve.ReserveManagerSelectType.All)
                {
                    selectType = currentUser.UserGeneralType > 0 ?
                        Reserve.ReserveManagerSelectType.Host : Reserve.ReserveManagerSelectType.Guest;
                }
                Dictionary<Reserve.ReserveCategory, int> countDict;
                var reserves = reserveService.GetReserveDashboardItems(currentUser,
                    selectType, category, reserve_id, status, out countDict);
                if (initialPayId > 0)
                {
                    var res = reserveService.Find(initialPayId);
                    ViewBag.totalPrice = res.TotalPrice;
                    ViewBag.depositePrice = res.DepositPrice;
                }

                var model = new List<ReserveDashboardItemDTO>();
                var index = 0;
                var isHost = selectType == Reserve.ReserveManagerSelectType.Host;
                var isGuest = !isHost;
                foreach (var reserve in reserves)
                {
                    var advertise = reserve.Advertise;
                    var paidAmount = accounting.GetReservePaidAmount(reserve.Id, Reserve.StatusStringType.Guest);
                    var unreadChatCount = chatService.GetNotReadCountByReserveId(reserve.Id, currentUser.Id);
                    var rulesDict = advertiseService.GetRulesDictionary(advertise.Id);
                    var item = ReserveDashboardItemDTO.Generate(
                        reserve, index, isGuest, isHost, userAccessor.CurrentUser.Id,
                        paidAmount + reserve.CouponPrice + reserve.PrizePrice,
                        unreadChatCount, rulesDict);
                    model.Add(item);
                    index++;
                }
                long reserveIdLong = -1;
                if (!string.IsNullOrEmpty(reserve_id))
                    reserveIdLong = long.Parse(reserve_id);
                ViewBag.user_id = userAccessor.CurrentUser.Id;
                ViewBag.reserve_id = reserveIdLong <= 0 ? "" : reserve_id.ToString();
                ViewBag.status = status;
                ViewBag.category = category;
                ViewBag.initialPayId = initialPayId;
                ViewBag.selectType = selectType;
                ViewBag.countDict = countDict;
                ViewBag.msg = msg;
                var payment_reserve_id = TempData.GetObjectFromJson<long>("payment_reserve_id");
                if (payment_reserve_id > 0)
                {
                    ViewBag.paymentReserve = reserveService.Find(payment_reserve_id);
                }
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ReserveItemManager", exc);
                return StatusCode(404, "صفحه ی مورد نظر موجود نمی باشد .");
            }
        }

        [Authorize]
        public ActionResult BankPay(long reserve_id,
            int pay_reserve_type, bool useCoupon = false, bool usePrize = false, long couponId = 0)
        {
            try
            {
                long payment_id;
                var result = accounting.GuestPayReserve(userAccessor.CurrentUser.Id, reserve_id,
                    pay_reserve_type, out payment_id, userAccessor.DoerUser.Id,
                    ActionLog.ActionSourceEnum.WebsiteDashboard, useCoupon, usePrize, couponId);
                switch (result)
                {
                    case Reserve.GuestPayResult.ReadyToPay:
                        reserveAutoCancelService.UpdateScheduledTime(reserve_id);
                        return Redirect($"/cart/performpay?payment_id={payment_id}&redirectUrl=amlakbashi://app");
                    default:
                        return Redirect(Request.Headers["referer"].ToString());
                }
            }
            catch (Exception exc)
            {
                logger.Error("App.Reserve.BankPay", exc);
                TempData["msg"] = "خطایی رخ داده است، لطفا دوباره امتحان کنید";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        public ActionResult WalletPay(long reserve_id,
            int pay_reserve_type, bool useCoupon = false, bool usePrize = false, long couponId = 0)
        {
            try
            {
                long payment_id;
                var reserve = reserveService.Find(reserve_id);
                var result = accounting.GuestPayReserveWithCredit(reserve.UserID,
                    reserve_id, pay_reserve_type, out payment_id, userAccessor.DoerUser.Id,
                    ActionLog.ActionSourceEnum.WebsiteDashboard, useCoupon, usePrize, couponId);
                switch (result)
                {
                    case Reserve.GuestPayResult.NotEnoughCredit:
                        TempData["msg"] = "متاسفانه موجودی حساب شما کم است";
                        return Redirect(Request.Headers["referer"].ToString());
                    case Reserve.GuestPayResult.Paid:
                        var msg = " پرداخت شما با موفقیت انجام شد . شماره تراکنش پرداخت شما " + payment_id + "می باشد .";
                        TempData["payment_success_msg"] = msg;
                        TempData.SetObjectAsJson("payment_transaction_id", payment_id);
                        TempData.SetObjectAsJson("payment_reserve_id", reserve_id);
                        return Redirect("/app/reserve/list?category=2&selecttype=1");
                    default:
                        return Redirect(Request.Headers["referer"].ToString());
                }
            }
            catch (Exception exc)
            {
                logger.Error("App.Reserve.WalletPay", exc);
                TempData["msg"] = "خطایی رخ داده است، لطفا دوباره امتحان کنید";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        public ActionResult Voucher(long reserve_id)
        {
            var model = reserveService.GenerateVoucher(reserve_id, userAccessor.CurrentUser.Id);
            return View(model);
        }

        [Authorize]
        public ActionResult Invoice(int? page)
        {
            int user_id = userAccessor.CurrentUser.Id;
            var all_reserves = reserveService.GetListByUserId(user_id, true);
            var minDate = DateTime.Now.Date.AddMonths(-6);
            all_reserves = all_reserves.Where(x => x.EndDate >= minDate).ToList();

            var model = all_reserves.Where(w => w.ReservePayments.Any() || w.Status == Reserve.ReserveStatus.Reserved ||
                w.Status == Reserve.ReserveStatus.CashPay || w.Status == Reserve.ReserveStatus.Started ||
                w.Status == Reserve.ReserveStatus.Completed).ToList();

            ViewBag.UserID = user_id;
            var PageNumber = page ?? 1;
            var onePageOfModel = model.ToPagedList(PageNumber, 10);
            var pageCount = (int)Math.Ceiling(model.Count() / 10f);
            if (pageCount > 1 && PageNumber > pageCount)
                return Redirect("/errors/Http404");
            var firstRowNumber = 0;
            if (model.Any())
            {
                firstRowNumber = ((pageCount - PageNumber) * 10) + onePageOfModel.Count;
                if (pageCount > 1 && page < pageCount)
                {
                    var mod = model.Count() % 10;
                    if (mod > 0)
                        firstRowNumber -= (10 - mod);
                }
            }
            ViewBag.firstRowNumber = firstRowNumber;
            var dto = new List<InvoiceItemDTO>();
            DateTime _total_pay_date;
            long _total_transaction_id;
            DateTime _host_site_portion_pay_date;
            long _host_site_portion_transaction_id;

            var allReservePayments = accounting.GetAllReservePaymentsAsIQueriable();
            foreach (var item in onePageOfModel)
            {
                var _guestName = item.GuestUser.FullName;
                if (string.IsNullOrEmpty(_guestName))
                {
                    _guestName = item.UserID.ToString();
                }
                var _totalPaidPrice = accounting.GetReservePaymentPrice(
                        item.Id, ReservePayment.ReservePaymentType.GuestClearing,
                        out _total_pay_date, out _total_transaction_id, 0);
                var _depositePaidPrice = accounting.GetReservePaymentPrice(
                        item.Id, ReservePayment.ReservePaymentType.GuestDeposite,
                        out _total_pay_date, out _total_transaction_id, 0);
                var allRefundsOfReserve = allReservePayments.Where(
                    x => x.ReserveID == item.Id && x.PaymentType == (int)ReservePayment.ReservePaymentType.SiteRefundToGuest);
                var _netPaidPrice = (_totalPaidPrice +
                    _depositePaidPrice +
                    item.CouponPrice + item.PrizePrice) -
                ((long)((allRefundsOfReserve == null || allRefundsOfReserve.Any() == false) ? 0 : allRefundsOfReserve.Sum(x => x.Price)));

                long _guestPaidPrice = 0;
                var hostUserId = item.Advertise.UserID;
                var _hostPaidPrice = accounting.GetReservePaymentPrice(
                    item.Id, ReservePayment.ReservePaymentType.GuestDeposite,
                    out _host_site_portion_pay_date, out _host_site_portion_transaction_id, hostUserId);
                if (_hostPaidPrice <= 0)
                {
                    _guestPaidPrice = _netPaidPrice;
                }
                var clearingPayment = allReservePayments.FirstOrDefault(
                    x => x.ReserveID == item.Id && x.PaymentType == (int)ReservePayment.ReservePaymentType.SiteClearingToHost);
                var _isCleared = _hostPaidPrice > 0 || clearingPayment != null;
                var _clearingDateSring = "";
                long _clearingTransactionId = 0;
                if (clearingPayment != null)
                {
                    _clearingDateSring = DateTimeUtility.GregorianToPersianDate(
                        clearingPayment.CreateDate) + "_" + clearingPayment.CreateDate.ToString("HH:mm");
                    _clearingTransactionId = clearingPayment.TransactionID;
                }
                var _sitePortion = (long)(item.TotalPrice * 0.1f);
                var _hostPayablePrice = _hostPaidPrice > 0 ? 0 :
                    (_netPaidPrice - _sitePortion);
                var hasInstantReservePenalty = false;
                long instantReservePenaltyPrice = 0;
                if (item.InstantReserve && _guestPaidPrice <= 0 && _hostPaidPrice <= 0)
                {
                    var penaltyTransaction = accounting.GetCanselInstantReserveCreditTransaction(hostUserId, 100,
                        item.Id);
                    if (penaltyTransaction != null)
                    {
                        hasInstantReservePenalty = true;
                        instantReservePenaltyPrice = penaltyTransaction.Price * -1;
                    }
                    else
                    {
                        hasInstantReservePenalty = false;
                    }
                }
                dto.Add(new InvoiceItemDTO()
                {
                    id = item.Id,
                    accommodationId = item.AdvertiseID,
                    totalPrice = item.TotalPrice,
                    couponPrice = item.CouponPrice,
                    prizePrice = item.PrizePrice,
                    createDateString = DateTimeUtility.GregorianToPersianDate(
                        item.CreateDate).Remove(0, 2),
                    startDateString = DateTimeUtility.GregorianToPersianDate(
                        item.StartDate).Remove(0, 2),
                    endDateString = DateTimeUtility.GregorianToPersianDate(
                        item.EndDate).Remove(0, 2),
                    startDate = item.StartDate,
                    endDate = item.EndDate,
                    clearingDate = DateTimeUtility.GetSiteClearingDate(item.StartDate, item.EndDate),
                    totalPaidPrice = _totalPaidPrice,
                    sitePortion = _sitePortion,
                    depositePaidPrice = _depositePaidPrice,
                    netPaidPrice = _netPaidPrice,
                    stayDays = DateTimeUtility.GetDatRangeDays(item.StartDate, item.EndDate),
                    guestName = _guestName,
                    isCleared = _isCleared,
                    hostPaidPrice = _hostPaidPrice,
                    guestPaidPrice = _guestPaidPrice,
                    hostPayablePrice = _hostPayablePrice,
                    hasInstantReservePenalty = hasInstantReservePenalty,
                    instantReservePenaltyPrice = instantReservePenaltyPrice,
                    clearingDateString = _clearingDateSring,
                    clearingTransactionId = _clearingTransactionId
                });
            }
            ViewBag.dto = dto;
            return View(onePageOfModel);
        }
    }
}
