using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Reserves;
using Amlakbashi.Core.DTOs.WebService.Responses.Reserves;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
using Amlakbashi.Host.Filters;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiReserveController : ApiBaseController
    {
        private readonly IReserveAppService reserveService;
        private readonly IReserveAutoCancelAppService reserveAutoCancelService;
        private readonly IAccountingFacade accounting;
        private readonly IUserAccessor userAccessor;
        public ApiReserveController(IReserveAppService reserveService,
            IReserveAutoCancelAppService reserveAutoCancelService,
            IAccountingFacade accounting,
            IUserAccessor userAccessor)
        {
            this.reserveService = reserveService;
            this.reserveAutoCancelService = reserveAutoCancelService;
            this.accounting = accounting;
            this.userAccessor = userAccessor;
        }

        [HttpGet]
        public ReserveListResponse Get([FromQuery] ReserveListRequest request)
        {
            request.userId = userAccessor.CurrentUser.Id;
            request.panel = User.GetUserPanelType();
            var response = reserveService.Filter(request);
            foreach (var item in response.reserveList)
            {
                item.expireTime = reserveAutoCancelService.GetReserveExpireTime(item.reserveId);
            }
            return response;
        }

        [HttpGet("{id:long}")]
        public ReserveResponse Get(long id)
        {
            ReserveResponse response = reserveService.Find(id);
            return response;
        }

        [HttpGet("invoice/{reserveId:long}")]
        public IActionResult Invoice(long reserveId)
        {
            var response = reserveService.GetInvoice(reserveId, userAccessor.CurrentUser.Id);
            if (response == null)
            {
                return BadRequest();
            }
            return Ok(response);
        }

        [HttpPost("discount")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Guest)]
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

        [HttpPost("confirmstart/{id:long}")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Guest)]
        public async Task<IActionResult> ConfirmReserveStart(long id)
        {
            var result = await reserveService.ConfirmResidenceAsync(id, userAccessor.CurrentUser.Id,
                ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
            if (result.HasError() == false && result.Result)
            {
                return Ok();
            }
            return BadRequest(result.GetErrors());
        }

        [HttpPost("hostresponse")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Host)]
        public IActionResult SubmitHostResponse(ReserveHostResponseRequest request)
        {
            var reserve = reserveService.Find(request.reserveId);
            if (reserve == null || reserve.HostUserID != userAccessor.CurrentUser.Id ||
                (reserve.Status != Reserve.ReserveStatus.WaitForResponse &&
                reserve.Status != Reserve.ReserveStatus.WaitForReserve))
            {
                return Forbid();
            }
            reserveService.SetHostResponse(request.reserveId, request.hostResponse, true,
                ActionLog.ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id);
            return Ok();
        }
    }
}
