using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/mediator")]
    [EnableCors("frontendCorsPolicy")]
    public class ApiMediatorController : ApiBaseController
    {
        private readonly IAdvertiseAppService advertiseService;
        private readonly IPriceTableAppService priceTableService;

        public ApiMediatorController(IAdvertiseAppService advertiseService,
            IPriceTableAppService priceTableService)
        {
            this.advertiseService = advertiseService;
            this.priceTableService = priceTableService;
        }

        [HttpPost("advertise/calendar")]
        public async Task<IActionResult> UpdateAdvertiseCalendar(AdvertiseUpdateCalendarRequest request)
        {
            request.actionSource = ActionLog.ActionSourceEnum.MediatorApi;
            var result = await advertiseService.UpdateCalendarAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpPost("advertise/manualprice")]
        public async Task<IActionResult> UpdateAdvertiseManualPrice(AdvertiseUpdatePriceRequest request)
        {
            string msg;
            var done = priceTableService.SetAccommodationPriceInDate(
                request.advertiseId, request.fromDate, request.toDate, request.price, out msg);
            if (done == false)
            {
                return BadRequest(msg);
            }
            return Ok();
        }
    }
}
