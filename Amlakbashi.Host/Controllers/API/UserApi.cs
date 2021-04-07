using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Entities;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Amlakbashi.Data.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : BaseController
    {
        public JsonResult SignInFirstStep(string mobile, string cid, bool fcm_notification = false)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var correct = PhoneUtility.ValidateLocalNumber(mobile) ||
                    (mobile.Length > 10 &&
                        (mobile.Substring(0, 1) == "+" || mobile.Substring(0, 2) == "00"));
                if (!correct)
                {
                    return GenerateJsonResult(new
                    {
                        status = (int)Entities.User.SignInFirstStepStatus.Error,
                        msg = "شماره موبایل وارد شده صحیح نیست. لطفا پس از بررسی مجدد شماره درست را وارد کنید."
                    });
                }
                var international_mobile = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var number_is_for_iran = PhoneUtility.IsNumberForIran(international_mobile);
                var user = userService.GetActivatedUserByMainMobile(international_mobile);
                var identityUser = userService.GetActivatedIdentityUser(international_mobile);
                if (user != null && user.AccessType == (int)Entities.User.AccessTypeEnum.LoginBanned)
                {
                    return GenerateJsonResult(new
                    {
                        status = (int)Entities.User.SignInFirstStepStatus.Error,
                        msg = "امکان ورود به سایت برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید"
                    });
                }
                var isNew = false;
                if (identityUser == null)
                {
                    user = userService.GetByMainMobile(international_mobile);
                    identityUser = userService.GetIdentityUser(international_mobile);
                    if (identityUser == null)
                    {
                        string failReason;
                        user = new User();
                        user.Mobile = international_mobile;
                        user.MainMobile = international_mobile;
                        user.CreateDate = DateTime.Now;
                        user.ResponseFrom = 2;
                        user.ResponseTo = 2;
                        user.AmlakbashiScore = 1000;
                        userService.Insert(user);

                        identityUser = new AppUser()
                        {
                            UserName = international_mobile,
                            PhoneNumber = international_mobile,
                            CreateDate = DateTime.Now,
                            State = Entities.User.UserState.Acticved
                        };
                        userService.AddIdentityUser(identityUser);

                        failReason = null;
                        isNew = true;
                        if (user == null)
                        {
                            return GenerateJsonResult(new
                            {
                                status = (int)Entities.User.SignInFirstStepStatus.Error,
                                msg = failReason
                            });
                        }
                        user.State = (int)Entities.User.UserState.Acticved;
                    }
                }
                if (number_is_for_iran)
                {
                    var code = user.Code;
                    if (string.IsNullOrEmpty(user.Code) ||
                        user.SendVerification == null ||
                        (DateTime.Now - user.SendVerification) >
                        new TimeSpan(0, 0, 30, 0, 0))
                    {
                        code = new Random().Next(1111, 9999).ToString();
                        user.Code = code;
                        identityUser.Code = code;
                    }
                    user.SendVerification = DateTime.Now;
                    userService.UpdateSendVerification(user.Id, DateTime.Now, code);
                    var local_number = PhoneUtility.InternationalNumberToLocal(international_mobile);
                    userService.SendVerificationSms(local_number, code);
                    return GenerateJsonResult(new
                    {
                        status = (int)Entities.User.SignInFirstStepStatus.MobileLogin,
                        code = code,
                        mobile = local_number,
                        user_id = user.Id,
                        fname = string.IsNullOrEmpty(user.FName) ? "" : user.FName,
                        lname = string.IsNullOrEmpty(user.LName) ? "" : user.LName,
                        notification_token = fcm_notification ? user.FcmAppNotificationToken : user.AppNotificationToken,
                        isNew = isNew
                    });
                }
                else
                {
                    return GenerateJsonResult(new
                    {
                        status = (int)Entities.User.SignInFirstStepStatus.EmailLogin,
                        user_id = user.Id,
                        fname = string.IsNullOrEmpty(user.FName) ? "" : user.FName,
                        lname = string.IsNullOrEmpty(user.LName) ? "" : user.LName,
                        mobile = international_mobile,
                        notification_token = fcm_notification ? user.FcmAppNotificationToken : user.AppNotificationToken,
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = (int)Entities.User.SignInFirstStepStatus.Error,
                    msg = "متاسفانه خطایی رخ داده است"
                });
            }
        }

        public JsonResult VerifySigninCode(string mobile, string code)
        {
            try
            {
                var internationalMobile = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(internationalMobile);
                if (identityUser.Code == code)
                {
                    var userRoles = userManager.GetRolesAsync(identityUser).Result;
                    var authClaims = new List<Claim>();
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]));
                    var token = new JwtSecurityToken(
                            expires: DateTime.Now.AddHours(1440),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
                    return GenerateJsonResult(new
                    {
                        status = (int)Entities.User.SignInFirstStepStatus.MobileLogin,
                        mobile = mobile,
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }
                else
                {
                    return GenerateJsonResult(new
                    {
                        status = (int)Entities.User.SignInFirstStepStatus.Error,
                        msg = "کد وارد شده اشتباه است"
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = (int)Entities.User.SignInFirstStepStatus.Error,
                    msg = "متاسفانه خطایی رخ داده است"
                });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult Test()
        {
            return GenerateJsonResult("verify auth");
        }

        [Authorize(AuthenticationSchemes = bearerScheme, Policy = "SuperAdmins")]
        public JsonResult TestAdmin()
        {
            return GenerateJsonResult("verify auth");
        }

        public JsonResult SignInEmail(string email, string mobile, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var first_two_numbers = mobile.Substring(2, 2);
                mobile = mobile.Remove(0, 4).Insert(0, "+" + first_two_numbers + " ");
                var user = userService.GetActivatedUserByEmail(email);
                if (user == null)
                {
                    user = userService.GetByEmail(email);
                    if (user == null)
                    {
                        string failReason;
                        user = new User();
                        user.Mobile = mobile;
                        if (!string.IsNullOrEmpty(email))
                        {
                            user.Email = email;
                        }
                        user.MainMobile = mobile;
                        user.CreateDate = DateTime.Now;
                        user.ResponseFrom = 2;
                        user.ResponseTo = 2;
                        user.AmlakbashiScore = 1000;
                        userService.Insert(user);
                        failReason = null;
                        if (user == null)
                        {
                            return GenerateJsonResult(new
                            {
                                status = (int)Entities.User.SignInEmailStatus.Error,
                                msg = failReason
                            });
                        }
                    }
                    user.State = (int)Entities.User.UserState.Acticved;
                    userService.UpdateState(user.Id, true);
                }

                if (user.CreateDate == null)
                {
                    user.CreateDate = DateTime.Now;
                    userService.UpdateCreateDate(user.Id, DateTime.Now);
                }

                if (user.AccessType == (int)Entities.User.AccessTypeEnum.LoginBanned)
                {
                    return GenerateJsonResult(new
                    {
                        status = (int)Entities.User.SignInEmailStatus.Error,
                        msg = "امکان ورود به سایت برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید"
                    });
                }
                var code = user.Code;
                if (string.IsNullOrEmpty(user.Code) ||
                    user.SendVerification == null ||
                    (DateTime.Now - user.SendVerification) >
                    new TimeSpan(0, 0, 30, 0, 0))
                {
                    code = new Random().Next(1111, 9999).ToString();
                    user.Code = code;
                }
                user.SendVerification = DateTime.Now;
                userService.UpdateSendVerification(user.Id, DateTime.Now);
                string strbody = "<div style='direction:rtl;text-align:right;'><div>برای تایید ایمیل خود و ورود به اپلیکیشن املاک باشی کد زیر را در اپلیکیشن وارد کنید:</div>"
                + "<div style='direction:rtl;text-align:right;font-size:25'>" + code + "</div>"
                + "</div>";
                try
                {
                    EmailUtility.SendEmail(EmailSenderDepartment.Verification,
                    new List<string>() { email },
                    "املاک باشی - تایید ایمیل ورود",
                    strbody
                    );
                }
                catch (Exception exc)
                {
                    logger.Error("", exc);
                }

                user = userService.GetByEmail(email);
                user.SendVerification = DateTime.Now;
                userService.UpdateSendVerification(user.Id, DateTime.Now);
                return GenerateJsonResult(new
                {
                    status = (int)Entities.User.SignInEmailStatus.Done,
                    code = code,
                    mobile = mobile
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = (int)Entities.User.SignInEmailStatus.Error,
                    msg = "متاسفانه خطایی رخ داده است"
                });
            }
        }

        public JsonResult SetPresentorCode(string cid, string mobile, string presentorCode)
        {
            var internationalMobile = PhoneUtility.LocalNumberToInternational(mobile, 98);
            var user = userService.GetByMainMobile(internationalMobile);
            try
            {
                var prId = int.Parse(presentorCode);
                var prUser = userService.Find(prId);
                user.PresentorUserID = prUser.Id;
                userService.UpdatePresentorUser(user.Id, prUser.Id);
                if (user.PresentorUserID > 0)
                {
                    accounting.InsertDiscountCoupon(user.Id, DiscountCoupon.DiscountCouponType.Present, 5, user.PresentorUserID);
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        UserEmail = user.Email,
                        UserId = user.Id.ToString(),
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserMainMobile = user.MainMobile,
                        UserLoginPriority = user.LoginPriority,
                        Type = UserContactType.CouponPresent,
                        Extra1 = prUser.FullName,
                        Extra2 = "5%"
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch
            {
                return GenerateJsonResult(new
                {
                    status = (int)Entities.User.SignInFirstStepStatus.Error,
                    msg = "کد معرف اشتباه است. لطفا بررسی کنید."
                });
            }
        }

        public JsonResult GetUserCreditData(string cid, string token)
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
                    return GenerateJsonResult(new { credit = 0, creditTransactions = new List<ApiCreditTransactionDTO>() });
                }
                var model = accounting.GetCreditListByUserId(user.Id);
                var dtoList = new List<ApiCreditTransactionDTO>();
                foreach (var item in model)
                {
                    dtoList.Add(item);
                }
                long initial_credit = 0;
                var first_item = model.LastOrDefault();
                if (first_item != null)
                {
                    initial_credit = first_item.RemainedPrice - first_item.Price;
                }
                if (initial_credit != 0)
                {
                    dtoList.Add(new ApiCreditTransactionDTO()
                    {
                        id = 0,
                        dateString = "-",
                        price = initial_credit,
                        reasonString = initial_credit > 0 ? "مانده از قبل" : "بدهی از قبل",
                        remainedPrice = initial_credit,
                        comment1 = "",
                        comment2 = "",
                        reasonColor = "#4285F4"
                    });
                }
                return GenerateJsonResult(new { credit = user.Credit, creditTransactions = dtoList });
            }
            catch (Exception exc)
            {
                logger.Error("UserApi.GetUserCreditData", exc);
                return GenerateJsonResult(new { credit = 0, creditTransactions = new List<ApiCreditTransactionDTO>() });
            }
        }

        public JsonResult GetUserProfileData(string cid, string token)
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
                UserDTO user_data = user;
                var bankCard = bankCardService.GetByUserId(user.Id);
                if (bankCard != null)
                {
                    user_data.bankCardNumber = bankCard.BankCardNumber;
                    user_data.shabaNumber = bankCard.ShabaNumber;
                    user_data.bankFname = bankCard.FName;
                    user_data.bankLname = bankCard.LName;
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    data = user_data,
                    photoId = user.PhotoID
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult GetUserPublicInfo(string cid, int userId = 0,
            int advertiseId = 0, int reserveId = 0, bool forHost = false)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                User user = null;
                if (userId != 0)
                {
                    user = userService.Find(userId);
                }
                else if (advertiseId != 0)
                {
                    var advertise = advertiseService.Find(advertiseId);
                    user = userService.Find(advertise.UserID);
                }
                else
                {
                    var reserve = reserveService.Find(reserveId);
                    if (forHost)
                    {
                        var advertise = reserve.Advertise;
                        user = userService.Find(advertise.UserID);
                    }
                    else
                    {
                        user = userService.Find(reserve.UserID);
                    }
                }
                return GenerateJsonResult(new
                {
                    firstName = user.FName != null ? user.FName : "",
                    lastName = user.LName != null ? user.LName : "",
                    photoId = user.PhotoStatus == (int)Entities.User.UserPhotoState.publish ? user.PhotoID : 0
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    lastName = "",
                    photoId = 0
                });
            }
        }

        public JsonResult SetUserRealName(string cid, int userId,
            string fname, string lname)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                if (fname == null || fname == "")
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "لطفا نام خود را وارد کنید"
                    });
                }
                if (lname == null || lname == "")
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "لطفا نام خانوادگی خود را وارد کنید"
                    });
                }
                userService.UpdateFNameLName(userId, fname, lname);
                return GenerateJsonResult(new
                {
                    done = true
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        public JsonResult EditUser(string cid, string token, UserDTO userItem)
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
                string msg;
                List<string> errors;
                var userHasRefund = reserveService.UserHasRefundInProgress(userItem.id);
                var done = userService.Update(userItem, user.Id, userHasRefund,
                    ActionLog.ActionSourceEnum.Application, out errors);
                if (done)
                {
                    msg = "ویرایش پروفایل شما با موفقیت انجام شد";
                }
                else
                {
                    msg = errors.First();
                }
                return GenerateJsonResult(new
                {
                    done = done,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult UpdateUserNotificationToken(string cid, string token, string notificationToken, bool fcm_notification = false)
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
                        done = false
                    });
                }
                if (notificationToken == "null")
                {
                    notificationToken = null;
                }
                if (fcm_notification)
                {
                    //IQueryable<User> users = _db.Users;
                    //if (!string.IsNullOrEmpty(notificationToken))
                    //{
                    //    var users_with_same_token = users.Where(x => x.FcmAppNotificationToken == notificationToken);
                    //    foreach (var item in users_with_same_token)
                    //    {
                    //        item.FcmAppNotificationToken = null;
                    //    }
                    //    _db.SaveChanges();
                    //}
                    //var user_obj = users.FirstOrDefault(x => x.Id == user.Id);
                    //user_obj.FcmAppNotificationToken = notificationToken;
                    //_db.SaveChanges();
                    userService.UpdateFcmNotificationToken(user.Id, notificationToken);
                }
                else
                {
                    //if (user.AppNotificationToken != notificationToken)
                    //{
                    //    IQueryable<User> users = _db.Users;
                    //    if (!string.IsNullOrEmpty(notificationToken))
                    //    {
                    //        var users_with_same_token = users.Where(x => x.AppNotificationToken == notificationToken);
                    //        foreach (var item in users_with_same_token)
                    //        {
                    //            item.AppNotificationToken = null;
                    //        }
                    //        _db.SaveChanges();
                    //    }
                    //    var user_obj = users.FirstOrDefault(x => x.Id == user.Id);
                    //    user_obj.AppNotificationToken = notificationToken;
                    //    _db.SaveChanges();
                    //}
                    userService.UpdateAppNotificationToken(user.Id, notificationToken);
                }
                return GenerateJsonResult(new
                {
                    done = true
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                });
            }
        }

        public JsonResult GetReservePrizeData(string cid, string token)
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
                return GenerateJsonResult(new
                {
                    done = true,
                    presentorCode = user.Id.ToString(),
                    title = "روی لینک کلیک کنید. ویلا و سوئیت رزرو کنید و هدیه بگیرید ",
                    url = GeneralData.WebsiteUrl + "/user/p?c=" + user.Id
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { done = false, msg = "متاسفانه بارگذاری با خطا مواجه شد" });
            }
        }
    }
}

