using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.DTOs.WebService;
using Amlakbashi.Core.DTOs.WebService.Responses;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api")]
    public class ApiHomeController : ApiBaseController
    {
        private readonly IAdvertiseAppService residenceService;
        private readonly ICategoryAppService categoryService;
        private readonly IDiscountTableAppService discountTableService;
        private readonly IBlogPostAppService blogPostService;
        private readonly IPostAppService postService;
        public ApiHomeController(IAdvertiseAppService residenceService,
            ICategoryAppService categoryService,
            IDiscountTableAppService discountTableService,
            IBlogPostAppService blogPostService,
            IPostAppService postService)
        {
            this.residenceService = residenceService;
            this.categoryService = categoryService;
            this.discountTableService = discountTableService;
            this.blogPostService = blogPostService;
            this.postService = postService;
        }

        [HttpGet("home")]
        public HomePageResponse HomePage()
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
                    imageUrl = $"{GeneralData.WebsiteUrl}/file/accthumbxlarge?accid={item.Id}&fileid={item.MainPhotoId}",
                    tags = residenceService.GetAdvertiseTags(item),
                    nightlyPrice = item.BasePrice,
                    discountPercent = discount.Percent,
                    discountPrice = item.BasePrice - (item.BasePrice * discount.Percent * 0.01)
                });
            }

            // most liked advertises
            response.mostLiked = new List<HomePageMostLikedResponse>();
            var mostLikedAdvertises = residenceService.GetMostLiked(10);
            foreach (var item in mostLikedAdvertises)
            {
                response.mostLiked.Add(new HomePageMostLikedResponse()
                {
                    title = item.Title,
                    imageUrl = $"{GeneralData.WebsiteUrl}/file/accthumbxlarge?accid={item.Id}&fileid={item.MainPhotoId}",
                    tags = residenceService.GetAdvertiseTags(item),
                    nightlyPrice = item.BasePrice,
                    commentsCount = item.PublishedComments().Count(),
                    rating = item.AverageUsersScore
                });
            }

            // instant reserves advertises
            response.instant = new List<HomePageInstantResponse>();
            var mostLikedInstantReserveAdvertises = residenceService.GetMostLiked(10, true);
            foreach (var item in mostLikedInstantReserveAdvertises)
            {
                response.instant.Add(new HomePageInstantResponse()
                {
                    title = item.Title,
                    imageUrl = $"{GeneralData.WebsiteUrl}/file/accthumbxlarge?accid={item.Id}&fileid={item.MainPhotoId}",
                    tags = residenceService.GetAdvertiseTags(item),
                    nightlyPrice = item.BasePrice,
                    commentsCount = item.PublishedComments().Count(),
                    rating = item.AverageUsersScore,
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

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            var supportPhoneNumbers = GeneralLocalization.GetSupportPhoneNumbers();
            var address = GeneralLocalization.GetAmlakbashiAddress();
            var supportChatAutoQuestions = Enum.GetValues<SupportChat.AutoQuestion>().ToList();
            Dictionary<string, string> questions = new Dictionary<string, string>();
            foreach (var item in supportChatAutoQuestions)
            {
                questions.Add(SupportChatLocalization.GetQuestionTitle(item),
                    SupportChatLocalization.GetQuestionText(item));
            }
            return Ok(new
            {
                faq = questions,
                address = address,
                phoneNumbers = supportPhoneNumbers
            });
        }

        [HttpGet("{title}")]
        public IActionResult GetPost(string title)
        {
            int postId = 0;
            switch (title)
            {
                case "faq":
                    return Ok(GeneralLocalization.GetFaq());
                case "about":
                    postId = 4;
                    break;
                case "help":
                    postId = 8;
                    break;
                case "feedback":
                    postId = 24;
                    break;
                case "rules":
                    postId = 25;
                    break;
                default:
                    return NotFound();
            }
            var post = postService.Filter(Post.PostStatus.Published, postId).FirstOrDefault();
            return Ok(new
            {
                title = post.Title,
                description = post.Description
            });
        }

        [HttpGet("namevalue")]
        public IEnumerable<NameValueDTO> GetEnumNameValues(NameValueDTO.EnumType type = 0)
        {
            return NameValueDTO.GetEnumNameValues(type);
        }
    }
}
