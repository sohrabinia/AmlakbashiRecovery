using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using log4net;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.DTOs.ReserveDTOs;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using Amlakbashi.Accounting;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using static Amlakbashi.Core.Entities.ReserveSupport;
using static Amlakbashi.Core.Entities.Reserve;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Amlakbashi.Host.Extensions;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;
using static Amlakbashi.Core.Entities.ReservePayment;
using System.Threading.Tasks;
using Amlakbashi.Core.DTOs.WebService.Requests.Reserves;

namespace Amlakbashi.Host.Controllers
{
    public class ReserveController : BaseController
    {
        private readonly ICommentAppService commentService;
        private readonly IReserveSupportAppService reserveSupportService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IRegionAppService regionService;
        private readonly IReportItemAppService reportItemService;
        private readonly IBankCardAppService bankCardService;
        private readonly IUserAppService userService;
        private readonly IChatAppService chatService;
        private readonly IAccountingFacade accounting;
        private readonly IReserveAppService reserveService;
        private readonly IOccupiedTableAppService occupiedTableService;
        private readonly IInstantReserveAutoCancelAppService instantReserveAutoCancelService;
        private readonly IUserContactFacade userContact;
        private readonly IReserveSupportManager reserveSupportManager;
        private readonly IReserveAutoCancelAppService reserveAutoCancelService;
        private readonly IUserAccessor userAccessor;
        private readonly IReserveAdminHubServer reserveAdminHubServer;
        private readonly ILog logger;
        public ReserveController(ICommentAppService commentService,
            IReserveSupportAppService reserveSupportService,
            IAdvertiseAppService advertiseService,
            IRegionAppService regionService,
            IReportItemAppService reportItemService,
            IBankCardAppService bankCardService,
            IUserAppService userService,
            IChatAppService chatService,
            IAccountingFacade accounting,
            IUserContactFacade userContact,
            IReserveSupportManager reserveSupportManager,
            IReserveAppService reserveService,
            IOccupiedTableAppService occupiedTableService,
            IInstantReserveAutoCancelAppService instantReserveAutoCancelService,
            IReserveAutoCancelAppService reserveAutoCancelService,
            IUserAccessor userAccessor,
            IReserveAdminHubServer reserveAdminHubServer,
            ILog logger)
        {
            this.accounting = accounting;
            this.commentService = commentService;
            this.reserveSupportService = reserveSupportService;
            this.advertiseService = advertiseService;
            this.regionService = regionService;
            this.reportItemService = reportItemService;
            this.bankCardService = bankCardService;
            this.userService = userService;
            this.chatService = chatService;
            this.reserveService = reserveService;
            this.occupiedTableService = occupiedTableService;
            this.reserveAutoCancelService = reserveAutoCancelService;
            this.instantReserveAutoCancelService = instantReserveAutoCancelService;
            this.userContact = userContact;
            this.reserveSupportManager = reserveSupportManager;
            this.logger = logger;
            this.userAccessor = userAccessor;
            this.reserveAdminHubServer = reserveAdminHubServer;
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public ActionResult Admin()
        {
            return View();
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public ActionResult NewIndex(ReserveIndexDTO dto)
        {
            try
            {
                var reserves = reserveService.Filter(dto, userAccessor.CurrentUser.Id);

                var supporterList = new List<UserFullNameDTO>();
                var supporters = userService.GetAllEmployees().Select(s => s.PhoneNumber);
                foreach (var item in supporters)
                {
                    var supporter = userService.GetByMainMobile(item);
                    if (supporter != null)
                        supporterList.Add(new UserFullNameDTO() { id = supporter.Id, fullName = supporter.FullName });
                }
                dto.SupporterList = supporterList;

                dto.ReserveList = new List<ReserveAdminItemDTO>();
                ReserveSupport tempCurrentReserveSupport;
                bool refundDone;
                var currentUserId = userAccessor.CurrentUser.Id;
                foreach (var item in reserves)
                {
                    dto.ReserveList.Add(ReserveAdminItemDTO.Generate(item,
                        reserveSupportManager.Analyze(item, out tempCurrentReserveSupport, currentUserId),
                        accounting.GetReservePaidAmount(item.ReservePayments.ToList(), StatusStringType.Guest),
                        accounting.ReserveCanClear(item.Id),
                        accounting.ReserveShouldRefund(item.Id, item.Status, out refundDone), refundDone));
                }

                return View(dto);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.NewIndex", exc);
                return View();
            }
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public IActionResult GetReserveAdminDetails(long reserveId)
        {
            try
            {
                var reserve = reserveService.Find(reserveId);
                var model = reserveService.GetReserveIndexDetailsInfo(reserve);
                ReserveSupport tempCurrentReserveSupport;
                var supportStatus = reserveSupportManager.Analyze(reserve, out tempCurrentReserveSupport);
                bool canGrantSupport;
                switch (supportStatus)
                {
                    case ReserveSupport.SupporterStatus.Free:
                    case ReserveSupport.SupporterStatus.SupportingByOthers:
                    case ReserveSupport.SupporterStatus.Done:
                    case ReserveSupport.SupporterStatus.Expired:
                        canGrantSupport = (int)reserve.Status < 5 || (int)reserve.Status > 8;
                        break;
                    default:
                        canGrantSupport = false;
                        break;
                }
                model.SupportStateColor = ReserveSupport.GetSupportStatusColor(supportStatus);
                model.SupportStateString = ReserveSupport.GetSupportStatusString(supportStatus);
                model.CanGrantSupport = canGrantSupport;
                model.canBeDoneCheckout = accounting.ReserveCanClear(reserve.Id);
                model.canBeDoneEarlyCheckout = model.canBeDoneCheckout && reserve.EarlyCheckoutStatus == EarlyCheckoutEnum.ConfirmedByGuest;
                model.mustBeDoneCheckout = model.canBeDoneCheckout &&
                    DateTimeUtility.GetSiteClearingDate(reserve.StartDate, reserve.EndDate) <= DateTime.Now;
                bool refundDone;
                model.MustRefund = accounting.ReserveShouldRefund(reserve.Id, reserve.Status, out refundDone) && !refundDone;
                var guestPaidPrice = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(), StatusStringType.Guest);
                var hostPayablePrice = PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice,
                    guestPaidPrice, reserve.CouponPrice, reserve.PrizePrice);
                model.CanBePaidByHost = hostPayablePrice < 0 && (
                    reserve.Status != ReserveStatus.WaitForResponse &&
                    reserve.Status != ReserveStatus.Rejected &&
                    reserve.Status != ReserveStatus.CanceledByGuest &&
                    reserve.Status != ReserveStatus.CanceledByHost &&
                    reserve.Status != ReserveStatus.CanceledBySystem &&
                    reserve.Status != ReserveStatus.CancelRequestByGuest &&
                    reserve.Status != ReserveStatus.CancelRequestByHost);
                return View("_ReserveAdminDetails", model);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Index", exc);
                return View("_ReserveAdminDetails");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Chat(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                ViewBag.Id = reserve_id;
                ViewBag.GuestUserID = reserve.UserID;
                ViewBag.GuestPhotoID = reserve.GuestUser.PhotoID;
                ViewBag.HostUserID = reserve.HostUserID;
                ViewBag.HostPhotoID = reserve.HostUser.PhotoID;
                var model = chatService.UpdateSupportReadStatusByReserveId(reserve_id);
                reserveAdminHubServer.ChatReadFromServer(reserve_id, model.Count());
                return View(model.OrderByDescending(x => x.Id));
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Chat", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Reserve_View)]
        [HttpGet]
        public ActionResult PopupEdit(int reserveId = -1)
        {
            try
            {
                var reserve = reserveService.Find(reserveId);
                ReserveIndexEditDTO dto = reserve;
                return PartialView("_ReserveAdminEditForm", dto);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.PopupEdit(get)", exc);
                return PartialView("_ReserveAdminEditForm");
            }
        }

        [Authorize(Policy = Policies.Reserve_Edit_Normal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PopupEdit(ReserveIndexEditDTO dto)
        {
            try
            {
                var reserve = reserveService.Find(dto.Id);
                var currentIdentityUser = userService.GetIdentityUser(userAccessor.CurrentUser.PhoneNumber);
                var userAllowEdit = reserve.Status < ReserveStatus.Reserved ||
                    userService.UserAllowPolicy(currentIdentityUser, Policies.Reserve_Edit_Reserved);
                if (userAllowEdit == false)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز ویرایش ندارید"
                    });
                }
                if (reserve.TotalPrice != dto.TotalPrice ||
                    reserve.DepositPrice != dto.DepositePrice)
                {
                    if (dto.DepositePrice < 1 || dto.TotalPrice < 1)
                    {
                        userAllowEdit = userService.UserAllowPolicy(currentIdentityUser, Policies.Reserve_Payment_Actions);
                        if (userAllowEdit == false)
                        {
                            return GenerateJsonResult(new
                            {
                                status = 0,
                                msg = "شما مجوز صفر کردن مبلغ را ندارید"
                            });
                        }
                    }
                    userAllowEdit = userService.UserAllowPolicy(currentIdentityUser, Policies.Reserve_Edit_Price);
                    if (userAllowEdit == false)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            msg = "شما مجوز ویرایش مبلغ ندارید"
                        });
                    }
                }

                string msg;
                if (reserveService.UpdateNew(dto, out msg, userAccessor.CurrentUser.Id,
                    ActionLog.ActionSourceEnum.AdminPanel) == false)
                {
                    ViewBag.errorMsg = msg;
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = msg
                    });
                }
                if (reserveService.SetHostResponse(dto.Id, dto.HostResponse, true,
                    ActionLog.ActionSourceEnum.AdminPanel, userAccessor.CurrentUser.Id, true) == false)
                {
                    reserveService.SetStatus(dto.Id, dto.Status, true,
                        ActionLog.ActionSourceEnum.AdminPanel, userAccessor.CurrentUser.Id, true);
                }
                if (reserve.HostResponse == HostResponseEnum.Accepted)
                {
                    advertiseService.DeleteExtrinsicReserves(reserve.AdvertiseID, dto.PersinaStartDate, dto.PersinaEndDate);
                }
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Edit(post)", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }

        public IActionResult GetSupportInfo(long reserveId)
        {
            try
            {
                var dto = reserveService.GetReserveIndexSupportInfo(reserveId);
                return PartialView("_ReserveIndexSupportInfo", dto);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetReserveSupportInfo", exc);
                return PartialView("_ReserveIndexSupportInfo");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ReserveDashboardFiltered(int user_id = -1, string reserve_id = "", int status = -1, int category = -1)
        {
            try
            {
                return RedirectPermanent("/reserve/reserveitemmanager?user_id=" + user_id + "&reserve_id=" + reserve_id + "&status=" + status + "&category=" + category);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ReserveDashboardFiltered", exc);
                return StatusCode(404, "صفحه ی مورد نظر موجود نمی باشد .");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ReserveItemManager(string reserve_id = "", string user_id = "", int status = -1, int category = -1,
            long initialPayId = 0, ReserveManagerSelectType selectType = ReserveManagerSelectType.All,
            string msg = "")
        {
            try
            {
                var currentUser = userAccessor.CurrentUser;
                if (reserve_id != null)
                    reserve_id = StringUtility.PersianNumberToEnglish(reserve_id);
                if (selectType == ReserveManagerSelectType.All)
                {
                    selectType = currentUser.Type > 0 ?
                        ReserveManagerSelectType.Host : ReserveManagerSelectType.Guest;
                }
                Dictionary<ReserveCategory, int> countDict;
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
                var isHost = selectType == ReserveManagerSelectType.Host;
                var isGuest = !isHost;
                if (reserves != null && category == 2)
                {
                    reserves = reserves.OrderBy(o => o.StartDate).ToList();
                }
                foreach (var reserve in reserves)
                {
                    var paidAmount = accounting.GetReservePaidAmount(reserve.Id, StatusStringType.Guest);
                    var unreadChatCount = chatService.GetNotReadCountByReserveId(reserve.Id, currentUser.Id);
                    var rulesDict = advertiseService.GetRulesDictionary(reserve.AdvertiseID);
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

        public JsonResult GetReserveAvailability(int advertise_id, string from_date, string to_date, int number_of_guests)
        {
            try
            {
                bool is_occupied, guests_out_of_range;
                List<string> occupied_dates;
                var is_available = advertiseService.IsReserveAvailable(advertise_id, from_date,
                    to_date, number_of_guests, out is_occupied, out guests_out_of_range,
                    out occupied_dates);
                var days_count = DateTimeUtility.GetPersianDateRangeDays(from_date, to_date);
                return GenerateJsonResult(new
                {
                    val = 1,
                    is_available = is_available,
                    is_occupied = is_occupied,
                    guests_out_of_range = guests_out_of_range,
                    days_count = days_count
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetReserveAvailability", exc);
                return GenerateJsonResult(new { val = 0 });
            }
        }

        public JsonResult GetReservePrice(int advertise_id, string from_date, string to_date,
            int number_of_guests)
        {
            try
            {
                long without_discount_price;
                long couponCalculationPrice;
                var price = advertiseService.GetReservePrice(
                    advertise_id, from_date, to_date,
                    number_of_guests, out without_discount_price,
                    out couponCalculationPrice);
                return GenerateJsonResult(new
                {
                    val = 1,
                    price = price,
                    without_discount_price = without_discount_price
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetReservePrice", exc);
                return GenerateJsonResult(new { val = 0 });
            }
        }

        public JsonResult CheckReserve(int advertise_id, string from_date, string to_date,
            int number_of_guests = 0)
        {
            try
            {
                string msg;
                bool isInstantReserve = false;
                var ok = advertiseService.CheckReserve(userAccessor.CurrentUser.Id, advertise_id,
                    number_of_guests, from_date, to_date, out msg, out isInstantReserve);

                return GenerateJsonResult(new
                {
                    val = ok ? 1 : 0,
                    msg = msg,
                    isInstantReserve
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CheckReserve", exc);
                return GenerateJsonResult(new { val = 0, msg = "متاسفانه درخواست رزرو با خطا مواجه شد" });
            }
        }

        public JsonResult ReserveRequest(int advertise_id, string from_date, string to_date, int number_of_guests)
        {
            try
            {
                if (User.Identity.IsAuthenticated == false)
                {
                    return GenerateJsonResult(new { val = 2 });
                }
                var identityUser = userService.GetIdentityUser(User.Identity.Name);
                if (identityUser.Status != Entities.User.UserState.Acticved)
                {
                    return GenerateJsonResult(new { val = 3 });
                }
                string msg;
                long reserveId;
                var done = advertiseService.ReserveRequest(advertise_id,
                    userAccessor.CurrentUser.Id, from_date, to_date, number_of_guests, out msg, out reserveId);
                return GenerateJsonResult(new
                {
                    val = done ? 1 : 0,
                    reserveId = reserveId,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ReserveRequest", exc);
                return GenerateJsonResult(new { val = 0, msg = "متاسفانه درخواست رزرو با خطا مواجه شد" });
            }
        }

        public JsonResult CancelReserve(long reserve_id, int cancel_reason_code, string cancel_reason_string, bool is_host)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var currentUser = userAccessor.CurrentUser;
                if (!(is_host && reserve.HostUserID == currentUser.Id) &&
                    !(!is_host && reserve.UserID == currentUser.Id))
                {
                    return GenerateJsonResult(new
                    {
                        val = 0,
                        msg = "شما مجوز این کار را ندارید",
                        isPending = true
                    });
                }
                string msg;
                bool isPending;
                reserveService.CancelReserve(userAccessor.CurrentUser,
                    reserve_id, cancel_reason_code, cancel_reason_string,
                    is_host, out msg, out isPending, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
                return GenerateJsonResult(new { val = 1, msg = msg, isPending = isPending });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CancelReserve", exc);
                return GenerateJsonResult(new
                {
                    val = 0,
                    msg = "متاسفانه درخواست لغو رزرو با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public IActionResult ShowCancelDialog(long reserveId)
        {
            var reserve = reserveService.Find(reserveId);
            var userType = reserve.UserID == userAccessor.CurrentUser.Id ?
                Entities.User.UserGeneralTypeEnum.Guest : Entities.User.UserGeneralTypeEnum.Host;
            ViewBag.reserveId = reserveId;
            return PartialView("_CancelDialog", Reserve.GetReserveCancelReasonsByUserType(userType));
        }

        [Authorize]
        public IActionResult GetCancelationInfo(ReservePostCancelRequest request)
        {
            request.userId = userAccessor.CurrentUser.Id;
            request.actionSource = ActionLog.ActionSourceEnum.WebsiteDashboard;
            var result = reserveService.GetCancelationInfo(request);
            if (result.HasError())
            {
                ViewBag.hasError = true;
                ViewBag.errorMessage = result.GetErrors().First();
            }
            return PartialView("_CancelationInfo", result.Result);
        }

        [Authorize]
        public async Task<JsonResult> Cancel(ReservePostCancelRequest request)
        {
            request.userId = userAccessor.CurrentUser.Id;
            request.actionSource = ActionLog.ActionSourceEnum.WebsiteDashboard;
            var result = await reserveService.CancelAsync(request);
            if (result.HasError())
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = result.GetErrors().First()
                });
            }
            return GenerateJsonResult(new
            {
                status = 1
            });
        }

        public JsonResult RefuseCancelReserve(long reserve_id, bool is_host)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var currentUser = userAccessor.CurrentUser;
                if ((is_host ? reserve.HostUserID != currentUser.Id :
                    reserve.UserID != currentUser.Id) ||
                    reserve.Status != (is_host ? ReserveStatus.CancelRequestByHost :
                    ReserveStatus.CancelRequestByGuest))
                {
                    return GenerateJsonResult(new { val = 0, msg = "شما مجوز این کار را ندارید" });
                }
                string msg;
                reserveService.RefuseCancelReserve(currentUser, reserve_id, is_host,
                    out msg, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
                return GenerateJsonResult(new { val = 1, msg = msg });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.RefuseCancelReserve", exc);
                return GenerateJsonResult(new
                {
                    val = 0,
                    msg = "متاسفانه درخواست انصراف از لغو رزرو با خطا مواجه شد"
                });
            }
        }

        public JsonResult CashPay(long reserve_id)
        {
            try
            {
                string msg;
                var done = reserveService.CashPay(reserve_id, out msg,
                    userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.WebsiteDashboard,
                    userAccessor.DoerUser.Id);
                return GenerateJsonResult(new { val = done ? 1 : 0, msg = msg });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CachPay", exc);
                return GenerateJsonResult(new
                {
                    val = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید"
                });
            }
        }

        public JsonResult ConfirmCashPay(long reserve_id, bool payed)
        {
            try
            {
                string msg;
                var done = reserveService.ConfirmCashPay(reserve_id, payed, out msg,
                    userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.WebsiteDashboard,
                    userAccessor.DoerUser.Id);
                return GenerateJsonResult(new
                {
                    val = done ? 1 : 0,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ConfirmCashPay", exc);
                return GenerateJsonResult(new
                {
                    val = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید"
                });
            }
        }

        [Authorize]
        public ActionResult ConfirmCashPayByNotif(int reserve_id, bool payed)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                if ((reserve.HostUserID != userAccessor.CurrentUser.Id || reserve.Status != Reserve.ReserveStatus.CashPay))
                {
                    return Redirect("/");
                }
                var msg = "";
                if (payed)
                {
                    DateTime canStartTime;
                    if (reserveService.CanReserveStarted(reserve_id, out canStartTime))
                    {
                        reserveService.SetStatus(reserve_id, Reserve.ReserveStatus.Started,
                            true, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
                    }
                    msg = "شما پراخت نقدی مهمان را تایید کردید";
                }
                else
                {
                    reserveService.SetStatus(reserve_id, Reserve.ReserveStatus.Reserved,
                        true, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
                    msg = "شما پراخت نقدی مهمان را تایید نکردید";
                }
                ViewBag.msg = msg;
                ViewBag.user_id = reserve.HostUserID;
                ViewBag.selectType = Reserve.ReserveManagerSelectType.Host;
                return View("ReserveItemManager");
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ConfirmCashPay", exc);
                ViewBag.msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید";
                ViewBag.user_id = userAccessor.CurrentUser.Id;
                ViewBag.selectType = Reserve.ReserveManagerSelectType.Host;
                return View("ReserveItemManager");
            }
        }

        public JsonResult Start(long reserve_id)
        {
            try
            {
                var currentUser = userAccessor.CurrentUser;
                string msg;
                var started = reserveService.StartStay(reserve_id,
                    currentUser.Id, out msg, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
                return GenerateJsonResult(new
                {
                    val = started ? 1 : 0,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Start", exc);
                return GenerateJsonResult(new
                {
                    val = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید"
                });
            }
        }

        [Authorize]
        public JsonResult ReserveResponse(int reserve_id, int host_response)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                if (reserve.HostUserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new { val = 0 });
                }
                reserveService.SetHostResponse(reserve_id, (Reserve.HostResponseEnum)host_response,
                    true, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
                var msg = "جواب شما ثبت شد";
                var rejectReason = "";
                switch ((Reserve.HostResponseEnum)host_response)
                {
                    case Reserve.HostResponseEnum.Accepted:
                        if (reserve.StartDate.Date == DateTime.Now.Date)
                        {
                            advertiseService.SetAsTodayEmpty(reserve.AdvertiseID);
                        }
                        msg = "شما درخواست رزرو را پذیرفتید. به محض پاسخ مهمان نتیجه از طریق پیامک به اطلاع شما خواهد رسید";
                        break;
                    case Reserve.HostResponseEnum.Rejected:
                        msg = "درخواست رزرو رد شد";
                        break;
                    case Reserve.HostResponseEnum.RejectedPrice:
                        msg = "درخواست رزرو رد شد. شما مینوانید از بخش آگهی های من قیمت هر روز را جداگانه تعریف کنید و یا قیمت آگهی خود را ویرایش کنید";
                        rejectReason = "price";
                        break;
                    case Reserve.HostResponseEnum.RejectedHomeFull:
                        msg = "درخواست رزرو رد شد. شما میتوانید از بخش آگهی های من و با کلیک بر روی دکمه تعیین روز های پر، روزهایی که پر هستند را تعیین کنید";
                        rejectReason = "home_full";
                        break;
                }
                return GenerateJsonResult(new
                {
                    val = 1,
                    msg = msg,
                    rejectReason = rejectReason
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ReserveResponse", exc);
                return GenerateJsonResult(new
                {
                    val = 0,
                    msg = "متاسفانه جواب درخواست رزرو با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public ActionResult ReserveResponseByNotif(int reserve_id, int host_response)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                if (reserve.HostUserID != userAccessor.CurrentUser.Id)
                {
                    return RedirectToAction("PublicLogin", "User",
                        new
                        {
                            returnUrl = "/reserve/reserveresponsebynotif?reserve_id=" +
                                reserve_id + "&host_response=" + host_response
                        });
                }
                reserveService.SetHostResponse(reserve_id, (Reserve.HostResponseEnum)host_response,
                    true, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
                var msg = "جواب شما ثبت شد";
                switch ((Reserve.HostResponseEnum)host_response)
                {
                    case Reserve.HostResponseEnum.Accepted:
                        msg = "شما درخواست رزرو را پذیرفتید. به محض پاسخ مهمان نتیجه از طریق پیامک به اطلاع شما خواهد رسید";
                        break;
                    case Reserve.HostResponseEnum.Rejected:
                        msg = "درخواست رزرو رد شد";
                        break;
                    case Reserve.HostResponseEnum.RejectedPrice:
                        msg = "درخواست رزرو رد شد. شما مینوانید از بخش آگهی های من قیمت هر روز را جداگانه تعریف کنید و یا قیمت آگهی خود را ویرایش کنید";
                        break;
                    case Reserve.HostResponseEnum.RejectedHomeFull:
                        msg = "درخواست رزرو رد شد. شما میتوانید از بخش آگهی های من و با کلیک بر روی دکمه تعیین روز های پر، روزهایی که پر هستند را تعیین کنید";
                        break;
                }

                return Redirect("/reserve/reserveitemmanager?reserve_id=" + reserve_id +
                    "&user_id=" + reserve.HostUserID +
                    "&selecttype=" + (int)Reserve.ReserveManagerSelectType.Host);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ReserveResponseByNotif", exc);
                ViewBag.host_response_status = 0;
                ViewBag.host_response_msg = "متاسفانه جواب درخواست رزرو با خطا مواجه شد";
                return Redirect("/reserve/reserveitemmanager");
            }
        }

        public void ReserveSmsResponse(string from, string to, string message)
        {
            try
            {
                var code = 0;
                var mobile = PhoneUtility.LocalNumberToInternational(from, 98);
                int.TryParse(message, out code);
                var user = userService.GetByMainMobile(mobile);
                if (user == null)
                {
                    return;
                }
                var reserve = reserveService.FirstHavingUserId(user.Id, Reserve.ReserveStatus.WaitForResponse);
                if (reserve == null)
                    return;
                switch (code)
                {
                    case 1:
                        reserveService.SetHostResponse(reserve.Id, Reserve.HostResponseEnum.Accepted,
                            true, ActionLog.ActionSourceEnum.WebsiteDashboard, user.Id);
                        break;
                    case 2:
                        reserveService.SetHostResponse(reserve.Id, Reserve.HostResponseEnum.Rejected,
                            true, ActionLog.ActionSourceEnum.WebsiteDashboard, user.Id);
                        break;
                    case 3:
                        reserveService.SetHostResponse(reserve.Id, Reserve.HostResponseEnum.RejectedHomeFull,
                            true, ActionLog.ActionSourceEnum.WebsiteDashboard, user.Id);
                        break;
                    case 4:
                        reserveService.SetHostResponse(reserve.Id, Reserve.HostResponseEnum.RejectedPrice,
                            true, ActionLog.ActionSourceEnum.WebsiteDashboard, user.Id);
                        break;
                }
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ReserveSmsResponse", exc);
            }
        }

        public JsonResult Finish(int reserve_id)
        {
            try
            {
                string msg;
                var done = reserveService.FinishStay(reserve_id, userAccessor.CurrentUser.Id, out msg,
                    ActionLog.ActionSourceEnum.Website, userAccessor.DoerUser.Id, true);
                return GenerateJsonResult(new
                {
                    val = done ? 1 : 0,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Finish", exc);
                return GenerateJsonResult(new
                {
                    val = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید"
                });
            }
        }

        [Authorize]
        public ActionResult GuestPayReserve(long reserve_id,
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
                    case GuestPayResult.ReadyToPay:
                        reserveAutoCancelService.UpdateScheduledTime(reserve_id);
                        return RedirectToAction("performpay", "cart", new { paymentid = payment_id });
                    default:
                        return Redirect(Request.Headers["referer"].ToString());
                }
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GuestPayReserve", exc);
                TempData["msg"] = "خطایی رخ داده است، لطفا دوباره امتحان کنید .";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        public ActionResult GuestPayReserveWithCredit(long reserve_id,
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
                    case GuestPayResult.NotEnoughCredit:
                        TempData["msg"] = "متاسفانه موجودی حساب شما کم است";
                        return Redirect(Request.Headers["referer"].ToString());
                    case GuestPayResult.Paid:
                        var msg = " پرداخت شما با موفقیت انجام شد . شماره تراکنش پرداخت شما " + payment_id + "می باشد .";
                        TempData["payment_success_msg"] = msg;
                        TempData.SetObjectAsJson("payment_transaction_id", payment_id);
                        TempData.SetObjectAsJson("payment_reserve_id", reserve_id);
                        return Redirect("/reserve/reserveitemmanager?category=2&selecttype=1");
                    default:
                        return Redirect(Request.Headers["referer"].ToString());
                }
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GuestPayReserveWithCredit", exc);
                TempData["msg"] = "خطایی رخ داده است، لطفا دوباره امتحان کنید .";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public JsonResult PayReserveWithCreditHost(long reserve_id, int pay_reserve_type)
        {
            try
            {
                var pay_type = (ReservePayment.ReservePaymentType)pay_reserve_type;
                var current_reserve_status = 1;
                switch (pay_type)
                {
                    case ReservePayment.ReservePaymentType.GuestDeposite:
                    case ReservePayment.ReservePaymentType.GuestClearing:
                        bool already_payed;
                        long price;
                        var reserve = reserveService.Find(reserve_id);
                        current_reserve_status = (int)reserve.Status;
                        var payment_id = accounting.PayAmlakbashiPortion(reserve_id, pay_type, out already_payed,
                            out price, ReservePayment.ReservePaymentMethod.AmlakbashiCredit, reserve.HostUserID, userAccessor.DoerUser.Id);
                        if (already_payed)
                        {
                            if (reserve.Status == Reserve.ReserveStatus.WaitForReserve)
                            {
                                reserveService.SetStatus(reserve_id, Reserve.ReserveStatus.Reserved, false,
                                    ActionLog.ActionSourceEnum.AdminPanel, userAccessor.DoerUser.Id);
                                current_reserve_status = (int)Reserve.ReserveStatus.Reserved;
                            }
                            else if (reserve.Status == Reserve.ReserveStatus.Reserved)
                            {
                                DateTime canStartTime;
                                if (reserveService.CanReserveStarted(reserve_id, out canStartTime))
                                {
                                    reserveService.SetStatus(reserve_id, Reserve.ReserveStatus.Started, false,
                                        ActionLog.ActionSourceEnum.AdminPanel, userAccessor.DoerUser.Id);
                                    current_reserve_status = (int)Reserve.ReserveStatus.Started;
                                }
                            }
                            return GenerateJsonResult(new
                            {
                                status = 1,
                                msg = "پرداخت قبلا انجام شده است",
                                new_reserve_status = current_reserve_status
                            });
                        }
                        else
                        {
                            current_reserve_status = (int)reserveService.FinalizeReserve(reserve_id, payment_id, price, ReservePayment.ReservePaymentMethod.AmlakbashiCredit, ActionLog.ActionSourceEnum.AdminPanel, userAccessor.DoerUser.Id, reserve.HostUserID, 0, 0, false);
                            var msg = " پرداخت شما با موفقیت انجام شد . شماره تراکنش پرداخت شما " + payment_id + "می باشد .";
                            return GenerateJsonResult(new
                            {
                                status = 1,
                                msg = msg,
                                new_reserve_status = current_reserve_status
                            });
                        }
                    case ReservePayment.ReservePaymentType.SiteDepositeToHost:
                    case ReservePayment.ReservePaymentType.SiteClearingToHost:
                    case ReservePayment.ReservePaymentType.SiteRefundToGuest:
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            msg = "در حال حاضر امکان انجام این عملیات وجود ندارد"
                        });
                    default:
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            msg = "متاسفانه عملیات با خطا مواجه شد"
                        });
                }
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.PayReserveWithCreditHost", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult CallForRequest(long reserve_id)
        {
            try
            {
                bool sent = false;
                var reserve = reserveService.Find(reserve_id);
                if (reserve.Status == ReserveStatus.WaitForResponse)
                {
                    reserveService.SendReserveRequestCall(reserve_id);
                    sent = true;
                }
                return GenerateJsonResult(new
                {
                    status = sent ? 1 : 2
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CallForRequest", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult CallForPayment(long reserve_id)
        {
            try
            {
                var sent = false;
                var reserve = reserveService.Find(reserve_id);
                if (reserve.Status == ReserveStatus.WaitForReserve)
                {
                    reserveService.SendPayReserveCall(reserve_id);
                    sent = true;
                }
                return GenerateJsonResult(new
                {
                    status = sent ? 1 : 2
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CallForPayment", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult CancelBySystem(long reserve_id)
        {
            try
            {
                var done = reserveService.SystemCancelReserve(reserve_id);
                return GenerateJsonResult(new
                {
                    status = done ? 1 : 2
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CancelBySystem", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public IActionResult GetSiteClearingHostInfo(long reserveId)
        {
            try
            {
                var reserve = reserveService.Find(reserveId);
                if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePayment.ReservePaymentType.SiteClearingToHost))
                {
                    return PartialView("_SiteClearingHostInfo");
                }

                var hostUser = reserve.HostUser;

                var guestPaidAmount = accounting.GetReservePaidAmount(reserveId, StatusStringType.Guest);
                var payablePrice = PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice, guestPaidAmount,
                    reserve.CouponPrice, reserve.PrizePrice);

                long clearedDepositeAmount = 0;
                if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteDepositeToHost))
                {
                    clearedDepositeAmount = reserve.ReservePayments.FirstOrDefault(f => f.PaymentType == (int)ReservePaymentType.SiteDepositeToHost).Price;
                    payablePrice -= clearedDepositeAmount;
                }

                var hostName = string.IsNullOrEmpty(hostUser.FullName) ?
                    hostUser.GetPhoneNumber(Entities.User.PhoneType.MainMobile) : hostUser.FullName;

                var hostBankCard = bankCardService.GetByUserId(hostUser.Id);
                var bankCardName = hostBankCard != null ?
                    ((hostBankCard.FName != null ? hostBankCard.FName + " " : "") +
                    (hostBankCard.LName != null ? hostBankCard.LName : "")) : "";

                SitePaymentDTO model = new SitePaymentDTO()
                {
                    ReserveId = reserveId,
                    Days = (int)(reserve.EndDate - reserve.StartDate).TotalDays,
                    TotalPrice = reserve.TotalPrice,
                    GuestPayedPrice = guestPaidAmount,
                    SitePortion = reserve.TotalPrice / 10,
                    ClearingDepositeAmount = clearedDepositeAmount,
                    PayablePrice = payablePrice,
                    PayablePriceRaw = payablePrice * 10,
                    BankCardNumber = hostBankCard != null &&
                            !string.IsNullOrEmpty(hostBankCard.BankCardNumber) ?
                            hostBankCard.BankCardNumber : "",
                    BankCardName = bankCardName,
                    BankCardVerified = hostBankCard != null &&
                            hostBankCard.BankCardStatus == (int)BankCard.BankCardStatusEnum.Verified,
                    BankCardId = hostBankCard != null ? hostBankCard.Id : 0,
                    ShebaVerified = hostBankCard != null &&
                            hostBankCard.ShabaStatus == (int)BankCard.BankCardStatusEnum.Verified,
                    ShebaNumber = hostBankCard != null &&
                            !string.IsNullOrEmpty(hostBankCard.ShabaNumber) ?
                            hostBankCard.ShabaNumber : "",
                    UserName = hostName,
                    UserId = hostUser.Id,
                    UserCredit = hostUser.WalletAmount,
                    HasFailureExpenditurePayment = reserve.HasFailureExpenditurePayment()
                };
                return PartialView("_SiteClearingHostInfo", model);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetSiteClearingHostInfo", exc);
                return PartialView("_SiteClearingHostInfo");
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public IActionResult GetSiteRefundGuestInfo(long reserveId)
        {
            try
            {
                var reserve = reserveService.Find(reserveId);
                var guest = reserve.GuestUser;
                var guestCard = bankCardService.GetByUserId(guest.Id);
                var guestPayedPrice = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                    Reserve.StatusStringType.Guest);

                var bankCardName = guestCard != null ?
                    ((guestCard.FName != null ? guestCard.FName + " " : "") +
                    (guestCard.LName != null ? guestCard.LName : "")) : "";

                var guestName = string.IsNullOrEmpty(guest.FullName) ?
                    guest.GetPhoneNumber(Entities.User.PhoneType.MainMobile) : guest.FullName;

                SitePaymentDTO model = new SitePaymentDTO()
                {
                    ReserveId = reserveId,
                    ReserveStatus = reserve.Status,
                    TotalPrice = reserve.TotalPrice,
                    GuestPayedPrice = guestPayedPrice,
                    PayablePriceRaw = guestPayedPrice * 10,
                    PayablePrice = guestPayedPrice,
                    BankCardNumber = guestCard != null &&
                            !string.IsNullOrEmpty(guestCard.BankCardNumber) ?
                            guestCard.BankCardNumber : "ثبت نشده",
                    BankCardName = !string.IsNullOrEmpty(bankCardName) ?
                            bankCardName : "بدون نام",
                    BankCardVerified = guestCard != null &&
                            guestCard.BankCardStatus == (int)BankCard.BankCardStatusEnum.Verified,
                    UserId = guest.Id,
                    UserName = guestName
                };
                return PartialView("_SiteRefundGuestInfo", model);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetSiteRefundGuestInfo", exc);
                return PartialView("_SiteRefundGuestInfo");
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public IActionResult ManualSitePayment(long reserveId, long price)
        {
            try
            {
                ViewBag.hasFailureExpenditurePayment = reserveService.Find(reserveId).HasFailureExpenditurePayment();
                ViewBag.reserveId = reserveId;
                ViewBag.userId = userAccessor.CurrentUser.Id;
                ViewBag.price = price;
                return PartialView("_ManualSitePayment");
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ManualSitePayment(Get)", exc);
                return PartialView("_ManualSitePayment");
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        [HttpPost]
        public IActionResult ManualSitePayment(long reserveId, int userId, long transactionId,
            long referenceId, int paymentMethod, long price, bool sendSms)
        {
            try
            {
                if (price < 1)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "مبلغ پرداختی کمتر از حد مجاز است"
                    });
                }

                var reserve = reserveService.Find(reserveId);
                if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePayment.ReservePaymentType.SiteClearingToHost))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این رزرو قبلا تسویه شده است. برای ثبت، ابتدا تسویه قبلی را بیعانه کنید"
                    });
                }

                if (transactionId <= 0)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "لطفا شماره تراکنش را وارد کنید"
                    });
                }

                var bankCard = bankCardService.GetByUserId(reserve.HostUserID);
                if (bankCard.ShabaStatus == (int)BankCard.BankCardStatusEnum.NotVerified)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره شبای کاربر تایید نشده است"
                    });
                }
                var user = userService.Find(userId);
                if (user == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شناسه کاربری وارد شده اشتباه است"
                    });
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
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "مهمان پرداختی برای این رزرو انجام نداده است"
                    });
                }

                var reservePayment = accounting.InsertReservePayment(userId, reserveId, transactionId, referenceId,
                    reservePaymentType, price, (ReservePayment.ReservePaymentMethod)paymentMethod, userAccessor.CurrentUser.Id);

                if (reservePayment == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره تراکنش تکراری است"
                    });
                }

                if (sendSms)
                {
                    var identityUser = userService.GetIdentityUser(reserve.HostUser.PhoneNumber);
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserMainMobile = reserve.HostUser.GetNoticesPhoneNumber(),
                        UserAppNotificationToken = reserve.HostUser.AppNotificationToken,
                        UserEmail = identityUser.Email,
                        UserFcmAppNotificationToken = reserve.HostUser.FcmAppNotificationToken,
                        UserNotificationToken = reserve.HostUser.NotificationToken,
                        Type = UserContactType.SiteClearingHost,
                        Price = price.ToString(),
                        ReserveId = reserve.Id.ToString(),
                        TransactionId = transactionId.ToString(),
                        AdvertiseId = reserve.AdvertiseID.ToString()
                    });
                }

                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "پرداخت به کاربر با موفقیت ثبت شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ManualSitePayment(Post)", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        [HttpGet]
        public IActionResult AutoSiteClearingHost(long reserveId)
        {
            try
            {
                var reserve = reserveService.Find(reserveId);
                var guestPaidAmount = accounting.GetReservePaidAmount(reserveId, StatusStringType.Guest);
                var payablePrice = PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice, guestPaidAmount,
                    reserve.CouponPrice, reserve.PrizePrice);

                long clearedDepositeAmount = 0;
                if (reserve.ReservePayments.Any(a => a.PaymentType == (int)ReservePaymentType.SiteDepositeToHost))
                {
                    clearedDepositeAmount = reserve.ReservePayments.FirstOrDefault(f => f.PaymentType == (int)ReservePaymentType.SiteDepositeToHost).Price;
                    payablePrice -= clearedDepositeAmount;
                }

                var hostBankCard = bankCardService.GetByUserId(reserve.HostUserID);
                var bankCardName = hostBankCard != null ?
                    ((hostBankCard.FName != null ? hostBankCard.FName + " " : "") +
                    (hostBankCard.LName != null ? hostBankCard.LName : "")) : "";

                if (hostBankCard == null || hostBankCard.ShabaStatus == (int)BankCard.BankCardStatusEnum.NotVerified)
                {
                    ViewBag.error = "شماره شبای کاربر تایید نشده است";
                    return PartialView("_AutoSiteClearingHost");
                }

                if (payablePrice < 1)
                {
                    ViewBag.error = "مبلغ پرداختی کمتر از حد مجاز است";
                    return PartialView("_AutoSiteClearingHost");
                }

                ViewBag.reserveId = reserveId;
                ViewBag.shebaNumber = hostBankCard.ShabaNumber;
                ViewBag.bankCardName = bankCardName;
                ViewBag.price = payablePrice * 10;
                return PartialView("_AutoSiteClearingHost");
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.AutoSiteClearingHost", exc);
                ViewBag.error = "عملیات با خطای فنی مواجه شد";
                return PartialView("_AutoSiteClearingHost");
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        [HttpPost]
        public IActionResult AutoSiteClearingHost(long reserveId, bool sendSms)
        {
            try
            {
                var result = accounting.SiteClearingHostAutoPayment(reserveId, userAccessor.CurrentUser.Id);
                if (result.HasError == false && sendSms)
                {
                    var user = userService.Find(result.UserId);
                    var identityUser = userService.GetIdentityUser(user.PhoneNumber);
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserMainMobile = user.GetNoticesPhoneNumber(),
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserEmail = identityUser.Email,
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        Type = UserContactType.SiteClearingHost,
                        Price = result.PayablePrice.ToString(),
                        ReserveId = reserveId.ToString(),
                        TransactionId = result.TraceNumber,
                        AdvertiseId = result.AdvertiseId.ToString()
                    });
                }
                return GenerateJsonResult(new
                {
                    status = result.HasError ? 0 : 1,
                    msg = result.HasError ? result.ErrorMessage : result.Message,
                    traceNumber = result.TraceNumber,
                    recieverFullName = result.RecieverFullName
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.AutoSiteClearingHost", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        [HttpPost]
        public IActionResult CanceledReserveClearing(long reserveId, long guestClearingAmount,
            long hostClearingAmount, long siteClearingAmount)
        {
            try
            {
                var reserve = reserveService.Find(reserveId);
                var guestPayedPrice = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                    Reserve.StatusStringType.Guest);

                if (reserve.Status != ReserveStatus.CanceledByGuest && reserve.Status != ReserveStatus.CanceledByHost)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "عملیات امکان پذیر نمی باشد"
                    });
                }

                if (reserve.Status == ReserveStatus.CanceledByGuest &&
                    guestPayedPrice != (guestClearingAmount + hostClearingAmount + siteClearingAmount))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "مبالغ وارد شده صحیح نمی باشد"
                    });
                }

                long newCredit;
                var guestRefundCreditTransactionId = accounting.IncreaseCredit(reserve.UserID, guestPayedPrice, 0,
                    reserveId, out newCredit, CreditTransaction.WalletTransactionReason.Refund);

                accounting.InsertReservePayment(userAccessor.CurrentUser.Id, reserveId, guestRefundCreditTransactionId, 0,
                    ReservePaymentType.SiteRefundToGuest, guestPayedPrice,
                    ReservePaymentMethod.AmlakbashiCredit, userAccessor.CurrentUser.Id);

                var transactionCause = $"خسارت کنسلی رزرو {reserveId}";
                if (reserve.Status == ReserveStatus.CanceledByGuest)
                {
                    var guestDecreaseAmount = guestPayedPrice - guestClearingAmount;
                    if (guestDecreaseAmount > 0)
                    {
                        accounting.DecreaseCredit(reserve.UserID, guestDecreaseAmount, 0, reserveId,
                            out newCredit, CreditTransaction.WalletTransactionReason.Other, transactionCause);
                    }
                    if (hostClearingAmount > 0)
                    {
                        accounting.IncreaseCredit(reserve.HostUserID, hostClearingAmount, 0, reserveId,
                            out newCredit, CreditTransaction.WalletTransactionReason.Other, transactionCause);
                    }
                }
                else
                {
                    if (siteClearingAmount > 0)
                    {
                        accounting.DecreaseCredit(reserve.HostUserID, siteClearingAmount, 0, reserveId,
                            out newCredit, CreditTransaction.WalletTransactionReason.Other, transactionCause);
                    }
                }

                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "تسویه رزرو کنسل شده با موفقیت انجام شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CanceledReserveClearing", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public JsonResult SiteClearingWithCredit(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var guestPaidAmount = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                    StatusStringType.Guest);
                var payable_price = PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice, guestPaidAmount,
                    reserve.CouponPrice, reserve.PrizePrice);
                var days = (int)(reserve.EndDate - reserve.StartDate).TotalDays;
                long newCredit;
                var transaction_id = accounting.IncreaseCredit(reserve.HostUserID, payable_price, 0, reserve_id,
                    out newCredit, CreditTransaction.WalletTransactionReason.Clearing);
                if (accounting.InsertReservePayment(userAccessor.CurrentUser.Id,
                    reserve_id, transaction_id, 0,
                    ReservePayment.ReservePaymentType.SiteClearingToHost,
                    payable_price, ReservePayment.ReservePaymentMethod.AmlakbashiCredit,
                    userAccessor.CurrentUser.Id) == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره تراکنش تکراری است"
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "تسویه شد",
                    payable_price = payable_price,
                    transaction_id = transaction_id
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.SiteClearingWithCredit", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public JsonResult SendSiteClearingWithCreditSms(long reserve_id, long payable_price, long transaction_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var identityUser = userService.GetIdentityUser(reserve.HostUser.PhoneNumber);
                userService.SendMessage(new UserContactDTO()
                {
                    UserMainMobile = reserve.HostUser.GetNoticesPhoneNumber(),
                    UserAppNotificationToken = reserve.HostUser.AppNotificationToken,
                    UserEmail = identityUser.Email,
                    UserFcmAppNotificationToken = reserve.HostUser.FcmAppNotificationToken,
                    UserNotificationToken = reserve.HostUser.NotificationToken,
                    Type = UserContactType.SiteClearingHostWithCredit,
                    TransactionId = transaction_id.ToString(),
                    Price = payable_price.ToString(),
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    ReserveId = reserve_id.ToString()
                });
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("SendSiteClearingWithCreditSms", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public JsonResult SiteRefundGuestWithCredit(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var guest_payed_price = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                    Reserve.StatusStringType.Guest);
                long newCredit;
                var transaction_id = accounting.IncreaseCredit(reserve.UserID, guest_payed_price, 0, reserve_id,
                    out newCredit, CreditTransaction.WalletTransactionReason.Refund);
                if (accounting.InsertReservePayment(userAccessor.CurrentUser.Id,
                    reserve_id, transaction_id, 0,
                    ReservePayment.ReservePaymentType.SiteRefundToGuest,
                    guest_payed_price, ReservePayment.ReservePaymentMethod.AmlakbashiCredit,
                    userAccessor.CurrentUser.Id) == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره تراکنش تکراری است"
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "عودت داده شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.SiteRefundGuestWithCredit", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ReserveManager(int user_id = -1, string msg = "",
            ReserveManagerSelectType selectType = ReserveManagerSelectType.All)
        {
            var queryString = HtmlUtility.AddToQueryString("", "user_id", userAccessor.CurrentUser.Id.ToString());
            if (!string.IsNullOrEmpty(msg))
            {
                queryString = HtmlUtility.AddToQueryString(queryString, "msg", msg);
            }
            queryString = HtmlUtility.AddToQueryString(queryString, "selecttype", ((int)selectType).ToString());
            queryString = HtmlUtility.AddToQueryString(queryString, "category", "0");
            var url = "/reserve/reserveitemmanager" + queryString;
            return Redirect(url);
        }

        [HttpGet]
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
                var hostUserId = item.HostUserID;
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

        [Authorize]
        [HttpGet]
        public ActionResult GenerateGuestReceipt(long reserveId)
        {
            var model = reserveService.GenerateVoucher(reserveId, userAccessor.CurrentUser.Id);
            if (model == null)
            {
                return RedirectToAction("AccessDenied", "Errors");
            }
            return View("Voucher", model);
        }

        public JsonResult GetReserveInfo(long accommodation_id)
        {
            var startDate = DateTime.Now.TimeOfDay.Hours > 3 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(-1);
            var acc = advertiseService.Find(accommodation_id);

            var occDatesFrom = acc.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s));
            //var to_occupied_dates = SerializeUtility.SerializeToJS(
            //    ReserveDepend.GetAdvertiseUnavailableDates(accommodation_id,
            //    ReserveDepend.OccupiedSelectType.ForTo, ReserveDepend.OccupiedSource.All, advertise));
            var advertise_rules = advertiseService.GetRulesDictionary(acc.Id);
            var rules_string = "";
            foreach (var item in advertise_rules)
            {
                rules_string += "<br/>" + item.Key + ": " + item.Value;
            }
            var priceDict = advertiseService.GetAccPriceDatesInfo(accommodation_id);
            return GenerateJsonResult(new
            {
                occupiedList = occDatesFrom,
                rules_string = rules_string,
                priceDict = priceDict
            });
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult AddSupporterInfoToReserve(long reserve_id, string text)
        {
            try
            {
                reserveService.UpdateSupporterInfo(reserve_id, text, userAccessor.CurrentUser);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.AddSupporterInfoToReserve", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult ToggleShouldFollow(long reserve_id, string text)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                if (!reserve.shouldFollow && string.IsNullOrEmpty(text))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "لطفا متن دلیل پیگیری را وارد کنید"
                    });
                }
                if (reserve.shouldFollow)
                {
                    var currentUserIdentity = userService.GetIdentityUser(userAccessor.CurrentUser.PhoneNumber);
                    var editAllowed = userService.UserAllowPolicy(currentUserIdentity, Policies.Reserve_Edit_Reserved);
                    if (editAllowed == false)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            msg = "شما دسترسی حذف از پیگیری را ندارید"
                        });
                    }
                }
                reserveService.UpdateShouldFollow(reserve_id, text, userAccessor.CurrentUser);
                return GenerateJsonResult(new
                {
                    status = 1,
                    new_status = reserve.shouldFollow
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public JsonResult GetShouldFollowState(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    shouldFollow = reserve.shouldFollow
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetShouldFollowState", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult NextCallState(long reserve_id, string hostOrGuest)
        {
            try
            {
                var new_state = reserveService.UpdateCallState(reserve_id, hostOrGuest);
                var new_state_color = ReserveStyleHelper.GetCallStateColor(new_state);
                return GenerateJsonResult(new
                {
                    status = 1,
                    new_state = new_state,
                    new_state_color = new_state_color
                });
            }
            catch (Exception exc)
            {
                logger.Error("Rreserve.NextCallState", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public ActionResult GetReserveSupporterInfo(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                return PartialView("_ReserveSupporterInfo", reserve);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetReserveSupporterInfo", exc);
                return Content("");
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult ToggleDisableAutoCancel(long id, bool active)
        {
            try
            {
                reserveService.UpdateDisableAutoCancel(id, active);
                if (active == false)
                {
                    reserveAutoCancelService.UpdateScheduledTime(id);
                }
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ToggleDisableAutoCancel", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult ToggleAccVisited(long id, bool active)
        {
            try
            {
                reserveService.UpdateAccVisitedByGuest(id, active);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ToggleAccVisited", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetReserveItemPartial(long reserve_id, int index,
            bool is_guest, bool is_host)
        {
            try
            {
                int user_id = userAccessor.CurrentUser.Id;
                var reserve = reserveService.Find(reserve_id);
                if (reserve.UserID != user_id &&
                    reserve.HostUserID != user_id)
                {
                    return RedirectToAction("AccessDenied", "Errors");
                }
                var paidAmount = accounting.GetReservePaidAmount(reserve.Id, StatusStringType.Guest);
                var unreadChatCount = chatService.GetNotReadCountByReserveId(reserve.Id, user_id);
                var rulesDict = advertiseService.GetRulesDictionary(reserve.AdvertiseID);
                var model = ReserveDashboardItemDTO.Generate(
                    reserve, index, is_guest, is_host, user_id,
                    paidAmount + reserve.CouponPrice + reserve.PrizePrice,
                    unreadChatCount, rulesDict);
                return PartialView("_ReserveItem", model);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.GetReserveItemPartial", exc);
                return BadRequest();
            }
        }

        [Authorize]
        public JsonResult SetAsPaymentRegistered(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                if (reserve.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new { status = 0 });
                }
                reserveService.UpdatePaymentGTAGRegistered(reserve_id, true);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.SetAsPaymentRegistered", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult DoSupport(long reserve_id,
            bool force = false, string transfer_reason = null)
        {
            try
            {
                var reserve = reserveService.GetReserveIncludingSupport(reserve_id);
                if (!force)
                {
                    ReserveSupport currentReserveSupport;
                    var analyzeResult = reserveSupportManager.Analyze(reserve,
                        out currentReserveSupport, userAccessor.CurrentUser.Id);
                    switch (analyzeResult)
                    {
                        case SupporterStatus.SupportingByOthers:
                        case SupporterStatus.SupportingByYou:
                        case SupporterStatus.Done:
                        case SupporterStatus.Expired:
                            if (currentReserveSupport.SupporterID == userAccessor.CurrentUser.Id &&
                                analyzeResult != SupporterStatus.Expired)
                            {
                                return GenerateJsonResult(new
                                {
                                    status = 0,
                                    msg = "شما هم اکنون در حال پیگیری این رزرو هستید."
                                });
                            }
                            if (analyzeResult == SupporterStatus.Done)
                            {
                                if ((int)reserve.Status > 4 && (int)reserve.Status < 9)
                                {
                                    return GenerateJsonResult(new
                                    {
                                        status = 0,
                                        msg = "این رزرو انجام شده است و نیازی به پشتیبانی ندارد"
                                    });
                                }
                            }
                            var current_supporter_name =
                                userService.Find((int)currentReserveSupport.SupporterID).FullName;
                            if (string.IsNullOrEmpty(current_supporter_name))
                            {
                                current_supporter_name = currentReserveSupport.SupporterID.ToString();
                            }
                            if (!(analyzeResult == SupporterStatus.Expired &&
                                currentReserveSupport.SupporterID == userAccessor.CurrentUser.Id))
                            {
                                return GenerateJsonResult(new
                                {
                                    status = 2,
                                    msg = current_supporter_name + " در حال پشتیبانی رزرو های این مهمان است. فقط در صورت عدم حضور این پشتیبان و یا این که خود شخص از شما خواسته روی دکمه بله کلیک کنید"
                                });
                            }
                            break;
                    }
                }
                reserveSupportManager.AddSupporterToReserve(reserve_id, userAccessor.CurrentUser.Id,
                    transfer_reason);
                var supporterName = userAccessor.CurrentUser.FullName;
                if (string.IsNullOrEmpty(supporterName))
                {
                    supporterName = userAccessor.CurrentUser.Id.ToString();
                }
                var supporterPhoto = "";

                if (userAccessor.CurrentUser.PhotoID > 0)
                {
                    supporterPhoto = string.Format("/file/imgThumb?FileID={0}&w=200&h=200",
                        userAccessor.CurrentUser.PhotoID);
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    supporterName = supporterName,
                    supporterPhoto = supporterPhoto
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.DoSupport", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطای فنی مواجه شد"
                });
            }
        }

        public PartialViewResult RatingDialog(long reserveid, int userid = 0)
        {
            var reserve = reserveService.Find(reserveid);
            if (reserve.UserID == userAccessor.CurrentUser.Id)
            {
                reserveService.UpdateRatingShownToGuest(reserveid, true);
            }
            var user_id = userid > 0 ? userid : reserve.UserID;
            ViewBag.user_id = user_id;
            ViewBag.comment = commentService.GetByAccSenderUser(reserve.AdvertiseID,
                user_id > 0 ? user_id : userAccessor.CurrentUser.Id);
            var model = UserRatingDTO.Generate(reportItemService.GetAccUserRatings(
                reserve.AdvertiseID, user_id) as List<ReportItem>, reserve.AdvertiseID,
                reserve.Advertise.Title, regionService.Find(reserve.Advertise.CityId == null ? 0 :
                (int)reserve.Advertise.CityId).PersianName);
            return PartialView("_Rating", model);
        }

        public JsonResult GetReserveToRate(int userId = -1)
        {
            try
            {
                if (userId == -1)
                {
                    userId = userAccessor.CurrentUser.Id;
                }
                if (userId < 1)
                {
                    return GenerateJsonResult(new { status = 0 });
                }
                var reserves = reserveService.GetListByUserId(userId, Reserve.ReserveStatus.Completed, false).AsQueryable();
                var comments = commentService.GetListBySenderUserId(userId);
                var reportItems = reportItemService.GetListByUserId(userId);
                var exception_ids = new List<long>();
                foreach (var item in reserves)
                {
                    if (comments.Any(x => x.AdvertiseID == item.AdvertiseID) ||
                        reportItems.Any(x => x.AdvertiseID == item.AdvertiseID))
                    {
                        exception_ids.Add(item.Id);
                    }
                }
                reserves = reserves.Where(x => !exception_ids.Contains(x.Id));
                var reserveToRate = reserves.FirstOrDefault();
                if (reserveToRate == null)
                {
                    return GenerateJsonResult(new { status = 0 });
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    reserveToRate = reserveToRate.Id
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize]
        public PartialViewResult ReservePaymentDialog(long id)
        {
            var reserve = reserveService.Find(id);
            if (reserve.UserID != userAccessor.CurrentUser.Id)
            {
                return null;
            }
            var guestUser = reserve.GuestUser;
            var coupon = accounting.GetMostValuableDiscountCouponIfAny(guestUser.Id);
            var model = ReservePaymentDTO.Generate(reserve,
                accounting.GetReservePrizeAvailable(reserve.TotalPrice, guestUser.GiftWalletAmount),
                coupon == null ? 0 : accounting.CalculateDiscountCouponPrice(coupon.Percent, reserve.CouponCalculationPrice),
                coupon == null ? 0 : coupon.Id,
                accounting.GetReservePaidAmount(reserve.Id, Reserve.StatusStringType.Guest));
            return PartialView("_ReservePayment", model);
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public PartialViewResult AddGuestPaymentPopup(long id)
        {
            var reserve = reserveService.Find(id);
            var paidPrice = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                Reserve.StatusStringType.Guest);
            var depositeAvailable = paidPrice < 1 &&
                reserve.CouponID < 1 &&
                reserve.PrizePrice < 1 &&
                reserve.DepositPrice < reserve.TotalPrice;
            var finalPaidPrice = paidPrice + reserve.CouponPrice +
                reserve.PrizePrice;
            var totalPrice = reserve.TotalPrice - finalPaidPrice;
            var depositePrice = reserve.DepositPrice;
            ViewBag.depositeAvailable = depositeAvailable;
            ViewBag.totalPrice = totalPrice;
            ViewBag.depositePrice = depositePrice;
            ViewBag.alreadyPaid = finalPaidPrice >= reserve.TotalPrice;
            ViewBag.userFullName = reserve.GuestUser.FullName;
            return PartialView("_AddGuestPayment", reserve);
        }

        public IActionResult InvoiceItemPopup(long id)
        {
            var reserve = reserveService.Find(id);
            if (reserve.HostUserID != userAccessor.CurrentUser.Id)
            {
                return null;
            }
            DateTime _total_pay_date;
            long _total_transaction_id;
            DateTime _host_site_portion_pay_date;
            long _host_site_portion_transaction_id;

            var allReservePayments = accounting.GetAllReservePaymentsAsIQueriable();
            var _guestName = reserve.GuestUser.FullName;
            if (string.IsNullOrEmpty(_guestName))
            {
                _guestName = reserve.UserID.ToString();
            }
            var _totalPaidPrice = accounting.GetReservePaymentPrice(
                    reserve.Id, ReservePayment.ReservePaymentType.GuestClearing,
                    out _total_pay_date, out _total_transaction_id, 0);
            var _depositePaidPrice = accounting.GetReservePaymentPrice(
                    reserve.Id, ReservePayment.ReservePaymentType.GuestDeposite,
                    out _total_pay_date, out _total_transaction_id, 0);
            var allRefundsOfReserve = allReservePayments.Where(
                x => x.ReserveID == reserve.Id && x.PaymentType == (int)ReservePayment.ReservePaymentType.SiteRefundToGuest);
            var _netPaidPrice = (_totalPaidPrice +
                _depositePaidPrice +
                reserve.CouponPrice + reserve.PrizePrice) -
            ((long)((allRefundsOfReserve == null || allRefundsOfReserve.Any() == false) ? 0 : allReservePayments.Sum(x => x.Price)));

            long _guestPaidPrice = 0;
            var hostUserId = reserve.HostUserID;
            var _hostPaidPrice = accounting.GetReservePaymentPrice(
                reserve.Id, ReservePayment.ReservePaymentType.GuestDeposite,
                out _host_site_portion_pay_date, out _host_site_portion_transaction_id, hostUserId);
            if (_hostPaidPrice <= 0)
            {
                _guestPaidPrice = _netPaidPrice;
            }
            var clearingPayment = allReservePayments.FirstOrDefault(
                x => x.ReserveID == reserve.Id && x.PaymentType == (int)ReservePayment.ReservePaymentType.SiteClearingToHost);
            var _isCleared = _hostPaidPrice > 0 || clearingPayment != null;
            string _clearingDateSring = "";
            long _clearingTransactionId = 0;
            if (clearingPayment != null)
            {
                _clearingDateSring = DateTimeUtility.GregorianToPersianDate(
                    clearingPayment.CreateDate) + "_" + clearingPayment.CreateDate.ToString("HH:mm");
                _clearingTransactionId = clearingPayment.TransactionID;
            }
            var _sitePortion = (long)(reserve.TotalPrice * 0.1f);
            var _hostPayablePrice = _hostPaidPrice > 0 ? 0 :
                (_netPaidPrice - _sitePortion);
            var hasInstantReservePenalty = false;
            long instantReservePenaltyPrice = 0;
            if (reserve.InstantReserve && _guestPaidPrice <= 0 && _hostPaidPrice <= 0)
            {
                var penaltyTransaction = accounting.GetCanselInstantReserveCreditTransaction(hostUserId, 100,
                    reserve.Id);
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
            var model = new InvoiceItemDTO()
            {
                id = reserve.Id,
                accommodationId = reserve.AdvertiseID,
                totalPrice = reserve.TotalPrice,
                couponPrice = reserve.CouponPrice,
                prizePrice = reserve.PrizePrice,
                createDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.CreateDate).Remove(0, 2),
                startDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.StartDate).Remove(0, 2),
                endDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.EndDate).Remove(0, 2),
                startDate = reserve.StartDate,
                endDate = reserve.EndDate,
                clearingDate = DateTimeUtility.GetSiteClearingDate(reserve.StartDate, reserve.EndDate),
                totalPaidPrice = _totalPaidPrice,
                sitePortion = _sitePortion,
                depositePaidPrice = _depositePaidPrice,
                netPaidPrice = _netPaidPrice,
                stayDays = DateTimeUtility.GetDatRangeDays(reserve.StartDate, reserve.EndDate),
                guestName = _guestName,
                isCleared = _isCleared,
                hostPaidPrice = _hostPaidPrice,
                guestPaidPrice = _guestPaidPrice,
                hostPayablePrice = _hostPayablePrice,
                hasInstantReservePenalty = hasInstantReservePenalty,
                instantReservePenaltyPrice = instantReservePenaltyPrice,
                clearingDateString = _clearingDateSring,
                clearingTransactionId = _clearingTransactionId
            };
            return PartialView("_InvoicePopup", model);
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public JsonResult AddGuestPayment(long id, int type,
            int method, long price, long transactionId)
        {
            try
            {
                var reserve = reserveService.Find(id);

                var reservePayment = new ReservePayment()
                {
                    CreateDate = DateTime.Now,
                    OperatorID = userAccessor.CurrentUser.Id,
                    PaymentMethod = method,
                    PaymentType = type,
                    Price = price,
                    TransactionID = transactionId,
                    ReserveID = id,
                    UserID = reserve.UserID
                };
                var invalidData = false;
                var errorMessage = "";

                if (reservePayment.Price <= 0)
                {
                    invalidData = true;
                    errorMessage = "لطفا قیمت را وارد کنید";
                }
                else if ((ReservePayment.ReservePaymentMethod)method == ReservePayment.ReservePaymentMethod.BankCard &&
                    reservePayment.TransactionID <= 0)
                {
                    invalidData = true;
                    errorMessage = "لطفا شماره تراکنش را وارد کنید";
                }
                else if (accounting.ReservePaymentExists(reservePayment.TransactionID, reservePayment.PaymentMethod))
                {
                    invalidData = true;
                    errorMessage = "شماره تراکنش تکراری می باشد";
                }
                else if ((ReservePayment.ReservePaymentMethod)method == ReservePayment.ReservePaymentMethod.AmlakbashiCredit &&
                    reserve.GuestUser.WalletAmount < price)
                {
                    invalidData = true;
                    errorMessage = "موجودی کیف پول کافی نمی باشد";
                }
                if (invalidData)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = errorMessage
                    });
                }
                if ((ReservePayment.ReservePaymentMethod)method == ReservePayment.ReservePaymentMethod.AmlakbashiCredit)
                {
                    long newCredit;
                    var creditTransactionId = accounting.DecreaseCredit(reserve.UserID, price, 0, reserve.Id, out newCredit,
                        CreditTransaction.WalletTransactionReason.Reserve, null,
                        null, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                    reservePayment.TransactionID = creditTransactionId;
                }
                accounting.InsertReservePayment(reservePayment);
                if (reservePayment.PaymentType == (int)ReservePayment.ReservePaymentType.GuestDeposite
                    || reservePayment.PaymentType == (int)ReservePayment.ReservePaymentType.GuestClearing)
                {
                    var paidPrice = accounting.GetReservePaidAmount(reservePayment.ReserveID, Reserve.StatusStringType.Guest);
                    if (reserve.Status == Reserve.ReserveStatus.WaitForReserve)
                    {
                        if (paidPrice + reserve.CouponPrice + reserve.PrizePrice >= reserve.DepositPrice)
                            reserveService.SetStatus(reservePayment.ReserveID, Reserve.ReserveStatus.Reserved, true,
                                ActionLog.ActionSourceEnum.AdminPanel, userAccessor.CurrentUser.Id);
                    }
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "پرداخت مورد نظر با موفقیت اضافه شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.AddGuestPayment", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        public JsonResult SubmitDiscountCode(string code, long reserveId)
        {
            if (string.IsNullOrEmpty(code))
            {
                return GenerateJsonResult(new { status = 0, msg = "لطفا کد را وارد کنید" });
            }
            var lowerCode = code.ToLower();
            var discountCodeType = lowerCode == "amb5" ? DiscountCoupon.DiscountCouponType.Moupon :
                lowerCode == "inst8" ? DiscountCoupon.DiscountCouponType.Instagram :
                lowerCode == "pedar1400" ? DiscountCoupon.DiscountCouponType.Pedar1400 :
                DiscountCoupon.DiscountCouponType.Unset;

            if (discountCodeType == DiscountCoupon.DiscountCouponType.Unset)
            {
                return GenerateJsonResult(new { status = 0, msg = "کد وارد شده اشتباه است" });
            }

            var startDate = DateTime.Parse("01/01/2021");
            var identityUser = userService.GetIdentityUser(userAccessor.CurrentUser.PhoneNumber);
            if ((discountCodeType == DiscountCoupon.DiscountCouponType.Moupon ||
                discountCodeType == DiscountCoupon.DiscountCouponType.Instagram) &&
                identityUser.CreateDate.Value.Date < startDate.Date)
            {
                return GenerateJsonResult(new { status = 0, msg = "شما مجوز استفاده از این کد تخفیف را ندارید" });
            }

            if (discountCodeType == DiscountCoupon.DiscountCouponType.Pedar1400 &&
                DateTime.Now.Date > DateTime.Parse("02/15/2022"))
            {
                return GenerateJsonResult(new { status = 0, msg = "کد وارد شده اشتباه است" });
            }

            var coupon = accounting.FindDiscountCoupon(userAccessor.CurrentUser.Id, discountCodeType);
            if (coupon == null)
            {
                coupon = accounting.InsertDiscountCoupon(userAccessor.CurrentUser.Id, discountCodeType, 5);
            }
            else
            {
                if (coupon.UsingReserveID > 0)
                {
                    return GenerateJsonResult(new { status = 0, msg = "این کد تخفیف استفاده شده است" });
                }
            }
            var reserve = reserveService.Find(reserveId);
            var discountPrice = accounting.CalculateDiscountCouponPrice(coupon.Percent, reserve.CouponCalculationPrice);
            return GenerateJsonResult(new
            {
                status = 1,
                couponId = coupon.Id,
                discountPrice = discountPrice,
                msg = "کد تخفیف اعمال شد"
            });
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public IActionResult ReserveByPaymentReinquiry(long reserveId, long paymentId)
        {
            try
            {
                string msg;
                var result = reserveService.ReserveByPaymentReinquiry(reserveId, paymentId, out msg);
                return GenerateJsonResult(new
                {
                    status = result ? 1 : 0,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ReserveByPaymentReinquiry", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد."
                });
            }
        }

        [Authorize]
        public IActionResult UpdateEarlyCheckout(long reserveId, Reserve.EarlyCheckoutEnum earlyCheckout)
        {
            try
            {
                var result = reserveService.UpdateEarlyCheckout(reserveId, userAccessor.CurrentUser.Id, earlyCheckout);
                return GenerateJsonResult(new
                {
                    status = result.HasError() ? 0 : 1,
                    msg = result.GetErrors().FirstOrDefault()
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ConfirmEarlyCheckout", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد."
                });
            }
        }
    }
}