using System;
using System.Collections.Generic;
using System.Linq;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.ReserveDTOs.ApiDTOs;
using static Amlakbashi.Core.Entities.Reserve;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : BaseController
    {
        public JsonResult CheckReserve(int advertise_id, string from_date, string to_date,
            int number_of_guests, string cid, string token)
        {
            var user = GetUser(token);
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var advertise = advertiseService.Find(advertise_id);
                if (number_of_guests < 1)
                {
                    return GenerateJsonResult(
                        new { status = 0, msg = "لطفا تعداد نفرات را وارد کنید" });
                }
                if (from_date == to_date)
                {
                    return GenerateJsonResult(
                            new { status = 0, msg = "تاریخ ورود و تاریخ خروج نمیتوانند یکی باشند" }
                        );
                }
                var days = DateTimeUtility.GetPersianDateRangeDays(from_date, to_date);
                if (advertise.MinReserveDays > 0 && days < advertise.MinReserveDays)
                {
                    return GenerateJsonResult(
                        new { status = 0, msg = "برای رزرو این اقامتگاه باید حداقل " + advertise.MinReserveDays + "  شب اقامت کنید. برای اقامت " + days + " شبه میتوانید اقامتگاه های دیگر را رزرو کنید." }
                    );
                }
                if (advertise.MaxReserveDays > 0 && days > advertise.MaxReserveDays)
                {
                    return GenerateJsonResult(
                        new { status = 0, msg = "شما میتوانید حداکثر " + advertise.MaxReserveDays + "  شب در این اقامتگاه اقامت کنید. برای اقامت طولانی تر میتوانید اقامتگاه های دیگر را رزرو کنید." }
                    );
                }
                var todayUnix = DateTimeUtility.DateValueOfJS(DateTime.Now.Date);
                if (advertise.unixNorouzMinRequestDate > todayUnix &&
                    DateTimeUtility.IsNorouz(DateTimeUtility.PersianDateRangeToList(from_date, to_date, true, false)))
                {
                    var minDateString = DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(advertise.unixNorouzMinRequestDate));
                    return GenerateJsonResult(
                        new { status = 0, msg = "برای رزرو نوروزی این اقامتگاه میتوانید از تاریخ " + minDateString + " اقدام کنید و یا اقامتگاه های دیگر را رزرو کنید." }
                    );
                }

                if (advertiseService.GetOccupiedDatesInRange(advertise_id, from_date, to_date).Any())
                {
                    return GenerateJsonResult(
                            new { status = 0, msg = "متاسفانه بعضی از روز های انتخاب شده پر هستند" }
                        );
                }
                long without_discount_price, couponCalPrice;
                var total_price = advertiseService.GetReservePrice(advertise_id, from_date, to_date, number_of_guests,
                    out without_discount_price, out couponCalPrice);
                long depositePrice;
                if (days > 3)
                {
                    depositePrice = (long)Math.Round(total_price * 0.3f);
                }
                else
                {
                    var deposite = (long)Math.Round((double)total_price / (double)days);
                    depositePrice = (long)(Math.Max(Math.Round(deposite / 1000f, 0), 1) * 1000);
                }
                if (advertise.Count < 1 &&
                    reserveService.UserHasSimilarReserve(user.Id, advertise_id,
                        DateTimeUtility.PersianDateToGregorian(from_date),
                        DateTimeUtility.PersianDateToGregorian(to_date)))
                {
                    return GenerateJsonResult(
                            new { status = 0, msg = "شما یک درخواست مشابه برای این آگهی دارید، برای درخواست جدید درخواست قبلی را لغو کنید" }
                        );
                }
                return GenerateJsonResult(
                    new { status = 1, price = total_price, withoutDiscountPrice = without_discount_price }
                );
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.CheckReserve", exc);
                return GenerateJsonResult(
                    new { status = 0, msg = "متاسفانه درخواست رزرو با خطا مواجه شد" }
                );
            }
        }

        public JsonResult ReserveRequest(int advertise_id, string from_date, string to_date,
            int number_of_guests, string cid, string token,
            int buildNumber = 0)
        {
            var user = GetUser(token);
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "برای درخواست رزرو ابتدا باید با حساب کاربری خود وارد شوید",
                    });
                }
                if (user.AccessType == (int)Entities.User.AccessTypeEnum.ReserveBanned || user.State == (int)Entities.User.AccessTypeEnum.LoginBanned)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "امکان درخواست رزرو برای شما مسدود شده است. جهت فعالسازی با پشتیبانی تماس بگیرید",
                    });
                }
                var checkResult = CheckReserve(advertise_id, from_date, to_date,
                    number_of_guests, cid, token);
                dynamic data = checkResult.Value;
                if (data.status == 0)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = data.msg,
                    });
                }
                var advertise = advertiseService.Find(advertise_id);
                bool instantReserve = false;
                instantReserve = advertise.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed;
                if (instantReserve)
                {
                    var formDateGregortian = DateTimeUtility.PersianDateToGregorian(from_date);
                    instantReserve = formDateGregortian <= DateTime.Now.AddDays(advertise.MaxInstantReserveStart).Date;
                }
                string msg;
                long reserveId;
                var done = advertiseService.ReserveRequest(advertise_id,
                    user.Id, from_date, to_date, number_of_guests,
                    instantReserve, out msg, out reserveId);
                return GenerateJsonResult(new
                {
                    status = done ? 1 : 0,
                    reserveId = reserveId,
                    instantReserve = instantReserve,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.ReserveRequest", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه درخواست رزرو با خطا مواجه شد",
                });
            }
        }

        public JsonResult ReserveResponse(string cid, string token, int reserve_id, int host_response)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            try
            {
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "برای پاسخ به درخواست رزرو ابتدا باید با حساب کاربری خود وارد شوید",
                    });
                }
                var reserve = reserveService.Find(reserve_id);
                if (reserve.Advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new { status = 0, msg = "شما میزبان این آگهی نیستید" });
                }
                reserveService.SetHostResponse(reserve_id, (Reserve.HostResponseEnum)host_response,
                    true, ActionLog.ActionSourceEnum.Application, user.Id);
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
                    status = 1,
                    msg = msg,
                    rejectReason = rejectReason
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.ReserveResponse", exc);
                return GenerateJsonResult(new { status = 0,
                    msg = "متاسفانه جواب درخواست رزرو با خطا مواجه شد" });
            }
        }

        public JsonResult ReserveHostOrGuest(string cid, string token)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { isHost = false, hasHostReserve = false, hasGuestReserve = false });
                }
                var user_id = user.Id;
                bool has_host_reserve;
                bool has_guest_reserve;
                reserveService.ExistHostGuest(user_id, out has_host_reserve, out has_guest_reserve);
                return GenerateJsonResult(new
                {
                    isHost = user.UserGeneralType > (int)Entities.User.UserGeneralTypeEnum.Guest,
                    hasHostReserve = has_host_reserve,
                    hasGuestReserve = has_guest_reserve,
                    profileImageId = user.PhotoStatus == (int)Entities.User.UserPhotoState.publish ? user.PhotoID : 0
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.ReserveHostOrGuest", exc);
                return GenerateJsonResult(new { isHost = false, hasHostReserve = false, hasGuestReserve = false, profileImageId = 0 });
            }
        }

        //categories: 0=waitforresponse 1=waitforpayment 2=reserved 3=finished 4=failed
        public JsonResult GetHostReserves(string cid, string token, int category)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { items = new List<ApiReserveItemDTO>() });
                }
                var all_reserves = reserveService.GetListByUserId(user.Id, category, true);
                List<ApiReserveItemDTO> dtoList = new List<ApiReserveItemDTO>();
                foreach (var item in all_reserves)
                {
                    ApiReserveItemDTO dto = item;
                    bool for_host = true;
                    var status = (Reserve.ReserveStatus)item.Status;
                    var host_user = userService.Find(item.Advertise.UserID);
                    var guest_user = userService.Find(item.UserID);
                    var call_available = status == Reserve.ReserveStatus.Reserved
                        || status == Reserve.ReserveStatus.CashPay
                        || status == Reserve.ReserveStatus.Started
                        || status == Reserve.ReserveStatus.CancelRequestByGuest
                        || status == Reserve.ReserveStatus.CancelRequestByHost;
                    dto.paidPrice = accounting.GetReservePaidAmount(item.ReservePayments.ToList(), Reserve.StatusStringType.Guest);
                    dto.partyMobile = !call_available ? "" : (for_host ?
                        guest_user.GetCallablePhoneNumber(Entities.User.PhoneType.OtherMobile1) :
                        host_user.GetCallablePhoneNumber(Entities.User.PhoneType.OtherMobile1));
                    dto.partyName = !call_available ? "" : (for_host ? guest_user.FullName : host_user.FullName);
                    dto.cancelAvailable = (!for_host && Reserve.CancelIsAvailableForGuest((int)item.Status)) ||
                        (for_host && Reserve.CancelIsAvailableForHost((int)item.Status));
                    dto.chatCount = chatService.GetNotReadCountByReserveId(item.Id, for_host ? host_user.Id : guest_user.Id);
                    dto.statusString = ReserveLocalization.GetStatusString((int)item.Status,
                        for_host ? Reserve.StatusStringType.Host : Reserve.StatusStringType.Guest, item.Id,
                        (Reserve.HostResponseEnum)item.HostResponse);
                    dtoList.Add(dto);
                }
                return GenerateJsonResult(new { items = dtoList });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.GetHostReserves", exc);
                return GenerateJsonResult(new { items = new List<ApiReserveItemDTO>() });
            }
        }

        //categories: 0=waitforresponse 1=waitforpayment 2=reserved 3=finished 4=failed
        public JsonResult GetGuestReserves(string cid, string token, int category)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { items = new List<ApiReserveItemDTO>() });
                }
                var all_reserves = reserveService.GetListByUserId(user.Id, category, false);
                List<ApiReserveItemDTO> dtoList = new List<ApiReserveItemDTO>();
                foreach (var item in all_reserves)
                {
                    ApiReserveItemDTO dto = item;
                    bool for_host = false;
                    var status = (Reserve.ReserveStatus)item.Status;
                    var host_user = userService.Find(item.Advertise.UserID);
                    var guest_user = userService.Find(item.UserID);
                    var call_available = status == Reserve.ReserveStatus.Reserved
                        || status == Reserve.ReserveStatus.CashPay
                        || status == Reserve.ReserveStatus.Started
                        || status == Reserve.ReserveStatus.CancelRequestByGuest
                        || status == Reserve.ReserveStatus.CancelRequestByHost;
                    dto.paidPrice = accounting.GetReservePaidAmount(item.ReservePayments.ToList(), Reserve.StatusStringType.Guest);
                    dto.partyMobile = !call_available ? "" : (for_host ?
                        guest_user.GetCallablePhoneNumber(Entities.User.PhoneType.OtherMobile1) :
                        host_user.GetCallablePhoneNumber(Entities.User.PhoneType.OtherMobile1));
                    dto.partyName = !call_available ? "" : (for_host ? guest_user.FullName : host_user.FullName);
                    dto.cancelAvailable = (!for_host && Reserve.CancelIsAvailableForGuest((int)item.Status)) ||
                        (for_host && Reserve.CancelIsAvailableForHost((int)item.Status));
                    dto.chatCount = chatService.GetNotReadCountByReserveId(item.Id, for_host ? host_user.Id : guest_user.Id);
                    dto.statusString = ReserveLocalization.GetStatusString((int)item.Status,
                        for_host ? Reserve.StatusStringType.Host : Reserve.StatusStringType.Guest, item.Id,
                        (Reserve.HostResponseEnum)item.HostResponse);
                    dtoList.Add(dto);
                }
                return GenerateJsonResult(new { items = dtoList });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.GetGuestReserves", exc);
                return GenerateJsonResult(new { items = new List<ApiReserveItemDTO>() });
            }
        }

        public JsonResult GetHostReserveBoxes(string cid, string token)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { });
                }
                var all_reserves = reserveService.GetListByUserId(user.Id, true).AsQueryable();
                List<long> reserve_ids = all_reserves.Select(x => x.Id).ToList();
                var all_chats = chatService.GetListAgainstUserId(user.Id, Chat.ChatStatusEnum.Sent,
                    Chat.ReadStatusEnum.NotRead, reserve_ids);
                IQueryable<Reserve> reserves;
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.WaitForResponse);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var waitForResponse = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = reserves.Count() > 0 ? new string[] { "برای پاسخ کلیک کنید" } : new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.WaitForReserve);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var waitForReserve = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = reserves.Count() > 0 ? new string[] { "منتظر پرداخت توسط مهمان باشید" } : new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.Reserved ||
                    x.Status == Reserve.ReserveStatus.Started ||
                    x.Status == Reserve.ReserveStatus.CashPay ||
                    x.Status == Reserve.ReserveStatus.CancelRequestByGuest ||
                    x.Status == Reserve.ReserveStatus.CancelRequestByHost);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var reservedItems = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.Completed);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var finishedItems = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.Rejected ||
                    x.Status == Reserve.ReserveStatus.CanceledByGuest ||
                    x.Status == Reserve.ReserveStatus.CanceledByHost ||
                    x.Status == Reserve.ReserveStatus.CanceledBySystem);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var failedItems = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = false,
                    infoMessages = new string[0]
                };
                return GenerateJsonResult(new
                {
                    waitForResponse = waitForResponse,
                    waitForReserve = waitForReserve,
                    reserved = reservedItems,
                    finished = finishedItems,
                    failed = failedItems
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.GetHostReserveBoxes", exc);
                return GenerateJsonResult(new { });
            }
        }

        public JsonResult GetGuestReserveBoxes(string cid, string token)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { });
                }
                var all_reserves = reserveService.GetListByUserId(user.Id, false).AsQueryable();
                var deleted_state = (int)Reserve.ReserveStatus.Deleted;
                var user_id = user.Id;
                all_reserves = all_reserves.Where(
                    x => x.UserID == user_id &&
                    (int)x.Status != deleted_state);
                List<long> reserve_ids = all_reserves.Select(x => x.Id).ToList();
                var all_chats = chatService.GetListAgainstUserId(user_id, Chat.ChatStatusEnum.Sent,
                    Chat.ReadStatusEnum.NotRead, reserve_ids);
                IQueryable<Reserve> reserves;
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.WaitForResponse);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var waitForResponse = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = reserves.Count() > 0 ? new string[] { "منتظر پاسخ میزبان باشید" } : new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.WaitForReserve);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var waitForReserve = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = reserves.Count() > 0 ? new string[] { "برای پرداخت و تکمیل رزرو کلیک کنید" } : new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.Reserved ||
                    x.Status == Reserve.ReserveStatus.Started ||
                    x.Status == Reserve.ReserveStatus.CashPay ||
                    x.Status == Reserve.ReserveStatus.CancelRequestByGuest ||
                    x.Status == Reserve.ReserveStatus.CancelRequestByHost);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var reservedItems = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.Completed);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var finishedItems = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = all_chats.Any(x => reserve_ids.Contains(x.ReserveID)),
                    infoMessages = new string[0]
                };
                reserves = all_reserves.Where(x => x.Status == Reserve.ReserveStatus.Rejected ||
                    x.Status == Reserve.ReserveStatus.CanceledByGuest ||
                    x.Status == Reserve.ReserveStatus.CanceledByHost ||
                    x.Status == Reserve.ReserveStatus.CanceledBySystem);
                reserve_ids = reserves.Select(x => x.Id).ToList();
                var failedItems = new ApiReserveBoxDTO()
                {
                    reserveIds = reserve_ids,
                    count = reserves.Count(),
                    hasChat = false,
                    infoMessages = new string[0]
                };
                return GenerateJsonResult(new
                {
                    waitForResponse = waitForResponse,
                    waitForReserve = waitForReserve,
                    reserved = reservedItems,
                    finished = finishedItems,
                    failed = failedItems
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.GetGuestReserveBoxes", exc);
                return GenerateJsonResult(new { });
            }
        }

        public JsonResult ReserveStartStay(string cid, string token, long reserve_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "شما مجوز این کار را ندارید" });
                }
                string msg;
                var started = reserveService.StartStay(reserve_id,
                    user.Id, out msg, ActionLog.ActionSourceEnum.Application,user.Id);
                return GenerateJsonResult(new
                {
                    status = started ? 1 : 0,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.ReserveStartStay", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید"
                });
            }
        }

        public JsonResult ReserveFinishStay(string cid, string token, long reserve_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "شما مجوز این کار را ندارید" });
                }
                string msg;
                var done = reserveService.FinishStay(reserve_id, user.Id, out msg, ActionLog.ActionSourceEnum.Application,
                    user.Id, true);
                return GenerateJsonResult(new
                {
                    status = done ? 1 : 0,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.ReserveFinishStay", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد. مجددا تلاش کنید"
                });
            }
        }

        public JsonResult CancelReserve(string cid, string token, long reserve_id, int cancel_reason_code, string cancel_reason_string, bool is_host)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var reserve = reserveService.Find(reserve_id);
                var advertise = reserve.Advertise;
                if (!(is_host && advertise.UserID == user.Id) &&
                    !(!is_host && reserve.UserID == user.Id))
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                string msg;
                bool isPending;
                reserveService.CancelReserve(user, reserve_id,
                    cancel_reason_code, cancel_reason_string, is_host, out msg, out isPending,
                    ActionLog.ActionSourceEnum.Application, user.Id);
                return GenerateJsonResult(new
                {
                    done = true,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.CancelReserve", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه خطایی رخ داده است. دوباره امتحان کنید"
                });
            }
        }

        public JsonResult RefuseCancelReserve(string cid, string token, long reserve_id, bool is_host)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var reserve = reserveService.Find(reserve_id);
                var advertsie = reserve.Advertise;
                if ((is_host ? advertsie.UserID != user.Id :
                    reserve.UserID != user.Id) ||
                    reserve.Status != (is_host ? ReserveStatus.CancelRequestByHost :
                    ReserveStatus.CancelRequestByGuest))
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                string msg;
                reserveService.RefuseCancelReserve(user, reserve_id, is_host,
                    out msg, ActionLog.ActionSourceEnum.Application, user.Id);
                return GenerateJsonResult(new
                {
                    done = true,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.RefuseCancelReserve", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه درخواست انصراف از لغو رزرو با خطا مواجه شد"
                });
            }
        }

        public ActionResult GuestCancelDiscussionMessages(string cid, string token, long reserve_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            var reserve = reserveService.Find(reserve_id);
            if (user.Id != reserve.UserID)
            {
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "شما مجوز این کار را ندارید"
                });
            }
            var messages = reserve.GetCancelDiscussionList();
            return GenerateJsonResult(new
            {
                done = true,
                messages = messages
            });
        }

        public ActionResult HostCancelDiscussionMessages(string cid, string token, long reserve_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            var reserve = reserveService.Find(reserve_id);
            if (user.Id != reserve.Advertise.UserID)
            {
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "شما مجوز این کار را ندارید"
                });
            }
            var messages = reserve.GetCancelDiscussionList();
            return GenerateJsonResult(new
            {
                done = true,
                messages = messages
            });
        }

        public JsonResult SendCancelDiscussionText(string cid, string token, long reserve_id, string text)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var reserve = reserveService.Find(reserve_id);
                var user_id = user.Id;
                var guest_user_id = reserve.UserID;
                var host_user_id = reserve.Advertise.UserID;
                var is_guest = user_id == guest_user_id;

                if (user_id != guest_user_id && user_id != host_user_id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                reserveService.UpdateCanselDiscussion(reserve.Id, text, user);
                return GenerateJsonResult(new
                {
                    done = true
                });
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.SendCancelDiscussionText", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult GetReservePaymentData(string cid, string token, long reserve_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "ابتدا با حساب کاربری خود وارد شوید" });
                }
                var reserve = reserveService.Find(reserve_id);
                if (reserve.UserID != user.Id)
                {
                    return GenerateJsonResult(new { status = 0, msg = "شما مجوز انجام این کار را ندارید" });
                }
                var coupon = accounting.GetMostValuableDiscountCouponIfAny(user.Id);
                long couponPrice = 0;
                if (coupon != null)
                {
                    couponPrice = accounting.CalculateDiscountCouponPrice(coupon.Percent, reserve.CouponCalculationPrice);
                }
                var data = new
                {
                    status = 1,
                    currentCredit = user.Credit,
                    couponPrice = couponPrice,
                    prizePrice = accounting.GetReservePrizeAvailable(reserve.TotalPrice, user.PrizeCredit),
                    couponCalculationPrice = reserve.CouponCalculationPrice
                };
                return GenerateJsonResult(data);
            }
            catch (Exception exc)
            {
                logger.Error("ReserveApi.GetReservePaymentData", exc);
                return GenerateJsonResult(new { status = 0, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }
    }
}

