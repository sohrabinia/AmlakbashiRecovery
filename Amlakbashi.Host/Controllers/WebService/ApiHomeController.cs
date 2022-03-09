using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.DTOs.WebService.Responses;
using Amlakbashi.Core.Entities;
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
        private readonly ICategoryAppService categoryService;
        private readonly IDiscountTableAppService discountTableService;
        private readonly IBlogPostAppService blogPostService;
        public ApiHomeController(IAdvertiseAppService advertiseService,
            ICategoryAppService categoryService,
            IDiscountTableAppService discountTableService,
            IBlogPostAppService blogPostService)
        {
            this.advertiseService = advertiseService;
            this.categoryService = categoryService;
            this.discountTableService = discountTableService;
            this.blogPostService = blogPostService;
        }

        [HttpGet]
        public HomePageResponse Get()
        {
            var response = new HomePageResponse();

            response.residencyTypes = new Dictionary<string, int>();
            response.residencyTypes.Add("villa", categoryService.GetForItemAction(-2, Advertise.AdvertiseType.Villa).CountAdvertise);
            response.residencyTypes.Add("furnished", categoryService.GetForItemAction(-2, Advertise.AdvertiseType.Apartment).CountAdvertise);
            response.residencyTypes.Add("ecotourism", categoryService.GetForItemAction(-2, Advertise.AdvertiseType.TourismAccommodation).CountAdvertise);
            response.residencyTypes.Add("hotel", categoryService.GetForItemAction(-2, Advertise.AdvertiseType.Hotel).CountAdvertise);

            // most viewed regions
            response.mostViewed = new List<HomePageMostViewedResponse>();
            var mostViewedDic = new Dictionary<int, string>();
            mostViewedDic.Add(55784, "tehran");
            mostViewedDic.Add(85173, "mazandaran");
            mostViewedDic.Add(55816, "kordan");
            mostViewedDic.Add(55979, "shiraz");
            mostViewedDic.Add(55978, "ramsar");
            mostViewedDic.Add(55786, "mashhad");
            mostViewedDic.Add(55827, "esfahan");
            var mostViewCategories = categoryService.GetListByIds(mostViewedDic.Keys.ToList());
            foreach (var item in mostViewCategories)
            {
                response.mostViewed.Add(new HomePageMostViewedResponse()
                {
                    cityName = item.RegionString,
                    imageUrl = $"{GeneralData.WebsiteUrl}/image/region/{mostViewedDic[item.Id]}.jpg",
                    residencyCount = item.CountAdvertise
                });
            }

            // discount advertises
            response.lastSeconds = new List<HomePageLastSecondsResponse>();
            var mostDiscountAccs = discountTableService.GetMostDiscountAdvertises(10);
            foreach (var item in mostDiscountAccs)
            {
                var discount = item.GetFirstDiscountData(true, true);
                response.lastSeconds.Add(new HomePageLastSecondsResponse()
                {
                    title = item.Title,
                    imageUrl = $"{GeneralData.WebsiteUrl}/file/accthumbxlarge?accid={item.Id}&fileid={item.PhotoID}",
                    tags = advertiseService.GetAdvertiseTags(item),
                    nightlyPrice = item.BasePrice,
                    discountPercent = discount.Percent,
                    discountPrice = item.BasePrice - (item.BasePrice * discount.Percent * 0.01)
                });
            }

            // most liked advertises
            response.mostLiked = new List<HomePageMostLikedResponse>();
            var mostLikedAdvertises = advertiseService.GetMostLiked(10);
            foreach (var item in mostLikedAdvertises)
            {
                response.mostLiked.Add(new HomePageMostLikedResponse()
                {
                    title = item.Title,
                    imageUrl = $"{GeneralData.WebsiteUrl}/file/accthumbxlarge?accid={item.Id}&fileid={item.PhotoID}",
                    tags = advertiseService.GetAdvertiseTags(item),
                    nightlyPrice = item.BasePrice,
                    commentsCount = item.PublishedComments().Count(),
                    rating = item.AverageUserRating
                });
            }

            // instant reserves advertises
            response.instant = new List<HomePageInstantResponse>();
            var mostLikedInstantReserveAdvertises = advertiseService.GetMostLiked(10, true);
            foreach (var item in mostLikedInstantReserveAdvertises)
            {
                response.instant.Add(new HomePageInstantResponse()
                {
                    title = item.Title,
                    imageUrl = $"{GeneralData.WebsiteUrl}/file/accthumbxlarge?accid={item.Id}&fileid={item.PhotoID}",
                    tags = advertiseService.GetAdvertiseTags(item),
                    nightlyPrice = item.BasePrice,
                    commentsCount = item.PublishedComments().Count(),
                    rating = item.AverageUserRating,
                    badgeText = "تحویل آنی"
                });
            }

            // blog posts
            response.mag = new List<HomePageMagResponse>();
            var blogPosts = blogPostService.GetNewItems(BlogPost.PlaceEnum.HomePage, 3);
            foreach (var item in blogPosts)
            {
                response.mag.Add(new HomePageMagResponse()
                {
                    title = item.Title,
                    imageUrl = $"{GeneralData.WebsiteUrl}/file/imgthumb?fileid={item.PhotoID}&w={331}&h={186}",
                    link = item.BlogLink,
                    summary = item.Text
                });
            }

            return response;
        }

        [HttpGet("old")]
        public async Task<IActionResult> GetOld()
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
