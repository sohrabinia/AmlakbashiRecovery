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
using Microsoft.AspNetCore.Http;
using Amlakbashi.Host.Extensions;
using Microsoft.AspNetCore.Hosting;
using Amlakbashi.Application.Services.FileServices.Interfaces;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/user")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiUserController : ApiBaseController
    {
        private readonly IUserAppService userService;
        private readonly IBankCardAppService bankCardService;
        private readonly IFileAppService fileService;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ApiUserController(IUserAppService userService,
            IBankCardAppService bankCardService,
            IFileAppService fileService,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            this.userService = userService;
            this.bankCardService = bankCardService;
            this.fileService = fileService;
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        [HttpPost]
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

        [AllowAnonymous]
        [HttpPost("verify")]
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
            return Ok(new {
                token = jwtToken
            });
        }

        [AllowAnonymous]
        [HttpPost("resendcode")]
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

        [AllowAnonymous]
        [HttpGet("host/{id:int}")]
        public IActionResult HostProfile(int id)
        {
            var user = userService.Find(id);
            if (user == null || user.Type == 0)
            {
                return NotFound();
            }
            var identityUser = userService.GetIdentityUser(user.PhoneNumber);
            HostProfileResponse response = user;
            response.hostCreateDate = StringUtility.EnglishNumberToPersian(DateTimeUtility.ConvertDate(identityUser.CreateDate.Value));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = userService.Find(User.GetId());
            if (user == null)
            {
                return NotFound();
            }
            var identityUser = await userService.FindIdentityByIdAsync(User.GetGuid());
            var bankCard = bankCardService.GetByUserId(user.Id);
            var response = new UserGetProfileResponse()
            {
                phoneNumber = user.PhoneNumber,
                phoneNumber2 = user.PhoneNumber2,
                phoneNumber3 = user.PhoneNumber3,
                landLinePhoneNumber = user.LandlinePhoneNumber,
                thirdPersonPhoneNumber = user.ThirdPersonPhoneNumber,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = identityUser.Email,
                bankCardNumber = bankCard.BankCardNumber,
                bankCardOwnerName = bankCard.FullName,
                shebaNumber = bankCard.ShabaNumber,
                noticesPhoneNumber = user.NoticesPhoneNumber,
                imageUrl = user.GetCurrentUserImageApiUrl()
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserPostProfileRequest request)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.id = User.GetId();
            var result = await userService.UpdateAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpPost("phonenumber/update")]
        public async Task<IActionResult> UpdateMainPhoneNumber(UpdatePhoneNumberRequest request)
        {
            var result = await userService.UpdateMainPhoneNumberAsync(User.GetId(), request.newPhoneNumber);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            if (result.Result) // foreigner number
            {
                var newToken = await userService.GenerateJwtTokenAsync(User.GetGuid(),
                    configuration["JwtConfig:Secret"], User.GetUserPanelType());
                return Ok(new
                {
                    requireVerify = false,
                    token = newToken
                });
            }
            return Ok(new {
                requireVerify = true
            });
        }

        [HttpPost("phonenumber/verify")]
        public async Task<IActionResult> VerifyMainPhoneNumber(VerifyPhoneNumberRequest request)
        {
            var result = await userService.VerifyNewMainPhoneNumber(User.GetId(), request.verifyCode);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            var newToken = await userService.GenerateJwtTokenAsync(User.GetGuid(),
                configuration["JwtConfig:Secret"], User.GetUserPanelType());
            return Ok(new
            {
                token = newToken
            });
        }

        [HttpGet("referralCode")]
        public IActionResult GetReferralCode()
        {
            return Ok(User.GetId());
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.Payment_Actions)]
        //[HttpGet("test")]
        //public IActionResult test()
        //{
        //    User.GetUserPanelType();
        //    return Ok();
        //}
    }
}
