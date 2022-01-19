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

            List<string> tags = new List<string>();
            tags.Add("آپارتمان");
            tags.Add("3 خوابه");
            tags.Add("تهران");
            tags.Add("تهران");

            response.residencyTypes = new Dictionary<string, int>();
            response.residencyTypes.Add("villa", 1295);
            response.residencyTypes.Add("furnished", 10893);
            response.residencyTypes.Add("ecotourism", 2666);
            response.residencyTypes.Add("hotel", 1313);

            response.mostViewed = new List<HomePageMostViewedResponse>();
            for (int i = 0; i < 10; i++)
            {
                response.mostViewed.Add(new HomePageMostViewedResponse()
                {
                    cityName = "یزد",
                    imageUrl = "/Images/AdImages/LuxuryApartment.png",
                    residencyCount = 212
                });
            }

            response.lastSeconds = new List<HomePageLastSecondsResponse>();
            for (int i = 0; i < 10; i++)
            {
                response.lastSeconds.Add(new HomePageLastSecondsResponse()
                {
                    title = "آپارتمان لاکچری استخردار شمال تهران",
                    imageUrl = "/Images/AdImages/LuxuryApartment.png",
                    tags = tags,
                    nightlyPrice = 4000000,
                    discountPercent = 17
                });
            }

            response.mostLiked = new List<HomePageMostLikedResponse>();
            for (int i = 0; i < 10; i++)
            {
                response.mostLiked.Add(new HomePageMostLikedResponse()
                {
                    title = "آپارتمان لاکچری استخردار شمال تهران",
                    imageUrl = "/Images/AdImages/LuxuryApartment.png",
                    tags = tags,
                    nightlyPrice = 4000000,
                    commentsCount = 126,
                    rating = 4.3
                });
            }

            response.instant = new List<HomePageInstantResponse>();
            for (int i = 0; i < 10; i++)
            {
                response.instant.Add(new HomePageInstantResponse()
                {
                    title = "آپارتمان لاکچری استخردار شمال تهران",
                    imageUrl = "/Images/AdImages/LuxuryApartment.png",
                    tags = tags,
                    nightlyPrice = 4000000,
                    commentsCount = 126,
                    rating = 4.3,
                    badgeText = "تحویل آنی"
                });
            }

            response.mag = new List<HomePageMagResponse>();
            for (int i = 0; i < 10; i++)
            {
                response.mag.Add(new HomePageMagResponse()
                {
                    title = "آپارتمان لاکچری استخردار شمال تهران",
                    imageUrl = "/Images/AdImages/LuxuryApartment.png",
                    link = "#",
                    summary = "...یکی ار مهمترین دلایلی که مردم شمال و جنوب کشور را برای سفر انتخاب میکنند، وجود سواحل بسیار زیبای"
                });
            }

            return Ok(response);
        }
    }
}
