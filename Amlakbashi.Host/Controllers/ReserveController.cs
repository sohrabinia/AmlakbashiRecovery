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

        [Auth(UserRoles.Admin)]
        public ActionResult Admin()
        {
            return View();
        }

        [Auth(UserRoles.Admin)]
        public ActionResult Index(int? page, long reserve_id = -1, long advertise_id = -1,
            int host_user_id = -1, int guest_user_id = -1, int reserve_status = -1,
            int host_response_status = -1, int general_status = -1,
            string site_clearing_date = "", int site_cleared_status = -1,
            string reserve_from_date = "", string reserve_to_date = "",
            string reserve_end_date = "",
            int stay_duration_from = -1, int stay_duration_to = -1,
            int reserve_support_status = 0, bool shouldFollow = false,
            int supporter_id = -1, int host_card_status = -1,
            int mainFilter = 0, int instantReserveFilter = 2,
            bool disableAutoCancel = false, bool accVisited = false)
        {
            try
            {
                var itemPerPage = 10;
                var model = reserveService.Filter(reserve_id, advertise_id, host_user_id, guest_user_id,
                    reserve_status, host_response_status, general_status, site_clearing_date, site_cleared_status,
                    reserve_from_date, reserve_to_date, reserve_end_date, stay_duration_from, stay_duration_to,
                    reserve_support_status, shouldFollow, supporter_id, host_card_status, mainFilter,
                    instantReserveFilter, disableAutoCancel, accVisited).AsQueryable();

                if (reserve_support_status > 0)
                {
                    var st = (ReserveSupport.SupporterStatus)reserve_support_status;
                    if (st == ReserveSupport.SupporterStatus.SupportingByYou)
                    {
                        supporter_id = userAccessor.CurrentUser.Id;
                    }
                    else
                    {
                        if (st != ReserveSupport.SupporterStatus.Done)
                        {
                            supporter_id = -1;
                        }
                        var now = DateTime.Now.Date;
                        model = model.Where(x => x.EndDate > now);
                        model = model.Where(x => x.Status != Reserve.ReserveStatus.Deleted);
                        model = reserveSupportManager.FilterBySupporterStatus(
                            userAccessor.CurrentUser.Id, model, st);
                    }
                }
                if (supporter_id > 0)
                {
                    IList<ReserveSupport> reserveSupports = reserveSupportService.GetListBySupporterId(supporter_id);
                    var reserve_ids = new List<long>();
                    foreach (var reserveSupport in reserveSupports)
                    {
                        reserve_ids.AddRange(reserveSupport.GetAllReserveIds());
                    }
                    reserve_ids = reserve_ids.Distinct().ToList();
                    model = model.Where(x => x.Status != Reserve.ReserveStatus.Deleted);
                    model = model.Where(x => reserve_ids.Contains(x.Id));
                }

                List<Reserve> finalModel;
                if (host_card_status > -1)
                {
                    IQueryable<BankCard> bankCards = bankCardService.GetAll();
                    var filteredIds = new List<long>();
                    BankCard bankCard;
                    if (host_card_status == 0) //shaba
                    {
                        foreach (var item in model)
                        {
                            bankCard = bankCards.FirstOrDefault(x => x.UserID == item.Advertise.UserID);
                            if (bankCard != null &&
                                !string.IsNullOrEmpty(bankCard.ShabaNumber))
                            {
                                filteredIds.Add(item.Id);
                            }
                        }
                    }
                    else if (host_card_status == 1) // bank card
                    {
                        foreach (var item in model)
                        {
                            bankCard = bankCards.FirstOrDefault(x => x.UserID == item.Advertise.UserID);
                            if (bankCard != null &&
                                string.IsNullOrEmpty(bankCard.ShabaNumber) &&
                                !string.IsNullOrEmpty(bankCard.BankCardNumber))
                            {
                                filteredIds.Add(item.Id);
                            }
                        }
                    }
                    else if (host_card_status == 2) // none
                    {
                        foreach (var item in model)
                        {
                            bankCard = bankCards.FirstOrDefault(x => x.UserID == item.Advertise.UserID);
                            if (bankCard == null ||
                                (string.IsNullOrEmpty(bankCard.ShabaNumber) &&
                                string.IsNullOrEmpty(bankCard.BankCardNumber)))
                            {
                                filteredIds.Add(item.Id);
                            }
                        }
                    }
                    model = model.Where(x => filteredIds.Contains(x.Id));
                    finalModel = model.ToList();
                    foreach (var item in finalModel)
                    {
                        item.Temp_HostPayablePrice = PriceUtility.CalculateHostPayablePrice(item.TotalPrice,
                            accounting.GetReservePaidAmount(item.ReservePayments.ToList(),
                                StatusStringType.Guest),
                            item.CouponPrice, item.PrizePrice);
                    }
                    finalModel = finalModel.OrderByDescending(x => x.Temp_HostPayablePrice).ToList();
                }
                else
                {
                    model = model.OrderByDescending(x => x.Id);
                    finalModel = model.ToList();
                }

                var PageNumber = page ?? 1;
                var onePageOfModel = finalModel.ToPagedList(PageNumber, itemPerPage);

                var supporterList = new List<UserFullNameDTO>();
                var supporters = TempRoles.AdminMobiles;
                foreach (var item in supporters)
                {
                    var supporter = userService.GetByMainMobile(item);
                    if (supporter != null)
                        supporterList.Add(new UserFullNameDTO() { id = supporter.Id, fullName = supporter.FullName });
                }
                ViewBag.reserve_id = reserve_id;
                ViewBag.advertise_id = advertise_id;
                ViewBag.host_user_id = host_user_id;
                ViewBag.guest_user_id = guest_user_id;
                ViewBag.reserve_status = reserve_status;
                ViewBag.host_response_status = host_response_status;
                ViewBag.general_status = general_status;
                ViewBag.site_clearing_date = site_clearing_date;
                ViewBag.reserve_from_date = reserve_from_date;
                ViewBag.reserve_to_date = reserve_to_date;
                ViewBag.reserve_end_date = reserve_end_date;
                ViewBag.site_cleared_status = site_cleared_status;
                ViewBag.stay_duration_from = stay_duration_from;
                ViewBag.stay_duration_to = stay_duration_to;
                ViewBag.reserve_support_status = reserve_support_status;
                ViewBag.shouldFollow = shouldFollow;
                ViewBag.supporter_id = supporter_id;
                ViewBag.supporterList = supporterList;
                ViewBag.host_card_status = host_card_status;
                ViewBag.mainFilter = mainFilter;
                ViewBag.instantReserveFilter = instantReserveFilter;
                ViewBag.disableAutoCancel = disableAutoCancel;
                ViewBag.accVisited = accVisited;
                var dto = new ReserveAdminDTO();
                dto.reserveList = new List<ReserveAdminItemDTO>();
                var list = onePageOfModel.ToList();
                ReserveSupport tempCurrentReserveSupport;
                bool refundDone;
                var currentUserId = userAccessor.CurrentUser.Id;
                var reserveToCheck = reserveService.GetReservesIncludingSupport(list.Select(s => s.Id).ToList()).ToList();
                var resIndex = 0;
                foreach (var checkItem in reserveToCheck)
                {
                    var item = list[resIndex];
                    dto.reserveList.Add(ReserveAdminItemDTO.Generate(item,
                        reserveSupportManager.Analyze(checkItem,
                    out tempCurrentReserveSupport, currentUserId),
                        accounting.GetReservePaidAmount(
                    item.ReservePayments.ToList(), StatusStringType.Guest),
                        accounting.ReserveCanClear(item.Id),
                        accounting.ReserveShouldRefund(item.Id,
                        item.Status, out refundDone), refundDone));
                    resIndex++;
                }
                ViewBag.reserveAdmin = dto;

                ViewBag.RowIndexStart = (PageNumber * itemPerPage) - itemPerPage;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Index", exc);
                return Redirect(Request.Headers["referer"].ToString());
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
                var acc = reserve.Advertise;
                ViewBag.HostUserID = acc.UserID;
                ViewBag.HostPhotoID = userService.Find(acc.UserID).PhotoID;
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

        [Auth(UserRoles.Admin)]
        [HttpGet]
        public ActionResult Edit(int reserve_id = -1)
        {
            try
            {
                ViewBag.msg = TempData["msg"];
                var model = reserveService.Find(reserve_id);
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Edit(get)", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Reserve reserve,
            string start_date = null, string end_date = null)
        {
            try
            {
                var objReserve = reserveService.Find(reserve.Id);
                if (objReserve.Status > ReserveStatus.Rejected && !(userAccessor.CurrentUser.Id == 3 ||
                    userAccessor.CurrentUser.Id == 1667 ||
                    userAccessor.CurrentUser.Id == 12 ||
                    userAccessor.CurrentUser.Id == 2122 ||
                    userAccessor.CurrentUser.Id == 19076 ||
                    userAccessor.CurrentUser.Id == 82119))
                {
                    ViewBag.errorMsg = "شما مجوز ویرایش ندارید";
                    return View();
                }
                string msg;
                if (reserveService.Update(reserve, start_date,
                    end_date, out msg, userAccessor.CurrentUser.Id,
                    ActionLog.ActionSourceEnum.AdminPanel) == false)
                {
                    ViewBag.errorMsg = msg;
                    return View();
                }
                if (!reserveService.SetHostResponse(reserve.Id, reserve.HostResponse, true, ActionLog.ActionSourceEnum.AdminPanel, userAccessor.CurrentUser.Id))
                    reserveService.SetStatus(reserve.Id, reserve.Status, true, ActionLog.ActionSourceEnum.AdminPanel, userAccessor.CurrentUser.Id);
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Edit(post)", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult Delete(long reserve_id)
        {
            try
            {
                string msg;
                var done = reserveService.Delete(reserve_id, out msg);
                return GenerateJsonResult(new { status = done ? 1 : 0, val = msg });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.Delete", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
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
                if (reserve_id != null)
                    reserve_id = StringUtility.PersianNumberToEnglish(reserve_id);
                if (selectType == ReserveManagerSelectType.All)
                {
                    selectType = userAccessor.CurrentUser.UserGeneralType > 0 ?
                        ReserveManagerSelectType.Host : ReserveManagerSelectType.Guest;
                }
                Dictionary<ReserveCategory, int> countDict;
                var reserves = reserveService.GetReserveDashboardItems(userAccessor.CurrentUser,
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
                var currentUser = userAccessor.CurrentUser;
                foreach (var reserve in reserves)
                {
                    var advertise = reserve.Advertise;
                    var paidAmount = accounting.GetReservePaidAmount(reserve.Id, StatusStringType.Guest);
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
                var ok = advertiseService.CheckReserve(userAccessor.CurrentUser.Id, advertise_id,
                    number_of_guests, from_date, to_date, out msg);

                return GenerateJsonResult(new
                {
                    val = ok ? 1 : 0,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CheckReserve", exc);
                return GenerateJsonResult(new { val = 0, msg = "متاسفانه درخواست رزرو با خطا مواجه شد" } );
            }
        }

        public JsonResult ReserveRequest(int advertise_id, string from_date, string to_date,
            int number_of_guests, bool instant_reserve = false)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return GenerateJsonResult(new { val = 2 });
                }
                if (userAccessor.CurrentUser.AccessType == (int)Entities.User.AccessTypeEnum.ReserveBanned || userAccessor.CurrentUser.AccessType == (int)Entities.User.AccessTypeEnum.LoginBanned)
                {
                    return GenerateJsonResult(new { val = 3 });
                }
                string msg;
                long reserveId;
                var done = advertiseService.ReserveRequest(advertise_id,
                    userAccessor.CurrentUser.Id, from_date, to_date, number_of_guests,
                    instant_reserve, out msg, out reserveId);
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
                var advertise = reserve.Advertise;
                var currentUser = userAccessor.CurrentUser;
                if (!(is_host && advertise.UserID == currentUser.Id) &&
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
                    is_host, out msg, out isPending, ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id) ;
                return GenerateJsonResult(new { val = 1, msg = msg, isPending = isPending });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.CancelReserve", exc);
                return GenerateJsonResult(new { val = 0,
                    msg = "متاسفانه درخواست لغو رزرو با خطا مواجه شد" });
            }
        }

        public JsonResult RefuseCancelReserve(long reserve_id, bool is_host)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertsie = reserve.Advertise;
                var currentUser = userAccessor.CurrentUser;
                if ((is_host ? advertsie.UserID != currentUser.Id :
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
                return GenerateJsonResult(new { val = 0,
                    msg = "متاسفانه درخواست انصراف از لغو رزرو با خطا مواجه شد" });
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
                return GenerateJsonResult(new { val = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید" });
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
                return GenerateJsonResult(new { val = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید" });
            }
        }

        [Authorize]
        public ActionResult ConfirmCashPayByNotif(int reserve_id, bool payed)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                if ((advertise.UserID != userAccessor.CurrentUser.Id || reserve.Status != Reserve.ReserveStatus.CashPay))
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
                ViewBag.user_id = advertise.UserID;
                ViewBag.selectType = Reserve.ReserveManagerSelectType.Host;

                return View("ReserveItemManager");
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.ConfirmCashPay", exc);
                ViewBag.msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید";
                ViewBag.user_id = userAccessor.CurrentUser.Id;
                ViewBag.selectType = Reserve.ReserveManagerSelectType.Host;
                return View("ReserveItemManager", reserveService.GetListByUserId(userAccessor.CurrentUser.Id,
                    Reserve.ReserveManagerSelectType.Host));
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
                return GenerateJsonResult(new { val = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید" });
            }
        }

        [Authorize]
        public JsonResult ReserveResponse(int reserve_id, int host_response)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                if (advertise.UserID != userAccessor.CurrentUser.Id)
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
                return GenerateJsonResult(new { val = 0,
                    msg = "متاسفانه جواب درخواست رزرو با خطا مواجه شد" });
            }
        }

        [Authorize]
        public ActionResult ReserveResponseByNotif(int reserve_id, int host_response)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                var userId = userAccessor.CurrentUser.Id;
                if (advertise.UserID != userId && userId != 1667)
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
                var rejectReason = "";
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
                        rejectReason = "price";
                        break;
                    case Reserve.HostResponseEnum.RejectedHomeFull:
                        msg = "درخواست رزرو رد شد. شما میتوانید از بخش آگهی های من و با کلیک بر روی دکمه تعیین روز های پر، روزهایی که پر هستند را تعیین کنید";
                        rejectReason = "home_full";
                        break;
                }
                //ViewBag.host_response_status = 1;
                //ViewBag.host_response = host_response;
                //ViewBag.host_response_msg = msg;
                //ViewBag.reject_reason = rejectReason;
                //ViewBag.host_response_reserve_id = reserve_id;
                //ViewBag.user_id = advertise.UserID;
                //ViewBag.selectType = Reserve.ReserveManagerSelectType.Host;
                //ViewBag.msg = "";

                return Redirect("/reserve/reserveitemmanager?reserve_id=" + reserve_id +
                    "&user_id=" + advertise.UserID +
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
            int pay_reserve_type, bool useCoupon = false, bool usePrize = false, long couponId = 0
            )
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
                        return RedirectToAction("performpay", "cart", new { payment_id = payment_id });
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

        [Auth(UserRoles.Admin)]
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
                        var advertise = reserve.Advertise;
                        //if (advertise.UserID != userAccessor.CurrentUser.Id)
                        //{
                        //    return new JsonResult()
                        //    {
                        //        Data = new { status = 0, msg = "شما مجوز این عملیات را ندارید" },
                        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        //    };
                        //}
                        var payment_id = accounting.PayAmlakbashiPortion(reserve_id, pay_type, out already_payed,
                            out price, ReservePayment.ReservePaymentMethod.AmlakbashiCredit, advertise.UserID, userAccessor.DoerUser.Id);
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
                            current_reserve_status = (int)reserveService.FinalizeReserve(reserve_id, payment_id, price, ReservePayment.ReservePaymentMethod.AmlakbashiCredit, ActionLog.ActionSourceEnum.AdminPanel, userAccessor.DoerUser.Id, advertise.UserID, 0, 0, false);
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

        [Auth(UserRoles.Admin)]
        public JsonResult CallForRequest(long reserve_id)
        {
            try
            {
                bool sent = false;
                var reserve = reserveService.Find(reserve_id);
                if (reserve.Status == ReserveStatus.WaitForResponse)
                {
                    var user = userService.Find(reserve.Advertise.UserID);
                    if (user.GetLoginProperty() == Entities.User.LoginPriorites.Mobile)
                    {
                        userContact.SendReserveRequestCall(user, reserve.AdvertiseID);
                    }
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

        [Auth(UserRoles.Admin)]
        public JsonResult CallForPayment(long reserve_id)
        {
            try
            {
                var sent = false;
                var reserve = reserveService.Find(reserve_id);
                if (reserve.Status == ReserveStatus.WaitForReserve)
                {
                    var guest_user = reserve.GuestUser;
                    if (guest_user.GetLoginProperty() == Entities.User.LoginPriorites.Mobile)
                    {
                        userContact.SendPayReserveCall(guest_user, reserve.AdvertiseID);
                    }
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

        [Auth(UserRoles.Admin)]
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

        [Auth(UserRoles.Admin)]
        public JsonResult SiteClearingHost(long reserve_id, bool confirmed = false,
            int? user_id = null, long transaction_id = 0, long ref_id = 0,
            int method_id = 0, long price = 0, bool? send_sms = null)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                var host_user = userService.Find(advertise.UserID);
                var guestPaidAmount = accounting.GetReservePaidAmount(reserve_id,
                        StatusStringType.Guest);

                var payable_price = PriceUtility.CalculateHostPayablePrice(reserve.TotalPrice, guestPaidAmount,
                    reserve.CouponPrice, reserve.PrizePrice);
                //var start_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.StartDate);
                //var end_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.EndDate);
                var days = (int)(reserve.EndDate - reserve.StartDate).TotalDays;
                if (!confirmed)
                {
                    var host_name = host_user.FName + " " + host_user.LName;
                    if (string.IsNullOrEmpty(host_name))
                        host_name = host_user.GetPhoneNumber(Amlakbashi.Core.Entities.User.PhoneType.MainMobile);
                    var host_bank_card = bankCardService.GetByUserId(host_user.Id);
                    var bank_card_name = host_bank_card != null ?
                        ((host_bank_card.FName != null ? host_bank_card.FName + " " : "") +
                        (host_bank_card.LName != null ? host_bank_card.LName : "")) : "";
                    return GenerateJsonResult(new
                    {
                        status = 2,
                        days = days,
                        total_price = string.Format("{0:n0}", reserve.TotalPrice) + " تومان",
                        guest_payed_price = string.Format("{0:n0}", guestPaidAmount) + " تومان",
                        site_portion = string.Format("{0:n0}", guestPaidAmount - payable_price) + " تومان",
                        payable_price = string.Format("{0:n0}", payable_price) + " تومان",
                        payable_price_raw = payable_price * 10,
                        bank_card_number = host_bank_card != null &&
                                !string.IsNullOrEmpty(host_bank_card.BankCardNumber) ?
                                host_bank_card.BankCardNumber : "ثبت نشده",
                        bank_card_name = !string.IsNullOrEmpty(bank_card_name) ?
                                bank_card_name : "بدون نام",
                        bank_card_verified = host_bank_card != null &&
                                host_bank_card.BankCardStatus == (int)BankCard.BankCardStatusEnum.Verified,
                        shaba_verified = host_bank_card != null &&
                                host_bank_card.ShabaStatus == (int)BankCard.BankCardStatusEnum.Verified,
                        host_credit = host_user.Credit,
                        shaba_number = host_bank_card != null &&
                                !string.IsNullOrEmpty(host_bank_card.ShabaNumber) ?
                                host_bank_card.ShabaNumber : "ثبت نشده",
                        host_name = host_name
                    });
                }
                if (user_id == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 3,
                        error = "",
                        default_user_id = userAccessor.CurrentUser.Id,
                        default_transaction_id = "",
                        default_price = payable_price
                    });
                }
                var user = userService.Find(user_id);
                if (user == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 3,
                        error = "کاربری با این کد کاربری وجود ندارد",
                        default_user_id = userAccessor.CurrentUser.Id,
                        default_transaction_id = transaction_id <= 0 ? "" : transaction_id.ToString(),
                        default_price = payable_price
                    });
                }
                if (transaction_id <= 0)
                {
                    return GenerateJsonResult(new
                    {
                        status = 3,
                        error = "لطفا شماره تراکنش را وارد کنید",
                        default_user_id = user_id,
                        default_transaction_id = "",
                        default_price = payable_price
                    });
                }
                if (send_sms == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 4
                    });
                }
                if ((bool)send_sms)
                {
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserLoginPriority = host_user.LoginPriority,
                        UserMainMobile = host_user.MainMobile,
                        UserAppNotificationToken = host_user.AppNotificationToken,
                        UserEmail = host_user.Email,
                        UserFcmAppNotificationToken = host_user.FcmAppNotificationToken,
                        UserNotificationToken = host_user.NotificationToken,
                        Type = UserContactType.SiteClearingHost,
                        Price = price.ToString(),
                        ReserveId = reserve.Id.ToString(),
                        TransactionId = transaction_id.ToString(),
                        AdvertiseId = reserve.AdvertiseID.ToString()
                    });
                }
                if (accounting.InsertReservePayment((int)user_id,
                    reserve_id, transaction_id, ref_id,
                    ReservePayment.ReservePaymentType.SiteClearingToHost,
                    price, (ReservePayment.ReservePaymentMethod)method_id,
                    userAccessor.CurrentUser.Id
                    ) == null)
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
                    msg = "تسویه شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.SiteClearingHost", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult SiteRefundGuest(long reserve_id, bool confirmed = false, int? user_id = null,
            long transaction_id = 0, long ref_id = 0, int method_id = 0, long price = 0,
            bool? send_sms = null, string new_bank_card_number = null,
            string new_bank_card_fname = null, string new_bank_card_lname = null)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                var guest_user = userService.Find(reserve.UserID);
                var guest_user_card = bankCardService.GetByUserId(guest_user.Id);
                var guest_payed_price = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                    Reserve.StatusStringType.Guest);
                //var payable_price = ReserveDepend.CalculateHostPayablePrice(reserve.TotalPrice, guest_payed_price);
                //var start_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.StartDate);
                //var end_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.EndDate);
                //var days = (int)(reserve.EndDate - reserve.StartDate).TotalDays;
                if (guest_user_card == null || string.IsNullOrEmpty(guest_user_card.BankCardNumber))
                {
                    if (!string.IsNullOrEmpty(new_bank_card_number))
                    {
                        if (BankUtility.ValidateBankCardNumber(new_bank_card_number))
                        {
                            if (string.IsNullOrEmpty(new_bank_card_fname) ||
                                string.IsNullOrEmpty(new_bank_card_lname))
                            {
                                return GenerateJsonResult(new
                                {
                                    status = 2,
                                    msg = "لطفا نام و نام خانوادگی صاحب کارت را وارد کنید"
                                });
                            }
                            if (guest_user_card != null)
                            {
                                guest_user_card.BankCardNumber = new_bank_card_number;
                                guest_user_card.FName = new_bank_card_fname;
                                guest_user_card.LName = new_bank_card_lname;
                                guest_user_card.LastModifyDate = DateTime.Now;
                                bankCardService.Update(guest_user_card, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                            }
                            else
                            {
                                guest_user_card = new BankCard()
                                {
                                    BankCardNumber = new_bank_card_number,
                                    FName = new_bank_card_fname,
                                    LName = new_bank_card_lname,
                                    BankCardStatus = (int)BankCard.BankCardStatusEnum.Verified,
                                    UserID = guest_user.Id,
                                    CreateDate = DateTime.Now,
                                    LastModifyDate = DateTime.Now
                                };
                                bankCardService.Insert(guest_user_card, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                            }
                        }
                        else
                        {
                            return GenerateJsonResult(new
                            {
                                status = 2,
                                msg = "شماره کارت وارد شده نامعتبر میباشد"
                            });
                        }
                    }
                    else
                    {
                        return GenerateJsonResult(new
                        {
                            status = 2,
                            msg = ""
                        });
                    }
                }
                if (!confirmed)
                {
                    var guest_name = guest_user.FName + " " + guest_user.LName;
                    if (string.IsNullOrEmpty(guest_name))
                        guest_name = guest_user.GetPhoneNumber(Amlakbashi.Core.Entities.User.PhoneType.MainMobile);
                    var bank_card_name = guest_user_card != null ?
                        ((guest_user_card.FName != null ? guest_user_card.FName + " " : "") +
                        (guest_user_card.LName != null ? guest_user_card.LName : "")) : "";
                    return GenerateJsonResult(new
                    {
                        status = 3,
                        total_price = string.Format("{0:n0}", reserve.TotalPrice) + " تومان",
                        guest_payed_price = string.Format("{0:n0}", guest_payed_price) + " تومان",
                        guest_payed_price_raw = guest_payed_price * 10,
                        bank_card_number = guest_user_card != null &&
                                !string.IsNullOrEmpty(guest_user_card.BankCardNumber) ?
                                guest_user_card.BankCardNumber : "ثبت نشده",
                        bank_card_name = !string.IsNullOrEmpty(bank_card_name) ?
                                bank_card_name : "بدون نام",
                        bank_card_verified = guest_user_card != null &&
                                guest_user_card.BankCardStatus == (int)BankCard.BankCardStatusEnum.Verified,
                        guest_name = guest_name
                    });
                }
                if (user_id == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 4,
                        error = "",
                        default_user_id = userAccessor.CurrentUser.Id,
                        default_transaction_id = "",
                        default_price = guest_payed_price
                    });
                }
                var user = userService.Find(user_id);
                if (user == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 4,
                        error = "کاربری با این کد کاربری وجود ندارد",
                        default_user_id = userAccessor.CurrentUser.Id,
                        default_transaction_id = transaction_id <= 0 ? "" : transaction_id.ToString(),
                        default_price = guest_payed_price
                    });
                }
                if (transaction_id <= 0)
                {
                    return GenerateJsonResult(new
                    {
                        status = 4,
                        error = "لطفا شماره تراکنش را وارد کنید",
                        default_user_id = user_id,
                        default_transaction_id = "",
                        default_price = guest_payed_price
                    });
                }
                if (send_sms == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 5
                    });
                }
                if ((bool)send_sms)
                {
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserLoginPriority = guest_user.LoginPriority,
                        UserMainMobile = guest_user.MainMobile,
                        UserAppNotificationToken = guest_user.AppNotificationToken,
                        UserEmail = guest_user.Email,
                        UserFcmAppNotificationToken = guest_user.FcmAppNotificationToken,
                        UserNotificationToken = guest_user.NotificationToken,
                        Type = UserContactType.SiteRefundGuest,
                        AdvertiseId = reserve.AdvertiseID.ToString(),
                        ReserveId = reserve.Id.ToString(),
                        TransactionId = transaction_id.ToString(),
                        Price = price.ToString()
                    });
                }
                if (accounting.InsertReservePayment((int)user_id,
                    reserve_id, transaction_id, ref_id,
                    ReservePayment.ReservePaymentType.SiteRefundToGuest,
                    price, (ReservePayment.ReservePaymentMethod)method_id,
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
                    msg = "عودت شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.SiteRefundGuest", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult SiteClearingWithCredit(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                var host_user = userService.Find(advertise.UserID);
                var guestPaidAmount = accounting.GetReservePaidAmount(
                    reserve.ReservePayments.ToList(), StatusStringType.Guest);
                var payable_price = PriceUtility.CalculateHostPayablePrice(
                    reserve.TotalPrice, guestPaidAmount, reserve.CouponPrice,
                    reserve.PrizePrice);
                //var start_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.StartDate);
                //var end_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.EndDate);
                var days = (int)(reserve.EndDate - reserve.StartDate).TotalDays;
                long newCredit;
                var transaction_id = accounting.IncreaseCredit(advertise.UserID, payable_price, 0, reserve_id, Entities.User.CreditTransactionCause.Clearing, out newCredit);
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

        [Auth(UserRoles.Admin)]
        public JsonResult SendSiteClearingWithCreditSms(long reserve_id, long payable_price, long transaction_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var user = userService.Find(reserve.Advertise.UserID);
                userService.SendMessage(new UserContactDTO()
                {
                    UserLoginPriority = user.LoginPriority,
                    UserMainMobile = user.MainMobile,
                    UserAppNotificationToken = user.AppNotificationToken,
                    UserEmail = user.Email,
                    UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                    UserNotificationToken = user.NotificationToken,
                    Type = UserContactType.SiteClearingHostWithCredit,
                    TransactionId = transaction_id.ToString(),
                    Price = payable_price.ToString(),
                    AdvertiseId = reserve.AdvertiseID.ToString(),
                    ReserveId = reserve_id.ToString()
                });
                return GenerateJsonResult(new { status = 1 });
            }
            catch(Exception exc)
            {
                logger.Error("SendSiteClearingWithCreditSms", exc);
                return GenerateJsonResult(new { status = 0 });
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

        [Auth(UserRoles.Admin)]
        public JsonResult SiteRefundGuestWithCredit(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                var guest_user = userService.Find(reserve.UserID);
                var guest_payed_price = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                    Reserve.StatusStringType.Guest);
                //var payable_price = ReserveDepend.CalculateHostPayablePrice(reserve.TotalPrice, guest_payed_price);
                //var start_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.StartDate);
                //var end_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.EndDate);
                //var days = (int)(reserve.EndDate - reserve.StartDate).TotalDays;
                long newCredit;
                var transaction_id = accounting.IncreaseCredit(reserve.UserID, guest_payed_price, 0, reserve_id, Entities.User.CreditTransactionCause.Refund, out newCredit);
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

        [Authorize]
        public JsonResult SetHostCallDate(long reserve_id)
        {
            try
            {
                reserveService.UpdateHostCallDate(reserve_id, DateTime.Now);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.SetHostCallDate", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize]
        public JsonResult SetGuestCallDate(long reserve_id)
        {
            try
            {
                reserveService.UpdateGuestCallDate(reserve_id, DateTime.Now);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Reserve.SetGuestCallDate", exc);
                return GenerateJsonResult(new { status = 0 });
            }
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

        [Authorize]
        [HttpGet]
        public ActionResult GenerateGuestReceipt(long reserve_id)
        {
            var model = reserveService.GenerateVoucher(reserve_id, userAccessor.CurrentUser.Id);
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

        [Auth(UserRoles.Admin)]
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

        [Auth(UserRoles.Admin)]
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
                    var my_user_id = userAccessor.CurrentUser.Id;
                    if (userAccessor.CurrentUser.Id != 3 &&
                        userAccessor.CurrentUser.Id != 12 &&
                        userAccessor.CurrentUser.Id != 1667 &&
                        userAccessor.CurrentUser.Id != 2122)
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

        [Auth(UserRoles.Admin)]
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

        [Auth(UserRoles.Admin)]
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

        [Auth(UserRoles.Admin)]
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

        [Auth(UserRoles.Admin)]
        public JsonResult ToggleDisableAutoCancel(long id, bool active)
        {
            try
            {
                reserveService.UpdateDisableAutoCancel(id, active);
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

        [Auth(UserRoles.Admin)]
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
            int user_id = userAccessor.CurrentUser.Id;
            var reserve = reserveService.Find(reserve_id);
            var advertise = reserve.Advertise;
            if (reserve.UserID != user_id &&
                advertise.UserID != user_id)
            {
                return RedirectToAction("AccessDenied", "Errors");
            }
            //ViewBag.index = index.ToString();
            //ViewBag.is_guest = is_guest.ToString();
            //ViewBag.is_host = is_host.ToString();
            //ViewBag.user_id = user_id;
            //ViewBag.chatCount = chatService.GetNotReadCountByReserveId(reserve_id, user_id);
            var paidAmount = accounting.GetReservePaidAmount(reserve.Id, StatusStringType.Guest);
            var unreadChatCount = chatService.GetNotReadCountByReserveId(reserve.Id, user_id);
            var rulesDict = advertiseService.GetRulesDictionary(advertise.Id);
            var model = ReserveDashboardItemDTO.Generate(
                reserve, index, is_guest, is_host, user_id,
                paidAmount + reserve.CouponPrice + reserve.PrizePrice,
                unreadChatCount, rulesDict);
            return PartialView("_ReserveItem", model);
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

        [Auth(UserRoles.Admin)]
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
            var acc = reserve.Advertise;
            var user_id = userid > 0 ? userid : reserve.UserID;
            ViewBag.user_id = user_id;
            ViewBag.comment = commentService.GetByAccSenderUser(reserve.AdvertiseID,
                user_id > 0 ? user_id : userAccessor.CurrentUser.Id);
            var model = UserRatingDTO.Generate(reportItemService.GetAccUserRatings(
                reserve.AdvertiseID, user_id) as List<ReportItem>, acc.Id,
                acc.Title, regionService.Find(acc.City == null ? 0 : (int)acc.City).PersianName);
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
                accounting.GetReservePrizeAvailable(reserve.TotalPrice, guestUser.PrizeCredit),
                coupon == null ? 0 : accounting.CalculateDiscountCouponPrice(coupon.Percent, reserve.CouponCalculationPrice),
                coupon == null ? 0 : coupon.Id,
                accounting.GetReservePaidAmount(reserve.Id, Reserve.StatusStringType.Guest));
            return PartialView("_ReservePayment", model);
        }

        [Auth(UserRoles.Admin)]
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

        public PartialViewResult InvoiceItemPopup(long id)
        {
            var reserve = reserveService.Find(id);
            var acc = reserve.Advertise;
            if (acc.UserID != userAccessor.CurrentUser.Id)
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
            var hostUserId = reserve.Advertise.UserID;
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

        [Auth(UserRoles.Admin)]
        public JsonResult AddGuestPayment(long id, int type,
            int method, long price, long transactionId)
        {
            try
            {
                var reserve = reserveService.Find(id);
                if ((ReservePayment.ReservePaymentMethod)method == ReservePayment.ReservePaymentMethod.AmlakbashiCredit)
                {
                    long newCredit;
                    var creditTransactionId = accounting.DecreaseCredit(reserve.UserID, price, 0, reserve.Id, out newCredit,
                        Entities.User.CreditTransactionCause.Reserve, null,
                        0, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                    if (creditTransactionId < 1)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            msg = "موجودی کیف پول مهمان کافی نیست"
                        });
                    }
                    transactionId = creditTransactionId;
                }
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
                    errorMessage = "لطفا قیمت را وارد کنید.";
                }
                else if (reservePayment.TransactionID <= 0)
                {
                    invalidData = true;
                    errorMessage = "لطفا شماره تراکنش را وارد کنید.";
                }
                else if (accounting.ReservePaymentExists(reservePayment.TransactionID, reservePayment.PaymentMethod))
                {
                    invalidData = true;
                    errorMessage = "شماره تراکنش تکراری میباشد.";
                }
                if (invalidData)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = errorMessage
                    });
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
            if (code.ToLower() != "inst8" && code.ToLower() != "amb5")
            {
                return GenerateJsonResult(new { status = 0, msg = "کد وارد شده اشتباه است" });
            }
            var startDate = DateTime.Parse("10/28/2020");
            var identityUser = userService.GetIdentityUser(userAccessor.CurrentUser.MainMobile);
            if (identityUser.CreateDate.Value.Date < startDate.Date)
            {
                return GenerateJsonResult(new { status = 0, msg = "شما مجوز استفاده از کد تخفیف را ندارید" });
            }
            var discountCodeType = code.ToLower() == "amb5" ? DiscountCoupon.DiscountCouponType.Moupon :
                DiscountCoupon.DiscountCouponType.Instagram;
            var coupon = accounting.FindDiscountCoupon(userAccessor.CurrentUser.Id, discountCodeType);
            if (coupon == null)
            {
                coupon = accounting.InsertDiscountCoupon(userAccessor.CurrentUser.Id, discountCodeType, 5);
            }
            else
            {
                if (coupon.UsingReserveID > 0)
                {
                    return GenerateJsonResult(new { status = 0, msg = "این کد استفاده شده است" });
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
    }
}