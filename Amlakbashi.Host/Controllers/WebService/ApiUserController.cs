using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Host.Controllers.Base;
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
using Amlakbashi.Host.Extensions;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/user")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiUserController : ApiBaseController
    {
        private readonly IUserAppService userService;
        private readonly IBankCardAppService bankCardService;
        private readonly IConfiguration configuration;
        public ApiUserController(IUserAppService userService,
            IBankCardAppService bankCardService,
            IConfiguration configuration)
        {
            this.userService = userService;
            this.bankCardService = bankCardService;
            this.configuration = configuration;
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
                bankCardNumber = bankCard?.BankCardNumber,
                bankCardOwnerFirstName = bankCard?.FName,
                bankCardOwnerLastName = bankCard?.LName,
                shebaNumber = bankCard?.ShabaNumber,
                noticesPhoneNumber = user.NoticesPhoneNumber,
                imageUrl = user.GetUserImageApiUrl()
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

        [AllowAnonymous]
        [HttpGet("host/{id:int}")]
        public IActionResult HostPage(int id)
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
