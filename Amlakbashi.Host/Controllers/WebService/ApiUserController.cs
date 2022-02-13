using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Host.Controllers.Base;
using Entities = Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.DTOs.WebService.Responses.User;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/user")]
    public class ApiUserController : ApiBaseController
    {
        private readonly IUserAppService userService;
        public ApiUserController(IUserAppService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public IActionResult LoginOrRegister(string phoneNumber)
        {
            var isCorrectNumber = string.IsNullOrEmpty(phoneNumber) == false && (PhoneUtility.ValidateLocalNumber(phoneNumber) ||
                (phoneNumber.Length > 10 && (phoneNumber.Substring(0, 1) == "+" || phoneNumber.Substring(0, 2) == "00")));
            if (!isCorrectNumber)
            {
                return BadRequest();
            }

            var internationalMobileNumber = PhoneUtility.CorrectPhoneNumberIfPossible(phoneNumber);
            var identityUser = userService.GetIdentityUser(internationalMobileNumber);

            if (identityUser == null)
            {
                return Register(internationalMobileNumber);
            }
            return Login(internationalMobileNumber);
        }

        private CreatedResult Register(string internationalMobileNumber)
        {
            var isIranNumber = PhoneUtility.IsNumberForIran(internationalMobileNumber);
            var verifyCode = new Random().Next(1111, 9999).ToString();
            var user = new Entities.User()
            {
                Mobile = internationalMobileNumber,
                MainMobile = internationalMobileNumber,
                AmlakbashiScore = 1000
            };
            userService.Insert(user);

            var identityUser = new AppUser()
            {
                UserName = internationalMobileNumber,
                PhoneNumber = internationalMobileNumber,
                CreateDate = DateTime.Now,
                State = Entities.User.UserState.InActived,
                Code = verifyCode,
                SendVerification = DateTime.Now
            };
            userService.AddIdentityUser(identityUser);

            if (isIranNumber)
            {
                var callableNumber = PhoneUtility.InternationalNumberToLocal(internationalMobileNumber);
                userService.SendVerificationSms(callableNumber, verifyCode);
            }

            var response = new LoginResponse()
            {
                isNewUser = true,
                mobileNumber = internationalMobileNumber,
                state = Entities.User.UserState.InActived,
                isIranNumber = isIranNumber
            };
            return Created("", response);
        }

        private IActionResult Login(string internationalMobileNumber)
        {
            var identityUser = userService.GetIdentityUser(internationalMobileNumber);

            // check blocked user
            if (identityUser != null && identityUser.State == Entities.User.UserState.Suspend)
            {
                return Forbid();
            }

            var isIranNumber = PhoneUtility.IsNumberForIran(internationalMobileNumber);
            var response = new LoginResponse()
            {
                mobileNumber = internationalMobileNumber,
                state = identityUser.State,
                isIranNumber = isIranNumber,
                isNewUser = false
            };

            // login with password
            if (identityUser.PasswordHash != null)
            {
                response.hasPassword = true;
                return Ok(response);
            }

            // login with verify code
            var user = userService.GetByMainMobile(internationalMobileNumber);
            var verifyCode = new Random().Next(1111, 9999).ToString();
            if (isIranNumber)
            {
                var callableNumber = PhoneUtility.InternationalNumberToLocal(internationalMobileNumber);
                userService.UpdateSendVerification(user.Id, DateTime.Now, verifyCode);
                userService.SendVerificationSms(callableNumber, verifyCode);
            }
            else if (identityUser.EmailConfirmed)
            {
                string strbody = $"<div style='direction:rtl;text-align:right;'><div>کد ورود شما در املاک باشی: {identityUser.EmailCode}</div></div>";
#if !DEBUG
                EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { identityUser.Email }, "تایید ایمیل", strbody);
#endif
            }
            return Ok(response);
        }
    }
}
