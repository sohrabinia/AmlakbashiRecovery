using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Host.Controllers.Base;
using Entities = Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amlakbashi.Core.DTOs.WebService.Responses.User;
using System.Text;
using Microsoft.Extensions.Configuration;
using Amlakbashi.Core.DTOs.WebService.Requests.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Http;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Extensions;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/user")]
    public class ApiUserController : ApiBaseController
    {
        private readonly IUserAppService userService;
        private readonly IUserAccessor userAccessor;
        private readonly IConfiguration configuration;
        public ApiUserController(IUserAppService userService,
            IUserAccessor userAccessor,
            IConfiguration configuration)
        {
            this.userService = userService;
            this.userAccessor = userAccessor;
            this.configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> LoginOrRegister(LoginRequest request)
        {
            var isCorrectNumber = string.IsNullOrEmpty(request.phoneNumber) == false && (PhoneUtility.ValidateLocalNumber(request.phoneNumber) ||
                (request.phoneNumber.Length > 10 && (request.phoneNumber.Substring(0, 1) == "+" || request.phoneNumber.Substring(0, 2) == "00")));
            if (!isCorrectNumber)
            {
                return BadRequest();
            }

            request.phoneNumber = PhoneUtility.CorrectPhoneNumberIfPossible(request.phoneNumber);
            var identityUser = userService.GetIdentityUser(request.phoneNumber);

            if (identityUser == null)
            {
                return await Register(request);
            }
            return await Login(request);
        }

        private async Task<IActionResult> Register(LoginRequest request)
        {
            var isIranNumber = PhoneUtility.IsNumberForIran(request.phoneNumber);

            if (isIranNumber == false && EmailUtility.ValidateEmail(request.email) == false)
            {
                ModelState.AddModelError(nameof(request.email), "email not valid");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.referralCode) == false)
            {
                var referralUser = userService.Find(int.Parse(request.referralCode));
                if (referralUser == null)
                {
                    ModelState.AddModelError(nameof(request.referralCode), "referral code not valid");
                    return BadRequest(ModelState);
                }
            }

            var identityUser = await userService.RegisterAsync(request);
            if (identityUser == null)
            {
                return BadRequest();
            }

            userService.SendVerifyCode(identityUser);
            var response = new LoginResponse()
            {
                isNewUser = true,
                guid = identityUser.Id,
                username = identityUser.UserName,
                state = Entities.User.UserState.InActived,
                isIranNumber = isIranNumber
            };
            return CreatedAtAction(nameof(Profile), response);
        }

        private async Task<IActionResult> Login(LoginRequest request)
        {
            var identityUser = userService.GetIdentityUser(request.phoneNumber);

            if (identityUser != null && identityUser.State == Entities.User.UserState.Suspend)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var isIranNumber = PhoneUtility.IsNumberForIran(request.phoneNumber);
            if (isIranNumber == false)
            {
                if (EmailUtility.ValidateEmail(request.email) == false)
                {
                    ModelState.AddModelError(nameof(request.email), "email not valid");
                    return BadRequest(ModelState);
                }
                await userService.UpdateEmailAsync(identityUser.Id, request.email, false);
            }

            await userService.UpdateVerifyCodeAsync(identityUser.Id);
            userService.SendVerifyCode(identityUser);

            var response = new LoginResponse()
            {
                guid = identityUser.Id,
                username = identityUser.UserName,
                state = identityUser.State,
                isIranNumber = isIranNumber,
                isNewUser = false
            };
            return Ok(response);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> LoginVerify(LoginVerifyRequest request)
        {
            var identityUser = await userService.GetIdentityUserByIdAsync(request.guid);
            if (identityUser == null)
            {
                return NotFound();
            }
            if (identityUser.Code != request.verifyCode)
            {
                return BadRequest("verify code not valid");
            }
            if ((DateTime.Now - identityUser.SendVerification) > new TimeSpan(0, 0, 5, 0, 0))
            {
                return BadRequest("verify code lifetime ended");
            }

            var isIranNumber = PhoneUtility.IsNumberForIran(identityUser.UserName);
            if (isIranNumber && identityUser.PhoneNumberConfirmed == false)
            {
                await userService.UpdatePhoneNumberConfirmedAsync(identityUser.Id, true);
            }
            if (isIranNumber == false && identityUser.EmailConfirmed == false)
            {
                await userService.UpdateEmailConfirmedAsync(identityUser.Id, true);
            }
            var jwtToken = await userService.GenerateJwtTokenAsync(identityUser.Id, configuration["JwtConfig:Secret"]);
            return Ok(new {
                token = jwtToken
            });
        }

        [HttpPost("resendcode")]
        public async Task<IActionResult> ResendVerifyCode(ResendVerifyCodeRequest request)
        {
            var identityUser = await userService.GetIdentityUserByIdAsync(request.guid);
            if (identityUser == null)
            {
                return NotFound();
            }
            await userService.UpdateVerifyCodeAsync(identityUser.Id);
            userService.SendVerifyCode(identityUser);
            return NoContent();
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var jwtSecret = configuration["JwtConfig:Secret"];
            var principal = userService.GetPrincipalFromJwtToken(request.token, jwtSecret);
            var refreshTokenClaim = principal.GetRefreshToken();
            if (principal == null || string.IsNullOrEmpty(refreshTokenClaim))
            {
                return Unauthorized();
            }
            var identityUser = await userService.GetIdentityUserByIdAsync(principal.GetGuid());
            if (identityUser == null || identityUser.SecurityStamp != refreshTokenClaim)
            {
                return Unauthorized();
            }
            var newToken = await userService.GenerateJwtTokenAsync(identityUser.Id, jwtSecret);
            return new ObjectResult(new
            {
                token = newToken
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult Profile()
        {
            var user = userAccessor.CurrentUser;
            if (user == null || user.Id == 0)
            {
                return NotFound();
            }
            var response = new UserProfileResponse()
            {
                id = user.Id,
                mainMobile = user.MainMobile,
                mobile1 = user.Mobile,
                mobile2 = user.Mobile2,
                tell = user.Tell,
                thirdPersonTell = user.ThirdPersonTell,
                fname = user.FName,
                lname = user.LName,
                email = ""
            };
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.Payment_Actions)]
        [HttpGet("test")]
        public IActionResult test()
        {
            return Ok();
        }
    }
}
