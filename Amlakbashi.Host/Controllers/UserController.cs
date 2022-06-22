using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Entities;
using Entities = Amlakbashi.Core.Entities;
using log4net;
using Amlakbashi.Core.Common.Utilities;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Accounting;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using Amlakbashi.Host.Extensions;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Identity;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Amlakbashi.Core.Services.Interfaces;

namespace Amlakbashi.Host.Controllers
{
    public class UserController : BaseController
    {
        private readonly ILog logger;
        private readonly IBankCardAppService bankCardService;
        private readonly IUserAppService userService;
        private readonly IReserveAppService reserveService;
        private readonly IFileAppService fileService;
        private readonly IAccountingFacade accounting;
        private readonly IUserContactFacade userContact;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IUserAccessor userAccessor;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IWebHostEnvironment host;
        public UserController(IUserAppService userService,
            IBankCardAppService bankCardService,
            IAccountingFacade accounting,
            IUserContactFacade userContact,
            IReserveAppService reserveService,
            IFileAppService fileService,
            IAdvertiseAppService advertiseService,
            IUserAccessor userAccessor,
            SignInManager<AppUser> signInManager,
            IWebHostEnvironment host,
            ILog logger)
        {
            this.userService = userService;
            this.bankCardService = bankCardService;
            this.accounting = accounting;
            this.userContact = userContact;
            this.reserveService = reserveService;
            this.fileService = fileService;
            this.advertiseService = advertiseService;
            this.logger = logger;
            this.userAccessor = userAccessor;
            this.signInManager = signInManager;
            this.host = host;
        }

        [Authorize(Policy = Policies.User_Impersonate)]
        public IActionResult Impersonate(int userId, string url)
        {
            try
            {
                var user = userService.Find(userId, true);
                var identityUser = userService.GetIdentityUser(user.PhoneNumber);
                var admin = userAccessor.CurrentUser;

                var employeesNumber = userService.GetAllEmployees().Select(s => s.PhoneNumber).ToList();
                if (employeesNumber.Contains(identityUser.PhoneNumber))
                {
                    TempData["userIsEmployees"] = true;
                    return Redirect("/errors/accessdenied");
                }

                var expireTime = DateTime.Now.AddMinutes(30);
                var claims = new List<Claim>
                {
                    new Claim("ImpersonateAdminUsername", admin.PhoneNumber),
                    new Claim("Impersonate", "true"),
                    new Claim("ImpersonateExpireTime", expireTime.ToString())
                };
                var result = userService.AddClaimsToUser(user.PhoneNumber, claims);
                if (result == false)
                {
                    TempData["userIsImpersonated"] = true;
                    return Redirect("/errors/accessdenied");
                }

                AuthenticationProperties prop = new AuthenticationProperties();
                prop.ExpiresUtc = expireTime;
                prop.IssuedUtc = expireTime;
                prop.IsPersistent = true;

                userService.SignOut();
                signInManager.SignInAsync(identityUser, prop).Wait();

                HttpContext.Response.Cookies.Append(
                    ImpersonateData.ImpersonateCookieName,
                    ImpersonateData.ImpersonateCookieValue,
                    new CookieOptions()
                    {
                        Expires = expireTime,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Secure = true
                    }
                );

                logger.Info("Impersonation: " + admin.FullName + " (id:" + admin.Id + ") impersonate to " +
                    user.FullName + " (id:" + user.Id + ")");

                if (!string.IsNullOrEmpty(url))
                {
                    return Redirect(url);
                }
                return Redirect("/dashboard");
            }
            catch (Exception exc)
            {
                logger.Error("User.Impersonate", exc);
                return Redirect("/errors/http500");
            }
        }

        public IActionResult ImpersonateLogout()
        {
            try
            {
                if (User.IsImpersonatedUser() == false)
                {
                    return NotFound();
                }

                var adminUsername = User.GetImpersonatedAdminUsername();
                var admin = userService.GetIdentityUser(adminUsername);

                var claims = new List<Claim>
                {
                    new Claim("ImpersonateAdminUsername", adminUsername),
                    new Claim("Impersonate", "true"),
                    new Claim("ImpersonateExpireTime", User.GetImpersonateExpireTime())
                };
                userService.RemoveClaimsFromUser(User.Identity.Name, claims);
                HttpContext.Response.Cookies.Delete(ImpersonateData.ImpersonateCookieName);

                //signInManager.SignOutAsync().Wait();
                userService.SignOut();
                signInManager.SignInAsync(admin, true).Wait();
                HttpContext.Session.Clear();

                logger.Info("Impersonation: " + userAccessor.DoerUser.FullName + " (id:" + userAccessor.DoerUser.Id + ") exist from " +
                    userAccessor.CurrentUser.FullName + " (id:" + userAccessor.CurrentUser.Id + ")");

                return Redirect("/user/index");
            }
            catch (Exception exc)
            {
                logger.Error("User.Impersonate", exc);
                return Redirect("/errors/http500");
            }
        }

        #region [ admin ]

        [Authorize(Policy = Policies.Admin_General)]
        public ActionResult Admin()
        {
            return View();
        }

        [Authorize(Policy = Policies.Admin_General)]
        public ActionResult Index(int? page, string uname = "", int photo = -1,
            string username = "", string mobile = "", int code = -1, int ownership = -1, int sort_order = -1,
            int status = -1, int advertise_count = -1,
            int complete_profile_status = -1, int complete_profile_contact_status = -1,
            int user_general_type = -1,
            int userFilterType = -1, int card_status = -1, string minReserveNorouzFromDate = "",
            string Province = "-1", string City = "-1", string Area = "-1",
            string advertiseId = "-1")
        {
            try
            {
                IQueryable<User> model = userService.GetAllAsIQueryable();
                if (code > 0)
                    model = model.Where(u => u.Id == code);

                if (photo > -1)
                    model = model.Where(u => u.PhotoStatus == photo);

                if (ownership > -1)
                    model = model.Where(u => u.OwnerShip == ownership);

                if (!string.IsNullOrEmpty(username))
                {
                    if (username.Substring(0, 1) == "0")
                    {
                        username = username.Remove(0, 1);
                        username = username.Insert(0, "+98 ");
                    }
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    if (mobile.Substring(0, 1) == "0")
                    {
                        mobile = mobile.Remove(0, 1);
                        mobile = mobile.Insert(0, "+98 ");
                    }
                }
                if (!string.IsNullOrEmpty(uname))
                    model = model.Where(u => (u.FirstName + " " + u.LastName).Contains(uname));

                if (!string.IsNullOrEmpty(username))
                    model = model.Where(u => u.PhoneNumber.Contains(username));

                if (!string.IsNullOrEmpty(mobile))
                {
                    model = model.Where(u => u.PhoneNumber.Contains(mobile) || u.PhoneNumber2.Contains(mobile) ||
                        u.PhoneNumber3.Contains(mobile));
                }
                if (status != -1)
                {
                    var identityUserList = userService.GetAllIdentityUsernamesByState((Entities.User.UserState)status);
                    model = model.Where(x => identityUserList.Contains(x.PhoneNumber));
                }
                if (userFilterType > -1)
                {
                    switch ((Entities.User.UserFilterType)userFilterType)
                    {
                        case Entities.User.UserFilterType.Guest:
                            model = model.Where(x => x.Type == (int)Entities.User.UserGeneralTypeEnum.Guest);
                            break;
                        case Entities.User.UserFilterType.ActiveHost:
                            model = model.Where(x => x.Type == (int)Entities.User.UserGeneralTypeEnum.Host);
                            IQueryable<Advertise> allAdvertises = advertiseService.GetAllAsIQueriable();
                            var userIds = allAdvertises.Where(x => x.Status == AdvertiseStatus.Published).Select(x => x.UserID).Distinct().ToList();
                            model = model.Where(x => userIds.Contains(x.Id));
                            break;
                        case Entities.User.UserFilterType.Host:
                            model = model.Where(x => x.Type == (int)Entities.User.UserGeneralTypeEnum.Host);
                            break;
                        case Entities.User.UserFilterType.Staff:
                            var staffMobiles = userService.GetAllEmployees().Select(s => s.UserName).ToList();
                            model = model.Where(x => staffMobiles.Contains(x.PhoneNumber));
                            break;
                        case Entities.User.UserFilterType.InstantReserveRequest:
                            model = model.Where(x => x.InstantReserveAccess == Entities.User.InstantReserveAccessEnum.Requested);
                            break;
                        case Entities.User.UserFilterType.InstantReserveAllow:
                            model = model.Where(x => x.InstantReserveAccess == Entities.User.InstantReserveAccessEnum.Verified);
                            break;
                        case Entities.User.UserFilterType.PhotoChangeRequest:
                            model = model.Where(x => x.PhotoStatus == (int)Entities.User.UserPhotoState.ready_publish);
                            break;
                    }
                }
                if (user_general_type != -1)
                {
                    model = model.Where(x => x.Type == user_general_type);
                }
                if (complete_profile_contact_status != -1)
                {
                    if (complete_profile_contact_status == 0)
                    {
                        model = model.Where(x => string.IsNullOrEmpty(x.ContactPhone) || x.ContactPhone == "0");
                    }
                    else
                    {
                        model = model.Where(x => !string.IsNullOrEmpty(x.ContactPhone) && x.ContactPhone == complete_profile_contact_status.ToString());
                    }
                }
                if (complete_profile_status != -1)
                {
                    if (complete_profile_status == 0)
                    {
                        model = model.Where(x => string.IsNullOrEmpty(x.FirstName) ||
                                                 string.IsNullOrEmpty(x.LastName) ||
                                                 string.IsNullOrEmpty(x.ThirdPersonPhoneNumber));
                    }
                    else if (complete_profile_status == 1)
                    {
                        model = model.Where(x => !string.IsNullOrEmpty(x.FirstName) &&
                         !string.IsNullOrEmpty(x.LastName) &&
                         !string.IsNullOrEmpty(x.ThirdPersonPhoneNumber));
                    }
                }

                if (!string.IsNullOrEmpty(minReserveNorouzFromDate))
                {
                    var gregorianDate = DateTimeUtility.PersianDateToGregorian(
                        StringUtility.PersianNumberToEnglish(minReserveNorouzFromDate).Replace('/', ','));
                    var unixDate = DateTimeUtility.DateValueOfJS(gregorianDate);
                    IQueryable<Advertise> advertises = advertiseService.GetAllAsIQueriable();
                    advertises = advertises.Where(x => x.Status != AdvertiseStatus.Deleted);
                    advertises = advertises.Where(x => x.unixNorouzMinRequestDate >= unixDate);
                    var userIds = advertises.Select(x => x.UserID).Distinct().ToList();
                    model = model.Where(x => userIds.Contains(x.Id));
                }

                var province = int.Parse(Province);
                var city = int.Parse(City);
                var area = int.Parse(Area);

                if (area > -1 || city > -1 || province > -1)
                {
                    model = model.Where(x => x.Type == (int)Entities.User.UserGeneralTypeEnum.Host);
                    var adminMobiles = userService.GetAllEmployees().Select(s => s.PhoneNumber)
                        .Select(s => PhoneUtility.LocalNumberToInternational(s, 98)).ToList();
                    model = model.Where(x => !adminMobiles.Contains(x.PhoneNumber));
                    if (area > -1)
                    {
                        model = model.Where(w => w.Advertises.Any(
                            wa => wa.Status != AdvertiseStatus.Deleted &&
                            wa.Area == area));
                    }
                    else if (city > -1)
                    {
                        model = model.Where(w => w.Advertises.Any(
                            wa => wa.Status != AdvertiseStatus.Deleted &&
                            wa.City == city));
                    }
                    else if (province > -1)
                    {
                        model = model.Where(w => w.Advertises.Any(
                            wa => wa.Status != AdvertiseStatus.Deleted &&
                            wa.Province == province));
                    }
                }

                if (card_status > -1)
                {
                    IQueryable<BankCard> bankCards = bankCardService.GetAll();

                    if (card_status == 0) //shaba
                    {
                        var userIds = bankCards.Where(w => w.ShabaNumber != null && w.ShabaNumber != "")
                            .Select(s => s.UserID).ToList();
                        model = model.Where(w => userIds.Contains(w.Id));
                    }
                    else if (card_status == 1) // bank card
                    {
                        var userIds = bankCards.Where(w => (w.ShabaNumber == null || w.ShabaNumber == "") &&
                            (w.BankCardNumber != null && w.BankCardNumber != ""))
                            .Select(s => s.UserID).ToList();
                        model = model.Where(w => userIds.Contains(w.Id));
                    }
                    else if (card_status == 2) // none
                    {
                        var userIds = bankCards.Where(w => (w.ShabaNumber != null && w.ShabaNumber != "") ||
                            (w.BankCardNumber != null && w.BankCardNumber != ""))
                            .Select(s => s.UserID).ToList();
                        model = model.Where(w => !userIds.Contains(w.Id));
                    }
                }

                var accId = long.Parse(advertiseId);

                if (accId > 0)
                {
                    var advertise = advertiseService.Find(accId);
                    model = model.Where(x => x.Id == advertise.UserID);
                }

                if (advertise_count > 2)
                {
                    model = model.Where(w => w.Advertises.Count > 2);
                }
                else if (advertise_count > -1)
                {
                    model = model.Where(w => w.Advertises.Count == advertise_count);
                }

                model = model.OrderByDescending(u => u.UserScore);
                if (sort_order == 0)//By Advertise Count
                {
                    model = model.OrderByDescending(u => u.Advertises.Count);
                }
                else if (sort_order == 1)//By User Credit
                {
                    model = model.OrderByDescending(u => u.WalletAmount);
                }
                else if (sort_order == 2)//By No Response Reserves
                {
                    model = model.OrderByDescending(u => u.HostReserves.Count(r => r.HostResponse == 0));
                }
                else if (sort_order == 3)//By Rejected Reserves
                {
                    model = model.OrderByDescending(u => u.HostReserves.Count(r => r.Status == 0));
                }
                else if (sort_order == 4)//By Rejected For Home Full
                {
                    model = model.OrderByDescending(u => u.HostReserves.Count(r => (int)r.HostResponse == 4));
                }
                else if (sort_order == 5)//By Reserved
                {
                    model = model.OrderByDescending(u => u.HostReserves.Count(r => (int)r.Status >= 5 && (int)r.Status <= 8));
                }
                else if (sort_order == 6)//By Canceled Reserves
                {
                    model = model.OrderByDescending(u => u.HostReserves.Count(r => (int)r.Status >= 10 && (int)r.Status <= 12));
                }

                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);

                UserIndexDTO userListDTO = new UserIndexDTO()
                {
                    Code = code,
                    Mobile = mobile,
                    Uname = uname,
                    Photo = photo,
                    Username = username,
                    Ownership = ownership,
                    SortOrder = sort_order,
                    Status = status,
                    AdvertiseCount = advertise_count,
                    CompleteProfileStatus = complete_profile_status,
                    CompleteProfileContactStatus = complete_profile_contact_status,
                    UserGeneralType = user_general_type,
                    Province = province,
                    City = city,
                    Area = area,
                    AdvertiseId = accId,
                    UserFilterType = (Entities.User.UserFilterType)userFilterType,
                    CardStatus = card_status,
                    MinReserveNorouzFromDate = minReserveNorouzFromDate,
                    RowIndexStart = (PageNumber * 20) - 20,
                    UserItems = new List<UserIndexItemDTO>()
                };

                foreach (var item in onePageOfModel)
                {
                    UserIndexItemDTO itemDTO = new UserIndexItemDTO()
                    {
                        User = item,
                        BankCard = bankCardService.GetByUserId(item.Id),
                        InstantReserveCancel = advertiseService.GetInstantReserveCancelCount(item.Id),
                        State = userService.GetIdentityUser(item.PhoneNumber).State
                    };
                    userListDTO.UserItems.Add(itemDTO);
                }
                ViewBag.dto = userListDTO;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("User.Index", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.User_General_Edit)]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var user = userService.Find(id);
                var identityUser = userService.GetIdentityUser(user.PhoneNumber);
                var model = UserEditDTO.Generate(user, identityUser);
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("User.Edit(get)", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.User_General_Edit)]
        [HttpPost]
        public ActionResult Edit(UserEditDTO editedUser)
        {
            try
            {
                if (editedUser.IsValid())
                {
                    userService.Update(editedUser, userAccessor.DoerUser.Id);
                    return RedirectToAction(nameof(Edit), new { id = editedUser.Id });
                }
                return View(editedUser);
            }
            catch (Exception exc)
            {
                logger.Error("User.Edit(post)", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        public JsonResult Delete(int uid)
        {
            try
            {
                userService.Delete(uid, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("User.Delete", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            try
            {
                var identityUser = userService.GetIdentityUser(User.Identity.Name);
                ViewBag.hasPass = string.IsNullOrEmpty(identityUser.PasswordHash) ? false : true;
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                var identityUser = userService.GetIdentityUser(User.Identity.Name);
                ViewBag.hasPass = string.IsNullOrEmpty(identityUser.PasswordHash) ? false : true;
                if (string.IsNullOrEmpty(newPassword) || newPassword != confirmPassword)
                {
                    ViewBag.errors = new List<string>()
                    {
                        "لطفا رمز عبور و تکرار آن را به درستی وارد کنید"
                    };
                    return View();
                }
                var result = userService.ChangePassword(User.Identity.Name, currentPassword, newPassword);
                if (result.Succeeded)
                {
                    var user = userService.GetIdentityUser(User.Identity.Name);
                    userService.SignOut();
                    signInManager.SignInAsync(user, true).Wait();
                    TempData["suc"] = "تغییر رمز عبور با موفقیت انجام شد";
                    return Redirect("/user/profilemanager?userid=" + userAccessor.CurrentUser.Id);
                }
                var errorList = new List<string>();
                foreach (var item in result.Errors)
                {
                    errorList.Add(UserLocalization.GetIdentityPasswordErrorString(item.Code, item.Description));
                }
                ViewBag.errors = errorList;
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.User_Identity_Edit)]
        public JsonResult ChangeUserStatus(int uid, bool status)
        {
            try
            {
                userService.UpdateState(uid, status, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize(Policy = Policies.User_General_Edit)]
        public JsonResult changeCompleteProfileContactStatus(int uid, bool status)
        {
            try
            {
                userService.UpdateContactPhone(uid, status);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize(Policy = Policies.User_PFP_Edit)]
        public JsonResult PhotoPublish(int uid)
        {
            try
            {
                userService.UpdatePhotoStatus(uid, Entities.User.UserPhotoState.publish, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize(Policy = Policies.User_PFP_Edit)]
        public JsonResult PhotoDisapprove(int uid)
        {
            try
            {
                userService.UpdatePhotoStatus(uid, Entities.User.UserPhotoState.not_verified, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        #endregion

        #region [ Login ]

        [HttpGet]
        public ActionResult MobileLogin(string returnUrl = "/dashboard",
            string presentorCode = "")
        {
            if (!string.IsNullOrEmpty(presentorCode))
            {
                try
                {
                    var presentor = userService.Find(int.Parse(presentorCode));
                    ViewBag.presentorCode = presentorCode;
                    ViewBag.presentor = presentor;
                }
                catch { }
            }
            ViewBag.returnUrl = returnUrl;
            ViewBag.msg = TempData["msg"];
            return View();
        }

        public ActionResult P(string c)
        {
            return Redirect("/user/mobilelogin?returnUrl=/&presentorcode=" + c);
        }

        public JsonResult PopupLogin(string mobile = null, bool send_verification = true)
        {
            try
            {
                if (string.IsNullOrEmpty(mobile))
                {
                    return GenerateJsonResult(new { status = 0, msg = "شماره موبایل اجباری میباشد" });
                }
                var international_mobile = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var isNumberForIran = PhoneUtility.IsNumberForIran(international_mobile);
                var callableNumber = isNumberForIran ? PhoneUtility.InternationalNumberToLocal(international_mobile) :
                    PhoneUtility.InternationalNumberToCallable(international_mobile);
                var code = new Random().Next(1111, 9999).ToString();

                if (!PhoneUtility.ValidateInternationalNumber(international_mobile))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره موبایل اشتباه است"
                    });
                }

                var user = userService.GetByMainMobile(international_mobile);
                var identityUser = userService.GetIdentityUser(international_mobile);

                if (identityUser == null)
                {
                    user = new User()
                    {
                        PhoneNumber = international_mobile,
                        AmlakbashiScore = 1000,
                        NoticesPhoneNumber = Entities.User.NoticesPhoneNumberEnum.PhoneNumber
                    };
                    userService.Insert(user);
                    user = userService.GetByMainMobile(international_mobile);

                    identityUser = new AppUser()
                    {
                        UserName = international_mobile,
                        PhoneNumber = international_mobile,
                        CreateDate = DateTime.Now,
                        PhoneNumberConfirmed = false,
                        State = Entities.User.UserState.InActived,
                        IsForeigner = isNumberForIran == false
                    };
                    userService.AddIdentityUser(identityUser);

                    if (isNumberForIran)
                    {
                        userService.UpdateSendVerification(user.Id, DateTime.Now, code);
                        userService.SendVerificationSms(callableNumber, code);
                    }

                    return GenerateJsonResult(new
                    {
                        status = 1,
                        mobile = mobile,
                        isNew = true,
                        isNumberForIran = isNumberForIran
                    });
                }

                if (identityUser.State == Entities.User.UserState.Suspend)
                {
                    return GenerateJsonResult(new
                    {
                        status = 2,
                        mobile = mobile,
                        isNumberForIran = isNumberForIran
                    });
                }

                if (identityUser.PasswordHash != null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 3,
                        mobile = mobile,
                        isNumberForIran = isNumberForIran
                    });
                }

                if (isNumberForIran == false)
                {
                    if (identityUser.EmailConfirmed && identityUser.State == Entities.User.UserState.Acticved)
                    {
                        if (string.IsNullOrEmpty(identityUser.EmailCode) || identityUser.SendVerification == null ||
                            (DateTime.Now - identityUser.SendVerification) > new TimeSpan(0, 0, 5, 0, 0))
                        {
                            identityUser.EmailCode = new Random().Next(111111, 999999).ToString();
                            identityUser.SendVerification = DateTime.Now;
                            userService.UpdateIdentityUser(identityUser);
                        }
                        string strbody = $"<div style='direction:rtl;text-align:right;font-size:16px;'><div>کد تایید شما در املاک باشی: <b>{identityUser.EmailCode}</b></div></div>";
#if !DEBUG
                        EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { identityUser.Email },
                            "املاک باشی: کد تایید ورود به حساب کاربری", strbody);
#endif
                    }
                    return GenerateJsonResult(new
                    {
                        status = 5,
                        mobile = mobile,
                        isNumberForIran = isNumberForIran,
                        doLogin = identityUser.EmailConfirmed && identityUser.State == Entities.User.UserState.Acticved
                    });
                }

                if (identityUser.PhoneNumberConfirmed == false || identityUser.State == Entities.User.UserState.InActived)
                {
                    userService.UpdateSendVerification(user.Id, DateTime.Now, code);
                    userService.SendVerificationSms(callableNumber, code);

                    return GenerateJsonResult(new
                    {
                        status = 1,
                        mobile = mobile,
                        isNumberForIran = isNumberForIran
                    });
                }

                if (identityUser.CreateDate == null)
                {
                    userService.UpdateCreateDate(user.Id, DateTime.Now);
                }

                userService.UpdateSendVerification(user.Id, DateTime.Now, code);
                userService.SendVerificationSms(callableNumber, code);
                return GenerateJsonResult(new
                {
                    status = 4,
                    mobile = mobile,
                    isNumberForIran = isNumberForIran
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupLogin", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد: " + exc.Message
                });
            }
        }

        public IActionResult PopupLoginForeignNumberEmail(string mobile, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا آدرس ایمیل خود را وارد کنید" });
                }
                var mobileInternational = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(mobileInternational);
                if (identityUser != null)
                {
                    var code = new Random().Next(111111, 999999).ToString();
                    identityUser.EmailCode = code;
                    identityUser.Email = email;
                    identityUser.SendVerification = DateTime.Now;
                    identityUser.EmailConfirmed = false;
                    userService.UpdateIdentityUser(identityUser);
                    string strbody = $"<div style='direction:rtl;text-align:right;font-size:16px;'><div>کد تایید شما در املاک باشی: <b>{code}</b></div></div>";
#if !DEBUG
                EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { email },
                    "املاک باشی: تایید ایمیل ثبت نام", strbody);
#endif
                    return GenerateJsonResult(new { status = 1 });
                }
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupLoginForeignNumberEmail", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
        }

        public IActionResult PopupVerifyForeignNumberEmail(string mobile, string code, bool beLogin = false)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا کد ارسال شده به ایمیل خود را وارد کنید" });
                }
                var mobileInternational = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(mobileInternational);
                if (identityUser != null && identityUser.EmailCode == code)
                {
                    identityUser.EmailConfirmed = true;
                    identityUser.PhoneNumberConfirmed = true;
                    userService.UpdateIdentityUser(identityUser);
                    if (beLogin)
                    {
                        signInManager.SignInAsync(identityUser, true).Wait();
                    }
                    return GenerateJsonResult(new { status = 1 });
                }
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupLoginCode", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
        }

        public IActionResult PopupForgotPassForeignNumberEmail(string mobile)
        {
            try
            {
                var mobileInternational = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(mobileInternational);
                if (identityUser != null)
                {
                    if (string.IsNullOrEmpty(identityUser.EmailCode) || identityUser.SendVerification == null ||
                        (DateTime.Now - identityUser.SendVerification) > new TimeSpan(0, 0, 5, 0, 0))
                    {
                        identityUser.EmailCode = new Random().Next(111111, 999999).ToString();
                        identityUser.SendVerification = DateTime.Now;
                        userService.UpdateIdentityUser(identityUser);
                    }
                    string strbody = $"<div style='direction:rtl;text-align:right;font-size:16px'><div>کد تایید شما در املاک باشی: <b>{identityUser.EmailCode}</b></div></div>";
#if !DEBUG
                EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { identityUser.Email },
                    "املاک باشی: فراموشی رمز عبور", strbody);
#endif
                    return GenerateJsonResult(new { status = 1 });
                }
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupLoginForeignNumberEmail", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
        }

        public ActionResult ResendSms(string returnUrl)
        {
            HttpContext.Session.Remove("mobile");
            return RedirectToAction("MobileLogin", new { returnUrl = returnUrl });
        }

        public JsonResult PopupResendSms()
        {
            HttpContext.Session.Remove("mobile");
            ViewBag.msg = TempData["msg"];
            return GenerateJsonResult(new { status = 1 });
        }

        public JsonResult PopupSendSmsAgain(string mobile)
        {
            try
            {
                var mobile_international = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                if (!PhoneUtility.ValidateInternationalNumber(mobile_international))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره موبایل وارد شده صحیح نمی باشد"
                    });
                }
                var user = userService.GetByMainMobile(mobile_international);
                var identityUser = userService.GetIdentityUser(mobile_international);
                if (identityUser != null)
                {
                    if (identityUser.IsForeigner)
                    {
                        if (string.IsNullOrEmpty(identityUser.EmailCode) || identityUser.SendVerification == null ||
                        (DateTime.Now - identityUser.SendVerification) > new TimeSpan(0, 0, 5, 0, 0))
                        {
                            identityUser.EmailCode = new Random().Next(111111, 999999).ToString();
                            identityUser.SendVerification = DateTime.Now;
                            userService.UpdateIdentityUser(identityUser);
                        }
                        string strbody = $"<div style='direction:rtl;text-align:right;font-size:16px'><div>کد تایید شما در املاک باشی: <b>{identityUser.EmailCode}</b></div></div>";
#if !DEBUG
                        EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { identityUser.Email },
                            "املاک باشی: کد تایید ورود به حساب کاربری", strbody);
#endif
                    }
                    else
                    {
                        var code = identityUser.Code;
                        if (string.IsNullOrEmpty(code) ||
                            identityUser.SendVerification == null ||
                            (DateTime.Now - identityUser.SendVerification) > new TimeSpan(0, 0, 5, 0, 0))
                        {
                            code = new Random().Next(1111, 9999).ToString();
                            userService.UpdateSendVerification(user.Id, DateTime.Now, code);
                        }

                        var mobileNumber = PhoneUtility.IsNumberForIran(mobile_international) ?
                            PhoneUtility.InternationalNumberToLocal(mobile_international) :
                            PhoneUtility.InternationalNumberToCallable(mobile_international);
                        userService.SendVerificationSms(mobileNumber, code);
                        ViewBag.msg = TempData["msg"];
                        //return GenerateJsonResult(new { status = 1 });
                    }
                    return GenerateJsonResult(new { status = 1 });
                }
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "شماره موبایل وارد شده اشتباه است"
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupSendSmsAgain", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SaveNewPass(string mobile, string code, string password, string confirmPassword, bool verifyByEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(password) || password != confirmPassword)
                {
                    return GenerateJsonResult(new { status = 0, msg = "رمز عبور و تاییدیه آن را به درستی وارد کنید" });
                }
                var mobile_international = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(mobile_international);
                if (verifyByEmail)
                {
                    if (identityUser.EmailCode != code)
                    {
                        return GenerateJsonResult(
                        new
                        {
                            status = 0,
                            errors = "کد وارد شده صحیح نیست"
                        });
                    }
                }
                else
                {
                    if (identityUser.Code != code)
                    {
                        return GenerateJsonResult(
                        new
                        {
                            status = 0,
                            errors = "کد وارد شده صحیح نیست"
                        });
                    }
                }
                var result = userService.ChangeIdentityUserPassword(mobile_international, password);
                if (result.Succeeded)
                {
                    identityUser = userService.GetIdentityUser(mobile_international);
                    signInManager.SignInAsync(identityUser, true).Wait();
                    return GenerateJsonResult(new { status = 1 });
                }
                else
                {
                    var firstError = result.Errors.FirstOrDefault();
                    return GenerateJsonResult(new { status = 0, msg = UserLocalization.GetIdentityPasswordErrorString(firstError.Code, firstError.Description) });
                }
            }
            catch (Exception exc)
            {
                logger.Error("User.SaveNewPass", exc);
                return GenerateJsonResult(new { status = 0, msg = GeneralLocalization.GetExceptionMessage(exc) });
            }
        }

        public JsonResult PopupVerifyCode(string mobile, string code)
        {
            try
            {
                var mobile_international = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(mobile_international);
                var correct = identityUser != null && code == identityUser.Code;
                if (correct)
                {
                    identityUser.PhoneNumberConfirmed = true;
                    userService.UpdateIdentityUser(identityUser);
                }
                var user = userService.GetByMainMobile(mobile_international);
                return GenerateJsonResult(new
                {
                    status = 1,
                    correct = correct,
                    fname = user.FirstName,
                    lname = user.LastName
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupVerifyCode", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        public JsonResult PopupLoginCode(string mobile, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا کد ارسال شده به شماره خود را وارد کنید" });
                }
                var mobileInternational = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(mobileInternational);
                if (identityUser != null && identityUser.Code == code)
                {
                    signInManager.SignInAsync(identityUser, true).Wait();
                    return GenerateJsonResult(new { status = 1 });
                }
                return GenerateJsonResult(new { status = 0, msg = "کد وارد شده اشتباه است" });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupLoginCode", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
        }

        public JsonResult PopupLoginPass(string mobile, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا رمز خود را وارد کنید" });
                }
                var mobileInternational = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var result = signInManager.PasswordSignInAsync(mobileInternational, password, true, false).Result;
                if (result.Succeeded)
                {
                    return GenerateJsonResult(new { status = 1 });
                }
                return GenerateJsonResult(new { status = 0, msg = "رمز وارد شده اشتباه است" });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupLoginPass", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
        }

        public JsonResult PopupLoginRegister(string mobile, string code, string fname = null,
            string lname = null, string presentorCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(presentorCode) == false)
                {
                    presentorCode = StringUtility.PersianNumberToEnglish(presentorCode);
                }
                var mobile_international = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                int user_id;
                string errorMsg;
                var identityUser = userService.GetIdentityUser(mobile_international);
                if (identityUser != null && identityUser.Code == code)
                {
                    if (identityUser.State == Entities.User.UserState.Suspend)
                    {
                        return GenerateJsonResult(new { status = 0, msg = "حساب کاربری شما معلق شده است. لطفا با پشتیبان تماس بگیرید" });
                    }
                    var verify = userService.VerifyLogin(mobile_international, out user_id, presentorCode, out errorMsg);
                    if (verify == false)
                    {
                        return GenerateJsonResult(new { status = 0, msg = errorMsg });
                    }
                    if (identityUser.State == Entities.User.UserState.InActived)
                    {
                        Dictionary<string, string> errors;
                        if (userService.SignInRegister(user_id, fname, lname, out errors))
                        {
                            userService.UpdateState(user_id, true);
                        }
                        else
                        {
                            return GenerateJsonResult(new
                            {
                                status = 0,
                                msg = errors.First().Value
                            });
                        }
                    }
                    signInManager.SignInAsync(identityUser, true).Wait();
                    return GenerateJsonResult(new { status = 1 });
                }
                else
                {
                    return GenerateJsonResult(new { status = 0, msg = "کد وارد شده صحیح نیست" });
                }
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupLoginRegister", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = GeneralLocalization.GetExceptionMessage(exc)
                });
            }
        }

        public JsonResult PopupRegisterForeignNumber(string mobile, string code, string fname = null,
            string lname = null, string presentorCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(presentorCode) == false)
                {
                    presentorCode = StringUtility.PersianNumberToEnglish(presentorCode);
                }
                var mobile_international = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                int user_id;
                string errorMsg;
                var identityUser = userService.GetIdentityUser(mobile_international);
                if (identityUser != null && identityUser.EmailCode == code)
                {
                    if (identityUser.State == Entities.User.UserState.Suspend)
                    {
                        return GenerateJsonResult(new { status = 0, msg = "حساب کاربری شما معلق شده است. لطفا با پشتیبان تماس بگیرید" });
                    }
                    var verify = userService.VerifyLogin(mobile_international, out user_id, presentorCode, out errorMsg);
                    if (verify == false)
                    {
                        return GenerateJsonResult(new { status = 0, msg = errorMsg });
                    }
                    if (identityUser.State == Entities.User.UserState.InActived)
                    {
                        Dictionary<string, string> errors;
                        if (userService.SignInRegister(user_id, fname, lname, out errors))
                        {
                            userService.UpdateState(user_id, true);
                        }
                        else
                        {
                            return GenerateJsonResult(new
                            {
                                status = 0,
                                msg = errors.First().Value
                            });
                        }
                    }
                    signInManager.SignInAsync(identityUser, true).Wait();
                    return GenerateJsonResult(new { status = 1 });
                }
                else
                {
                    return GenerateJsonResult(new { status = 0, msg = "کد وارد شده صحیح نیست" });
                }
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupRegisterForeignNumber", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = GeneralLocalization.GetExceptionMessage(exc)
                });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult PopupRegisterEmail()
        {
            try
            {
                return PartialView("/Views/User/_PopupRegisterEmail.cshtml");
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult PopupRegisterEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || EmailUtility.ValidateEmail(email) == false)
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا آدرس ایمیل خود را به درستی وارد کنید" });
                }
                var identityUser = userService.GetIdentityUser(userAccessor.CurrentUser.PhoneNumber);
                var code = new Random().Next(111111, 999999).ToString();
                identityUser.EmailCode = code;
                identityUser.Email = email;
                identityUser.SendVerification = DateTime.Now;
                identityUser.EmailConfirmed = false;
                userService.UpdateIdentityUser(identityUser);
                string strbody = $"<div style='direction:rtl;text-align:right;font-size:16px;'><div>کد تایید شما در املاک باشی: <b>{code}</b></div></div>";
#if !DEBUG
                EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { email },
                    "املاک باشی: تایید ایمیل جدید", strbody);
#endif
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("User.PopupRegisterEmail", exc);
                return GenerateJsonResult(new { status = 0, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }

        [Authorize]
        public JsonResult PopupConfirmEmail(string emailCode)
        {
            try
            {
                var identityUser = userService.GetIdentityUser(userAccessor.CurrentUser.PhoneNumber);
                if (identityUser.EmailCode == emailCode)
                {
                    identityUser.EmailConfirmed = true;
                    userService.UpdateIdentityUser(identityUser);
                    return GenerateJsonResult(new { status = 1 });
                }
                return GenerateJsonResult(new { status = 0 });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [HttpGet]
        public ActionResult PublicLogin(string returnUrl = "/dashboard")
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.msg = TempData["msg"];
            return RedirectToAction("MobileLogin", new { returnUrl = returnUrl });
        }

        public ActionResult Signout()
        {
            userService.SignOut();
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        public ActionResult LogOff()
        {
            userService.SignOut();
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        // ???
        public JsonResult EditProfileMessageShown()
        {
            HttpContext.Session.SetBool("EditProfileMessageConfirmed", true);
            return GenerateJsonResult(new { status = 1 });
        }

        [HttpGet]
        [Authorize]
        public ActionResult UserCreditManager()
        {
            var user = userAccessor.CurrentUser;
            ViewBag.Credit = user.WalletAmount;
            ViewBag.UserID = user.Id;
            var model = accounting.GetCreditListByUserId(user.Id);
            return View(model);
        }

        public ActionResult ContactUserPopup(int user_id)
        {
            return PartialView("_UserContact", userService.Find(user_id));
        }

#endregion

        [Authorize]
        public JsonResult IncreaseCredit(long price, long? reserveId = null,
            long couponId = 0, long prizePrice = 0, long reservePrice = 0)
        {
            try
            {
                var payment = new Payment()
                {
                    UserID = userAccessor.CurrentUser.Id,
                    CreateDate = DateTime.Now,
                    Amount = price * 10,
                    ReserveID = reserveId,
                    CouponID = couponId,
                    PrizePrice = prizePrice,
                    ReservePrice = reservePrice,
                    ProductType = reserveId != null ? CreditTransaction.WalletTransactionTypeForPayment.Credit_Inc_Then_Res.ToString() :
                        CreditTransaction.WalletTransactionTypeForPayment.Credit_Increase.ToString()
                };
                accounting.InsertPayment(payment);
                return GenerateJsonResult(new { status = 1, pid = payment.Id });
            }
            catch (Exception exc)
            {
                logger.Error("User.IncreaseCredit", exc);
                return GenerateJsonResult(new { status = 0, pid = 0 });
            }
        }

        [Authorize(Policy = Policies.User_Credit)]
        public JsonResult AdminIncreaseCredit(int user_id, long amount, string transaction_cause, long transaction_id, bool send_sms = false)
        {
            try
            {
                if (user_id < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "نام کاربری اشتباه است" });
                }
                if (amount < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا مبلغ را وارد کنید" });
                }
                if (transaction_id < 1 && string.IsNullOrEmpty(transaction_cause))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "لطفا یکی از فیلدهای دلیل افزودن مبلغ و شماره تراکنش را پر کنید"
                    });
                }
                var cause = string.IsNullOrEmpty(transaction_cause) ? CreditTransaction.WalletTransactionReason.Charge : CreditTransaction.WalletTransactionReason.Other;
                long newCredit;
                var creditTransactionId = accounting.IncreaseCredit(user_id, amount, transaction_id, 0,
                    out newCredit, cause, transaction_cause, null, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);

                var price_string = string.Format("{0:n0}", amount) + " تومان";
                var new_credit_string = string.Format("{0:n0}", newCredit) + " تومان";
                if (send_sms)
                {
                    var user = userService.Find(user_id);
                    var identityUser = userService.GetIdentityUser(user.PhoneNumber);
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserMainMobile = user.GetNoticesPhoneNumber(),
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserEmail = identityUser.Email,
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        Type = UserContactType.UserCreditIncrease,
                        TransactionId = creditTransactionId.ToString(),
                        Price = amount.ToString(),
                        CauseString = CreditTransaction.GetCreditTransactionCauseString(cause, transaction_cause)
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "مبلغ " + price_string + " اضافه شد",
                    new_credit = new_credit_string
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.AdminIncreaseCredit", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.User_Credit)]
        public JsonResult AdminDecreaseCredit(int user_id, long amount, string transaction_cause, long transaction_id, bool send_sms = false)
        {
            try
            {
                if (user_id < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "نام کاربری اشتباه است" });
                }
                if (amount < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا مبلغ را وارد کنید" });
                }
                if (string.IsNullOrEmpty(transaction_cause))
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا دلیل کسر مبلغ را وارد کنید" });
                }
                var cause = Entities.CreditTransaction.WalletTransactionReason.Other;
                long newCredit;
                var creditTransactionId = accounting.DecreaseCredit(user_id,
                    amount, transaction_id, 0, out newCredit, cause, transaction_cause, 0,
                    userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                var price_string = string.Format("{0:n0}", amount) + " تومان";
                var new_credit_string = string.Format("{0:n0}", newCredit) + " تومان";
                if (send_sms)
                {
                    var user = userService.Find(user_id);
                    var identityUser = userService.GetIdentityUser(user.PhoneNumber);
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserMainMobile = user.GetNoticesPhoneNumber(),
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserEmail = identityUser.Email,
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        Type = UserContactType.UserCreditDecrease,
                        TransactionId = creditTransactionId.ToString(),
                        Price = Math.Abs(amount).ToString(),
                        CauseString = Entities.CreditTransaction.GetCreditTransactionCauseString(cause, transaction_cause)
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "مبلغ " + price_string + " کسر شد",
                    new_credit = new_credit_string
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }

        [Authorize]
        public JsonResult GetCurrentCredit()
        {
            try
            {
                var current_credit = userAccessor.CurrentUser.WalletAmount;
                return GenerateJsonResult(new { status = 1, current_credit = current_credit });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0, current_credit = 0 });
            }
        }

        public void SetFirstVisit()
        {
            HttpContext.Session.SetBool("first_visit", true);
        }

        [Authorize(Policy = Policies.Admin_General)]
        public JsonResult GetAllPhoneNumbers(int user_id)
        {
            var user = userService.Find(user_id);
            var main_mobile = PhoneUtility.NormalizePhoneNumber(
                user.GetPhoneNumber(Entities.User.PhoneType.MainMobile));
            var mobile_2 = PhoneUtility.NormalizePhoneNumber(
                user.GetPhoneNumber(Entities.User.PhoneType.OtherMobile1));
            var mobile_3 = PhoneUtility.NormalizePhoneNumber(
                user.GetPhoneNumber(Entities.User.PhoneType.OtherMobile2));
            var land_line = PhoneUtility.NormalizePhoneNumber(
                user.GetPhoneNumber(Entities.User.PhoneType.LandLine));
            var third_person = PhoneUtility.NormalizePhoneNumber(
                user.GetPhoneNumber(Entities.User.PhoneType.ThirdPerson));
            var full_name = user.FullName;
            if (string.IsNullOrEmpty(full_name))
            {
                full_name = "ثبت نشده";
            }
            return GenerateJsonResult(new
            {
                status = 1,
                main_mobile = main_mobile,
                mobile_2 = mobile_2,
                mobile_3 = mobile_3,
                land_line = land_line,
                third_person = third_person,
                full_name = full_name
            });
        }

        [Authorize(Roles = Roles.TechnicalManager + "," + Roles.TechnicalEmployee)]
        public void SendTestEmail()
        {
            try
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress("rasoul.shahhoseini@gmail.com"));
                message.From = new MailAddress("verification@amlakbashi.com");
                message.Subject = "Amlakbashi Email Verification";
                message.Body = "Please verify the Email you've entered.";
                message.IsBodyHtml = false;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "administrator",  // replace with valid value
                        Password = "@li#$%S0hR@b!@N98(8(0(*"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "mail.amlakbashi.com";
                    smtp.Port = 25;
                    smtp.EnableSsl = false;
                    smtp.Send(message);
                }
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
            }
        }

        public JsonResult IsUserAuthenticated()
        {
            return GenerateJsonResult(new
            {
                val = User.Identity.IsAuthenticated,
                impersonateData = new
                {
                    state = User.IsImpersonatedUser(),
                    fullName = User.IsImpersonatedUser() ? userAccessor.CurrentUser.FullName : ""
                }
            });
        }

        public JsonResult IsUserLoginBanned()
        {
            if (User.Identity.IsAuthenticated == false || string.IsNullOrEmpty(userAccessor.CurrentUser.PhoneNumber))
            {
                return GenerateJsonResult(new
                {
                    val = false
                });
            }
            var identityUser = userService.GetIdentityUser(userAccessor.CurrentUser.PhoneNumber);
            return GenerateJsonResult(
                new
                {
                    val = identityUser.State == Entities.User.UserState.Suspend,
                    user_id = userAccessor.CurrentUser.Id
                });
        }

        //public JsonResult LogoutAjax()
        //{
        //    try
        //    {
        //        //HttpContext.Session.SetString("mobile", null);
        //        //FormsAuthentication.SignOut();
        //        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //        HttpContext.Session.Clear();
        //        return GenerateJsonResult(new { status = 1 });
        //    }
        //    catch (Exception exc)
        //    {
        //        logger.Error("", exc);
        //        return GenerateJsonResult(new { status = 0 });
        //    }
        //}

        [Authorize]
        public JsonResult UpdateUserNotificationToken(string token)
        {
            try
            {
                userService.UpdateUserNotificationToken(userAccessor.CurrentUser.Id, token);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize]
        public JsonResult SetPermissionRequestDate(long ticks)
        {
            try
            {
                userService.UpdateLastNotifPermetionTicks(userAccessor.CurrentUser.Id, ticks);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize]
        public JsonResult GetPermissionRequestDate()
        {
            try
            {
                long ticks = userAccessor.CurrentUser.LastNotifPermitionTicks;
                return GenerateJsonResult(new { status = 1, ticks = ticks });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Roles = Roles.TechnicalManager + "," + Roles.TechnicalEmployee)]
        public void TestAppNotification(string target_action = "", string target_id = "0")
        {
            try
            {
                var user = userService.Find(1667);
                userService.SendNotificationApplication(user.FcmAppNotificationToken, "Test Title", "Test Body",
                    target_action, target_id);
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
            }
        }

        [Authorize]
        public JsonResult FetchUserId()
        {
            return GenerateJsonResult(new { status = 1, userId = userAccessor.CurrentUser.Id });
        }

        public ActionResult GetLoginPopup()
        {
            return PartialView("_LoginPopup");
        }

        [Authorize(Policy = Policies.User_Host_Support)]
        public ActionResult GetInstantReserveAccs(int userid)
        {
            var model = advertiseService.GetInstantReserveAdvertisesByUserId(userid, InstantReserveStatusEnum.Requested);
            var childrenIds = new List<long>();

            var allParents = model.Where(x => x.Childs.Any());
            foreach (var parent in allParents)
            {
                childrenIds.AddRange(parent.Childs.Select(x => x.Id));
            }
            model = model.Where(x => !childrenIds.Contains(x.Id)).ToList();
            ViewBag.userid = userid;
            return PartialView("_InstantReserveConfirm", model);
        }

        [Authorize(Policy = Policies.User_Host_Support)]
        public JsonResult ConfirmInstantReserve(long id)
        {
            try
            {
                var advertise = advertiseService.Find(id);
                advertiseService.UpdateInstantReserveStatus(advertise.UserID, InstantReserveStatusEnum.None, true);
                advertiseService.UpdateInstantReserveStatus(id, InstantReserveStatusEnum.Confirmed, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                userService.UpdateInstantReserveAccess(advertise.UserID, Entities.User.InstantReserveAccessEnum.Verified, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("User.ConfirmInstantReserve", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.User_Host_Support)]
        public JsonResult ConfirmAllInstantReserves(long userid)
        {
            try
            {
                var user = userService.Find((int)userid);
                advertiseService.UpdateInstantReserveStatus((int)userid, InstantReserveStatusEnum.Confirmed, true);
                userService.UpdateInstantReserveAccess(user.Id, Entities.User.InstantReserveAccessEnum.Verified, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("User.ConfirmAllInstantReserves", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.User_Host_Support)]
        public JsonResult CancellAllInstantReserves(long userid)
        {
            try
            {
                var user = userService.Find((int)userid);
                advertiseService.UpdateInstantReserveStatus((int)userid, InstantReserveStatusEnum.None, true);
                userService.UpdateInstantReserveAccess(user.Id, Entities.User.InstantReserveAccessEnum.None, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("User.CancellAllInstantReserves", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.User_Credit)]
        public JsonResult IncreasePrizeCreditCustom(int id, long amount, string title)
        {
            try
            {
                if (id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "کد کاربر انتخاب نشده است"
                    });
                }
                if (amount < 1)
                {
                    return GenerateJsonResult(new { status = 0, msg = "لطفا مبلغ را وارد کنید" });
                }
                if (string.IsNullOrEmpty(title))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "لطفا دلیل شارژ کیف هدیه را وارد کنید"
                    });
                }
                accounting.IncreasePrizeCredit(id, amount,
                    PrizeCreditTransaction.PrizeTransactionType.Custom,
                    0, title, userAccessor.CurrentUser.Id,
                    ActionLog.ActionSourceEnum.AdminPanel);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "کیف هدیه کد کاربر " + id + " مبلغ " + amount +
                    " به دلیل " + title + " شارژ شد."
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [HttpGet]
        public IActionResult ChangeDesc(int userId)
        {
            try
            {
                var user = userService.Find(userId);
                if (user != null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 1,
                        desc = string.IsNullOrEmpty(user.Description) ? "" : user.Description
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "این شناسه کاربری موجود نمی باشد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.GetUserDescription", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [HttpPost]
        public IActionResult ChangeDesc(int userId, string desc)
        {
            try
            {
                var user = userService.Find(userId);
                if (user != null)
                {
                    userService.UpdateDesc(userId, desc);
                    return GenerateJsonResult(new
                    {
                        status = 1,
                        msg = "توضیحات کاربر با موفقیت ویرایش شد"
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "این شناسه کاربری موجود نمی باشد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.GetUserDescription", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ProfileManager()
        {
            try
            {
                var user = userAccessor.CurrentUser;
                var identityUser = userService.GetIdentityUser(user.PhoneNumber);
                var model = UserDTO.Generate(user, identityUser);
                var bankCard = bankCardService.GetByUserId(user.Id);
                if (bankCard != null)
                {
                    model.bankCardNumber = bankCard.BankCardNumber;
                    model.shabaNumber = bankCard.ShabaNumber;
                    model.bankFname = bankCard.FName;
                    model.bankLname = bankCard.LName;
                }
                ViewBag.msg = TempData["msg"];
                ViewBag.suc = TempData["suc"];
                ViewBag.photoId = user.PhotoID;
                ViewBag.photoStatus = user.PhotoStatus;
                ViewBag.hasCanselReserve = reserveService.UserHasRefundInProgress(user.Id);
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("User.ProfileManager", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProfileManager(UserDTO user)
        {
            try
            {
                if (user.id != userAccessor.CurrentUser.Id)
                    return Redirect("/errors/http404");

                if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
                {
                    var uploadfile = Request.Form.Files[0];
                    var contentType = uploadfile.ContentType.ToLower();
                    if ((contentType == "image/png" ||
                        contentType == "image/gif" ||
                        contentType == "image/jpg" ||
                        contentType == "image/jpeg") == false)
                    {
                        TempData["msg"] = "فرمت عکس مورد قبول نمی باشد";
                        return RedirectToAction("profilemanager");
                    }

                    string filepath = $"~/content/users/user_{user.id}.jpg";
                    long PhotoID = 0;
                    if (userAccessor.CurrentUser.Photo != null)
                    {
                        PhotoID = userAccessor.CurrentUser.Photo.Id;
                        var oldFilePath = host.WebRootPath + userAccessor.CurrentUser.Photo.FilePath.Replace("~", "");
                        fileService.UpdateFilePath(PhotoID, filepath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    else
                    {
                        var newProfilePhoto = new File()
                        {
                            PostDate = DateTime.Now,
                            LastModifyDate = DateTime.Now,
                            UserID = user.id,
                            FilePath = filepath
                        };
                        PhotoID = fileService.Insert(newProfilePhoto);
                    }

                    if (!System.IO.Directory.Exists(host.WebRootPath + "/content/users"))
                        System.IO.Directory.CreateDirectory(host.WebRootPath + "/content/users");

                    using (var stream = System.IO.File.Create(host.WebRootPath + filepath.Replace("~", "")))
                    {
                        uploadfile.CopyTo(stream);
                    }

                    userService.UpdateProfilePhoto(user.id, PhotoID, Entities.User.UserPhotoState.ready_publish);

                    System.IO.DirectoryInfo IOdirectory = new System.IO.DirectoryInfo(System.IO.Path.Combine(host.WebRootPath, "content/imgcache"));
                    foreach (System.IO.FileInfo IOfile in IOdirectory.GetFiles())
                    {
                        IOfile.Delete();
                    }
                }
                List<string> errors;
                bool hasRefundInProgress = reserveService.UserHasRefundInProgress(user.id);
                var done = userService.Update(user, userAccessor.CurrentUser.Id, hasRefundInProgress, ActionLog.ActionSourceEnum.WebsiteDashboard, out errors);
                if (done)
                {
                    TempData["suc"] = "ویرایش پروفایل شما با موفقیت انجام شد";
                }
                else
                {
                    TempData["msg"] = errors.First();
                }

                return RedirectToAction("profilemanager");
            }
            catch (Exception exc)
            {
                logger.Error("User.ProfileManager", exc);
                return RedirectToAction("profilemanager");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangeMainPhoneNumber()
        {
            try
            {
                return View("_ChangeMainPhoneNumber");
            }
            catch (Exception exc)
            {
                logger.Error("User.ChangeMainPhoneNumber(get)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeMainPhoneNumber(string newMainPhoneNumber)
        {
            try
            {
                var result = await userService.UpdateMainPhoneNumberAsync(userAccessor.CurrentUser.Id, newMainPhoneNumber);
                if (result.HasError() == false && result.Result)
                {
                    var identityUser = await userService.FindIdentityByUsernameAsync(userAccessor.CurrentUser.PhoneNumber);
                    await signInManager.SignInAsync(identityUser, true);
                }
                return GenerateJsonResult(new
                {
                    status = result.HasError() ? 0 : result.Result ? 2 : 1,
                    msg = result.HasError() ? result.GetErrors().First() : null
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.ChangeMainPhoneNumber(post)", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VerifyChangeMainPhoneNumber(string verifyCode)
        {
            try
            {
                var result = await userService.VerifyNewMainPhoneNumber(userAccessor.CurrentUser.Id, verifyCode);
                if (result.HasError() == false)
                {
                    var identityUser = await userService.FindIdentityByUsernameAsync(userAccessor.CurrentUser.PhoneNumber);
                    await signInManager.SignInAsync(identityUser, true);
                }
                return GenerateJsonResult(new
                {
                    status = result.HasError() ? 0 : 1,
                    msg = result.HasError() ? result.GetErrors().First() : null
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.VerifyChangeMainPhoneNumber(post)", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.User_General_Edit)]
        [HttpGet]
        public ActionResult ChangeMainPhoneNumberByAdmin(int userId)
        {
            try
            {
                return View("_ChangeMainPhoneNumberByAdmin", userId);
            }
            catch (Exception exc)
            {
                logger.Error("User.ChangeMainPhoneNumberByAdmin(get)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.User_General_Edit)]
        [HttpPost]
        public async Task<IActionResult> ChangeMainPhoneNumberByAdmin(int userId, string newMainPhoneNumber)
        {
            try
            {
                var result = await userService.UpdateMainPhoneNumberAsync(userId, newMainPhoneNumber);
                return GenerateJsonResult(new
                {
                    status = result.HasError() ? 0 : result.Result ? 2 : 1,
                    msg = result.HasError() ? result.GetErrors().First() : null
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.ChangeMainPhoneNumberByAdmin(post)", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.User_General_Edit)]
        [HttpPost]
        public async Task<IActionResult> VerifyChangeMainPhoneNumberByAdmin(int userId, string verifyCode)
        {
            try
            {
                var result = await userService.VerifyNewMainPhoneNumber(userId, verifyCode);
                return GenerateJsonResult(new
                {
                    status = result.HasError() ? 0 : 1,
                    msg = result.HasError() ? result.GetErrors().First() : null
                });
            }
            catch (Exception exc)
            {
                logger.Error("User.VerifyChangeMainPhoneNumberByAdmin", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }
    }
}

