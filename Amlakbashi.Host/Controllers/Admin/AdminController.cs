using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Common.Utilities;
using log4net;
using Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.DTOs.PaymentDTOs.PaymentStatisticsDTOs;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;
using Amlakbashi.Core.Common.Caching;

namespace Portal.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ILog logger;
        private readonly IUserAppService userService;
        private readonly IAccountingFacade accounting;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IUserAccessor userAccessor;
        private readonly ICacheManager cacheManager;
        public AdminController(ILog logger,
            IAccountingFacade accounting,
            IAdvertiseAppService advertiseService,
            IUserAppService userService,
            IUserAccessor userAccessor,
            ICacheManager cacheManager)
        {
            this.logger = logger;
            this.userService = userService;
            this.advertiseService = advertiseService;
            this.accounting = accounting;
            this.userAccessor = userAccessor;
            this.cacheManager = cacheManager;
        }

        [Authorize(Policy = Policies.Admin_General)]
        public ActionResult Home()
        {
            return View();
        }

        [Authorize(Policy = Policies.Statistics_View)]
        public ActionResult UserChart(string type = "daily", int province = 0, int city = 0,
            int area = 0, int adtype = 0)
        {
            Dictionary<string, List<int>> model = new Dictionary<string, List<int>>();
            bool extra_filter = false;
            if (province > 0 || city > 0 || area > 0 || adtype > 0)
            {
                extra_filter = true;
            }
            List<int> user_list = advertiseService.FilterAdmin(province, city, area, adtype).Select(s => s.UserId).ToList();
            if (type == "monthly")
            {
                var toDate = DateTime.Today.AddDays(1);
                var fromDate = new DateTime(toDate.Year, toDate.Month, 1);
                List<int> MonthValue = new List<int>();
                for (int i = 0; i < 22; i++)
                {
                    int user_count = 0;
                    if (extra_filter)
                        user_count = userService.CountNewUserInDates(fromDate, toDate, user_list);
                    else
                        user_count = userService.CountNewUserInDates(fromDate, toDate);
                    MonthValue.Insert(0, user_count);
                    toDate = fromDate;
                    fromDate = fromDate.AddMonths(-1);
                }
                model.Add("all", MonthValue);
            }
            else if (type == "daily")
            {
                var toDate = DateTime.Today.AddDays(1);
                var fromDate = DateTime.Today;
                List<int> MonthValue = new List<int>();
                for (int i = 0; i < 40; i++)
                {
                    int user_count = 0;
                    if (extra_filter)
                        user_count = userService.CountNewUserInDates(fromDate, toDate, user_list);
                    else
                        user_count = userService.CountNewUserInDates(fromDate, toDate);

                    MonthValue.Insert(0, user_count);
                    toDate = fromDate;
                    fromDate = fromDate.AddDays(-1);
                }
                model.Add("all", MonthValue);
            }

            ViewBag.type = type;
            ViewBag.province = province;
            ViewBag.city = city;
            ViewBag.area = area;
            ViewBag.adtype = adtype;
            return View(model);
        }

        [Authorize(Policy = Policies.Statistics_View)]
        public ActionResult AdvertiseChart(string type = "daily", int province = 0, int userid = 0,
            int city = 0, int area = 0, int adtype = 0)
        {
            Dictionary<string, List<int>> model = new Dictionary<string, List<int>>();

            if (type == "monthly")
            {
                var toDate = DateTime.Today.AddDays(1);
                var fromDate = new DateTime(toDate.Year, toDate.Month, 1);
                List<int> DailyMonthValue = new List<int>();
                List<int> RentMonthValue = new List<int>();
                List<int> SaleMonthValue = new List<int>();
                for (int i = 0; i < 22; i++)
                {
                    var query = advertiseService.FilterAdmin(province, city, area, adtype, fromDate, toDate, userid);
                    int daily_count = query.Count();
                    int rent_count = 0;
                    int sale_count = 0;
                    DailyMonthValue.Insert(0, daily_count);
                    RentMonthValue.Insert(0, rent_count);
                    SaleMonthValue.Insert(0, sale_count);
                    toDate = fromDate;
                    fromDate = fromDate.AddMonths(-1);
                }
                model.Add("daily", DailyMonthValue);
                model.Add("sale", SaleMonthValue);
                model.Add("rent", RentMonthValue);
            }
            else if (type == "daily")
            {
                var toDate = DateTime.Today.AddDays(1);
                var fromDate = DateTime.Today;
                List<int> DailyMonthValue = new List<int>();
                List<int> RentMonthValue = new List<int>();
                List<int> SaleMonthValue = new List<int>();
                for (int i = 0; i < 40; i++)
                {
                    var query = advertiseService.FilterAdmin(province, city, area, adtype, fromDate, toDate, userid);
                    int daily_count = query.Count();
                    int rent_count = 0;
                    int sale_count = 0;
                    DailyMonthValue.Insert(0, daily_count);
                    RentMonthValue.Insert(0, rent_count);
                    SaleMonthValue.Insert(0, sale_count);
                    toDate = fromDate;
                    fromDate = fromDate.AddDays(-1);
                }
                model.Add("daily", DailyMonthValue);
                model.Add("sale", SaleMonthValue);
                model.Add("rent", RentMonthValue);
            }

            ViewBag.type = type;
            ViewBag.province = province;
            ViewBag.city = city;
            ViewBag.area = area;
            ViewBag.adtype = adtype;
            ViewBag.userid = userid;
            ViewBag.user = userService.Find(userid);
            return View(model);
        }

        [Authorize(Policy = Policies.Statistics_View)]
        public ActionResult AdminStatistic()
        {
            return View();
        }


        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public ActionResult Email()
        {
            try
            {
                ViewBag.suc = TempData["suc"];
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Admin.Email", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public ActionResult Sms()
        {
            try
            {
                ViewBag.suc = TempData["suc"];
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Admin.Sms", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public JsonResult SendSms(string template, int recieverUserID = 0,
            int ownership = 0, int province = 0, int city = 0, int area = 0,
            int adtype = 0, int tradeid = 0, int special = 0, int adstatus = 0,
            int userType = -1, bool confirmRequired = true, int norouzPriceStatus = 0)
        {
            try
            {
                IQueryable<User> recievers;
                if (recieverUserID > 0)
                {
                    recievers = userService.GetAllById(recieverUserID);
                }
                else
                {
                    recievers = userService.GetAllAsIQueryable();
                    if (ownership > 0)
                    {
                        recievers = recievers.Where(x => x.OwnerShip == ownership);
                    }
                    if (province > 0 || city > 0 || area > 0 || adtype > 0 ||
                        adstatus > -1 || tradeid > 0 || special > 0)
                    {
                        var advertises = advertiseService.FilterAdmin(province, city, area, adtype, false, adstatus);
                        var advertiseUserIds = advertises.Select(s => s.UserId);
                        recievers = recievers.Where(w => advertiseUserIds.Contains(w.Id));
                    }
                    if (userType > -1)
                    {
                        recievers = recievers.Where(w => w.Type == userType);
                    }
                    if (norouzPriceStatus == 1)
                    {
                        recievers = recievers.Where(w => w.Type == 1 &&
                            w.Advertises.Any(a => a.NowruzPrice == 0) == false);
                    }
                    else if (norouzPriceStatus == 2)
                    {
                        recievers = recievers.Where(w => w.Type == 1 &&
                            w.Advertises.Any(a => a.NowruzPrice == 0));
                    }
                }

                if (template == "SetNorouzPrice")
                {
                    recievers = recievers.Where(w => w.Type == 1);
                }

                if (confirmRequired)
                {
                    var users_count = recievers.Count();
                    return GenerateJsonResult(new { status = 1, usersCount = users_count });
                }
                else
                {
                    var phoneNumbers = recievers.Select(x => x.PhoneNumber).ToList();
                    phoneNumbers = phoneNumbers.Where(w => PhoneUtility.IsNumberForIran(w)).ToList();
                    if (template == "SetNorouzPrice")
                    {
                        var receiverUsers = recievers.ToList();
                        foreach (var recieverUser in receiverUsers)
                        {
                            userService.SendSms(new Amlakbashi.Core.Infrastructure.UserContact.UserContactDTO()
                            {
                                UserMainMobile = recieverUser.PhoneNumber,
                                Type = Amlakbashi.Core.Infrastructure.UserContact.UserContactType.HostUpdatePrice,
                                Extra1 = !string.IsNullOrEmpty(recieverUser.LastName) ? recieverUser.LastName : "-"
                            });
                        }
                        return GenerateJsonResult(new { status = 2, message = "" });
                    }
                    if (template == "GuestNorouzRules")
                    {
                        var receiverUsers = recievers.ToList();
                        foreach (var number in phoneNumbers)
                        {
                            userService.SendSms(new Amlakbashi.Core.Infrastructure.UserContact.UserContactDTO()
                            {
                                UserMainMobile = number,
                                Type = Amlakbashi.Core.Infrastructure.UserContact.UserContactType.GuestNorouzRules,
                                Extra1 = ""
                            });
                        }
                        return GenerateJsonResult(new { status = 2, message = "" });
                    }
                    var delay = 1;
                    foreach (var mobile in phoneNumbers)
                    {
                        userService.SendCustomSms(delay, mobile, template);
                        delay++;
                    }
                    return GenerateJsonResult(new { status = 2, message = "" });
                }
            }
            catch (Exception exc)
            {
                logger.Error("Admin.SendSms", exc);
                return GenerateJsonResult(new { status = 0, message = exc.Message });
            }
        }

        [Authorize(Policy = Policies.Statistics_View)]
        public ActionResult PaymentChart(string type = "daily", int province = 0, int city = 0,
            int area = 0, int adtype = 0)
        {
            try
            {
                Dictionary<string, List<long>> model = new Dictionary<string, List<long>>();
                bool extra_filter = false;
                if (province > 0 || city > 0 || area > 0 || (adtype > 0 && adtype != (int)Advertise.AdvertiseType.All))
                {
                    extra_filter = true;
                }
                List<int> user_list = advertiseService.FilterAdmin(province, city, area, adtype).Select(s => s.UserId).ToList();
                if (type == "monthly")
                {
                    var toDate = DateTime.Today.AddDays(1);
                    var fromDate = new DateTime(toDate.Year, toDate.Month, 1);
                    List<long> CountMonthValue = new List<long>();
                    List<long> AmountMonthValue = new List<long>();
                    for (int i = 0; i < 22; i++)
                    {
                        int payment_count = 0;
                        long payment_amount = 0;
                        var payments = accounting.GetPaymentRange(fromDate, toDate, 1, user_list);
                        payment_count = payments.Count;
                        payment_amount = payments.Select(s => (long?)s.Amount).Sum() ?? 0;
                        CountMonthValue.Insert(0, payment_count);
                        AmountMonthValue.Insert(0, payment_amount);
                        toDate = fromDate;
                        fromDate = fromDate.AddMonths(-1);
                    }
                    model.Add("amount", AmountMonthValue);
                    model.Add("count", CountMonthValue);
                }
                else if (type == "daily")
                {
                    var toDate = DateTime.Today.AddDays(1);
                    var fromDate = DateTime.Today;
                    List<long> AmountMonthValue1 = new List<long>();
                    List<long> AmountMonthValue2 = new List<long>();
                    for (int i = 0; i < 31; i++)
                    {
                        long payment_amount1 = 0;
                        long payment_amount2 = 0;

                        if (extra_filter)
                        {
                            payment_amount1 = accounting.GetPaymentRange(fromDate, toDate, 1, user_list).
                                Select(p => (long?)p.Amount).Sum() ?? 0;
                        }
                        else
                        {
                            payment_amount1 = accounting.GetPaymentRange(fromDate, toDate, 1, null, true).
                                Select(p => (long?)p.Amount).Sum() ?? 0;
                        }

                        var toDate2 = toDate.AddMonths(-1);
                        var fromDate2 = fromDate.AddMonths(-1);

                        if (extra_filter)
                        {
                            payment_amount2 = accounting.GetPaymentRange(fromDate2, toDate2, 1, user_list).
                                Select(p => (long?)p.Amount).Sum() ?? 0;
                        }
                        else
                        {
                            payment_amount2 = accounting.GetPaymentRange(fromDate2, toDate2, 1, null, true).
                                Select(p => (long?)p.Amount).Sum() ?? 0;

                        }

                        AmountMonthValue1.Insert(0, payment_amount1);
                        AmountMonthValue2.Insert(0, payment_amount2);

                        toDate = fromDate;
                        fromDate = fromDate.AddDays(-1);
                    }
                    model.Add("current month", AmountMonthValue1);
                    model.Add("previous month", AmountMonthValue2);
                }

                ViewBag.type = type;
                ViewBag.province = province;
                ViewBag.city = city;
                ViewBag.area = area;
                ViewBag.adtype = adtype;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Admin.PaymentChart", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Statistics_View)]
        public ActionResult PaymentComparisonChart(int province = 0, int city = 0,
            int area = 0, int adtype = 0, int first_year = 0, int first_month = 0,
            int second_year = 0, int second_month = 0)
        {
            try
            {
                bool extra_filter = area > 0 || city > 0 || province > 0 || (adtype > 0 && adtype != (int)Advertise.AdvertiseType.All);
                List<int> user_list = null;
                if (extra_filter)
                {
                    var query = advertiseService.FilterAdmin(province, city, area, adtype);
                    user_list = query.Select(x => x.UserId).ToList();
                }

                if (first_year <= 0)
                {
                    DateTimeUtility.GetCurrentPersianMonth(out first_year, out first_month);
                    DateTimeUtility.GetPreviousPersianMonth(out second_year, out second_month);
                }

                var model = new Dictionary<string, PaymentChartDTO>();
                var first_month_statistic = accounting.GeneratePaymentChart(first_year, first_month, extra_filter, user_list);
                var second_month_statistic = accounting.GeneratePaymentChart(second_year, second_month, extra_filter, user_list);
                var first_month_key = string.Format("{0} {1}", DateTimeUtility.GetPersianMonthName(first_month), first_year);
                var second_month_key = string.Format("{0} {1}", DateTimeUtility.GetPersianMonthName(second_month), second_year);
                model.Add(first_month_key, first_month_statistic);
                model.Add(second_month_key, second_month_statistic);

                ViewBag.province = province;
                ViewBag.city = city;
                ViewBag.area = area;
                ViewBag.adtype = adtype;
                ViewBag.first_year = first_year;
                ViewBag.first_month = first_month;
                ViewBag.second_year = second_year;
                ViewBag.second_month = second_month;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Admin.PaymentComparisonChart", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }


        [Authorize(Policy = Policies.Statistics_View)]
        public ActionResult ReserveFinanceChart(int selected_year = 0,
            int selected_month = 0)
        {
            try
            {
                if (selected_year <= 0)
                {
                    DateTimeUtility.GetCurrentPersianMonth(out selected_year, out selected_month);
                }

                var model = new Dictionary<string, PaymentChartDTO>();
                PaymentChartDTO total_price_statistic,
                    site_portion_statistic, host_creditor_statistic;
                accounting.GenerateReserveFinanceChart(selected_year,
                    selected_month, out total_price_statistic,
                    out site_portion_statistic, out host_creditor_statistic);
                var total_price_key = string.Format("Total Reserve {0} {1}", DateTimeUtility.GetPersianMonthName(selected_month), selected_year);
                var site_portion_key = string.Format("Site Portion {0} {1}", DateTimeUtility.GetPersianMonthName(selected_month), selected_year);
                var host_creditor_key = string.Format("Host Creditor {0} {1}", DateTimeUtility.GetPersianMonthName(selected_month), selected_year);
                model.Add(total_price_key, total_price_statistic);
                model.Add(site_portion_key, site_portion_statistic);
                model.Add(host_creditor_key, host_creditor_statistic);
                ViewBag.selected_year = selected_year;
                ViewBag.selected_month = selected_month;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Admin.ReserveFinanceChart", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public ActionResult SendGroupNotification()
        {
            ViewBag.msg = "";
            ViewBag.user_type = 1;
            ViewBag.province = -1;
            ViewBag.city = -1;
            ViewBag.area = -1;
            return View();
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        [HttpPost]
        public ActionResult SendGroupNotification(string title, string body, string click_action,
            int user_type = 1, int province = -1, int city = -1, int area = -1)
        {
            ViewBag.user_type = user_type;
            ViewBag.province = province;
            ViewBag.city = city;
            ViewBag.area = area;
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(body) || string.IsNullOrEmpty(click_action))
            {
                ViewBag.msg = "لطفا همه موارد را پر کنید";
                return View();
            }
            string msg = "";
            try
            {
                IQueryable<User> users = userService.GetAllAsIQueryable();
                var identityUserList = userService.GetAllIdentityUsernamesByState();
                var userList = users.Where(x => x.NotificationToken != null).ToList();
                userList = userList.Where(w => identityUserList.Contains(w.PhoneNumber)).ToList();
                switch (user_type)
                {
                    case 0:
                        userList = userList.Where(x => x.Type == 0).ToList();
                        break;
                    case 1:
                        userList = userList.Where(x => x.Type > 0).ToList();
                        var userIds = advertiseService.FilterAdmin(province, city, area).Select(s => s.UserId).Distinct().ToList();
                        userList = userList.Where(w => userIds.Contains(w.Id)).ToList();
                        break;
                }
                var tokens = userList.Select(x => x.NotificationToken).ToList();
                userService.SendGroupNotification(tokens, title, body, click_action);
                msg = string.Format("نوتیفیکیشن به {0} کاربر ارسال شد", tokens.Count());
            }
            catch (Exception exc)
            {
                logger.Error("Admin.SendGroupNotification", exc);
                msg = "خطای سیستم";
            }
            ViewBag.msg = msg;
            return View();
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public ActionResult SendWhatsappCoronaAdvMsg()
        {
            return View();
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public JsonResult GetWaCoronaAdvMsgs()
        {
            try
            {
                var result = GetWaCoronaAdvData();
                return GenerateJsonResult(new { status = 1, val = result });
            }
            catch (Exception exc)
            {
                logger.Error("GetWaCoronaAdvMsgs", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطای فنی مواجه شد" });
            }
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public JsonResult GetWaCoronaAdvTestMsgs()
        {
            try
            {
                var result = GetWaCoronaAdvTestData();
                return GenerateJsonResult(new { status = 1, val = result });
            }
            catch (Exception exc)
            {
                logger.Error("GetWaCoronaAdvTestMsgs", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطای فنی مواجه شد" });
            }
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public IActionResult CacheManagement()
        {
            return View();
        }

        [Authorize(Policy = Policies.Send_Message_To_Users)]
        public JsonResult ClearCache()
        {
            try
            {
                cacheManager.Clear();
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Admin.ClearCache", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        private WaCoronaAdvMsgHelper[] GetWaCoronaAdvData()
        {
            var identityUsers = userService.GetAllIdentityUsernamesByState();
            IQueryable<User> users = userService.GetAllAsIQueryable();
            users = users.Where(x => x.PhoneNumber != null && x.PhoneNumber.Length > 0 && x.PhoneNumber.StartsWith("+"));
            users = users.OrderByDescending(x => identityUsers.Contains(x.PhoneNumber)).
                ThenByDescending(x => x.Id);
            var result = new List<WaCoronaAdvMsgHelper>();
            foreach (var user in users)
            {
                var mobile = user.PhoneNumber.Replace("+98 ", "98");
                var userName = !string.IsNullOrEmpty(user.FirstName) ?
                            (user.FirstName + " عزیز، کاربر املاک باشی") :
                            (
                                !string.IsNullOrEmpty(user.LastName) ?
                                    (user.LastName + " عزیز، کاربر املاک باشی") :
                                    "کاربر عزیز املاک باشی"
                            );
                result.Add(new WaCoronaAdvMsgHelper() { mobile = mobile, userName = userName });
            }
            return result.ToArray();
        }

        private WaCoronaAdvMsgHelper[] GetWaCoronaAdvTestData()
        {
            var user1 = userService.Find(3);
            var user2 = userService.Find(17244);
            return new WaCoronaAdvMsgHelper[] {
                    new WaCoronaAdvMsgHelper() {
                        mobile = "989121197156",
                        userName = !string.IsNullOrEmpty(user1.FirstName) ?
                            (user1.FirstName + " عزیز، کاربر املاک باشی") :
                            (
                                !string.IsNullOrEmpty(user1.LastName) ?
                                    (user1.LastName + " عزیز، کاربر املاک باشی") :
                                    "کاربر عزیز املاک باشی"
                            )
                    },
                    new WaCoronaAdvMsgHelper() {
                        mobile = "989212085439",
                        userName = !string.IsNullOrEmpty(user2.FirstName) ?
                            (user2.FirstName + " عزیز، کاربر املاک باشی") :
                            (
                                !string.IsNullOrEmpty(user2.LastName) ?
                                    (user2.LastName + " عزیز، کاربر املاک باشی") :
                                    "کاربر عزیز املاک باشی"
                            )
                    }
            };
        }

        [System.Serializable]
        public class WaCoronaAdvMsgHelper
        {
            public string mobile { get; set; }
            public string userName { get; set; }
        }
    }
}
