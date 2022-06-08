using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/tp")]
    [AuthorizeThirdPartyApp]
    [EnableCors("thirdPartyCorsPolicy")]
    public class ApiThirdPartyController : ApiBaseController
    {
        private readonly IAdvertiseAppService advertiseService;
        private readonly IPriceTableAppService priceTableService;

        public ApiThirdPartyController(IAdvertiseAppService advertiseService,
            IPriceTableAppService priceTableService)
        {
            this.advertiseService = advertiseService;
            this.priceTableService = priceTableService;
        }

        [HttpPost("advertise/calendar")]
        public async Task<IActionResult> UpdateAdvertiseCalendar(AdvertiseUpdateCalendarRequest request)
        {
            request.actionSource = ActionLog.ActionSourceEnum.ThirdPartyApp;
            var result = await advertiseService.UpdateCalendarAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpPost("advertise/manualprice")]
        public IActionResult UpdateAdvertiseManualPrice(AdvertiseUpdatePriceRequest request)
        {
            var result = priceTableService.UpdateAdvertiseManualPrices(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }
    }
}
