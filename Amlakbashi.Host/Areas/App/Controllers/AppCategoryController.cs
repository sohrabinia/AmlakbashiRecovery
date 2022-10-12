using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.CategoryDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Area.App.Controllers.Base;
using Amlakbashi.Host.Authentication;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/category/[action]")]
    public class AppCategoryController : AppBaseController
    {
        private readonly ICategoryAppService categoryService;
        private readonly IRegionAppService regionService;
        private readonly ICacheManager cacheManager;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        public AppCategoryController(ICategoryAppService categoryService,
            IRegionAppService regionService,
            ICacheManager cacheManager,
            IUserAccessor userAccessor,
            ILog logger)
        {
            this.categoryService = categoryService;
            this.regionService = regionService;
            this.cacheManager = cacheManager;
            this.userAccessor = userAccessor;
            this.logger = logger;
        }

        public ActionResult Item(int regionType = 0,
            int regionId = 0,
            int type = 81, int countryDirection = 0,
            int page = 1, string discount_homes = null, string today_empty_homes = null,
            string frompaypernight = null, string topaypernight = null,
            string fromMetrazh = null, string toMetrazh = null,
            string region = null, string capacity = null,
            string room = null, string elevator = null, string pool = null,
            string empty_range_from = null, string empty_range_to = null,
            string norouz_special = null, string instant_reserve = null,
            int t = -1, int priceRangeType = 0, int wcType = -1, int exclusive = 0,
            int wifi = 0, int washingMachine = 0, int jacuzzi = 0,
            int poolTable = 0, int foosball = 0, int teaMaker = 0, int filming = 0,
            int rules_pets = 0, int rules_party = 0, int rules_smoking = 0,
            int parking = 0, int sort = 0, string roomList = "",
            string phrase = "", string hygieneProtocol = null, bool ajax = false, string path = null)
        {
            try
            {
                var rawUrl = HttpContext.Request.Path.Value;
                rawUrl = string.IsNullOrEmpty(HttpContext.Request.QueryString.Value) ?
                    rawUrl : rawUrl + HttpContext.Request.QueryString.Value;
                if (ajax == false)
                {
                    if (rawUrl.Last() == '/')
                    {
                        return RedirectPermanent(HtmlUtility.EncodeUrlForRedirect(rawUrl.Remove(rawUrl.Length - 1)));
                    }
                    if (page == 1 && HttpContext.Request.QueryString.Value.Contains("?page=1"))
                    {
                        return RedirectPermanent(HtmlUtility.EncodeUrlForRedirect(HtmlUtility.RemoveFromQueryString(rawUrl, "page", "1")));
                    }
                }
                Region targetLocation = null;
                DynamicCategory category = null;
                DynamicCategory subCategory = null;
                //var typeInt = Advertise.UrlStringToAdvertiseType(type);
                if (type == -1)
                {
                    if (ajax == true)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            url = path
                        });
                    }
                    else
                    {
                        return NotFound("صفحه ی مورد نظر موجود نمی باشد.");
                    }
                }
                if (regionType == -2)
                {
                    category = categoryService.GetForItemAction(regionType, (Advertise.AdvertiseType)type);
                }
                else if (regionType == -1)
                {
                    category = categoryService.GetForItemAction(regionType, (Advertise.AdvertiseType)type, (Region.CountryDirection)countryDirection);
                }
                else if (regionType == 0)
                {
                    targetLocation = regionService.Find(regionId);
                    if (targetLocation == null)
                    {
                        if (ajax)
                        {
                            return GenerateJsonResult(new
                            {
                                status = 0,
                                url = path
                            });
                        }
                        else
                        {
                            return NotFound("صفحه ی مورد نظر موجود نمی باشد.");
                        }
                    }
                    if (targetLocation.Type == 0)
                    {
                        //if (name != "استان-" + targetLocation.PersianName.Trim().Replace(" ", "-"))
                        //{
                        //    if (ajax)
                        //    {
                        //        return GenerateJsonResult(new
                        //        {
                        //            status = 0,
                        //            url = path
                        //        });
                        //    }
                        //    else
                        //    {
                        //        return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                        //    }
                        //}
                        category = categoryService.GetForItemAction(regionType, (Advertise.AdvertiseType)type, 0, targetLocation.Id, 0, 0);
                    }
                    else if (targetLocation.Type == 1)
                    {
                        //if (name != targetLocation.PersianName.Trim().Replace(" ", "-"))
                        //{
                        //    if (ajax)
                        //    {
                        //        return GenerateJsonResult(new
                        //        {
                        //            status = 0,
                        //            url = path
                        //        });
                        //    }
                        //    else
                        //    {
                        //        return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                        //    }
                        //}
                        category = categoryService.GetForItemAction(regionType, (Advertise.AdvertiseType)type, 0, 0, targetLocation.Id, 0);
                    }
                    else
                    {
                        //if (name != targetLocation.PersianName.Trim().Replace(" ", "-"))
                        //{
                        //    if (ajax)
                        //    {
                        //        return GenerateJsonResult(new
                        //        {
                        //            status = 0,
                        //            url = path
                        //        });
                        //    }
                        //    else
                        //    {
                        //        return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                        //    }
                        //}
                        category = categoryService.GetForItemAction(regionType, (Advertise.AdvertiseType)type, 0, 0, targetLocation.ParentID == null ? 0 : (int)targetLocation.ParentID, 0);
                        subCategory = categoryService.GetForItemAction(regionType, (Advertise.AdvertiseType)type, 0, 0, 0, targetLocation.Id);
                    }
                }
                if (category == null)
                {
                    if (ajax)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            url = path
                        });
                    }
                    else
                    {
                        return NotFound("صفحه ی مورد نظر موجود نمی باشد.");
                    }
                }
                var area = targetLocation != null && targetLocation.Type == 2 ?
                    targetLocation.Id : -1;
                List<int> deserializedRoomList = null;
                if (!string.IsNullOrEmpty(roomList))
                {
                    deserializedRoomList = Array.ConvertAll(
                        roomList.Split(','), x => int.Parse(x)).ToList();
                }
                if (!string.IsNullOrEmpty(phrase) && phrase.Last() == '-')
                    phrase = phrase.Remove(phrase.Length - 1, 1);

                // read from redis cache
                bool canUseCache = string.IsNullOrEmpty(phrase) && area < 1 &&
                    ((ajax == false && string.IsNullOrEmpty(Request.QueryString.Value)) ||
                    (ajax == true && path.Contains("?") == false));
                var cachedName = $"{CacheNames.Category_Item_}{category.Id}";
                if (canUseCache)
                {
                    var cachedData = cacheManager.Get<CategoryItemDTO>(cachedName);
                    if (cachedData != null)
                    {
                        if (ajax)
                        {
                            return PartialView("_AdvertiseListItems", cachedData);
                        }
                        else
                        {
                            return View(cachedData);
                        }
                    }
                }

                CategoryItemDTO categoryItemDTO = new CategoryItemDTO();
                var model = categoryService.GetFilteredAdvertises(category.Id,
                    area, frompaypernight, topaypernight, null, region, capacity,
                    room, elevator, pool, norouz_special, today_empty_homes,
                    empty_range_from, empty_range_to, discount_homes, instant_reserve,
                    t, (Advertise.priceRangeTypes)priceRangeType,
                    wcType, wifi == 1, washingMachine == 1, jacuzzi == 1, poolTable == 1,
                    foosball == 1, teaMaker == 1, filming == 1, exclusive == 1,
                    rules_pets == 1, rules_party == 1, rules_smoking == 1,
                    parking == 1, sort, deserializedRoomList, phrase.Replace("-", " "),
                    string.IsNullOrEmpty(hygieneProtocol) == false && hygieneProtocol == "1");

                categoryItemDTO.RawUrl = ajax ? path.Split('?')[0] : HttpContext.Request.Path.Value;
                categoryItemDTO.UrlWithParameters = ajax ? path : rawUrl;
                categoryItemDTO.Category = category;
                categoryItemDTO.Area = area;
                var provinceString = category.Province == null ? "" : category.RegionProvince.PersianName;
                var cityString = category.City == null ? "" : category.RegionCity.PersianName;
                var areaString = category.Area == null ? "" : category.RegionArea.PersianName;
                var countryDirectionString = Region.GetCountryDirectionString(category.CountryDirection);
                categoryItemDTO.ProvinceString = provinceString;
                categoryItemDTO.CityString = cityString;
                categoryItemDTO.AreaString = areaString;
                categoryItemDTO.CountryDirectionString = countryDirectionString;
                categoryItemDTO.CategoryH1Title = AdvertiseSeoLocalization
                    .GetTitle(category.MostAccType,
                    (int)category.Type, provinceString,
                    cityString, areaString, countryDirectionString);
                categoryItemDTO.Phrase = phrase;
                categoryItemDTO.FromPayPerNight = frompaypernight;
                categoryItemDTO.ToPayPerNight = topaypernight;
                categoryItemDTO.FromMetrazh = fromMetrazh;
                categoryItemDTO.ToMetrazh = toMetrazh;
                categoryItemDTO.Parking = parking;
                categoryItemDTO.HygieneProtocol = hygieneProtocol != null && hygieneProtocol == "1";
                categoryItemDTO.Region = region;
                categoryItemDTO.Capacity = capacity;
                categoryItemDTO.Room = room;
                categoryItemDTO.Elevator = elevator;
                categoryItemDTO.Pool = pool;
                categoryItemDTO.PriceRangeType = priceRangeType;
                categoryItemDTO.WcType = wcType;
                categoryItemDTO.Wifi = wifi;
                categoryItemDTO.WashingMachine = washingMachine;
                categoryItemDTO.Jacuzzi = jacuzzi;
                categoryItemDTO.PoolTable = poolTable;
                categoryItemDTO.Foosball = foosball;
                categoryItemDTO.TeaMaker = teaMaker;
                categoryItemDTO.Filming = filming;
                categoryItemDTO.Exclusive = exclusive;
                categoryItemDTO.RulesPets = rules_pets;
                categoryItemDTO.RulesParty = rules_party;
                categoryItemDTO.RulesSmoking = rules_smoking;
                categoryItemDTO.NorouzSpecial = norouz_special != null && norouz_special == "1";
                categoryItemDTO.TodayEmptyHomes = today_empty_homes != null && today_empty_homes == "1";
                categoryItemDTO.EmptyRangeFrom = empty_range_from;
                categoryItemDTO.EmptyRangeTo = empty_range_to;
                categoryItemDTO.DiscountHomes = discount_homes != null && discount_homes == "1";
                categoryItemDTO.InstantReserve = instant_reserve != null && instant_reserve == "1";
                categoryItemDTO.RoomList = roomList;
                categoryItemDTO.Type = (int)category.ParentAccType;
                categoryItemDTO.T = t;
                categoryItemDTO.Sort = sort;
                var areaRegionRelated = area < 1 ? null : regionService.Find(area).Related;
                var areaRegionRelatedIds = string.IsNullOrEmpty(areaRegionRelated) ? null : Array.ConvertAll(
                            areaRegionRelated.Trim(',').Split(','), x => int.Parse(x));
                categoryItemDTO.RelatedCategories = subCategory != null ?
                    categoryService.GetRelatedCategories(subCategory.Id, areaRegionRelatedIds, model.Count())
                    : (category == null ? new List<DynamicCategory>() :
                    categoryService.GetRelatedCategories(category.Id, areaRegionRelatedIds, model.Count()));

                categoryItemDTO.AnyTodayEmpty = today_empty_homes == "1" || model.Any(x => x.EmptyTonight || x.Childs.All(y => y.EmptyTonight));

                var pages_count = Math.Max(1, Math.Ceiling((float)((float)model.Count() / 12f)));
                categoryItemDTO.PagesCount = pages_count;
                categoryItemDTO.CurrentPageNumber = page;

                categoryItemDTO.Title = AdvertiseSeoLocalization.GetMetaTitle(category.MostAccType, (int)category.Type, provinceString, cityString, null, countryDirectionString);
                categoryItemDTO.Keywords = AdvertiseSeoLocalization.GetKeywords((int)category.Type, provinceString, cityString, areaString, countryDirectionString);
                categoryItemDTO.Description = AdvertiseSeoLocalization.GetDescription(category.MostAccType, (int)category.Type, provinceString, cityString, areaString, countryDirectionString);
                categoryItemDTO.AccTypeUrlString = AdvertiseUrlLocalization.AdvertiseTypeToUrlString((int)category.Type);

                categoryItemDTO.CategoryFaqTrustQuestion = CategoryFaqLocalization.CategoryFaqTrustQuestion(category);
                categoryItemDTO.CategoryFaqTrustAnswer = CategoryFaqLocalization.CategoryFaqTrustAnswer();
                categoryItemDTO.CategoryFaqPriceQuestion = CategoryFaqLocalization.CategoryFaqPriceQuestion(category);
                categoryItemDTO.CategoryFaqPriceAnswer = CategoryFaqLocalization.CategoryFaqPriceAnswer(category);
                categoryItemDTO.CategoryFaqAreasQuestion = CategoryFaqLocalization.CategoryFaqAreasQuestion(category);
                categoryItemDTO.CategoryFaqAreasAnswer = CategoryFaqLocalization.CategoryFaqAreasAnswer(category);
                categoryItemDTO.CategoryFaqEvidenceQuestion = CategoryFaqLocalization.CategoryFaqEvidenceQuestion(category);
                categoryItemDTO.CategoryFaqEvidenceAnswer = CategoryFaqLocalization.CategoryFaqEvidenceAnswer();
                categoryItemDTO.CategoryFaqReserveQuestion = CategoryFaqLocalization.CategoryFaqReserveQuestion(category);
                categoryItemDTO.CategoryFaqReserveAnswer = CategoryFaqLocalization.CategoryFaqReserveAnswer(category);
                categoryItemDTO.CategoryFaqHostQuestion = CategoryFaqLocalization.CategoryFaqHostQuestion();
                categoryItemDTO.CategoryFaqHostAnswer = CategoryFaqLocalization.CategoryFaqHostAnswer();

                categoryItemDTO.PriceOptions = new int[]
                {
                    30000, 50000, 100000, 150000,200000,
                    250000, 300000, 350000, 400000, 500000,
                    600000, 700000, 800000, 900000, 1000000,
                    1200000, 1400000, 1600000, 1800000,
                    2000000, 2500000, 3500000, 4000000,
                    5000000, 10000000
                };
                categoryItemDTO.MonthlyPriceOptions = new int[]
                {
                    300000, 1000000, 2000000, 3000000,
                    4000000, 5000000, 6000000, 7000000,
                    8000000, 9000000, 10000000, 11000000,
                    12000000, 14000000, 16000000, 18000000,
                    20000000, 25000000, 30000000, 40000000,
                    45000000, 50000000, 150000000
                };

                var dateString = "";
                if (!string.IsNullOrEmpty(empty_range_from))
                {
                    dateString = StringUtility.EnglishNumberToPersian(empty_range_from.Substring(5));
                }
                if (!string.IsNullOrEmpty(empty_range_to))
                {
                    if (dateString != "")
                    {
                        dateString += " تا ";
                    }
                    dateString += StringUtility.EnglishNumberToPersian(empty_range_to.Substring(5));
                }
                categoryItemDTO.DateString = dateString;

                var priceMin = priceRangeType == 3 ? 300000 : 30000;
                var priceMax = priceRangeType == 3 ? 150000000 : 10000000;
                int priceInt = 0;
                if (frompaypernight != null)
                {
                    priceInt = int.Parse(frompaypernight);
                    if (priceInt > 0)
                    {
                        priceMin = priceInt;
                        //priceMinString = String.Format("{0:n0}", priceInt) + " تومان";
                    }
                }
                categoryItemDTO.PriceMin = priceMin;
                if (topaypernight != null)
                {
                    priceInt = int.Parse(topaypernight);
                    if (priceInt > 0)
                    {
                        //priceMaxString = String.Format("{0:n0}", priceInt) + " تومان";
                        priceMax = priceInt;
                    }
                }
                categoryItemDTO.PriceMax = priceMax;

                var positionArray = (Advertise.PositionType[])(Enum.GetValues(typeof(Advertise.PositionType)));
                var positionItems = positionArray.ToList();
                positionItems.Remove(Advertise.PositionType.none);
                categoryItemDTO.PositionItems = positionItems;

                categoryItemDTO.RegionString = area > 0 ? areaString :
                    (category.Province != null || category.CountryDirection > 0 ? category.RegionString : "");

                var priceTypeString = priceRangeType == 0 ? "" :
                    priceRangeType == 1 ? "قیمت تعطیلات " :
                    priceRangeType == 2 ? "قیمت پیک تعطیلات " : "قیمت ماهانه ";
                var priceString = "";
                if (frompaypernight != null)
                {
                    var price = int.Parse(frompaypernight);
                    priceString = priceTypeString + "از " + PriceUtility.GetPriceString(price);
                }
                if (topaypernight != null)
                {
                    var price = int.Parse(topaypernight);
                    if (priceString != "")
                    {
                        priceString += " تا " + PriceUtility.GetPriceString(price);
                    }
                    else
                    {
                        priceString = priceTypeString + "تا " + PriceUtility.GetPriceString(price);
                    }
                }
                categoryItemDTO.PriceString = priceString;

                categoryItemDTO.AccTypeString = t > 0 ?
                    AdvertiseMainLocalization.GetAdvertiseTypePersianNameForUser((Advertise.AdvertiseType)t) :
                    (category.Type != Advertise.AdvertiseType.All ? category.TypeString : "");

                List<int> roomListIds = null;
                if (!string.IsNullOrEmpty(roomList))
                {
                    roomListIds = Array.ConvertAll(roomList.Split(','), x => int.Parse(x)).ToList();
                }
                categoryItemDTO.RoomListIds = roomListIds;

                string queryString = null;
                if (!string.IsNullOrEmpty(capacity) || dateString != "")
                {
                    if (!string.IsNullOrEmpty(empty_range_from))
                    {
                        queryString = "?";
                        queryString += ("empty_range_from=" + empty_range_from);
                    }
                    if (!string.IsNullOrEmpty(empty_range_to))
                    {
                        if (queryString == null)
                        {
                            queryString = "?";
                        }
                        else
                        {
                            queryString += "&";
                        }
                        queryString += ("empty_range_to=" + empty_range_to);
                    }
                    if (!string.IsNullOrEmpty(capacity))
                    {
                        if (queryString == null)
                        {
                            queryString = "?";
                        }
                        else
                        {
                            queryString += "&";
                        }
                        queryString += ("capacity=" + (capacity == null ? "0" : capacity));
                    }
                }
                categoryItemDTO.QueryString = queryString;

                if (page > pages_count || page < 1)
                {
                    if (ajax)
                    {
                        return GenerateJsonResult(new
                        {
                            status = 0,
                            url = path
                        });
                    }
                    else
                    {
                        return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                    }
                }

                categoryItemDTO.Provinces = regionService.GetByType(Region.AdvertiseRegion.Province);
                categoryItemDTO.Cities = regionService.GetChildren(category.Province == null ? 0 : (int)category.Province, Region.RegionStatus.HasAdvertise);
                categoryItemDTO.Areas = regionService.GetChildren(category.City == null ? 0 : (int)category.City, Region.RegionStatus.HasAdvertise);
                categoryItemDTO.ForAdvertisePage = true;

                model = model.Skip(12 * (page - 1)).Take(12);
                var user = userAccessor.CurrentUser;
                var userFavorite = user.Id > 0 ? user.Favorite : new List<UserFavorite>();
                categoryItemDTO.AdvertiseItems = new List<AccommodationCardDTO>();
                foreach (var item in model)
                {
                    var dto = (AccommodationCardDTO)item;
                    dto.Favourited = user.Id > 0 && userFavorite.Any(x => x.AdvertiseID == item.Id);
                    categoryItemDTO.AdvertiseItems.Add(dto);
                }

                if (canUseCache)
                {
                    cacheManager.Set(cachedName, categoryItemDTO);
                }

                if (ajax)
                {
                    return PartialView("_AdvertiseListItems", categoryItemDTO);
                }
                else
                {
                    return View(categoryItemDTO);
                }
            }
            catch (Exception exc)
            {
                logger.Error("Category.Item", exc);
                if (ajax)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        url = path
                    });
                }
                else
                {
                    return NotFound("صفحه ی مورد نظر موجود نمی باشد");
                }
            }
        }

        public ActionResult Category(int Type,
            int province = 0, int city = 0, int area = 0,
            bool norouz_special = false, bool today_empty_homes = false,
            bool discount_homes = false, int country_direction = 0, int page = 1,
            bool instant_reserve = false, int t = -1, int priceRangeType = 0,
            int perWC = 0, int euWC = 0, int filming = 0, int exclusive = 0,
            int wifi = 0, int washingMachine = 0, int jacuzzi = 0,
            int poolTable = 0, int foosball = 0, int teaMaker = 0,
            int rules_pets = 0, int rules_party = 0, int rules_smoking = 0,
            int parking = 0, int sort = 0, string roomList = "",
            int frompaypernight = -1, int topaypernight = -1,
            int fromMetrazh = -1, int toMetrazh = -1,
            int region = -1, int capacity = -1,
            int room = -1, int elevator = -1, int pool = -1,
            string empty_range_from = "", string empty_range_to = "",
            string phrase = "", bool hygieneProtocol = false, bool ajax = false)
        {
            province = Math.Max(province, 0);
            city = Math.Max(city, 0);
            area = Math.Max(area, 0);
            int regionType;
            if (province > 0)
            {
                regionType = 0;
            }
            else
            {
                if (country_direction > 0)
                {
                    regionType = -1;
                }
                else
                {
                    regionType = -2;
                }
            }
            int regionId = 0;
            string name = "";
            var type = AdvertiseUrlLocalization.AdvertiseTypeToUrlString(Type);
            if (province > 0)
            {
                country_direction = 0;
            }
            string url = null;
            Region location = null;
            if (province > 0)
                location = regionService.Find(area > 0 ? area : (city > 0 ? city : province));
            var locationString = location == null ? null :
                (
                    location.Type == 0 ? "استان-" + location.PersianName.Trim().Replace(" ", "-") :
                    location.PersianName.Trim().Replace(" ", "-")
                );
            //var category = _db.DynamicCategories.First(x => x.Type == Type &&
            //    x.Province == province && x.City == city &&
            //    x.Area == area && x.CountryDirection == country_direction);
            if (!string.IsNullOrEmpty(phrase) && province < 1)
            {
                if (phrase.Last() == '-')
                    phrase = phrase.Remove(phrase.Length - 1, 1);
                url = string.Format("/s/p/{0}{1}", phrase,
                    Type == 81 ? "" : "/" + AdvertiseUrlLocalization.AdvertiseTypeToUrlString(Type));
            }
            else
            {
                url = string.Format("/{0}{1}",
                location != null ? "s/" + location.Id + "/" + locationString :
                    (country_direction == 1 ? "شمال" : "ایران"),
                    Type == 81 ? "" : "/" + AdvertiseUrlLocalization.AdvertiseTypeToUrlString(Type));
                regionId = location != null ? location.Id : 0;
                name = locationString;
            }
            string query_string = "";
            if (page > 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "page", page.ToString());
            }
            if (t > 0)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "t", t.ToString());
            }
            if (sort > 0)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "sort", sort.ToString());
            }
            if (discount_homes)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "discount_homes", "1");
            }
            if (instant_reserve)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "instant_reserve", "1");
            }
            if (today_empty_homes)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "today_empty_homes", "1");
            }
            if (norouz_special)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "norouz_special", "1");
            }
            var fromPriceAvailable = frompaypernight.ToString() != "-1";
            var toPriceAvailable = topaypernight.ToString() != "-1";
            if (fromPriceAvailable)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "frompaypernight", frompaypernight.ToString());
            }
            if (toPriceAvailable)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "topaypernight", topaypernight.ToString());
            }
            if (fromMetrazh.ToString() != "-1")
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "fromMetrazh", fromMetrazh.ToString());
            }
            if (toMetrazh.ToString() != "-1")
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "toMetrazh", toMetrazh.ToString());
            }
            if (region.ToString() != "-1")
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "region", region.ToString());
            }
            if (capacity.ToString() != "-1")
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "capacity", capacity.ToString());
            }
            if (room.ToString() != "-1")
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "room", room.ToString());
            }
            if (!string.IsNullOrEmpty(roomList))
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "roomList", roomList);
            }
            if (elevator.ToString() != "-1")
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "elevator", elevator.ToString());
            }
            if (pool.ToString() != "-1")
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "pool", pool.ToString());
            }
            if (string.IsNullOrEmpty(empty_range_from) == false)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "empty_range_from", empty_range_from.ToString());
            }
            if (string.IsNullOrEmpty(empty_range_to) == false)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "empty_range_to", empty_range_to.ToString());
            }
            if (priceRangeType > 0 && (fromPriceAvailable || toPriceAvailable))
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "priceRangeType", priceRangeType.ToString());
            }
            var wcType = -1;
            if (perWC == 1 || euWC == 1)
            {
                if (perWC != 1)
                {
                    query_string = HtmlUtility.AddToQueryString(query_string,
                        "wcType", "129");
                    wcType = 129;
                }
                else if (euWC != 1)
                {
                    query_string = HtmlUtility.AddToQueryString(query_string,
                        "wcType", "127");
                    wcType = 127;
                }
                else
                {
                    query_string = HtmlUtility.AddToQueryString(query_string,
                        "wcType", "128");
                    wcType = 128;
                }
            }
            if (wifi == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "wifi", "1");
            }
            if (washingMachine == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "washingMachine", "1");
            }
            if (jacuzzi == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "jacuzzi", "1");
            }
            if (poolTable == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "poolTable", "1");
            }
            if (foosball == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "foosball", "1");
            }
            if (teaMaker == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "teaMaker", "1");
            }
            if (filming == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "filming", "1");
            }
            if (exclusive == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "exclusive", "1");
            }
            if (rules_pets == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "rules_pets", "1");
            }
            if (rules_party == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "rules_party", "1");
            }
            if (rules_smoking == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "rules_smoking", "1");
            }
            if (parking == 1)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "parking", "1");
            }
            if (hygieneProtocol)
            {
                query_string = HtmlUtility.AddToQueryString(query_string,
                    "hygieneProtocol", "1");
            }
            if (url[url.Length - 1] == '/')
            {
                url = url.Remove(url.Length - 1, 1);
            }
            var discountHomes = discount_homes ? 1 : 0;
            var todayEmpty = today_empty_homes ? 1 : 0;
            var instantReserve = instant_reserve ? 1 : 0;
            var norouzSpecial = norouz_special ? 1 : 0;
            var path = url + query_string;
            return RedirectToAction(nameof(Item), new
            {
                regionType,
                regionId,
                name,
                type,
                norouz_special = norouzSpecial,
                today_empty_homes = todayEmpty,
                discount_homes = discountHomes,
                countryDirection = country_direction,
                page,
                instant_reserve = instantReserve,
                t,
                priceRangeType,
                wcType,
                wifi,
                washingMachine,
                jacuzzi,
                poolTable,
                foosball,
                teaMaker,
                filming,
                exclusive,
                rules_pets,
                rules_party,
                rules_smoking,
                parking,
                sort,
                roomList,
                phrase,
                frompaypernight,
                topaypernight,
                fromMetrazh,
                toMetrazh,
                region,
                capacity,
                room,
                elevator,
                pool,
                empty_range_from,
                empty_range_to,
                hygieneProtocol = hygieneProtocol ? 1 : 0,
                ajax = true,
                path
            });
            //return Redirect(HtmlUtility.EncodeUrlForRedirect(path));
            //var redirectUri = WebUtility.UrlDecode(HttpContext.Request.Host + path);
            //Uri redirectURI = new Uri(HttpContext.Request.Host + path);
            //return Redirect(redirectURI.AbsoluteUri);
        }

        [ResponseCache(Duration = 60 * 15, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult Search(string phrase = "",
            string province = "-1", string city = "-1", string area = "-1")
        {
            ViewBag.Province = province;
            ViewBag.City = city;
            ViewBag.Area = area;
            ViewBag.search_string = phrase;
            try
            {
                var regions = regionService.GetBySearchRegion(phrase);
                if (regions.Any() == false && string.IsNullOrEmpty(phrase) == false)
                    ViewBag.showNotFound = true;
                var model = new List<SearchTableDTO>();
                foreach (var item in regions)
                {
                    model.Add(SearchTableDTO.GenerateForApp(item, regionService.GetRegionName(item.Type != 2 ? 0 : (int)item.ParentID)));
                }
                return PartialView("_Search", model);
            }
            catch (Exception exc)
            {
                logger.Error("App.Category.Search", exc);
                ViewBag.showNotFound = true;
                return PartialView("_Search", null);
            }
        }

        public JsonResult RegionSearchToUrl(int province, int city, int area)
        {
            try
            {
                string url = null;
                Region region = null;
                string title = null;
                if (area > 0)
                {
                    region = regionService.Find(area);
                }
                else if (city > 0)
                {
                    region = regionService.Find(city);
                }
                else if (province > 0)
                {
                    region = regionService.Find(province);
                }
                if (region == null)
                {
                    title = "";
                    url = "/app/category/item?regiontype=-2";
                }
                else
                {
                    url = $"/app/category/item?regiontype=0&regionid={region.Id}";
                    title = region.PersianName;
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    url = url,
                    title = title
                });
            }
            catch (Exception exc)
            {
                logger.Error("App.Category.RegionSearchToUrl", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }
    }
}
