using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
using Entities = Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Amlakbashi.Core.DTOs.WebService.Requests.Accounts;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/account")]

    public class ApiAccountController : ApiBaseController
    {
        private readonly IUserAppService userService;
        private readonly IConfiguration configuration;
        public ApiAccountController(IUserAppService userService,
            IConfiguration configuration)
        {
            this.userService = userService;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginOrRegister(LoginRequest request)
        {
            var isCorrectNumber = PhoneUtility.ValidatePhoneNumber(request.phoneNumber);
            if (isCorrectNumber == false)
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

            return Created("/api/user", new {
                isNewUser = true,
                guid = identityUser.Id,
                username = identityUser.UserName,
                state = Entities.User.UserState.InActived,
                stateDesc = Entities.User.UserState.InActived.ToString(),
                isIranNumber = isIranNumber
            });
        }

        private async Task<IActionResult> Login(LoginRequest request)
        {
            var identityUser = await userService.FindIdentityByUsernameAsync(request.phoneNumber);

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

            return Ok(new {
                guid = identityUser.Id,
                username = identityUser.UserName,
                state = identityUser.State,
                stateDesc = identityUser.State.ToString(),
                isIranNumber = isIranNumber,
                isNewUser = false,
            });
        }

        [AllowAnonymous]
        [HttpPost("login/verify")]
        public async Task<IActionResult> LoginVerify(LoginVerifyRequest request)
        {
            var identityUser = await userService.FindIdentityByIdAsync(request.guid);
            if (identityUser == null)
            {
                return NotFound();
            }
            if (identityUser.IsVerifyCodeValid(request.verifyCode) == false)
            {
                return BadRequest("verify code not valid");
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
            return Ok(new
            {
                token = jwtToken
            });
        }

        [AllowAnonymous]
        [HttpPost("login/resendcode")]
        public async Task<IActionResult> ResendVerifyCode(ResendVerifyCodeRequest request)
        {
            var identityUser = await userService.FindIdentityByIdAsync(request.guid);
            if (identityUser == null)
            {
                return NotFound();
            }
            await userService.UpdateVerifyCodeAsync(identityUser.Id);
            userService.SendVerifyCode(identityUser);
            return NoContent();
        }

        [AllowAnonymous]
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
            var identityUser = await userService.FindIdentityByIdAsync(principal.GetGuid());
            if (identityUser == null || identityUser.SecurityStamp != refreshTokenClaim)
            {
                return Unauthorized();
            }
            var newToken = await userService.GenerateJwtTokenAsync(identityUser.Id,
                jwtSecret, principal.GetUserPanelType());
            return Ok(new
            {
                token = newToken
            });
        }

        [HttpPost("panel")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePanel(ChangePanelRequest request)
        {
            var currentPanel = User.GetUserPanelType();
            if (currentPanel == request.panel)
            {
                return NoContent();
            }
            var newToken = await userService.GenerateJwtTokenAsync(User.GetGuid(),
                configuration["JwtConfig:Secret"], request.panel);
            return Ok(new
            {
                token = newToken
            });
        }
    }
}
