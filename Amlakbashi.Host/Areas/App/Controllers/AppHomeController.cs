using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.HomePageDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Area.App.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/home/[action]")]
    public class AppHomeController : AppBaseController
    {
        private readonly IUserAccessor userAccessor;
        private readonly ICategoryAppService categoryService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IDiscountTableAppService discountTableService;
        private readonly IRegionAppService regionService;
        private readonly IBlogPostAppService blogPostService;
        public AppHomeController(IUserAccessor userAccessor,
            ICategoryAppService categoryService,
            IAdvertiseAppService advertiseService,
            IDiscountTableAppService discountTableService,
            IRegionAppService regionService,
            IBlogPostAppService blogPostService)
        {
            this.userAccessor = userAccessor;
            this.categoryService = categoryService;
            this.advertiseService = advertiseService;
            this.discountTableService = discountTableService;
            this.regionService = regionService;
            this.blogPostService = blogPostService;
        }

        public static List<int> most_view_city_category_ids = new List<int> { 55784, 85173, 55816, 55979, 55978, 55786, 55827 };
        public static List<string> most_view_city_names = new List<string> { "اجاره روزانه خانه در تهران", "اجاره ویلا و سوئیت شمال", "اجاره ویلا و سوئیت کردان", "سوئیت و آپارتمان مبله شیراز", "اجاره ویلا و سوئیت رامسر", "سوئیت و آپارتمان مبله مشهد", "سوئیت و آپارتمان مبله اصفهان" };
        public static List<string> most_view_city_image_names = new List<string> { "tehran", "mazandaran", "kordan", "shiraz", "ramsar", "mashhad", "esfahan" };

#if !DEBUG
        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "amp_version" })]
#endif
        public ActionResult Main()
        {
            ViewBag.MessageShowOnReady = TempData["MessageShowOnReady"];
            ViewBag.home = true;
            var categories = new int[] { 55894, 55944, 55957, 55861, 55953 };
            var advertiseItemCount = 5;

            var user = userAccessor.CurrentUser;
            var userFavorites = user.Favorite == null ? new List<UserFavorite>() : user.Favorite.ToList();
            var home_categories = new List<HomePageCategoryDTO>();
            IList<Advertise> advertises;
            foreach (var id in categories)
            {
                var category = categoryService.Find(id);
                var home_cat = new HomePageCategoryDTO();
                home_cat.RegionId = category.GetRegionId();
                home_cat.CategoryID = category.Id;
                home_cat.category = category;
                home_cat.categoryUrl = CategoryUrlLocalization.CategoryToUrl(category);
                home_cat.Title = category.Title;
                home_cat.URL = category.URL;
                home_cat.CountAdvertise = category.CountAdvertise;
                home_cat.Advertises = new List<HomePageAdvertiseDTO>();
                home_cat.AdvertiseItems = new List<AccommodationCardDTO>();

                // Initial Advertises
                advertises = category.Advertises.OrderByDescending(o => o.ResidenceScore).Take(advertiseItemCount).ToList();
                foreach (var adv in advertises)
                {
                    var rate = adv.AverageUsersScore;
                    var review_count = adv.ReportItems.GroupBy(g => g.UserID).Count();
                    home_cat.Advertises.Add(new HomePageAdvertiseDTO()
                    {
                        Id = adv.Id,
                        Title = adv.Title,
                        Description = adv.Description,
                        ImageSource = adv.MainPhotoId > 0 ? string.Format("/عکس-آگهی/{0}", adv.Slug) : string.Format("/عکس-یافت-نشد-{0}-{1}", 240, 144),
                        Rate = rate,
                        ReviewCount = review_count
                    });
                    AccommodationCardDTO advItem = adv;
                    advItem.Favourited = user.Id > 0 && userFavorites.Any(x => x.AdvertiseID == adv.Id);
                    home_cat.AdvertiseItems.Add(advItem);
                }

                home_cat.category = category;
                home_cat.categoryUrl = CategoryUrlLocalization.CategoryToUrl(category);
                var provinceString = category.Province == null ? "" : category.RegionProvince.PersianName;
                var cityString = category.City == null ? "" : category.RegionCity.PersianName;
                var areaString = category.Area == null ? "" : category.RegionArea.PersianName;
                var countryDirectionString = Region.GetCountryDirectionString(category.CountryDirection);
                home_cat.categoryH1Title = AdvertiseSeoLocalization
                    .GetTitle(0, (int)category.Type, provinceString,
                    cityString, areaString, countryDirectionString);
                home_categories.Add(home_cat);
            }

            // Most View Regions
            var mostViewRegions = new List<MostViewRegionsDTO>();
            var mostViewCategoryList = categoryService.GetListByIds(most_view_city_category_ids);
            mostViewCategoryList = mostViewCategoryList.OrderBy(o => most_view_city_category_ids.IndexOf(o.Id)).ToList();
            var mostViewDic = regionService.GetRegionPersianNamesByCategoryList(mostViewCategoryList);
            int index = 0;
            foreach (var item in mostViewDic)
            {
                mostViewRegions.Add(new MostViewRegionsDTO()
                {
                    Title = item.Key.Title,
                    CityName = most_view_city_names[index],
                    ImageName = most_view_city_image_names[index],
                    Url = CategoryUrlLocalization.CategoryToUrl(item.Key),
                    MetaTitle = AdvertiseSeoLocalization.GetMetaTitle(0, (int)item.Key.Type, item.Value[0],
                        item.Value[1], item.Value[2], null),
                    RegionId = item.Key.GetRegionId()
                });
                index++;
            }

            // Most Discount Advertise
            var mostDiscountAccs = discountTableService.GetMostDiscountAdvertises(5);
            List<AccommodationCardDTO> itemDTOs = new List<AccommodationCardDTO>();
            foreach (var item in mostDiscountAccs)
            {
                var dto = (AccommodationCardDTO)item;
                dto.Favourited = user.Id > 0 && userFavorites.Any(x => x.AdvertiseID == item.Id);
                itemDTOs.Add(dto);
            }
            ViewBag.mostDiscountAdvertise = itemDTOs;
            var norouzItemDTOs = new List<AccommodationCardDTO>();
            var norouzAccs = advertiseService.GetNorouzAdvertises(5);
            //var norouzAccs = new List<Advertise>();
            foreach (var item in norouzAccs)
            {
                var dto = (AccommodationCardDTO)item;
                dto.Favourited = user.Id > 0 && userFavorites.Any(x => x.AdvertiseID == item.Id);
                norouzItemDTOs.Add(dto);
            }
            ViewBag.norouzAdvertises = norouzItemDTOs;

            ViewBag.advertiseItemCount = advertiseItemCount;
            ViewBag.homePageCategories = home_categories;
            ViewBag.blogPostNews = blogPostService.GetNewItems(BlogPost.PlaceEnum.HomePage, 3);
            return View(mostViewRegions);
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            ViewBag.userId = userAccessor.CurrentUser.Id;
            ViewBag.userGeneralType = userAccessor.CurrentUser.Type;
            ViewBag.alert_msg = TempData["alert"];
            return View();
        }
    }
}
