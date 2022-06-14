using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Reserves;
using Amlakbashi.Core.DTOs.WebService.Responses.Reserves;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
using Amlakbashi.Host.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        public ApiReserveController(IReserveAppService reserveService,
            IReserveAutoCancelAppService reserveAutoCancelService,
            IAccountingFacade accounting)
        {
            this.reserveService = reserveService;
            this.reserveAutoCancelService = reserveAutoCancelService;
            this.accounting = accounting;
        }

        [HttpGet]
        public ReserveListResponse Get([FromQuery] ReserveGetListRequest request)
        {
            request.userId = User.GetId();
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

        [HttpPost]
        public async Task<IActionResult> Submit(ReservePostRequest request)
        {
            request.userId = User.GetId();

            var validateResult = reserveService.Validate(request);
            if (validateResult.HasError())
            {
                return BadRequest(validateResult.GetErrors());
            }

            var submitResult = await reserveService.SubmitAsync(request);
            return Ok(new
            {
                reserveId = submitResult.Result
            });
        }

        [HttpGet("invoice/{reserveId:long}")]
        public IActionResult Invoice(long reserveId)
        {
            var response = reserveService.GetInvoice(reserveId, User.GetId());
            if (response == null)
            {
                return BadRequest();
            }
            return Ok(response);
        }

        [HttpPost("discount")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Guest)]
        public IActionResult AddDiscountCode(ReservePostDiscountCodeRequest request)
        {
            var discountCodeType = DiscountCoupon.GetDiscountCouponType(request.discountCode);
            if (discountCodeType == DiscountCoupon.DiscountCouponType.Unset)
            {
                return NotFound(new {
                    discountCode = "discount code is incorrect"
                });
            }
            var reserve = reserveService.Find(request.reserveId);
            var userId = User.GetId();
            if (reserve == null || reserve.UserID != userId)
            {
                return NotFound(new {
                    reserveId = "reserve id is incorrect"
                });
            }

            var coupon = accounting.FindDiscountCoupon(userId, discountCodeType);
            if (coupon == null)
            {
                coupon = accounting.InsertDiscountCoupon(userId, discountCodeType, 5);
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

        [HttpPost("hostresponse")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Host)]
        public IActionResult SubmitHostResponse(ReservePostHostResponseRequest request)
        {
            var reserve = reserveService.Find(request.reserveId);
            if (reserve == null || reserve.HostUserID != User.GetId() ||
                (reserve.Status != Reserve.ReserveStatus.WaitForResponse &&
                reserve.Status != Reserve.ReserveStatus.WaitForReserve))
            {
                return BadRequest();
            }
            reserveService.SetHostResponse(request.reserveId, request.hostResponse, true,
                ActionLog.ActionSourceEnum.WebsiteDashboard, User.GetId());
            return Ok();
        }

        [HttpPost("start")]
        [Panel(Core.Entities.User.UserGeneralTypeEnum.Guest)]
        public async Task<IActionResult> Start(ReservePostStartRequest request)
        {
            request.userId = User.GetId();
            request.actionSource = ActionLog.ActionSourceEnum.WebsiteDashboard;
            var result = await reserveService.StartAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(ReservePostCancelRequest request)
        {
            request.userId = User.GetId();
            request.panel = User.GetUserPanelType();
            request.actionSource = ActionLog.ActionSourceEnum.WebsiteDashboard;
            var result = await reserveService.CancelAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }
    }
}
