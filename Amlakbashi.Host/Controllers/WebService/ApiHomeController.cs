using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Responses;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/home")]
    public class ApiHomeController : ApiBaseController
    {
        private readonly IAdvertiseAppService advertiseService;
        public ApiHomeController(IAdvertiseAppService advertiseService)
        {
            this.advertiseService = advertiseService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = new HomePageResponse();

            response.residencyTypes = new Dictionary<string, int>();
            response.residencyTypes.Add("villa", 1295);
            response.residencyTypes.Add("furnished", 10893);
            response.residencyTypes.Add("ecotourism", 2666);
            response.residencyTypes.Add("hotel", 1313);

            response.mostViewed = new List<HomePageMostViewedResponse>();
            response.mostViewed.Add(new HomePageMostViewedResponse()
            {
                cityName = "یزد",
                imageUrl = "/Images/AdImages/LuxuryApartment.png",
                residencyCount = 212
            });

            return Ok(response);
        }
    }
}
