using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Reserves;
using Amlakbashi.Core.DTOs.WebService.Responses.Reserves;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/reserve")]
    public class ApiReserveController : ApiBaseController
    {
        private readonly IReserveAppService reserveService;
        private readonly IAccountingFacade accounting;
        private readonly IUserAccessor userAccessor;
        public ApiReserveController(IReserveAppService reserveService,
            IAccountingFacade accounting,
            IUserAccessor userAccessor)
        {
            this.reserveService = reserveService;
            this.accounting = accounting;
            this.userAccessor = userAccessor;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ReserveListResponse Get([FromQuery] ReserveListRequest request)
        {
            request.userId = userAccessor.CurrentUser.Id;
            request.userType = User.GetUserType();
            var response = reserveService.Filter(request);
            return response;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id:long}")]
        public ReserveResponse Get(long id)
        {
            ReserveResponse response = reserveService.Find(id);
            return response;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("invoice/{reserveId:long}")]
        public IActionResult Invoice(long reserveId)
        {
            var response = reserveService.GetInvoice(reserveId, userAccessor.CurrentUser.Id);
            if (response == null)
            {
                return Unauthorized();
            }
            return Ok(response);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpPut("cancel/{id:long}")]
        //public IActionResult Cancel(long id)
        //{

        //}

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("discount")]
        public IActionResult AddDiscountCode(ReserveAddDiscountCodeRequest request)
        {
            var discountCodeType = DiscountCoupon.GetDiscountCouponType(request.discountCode);
            if (discountCodeType == DiscountCoupon.DiscountCouponType.Unset)
            {
                return NotFound(new {
                    discountCode = "discount code is incorrect"
                });
            }
            var reserve = reserveService.Find(request.reserveId);
            if (reserve == null || reserve.UserID != userAccessor.CurrentUser.Id)
            {
                return NotFound(new {
                    reserveId = "reserve id is incorrect"
                });
            }

            var coupon = accounting.FindDiscountCoupon(userAccessor.CurrentUser.Id, discountCodeType);
            if (coupon == null)
            {
                coupon = accounting.InsertDiscountCoupon(userAccessor.CurrentUser.Id, discountCodeType, 5);
            }
            else
            {
                if (coupon.UsingReserveID > 0)
                {
                    return BadRequest(new {
                        discountCode = "discount code is used"
                    });
                }
            }
            var discountPrice = accounting.CalculateDiscountCouponPrice(coupon.Percent, reserve.CouponCalculationPrice);
            return Ok(new { 
                discountPrice = discountPrice
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("confirmstart/{id:long}")]
        public async Task<IActionResult> ConfirmReserveStart(long id)
        {
            var result = await reserveService.ConfirmResidenceAsync(id, userAccessor.CurrentUser.Id,
                ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
            if (result.IsValid && result.Result)
            {
                return Ok();
            }
            return BadRequest(result.ErrorMessages);
        }
    }
}
