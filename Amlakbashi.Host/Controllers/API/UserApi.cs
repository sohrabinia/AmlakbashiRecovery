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
using Amlakbashi.Core.Identity;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;

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
                        status = (int)Entities.User.SignInFirstStepResult.Error,
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
                        status = (int)Entities.User.SignInFirstStepResult.Error,
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
                        user.ResponseFrom = 2;
                        user.ResponseTo = 2;
                        user.AmlakbashiScore = 1000;
                        userService.Insert(user);

                        identityUser = new AppUser()
                        {
                            UserName = international_mobile,
                            PhoneNumber = international_mobile,
                            CreateDate = DateTime.Now,
                            State = Entities.User.UserState.InActived
                        };
                        userService.AddIdentityUser(identityUser);

                        failReason = null;
                        isNew = true;
                        if (user == null)
                        {
                            return GenerateJsonResult(new
                            {
                                status = (int)Entities.User.SignInFirstStepResult.Error,
                                msg = failReason
                            });
                        }
                    }
                }

                //if (number_is_for_iran)
                //{
                var code = identityUser.Code;
                if (identityUser.State != Entities.User.UserState.Acticved ||
                    identityUser.PhoneNumberConfirmed == false)
                {
                    if (string.IsNullOrEmpty(code) ||
                        identityUser.SendVerification == null ||
                        (DateTime.Now - identityUser.SendVerification) >
                        new TimeSpan(0, 0, 30, 0, 0))
                    {
                        code = new Random().Next(1111, 9999).ToString();
                        userService.UpdateSendVerification(user.Id, DateTime.Now, code);
                    }
                    var callableNumber = number_is_for_iran ?
                        PhoneUtility.InternationalNumberToLocal(international_mobile) :
                        PhoneUtility.InternationalNumberToCallable(international_mobile);
                    userService.SendVerificationSms(callableNumber, code);
                }
                return GenerateJsonResult(new
                {
                    status = identityUser.PhoneNumberConfirmed &&
                        identityUser.State == Entities.User.UserState.Acticved ?
                        (int)Entities.User.SignInFirstStepResult.EnterPassword :
                        (int)Entities.User.SignInFirstStepResult.MobileConfirm,
                    code = code,
                    mobile = international_mobile,
                    user_id = user.Id,
                    fname = string.IsNullOrEmpty(user.FName) ? "" : user.FName,
                    lname = string.IsNullOrEmpty(user.LName) ? "" : user.LName,
                    notification_token = fcm_notification ? user.FcmAppNotificationToken : user.AppNotificationToken,
                    isNew = isNew
                });
                //}
                //else
                //{
                //    return GenerateJsonResult(new
                //    {
                //        status = (int)Entities.User.SignInFirstStepStatus.EmailLogin,
                //        user_id = user.Id,
                //        fname = string.IsNullOrEmpty(user.FName) ? "" : user.FName,
                //        lname = string.IsNullOrEmpty(user.LName) ? "" : user.LName,
                //        mobile = international_mobile,
                //        notification_token = fcm_notification ? user.FcmAppNotificationToken : user.AppNotificationToken,
                //    });
                //}
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = (int)Entities.User.SignInFirstStepResult.Error,
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
                    identityUser.PhoneNumberConfirmed = true;
                    userService.UpdateIdentityUser(identityUser);
                    return GenerateJsonResult(new
                    {
                        status = 1,
                        mobile = mobile
                    });
                }
                else
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "کد وارد شده اشتباه است"
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه خطایی رخ داده است"
                });
            }
        }

        public JsonResult SendSmsAgain(string mobile)
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
                var user = userService.GetActivatedUserByMainMobile(mobile_international);
                var identityUser = userService.GetIdentityUser(mobile_international);
                if (identityUser != null)
                {
                    var code = new Random().Next(1111, 9999).ToString();
                    userService.UpdateSendVerification(user.Id, DateTime.Now, code);
                    var mobileNumber = PhoneUtility.IsNumberForIran(mobile_international) ?
                        PhoneUtility.InternationalNumberToLocal(mobile_international) :
                        PhoneUtility.InternationalNumberToCallable(mobile_international);
                    userService.SendVerificationSms(mobileNumber, code);
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
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SignInRegister(string mobile, string code, string fname = null, string lname = null,
            string password = null, string confirmPassword = null, string presentorCode = "")
        {
            try
            {
                var mobile_international = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                int user_id;
                string errorMsg;
                var identityUser = userService.GetIdentityUser(mobile_international);
                if (identityUser != null && identityUser.Code == code)
                {
                    if (identityUser.State == Entities.User.UserState.Suspend ||
                        identityUser.State == Entities.User.UserState.Deleted)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            errors = new Dictionary<string, string>() { { "suspend", "حساب کاربری شما معلق شده است. لطفا با پشتیبان تماس بگیرید" } }
                        });
                    }
                    var verify = userService.VerifyLogin(mobile_international, out user_id, presentorCode, out errorMsg);
                    if (verify == false)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            errors = new Dictionary<string, string>() { { "login", errorMsg } }
                        });
                    }
                    if (identityUser.State == Entities.User.UserState.InActived)
                    {
                        Dictionary<string, string> errors;
                        if (userService.SignInRegister(user_id, fname, lname, password,
                            confirmPassword, out errors))
                        {
                            userService.UpdateState(user_id, true);
                        }
                        else
                        {
                            return GenerateJsonResult(new
                            {
                                status = 0,
                                errors = errors
                            });
                        }
                    }
                    var jwtToken = userService.JwtSignIn(identityUser, Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]));
                    return GenerateJsonResult(new { status = 1, token = new JwtSecurityTokenHandler().WriteToken(jwtToken) });
                }
                else
                {
                    return GenerateJsonResult(new { status = 0, errors =
                        new Dictionary<string, string>() { { "code", "کد وارد شده صحیح نیست" } } });
                }
            }
            catch (Exception exc)
            {
                logger.Error("UserApi/SignInRegister", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    errors = new Dictionary<string, string>() { { "exception", GeneralLocalization.GetExceptionMessage(exc) } }
                });
            }
        }

        public JsonResult SaveNewPass(string mobile, string password, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(password) || password != confirmPassword)
                {
                    return GenerateJsonResult(new { status = 0, msg = "رمز عبور و تاییدیه آن را به درستی وارد کنید" });
                }
                var mobile_international = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                userService.ChangeIdentityUserPassword(mobile_international, password);
                var identityUser = userService.GetIdentityUser(mobile_international);
                var jwtToken = userService.JwtSignIn(identityUser, Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]));
                return GenerateJsonResult(new { status = 1, token = new JwtSecurityTokenHandler().WriteToken(jwtToken) });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        public JsonResult LoginPass(string mobile, string password)
        {
            try
            {
                var mobileInternational = PhoneUtility.CorrectPhoneNumberIfPossible(mobile);
                var identityUser = userService.GetIdentityUser(mobileInternational);
                var passwordOK = userManager.CheckPasswordAsync(identityUser, password).Result;
                if (passwordOK)
                {
                    var jwtToken = userService.JwtSignIn(identityUser, Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]));
                    return GenerateJsonResult(new { status = 1, token = new JwtSecurityTokenHandler().WriteToken(jwtToken) });
                }
                return GenerateJsonResult(new { status = 0, msg = "رمز وارد شده اشتباه است" });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        //public JsonResult SignInEmail(string email, string mobile, string cid)
        //{
        //    if (!ClientAuthenticate(cid))
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        var first_two_numbers = mobile.Substring(2, 2);
        //        mobile = mobile.Remove(0, 4).Insert(0, "+" + first_two_numbers + " ");
        //        var user = userService.GetActivatedUserByEmail(email);
        //        if (user == null)
        //        {
        //            user = userService.GetByEmail(email);
        //            if (user == null)
        //            {
        //                string failReason;
        //                user = new User();
        //                user.Mobile = mobile;
        //                if (!string.IsNullOrEmpty(email))
        //                {
        //                    user.Email = email;
        //                }
        //                user.MainMobile = mobile;
        //                user.CreateDate = DateTime.Now;
        //                user.ResponseFrom = 2;
        //                user.ResponseTo = 2;
        //                user.AmlakbashiScore = 1000;
        //                userService.Insert(user);
        //                failReason = null;
        //                if (user == null)
        //                {
        //                    return GenerateJsonResult(new
        //                    {
        //                        status = (int)Entities.User.SignInEmailStatus.Error,
        //                        msg = failReason
        //                    });
        //                }
        //            }
        //            user.State = (int)Entities.User.UserState.Acticved;
        //            userService.UpdateState(user.Id, true);
        //        }

        //        if (user.CreateDate == null)
        //        {
        //            user.CreateDate = DateTime.Now;
        //            userService.UpdateCreateDate(user.Id, DateTime.Now);
        //        }

        //        if (user.AccessType == (int)Entities.User.AccessTypeEnum.LoginBanned)
        //        {
        //            return GenerateJsonResult(new
        //            {
        //                status = (int)Entities.User.SignInEmailStatus.Error,
        //                msg = "امکان ورود به سایت برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید"
        //            });
        //        }
        //        var code = user.Code;
        //        if (string.IsNullOrEmpty(user.Code) ||
        //            user.SendVerification == null ||
        //            (DateTime.Now - user.SendVerification) >
        //            new TimeSpan(0, 0, 30, 0, 0))
        //        {
        //            code = new Random().Next(1111, 9999).ToString();
        //            user.Code = code;
        //        }
        //        user.SendVerification = DateTime.Now;
        //        userService.UpdateSendVerification(user.Id, DateTime.Now);
        //        string strbody = "<div style='direction:rtl;text-align:right;'><div>برای تایید ایمیل خود و ورود به اپلیکیشن املاک باشی کد زیر را در اپلیکیشن وارد کنید:</div>"
        //        + "<div style='direction:rtl;text-align:right;font-size:25'>" + code + "</div>"
        //        + "</div>";
        //        try
        //        {
        //            EmailUtility.SendEmail(EmailSenderDepartment.Verification,
        //            new List<string>() { email },
        //            "املاک باشی - تایید ایمیل ورود",
        //            strbody
        //            );
        //        }
        //        catch (Exception exc)
        //        {
        //            logger.Error("", exc);
        //        }

        //        user = userService.GetByEmail(email);
        //        user.SendVerification = DateTime.Now;
        //        userService.UpdateSendVerification(user.Id, DateTime.Now);
        //        return GenerateJsonResult(new
        //        {
        //            status = (int)Entities.User.SignInEmailStatus.Done,
        //            code = code,
        //            mobile = mobile
        //        });
        //    }
        //    catch (Exception exc)
        //    {
        //        logger.Error("", exc);
        //        return GenerateJsonResult(new
        //        {
        //            status = (int)Entities.User.SignInEmailStatus.Error,
        //            msg = "متاسفانه خطایی رخ داده است"
        //        });
        //    }
        //}

        public JsonResult SetPresentorCode(string cid, string mobile, string presentorCode)
        {
            var internationalMobile = PhoneUtility.LocalNumberToInternational(mobile, 98);
            var user = userService.GetByMainMobile(internationalMobile);
            try
            {
                var prId = int.Parse(presentorCode);
                var prUser = userService.Find(prId);
                var identityUser = userService.GetIdentityUser(user.MainMobile);
                user.PresentorUserID = prUser.Id;
                userService.UpdatePresentorUser(user.Id, prUser.Id);
                if (user.PresentorUserID > 0)
                {
                    accounting.InsertDiscountCoupon(user.Id, DiscountCoupon.DiscountCouponType.Present, 5, user.PresentorUserID);
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        UserEmail = identityUser.Email,
                        UserId = user.Id.ToString(),
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserMainMobile = user.MainMobile,
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
                    status = 0,
                    msg = "کد معرف اشتباه است. لطفا بررسی کنید."
                });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult GetUserCreditData(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
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

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult GetUserProfileData(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                var identityUser = userService.GetIdentityUser(user.MainMobile);
                var user_data = UserDTO.Generate(user, identityUser);
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
        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult EditUser(string cid, UserDTO userItem)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
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

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult UpdateUserNotificationToken(string cid, string notificationToken, bool fcm_notification = false)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
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

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult GetReservePrizeData(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
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

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult SetEmailAddress(string email)
        {
            try
            {
                var identityUser = userService.GetIdentityUser(GetUser().MainMobile);
                var code = new Random().Next(1111, 9999).ToString();
                identityUser.EmailCode = code;
                if (identityUser.Email != email)
                {
                    identityUser.EmailConfirmed = false;
                }
                identityUser.Email = email;
                userService.UpdateIdentityUser(identityUser);
#if !DEBUG
                string strbody = $"<div style='direction:rtl;text-align:right;'><div>کد تایید ایمیل شما در املاک باشی: {code}</div></div>";
                EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { email },
                    "تایید ایمیل ثبت نام", strbody);
#endif
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0, msg = "خطا در ارسال کد تایید" });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult VerifyEmailConfirmCode(string code)
        {
            try
            {
                var identityUser = userService.GetIdentityUser(GetUser().MainMobile);
                if (identityUser.EmailCode == code)
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

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult GetCurrentEmail()
        {
            try
            {
                var user = GetUser();
                var identityUser = userService.GetIdentityUser(user.MainMobile);
                return GenerateJsonResult(new { status = 1, email = identityUser.Email });
            }
            catch(Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }
    }
}

