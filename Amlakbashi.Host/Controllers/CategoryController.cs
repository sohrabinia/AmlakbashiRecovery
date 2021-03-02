using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.CategoryDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Host.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ILog logger;
        private readonly IRegionAppService regionService;
        private readonly ICategoryAppService categoryService;
        private readonly IUserAppService userService;
        private readonly IUserAccessor userAccessor;
        public CategoryController(ILog logger, IRegionAppService regionService,
            ICategoryAppService dynamicCategoryService, IUserAppService userService,
            IUserAccessor userAccessor)
        {
            this.logger = logger;
            this.regionService = regionService;
            this.categoryService = dynamicCategoryService;
            this.userService = userService;
            this.userAccessor = userAccessor;
        }

        [Auth(UserRoles.MasterAdmin)]
        public ActionResult DynamicIndex(int? page, int Type = 0, int Province = -1, int City = -1, int Area = -1,
            string sort = "viewcount", string q = "")
        {
            try
            {
                var model = categoryService.Filter((AdvertiseType)Type, Province, City, Area, sort, q);
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);
                ViewBag.RowIndexStart = (PageNumber * 20) - 20;
                ViewBag.Type = Type;
                ViewBag.Province = Province;
                ViewBag.City = City;
                ViewBag.Area = Area;
                ViewBag.sort = sort;
                ViewBag.q = q;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("Category.DynamicIndex", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.MasterAdmin)]
        public JsonResult DynamicDelete(int id)
        {
            try
            {
                categoryService.Delete(id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Category.DynamicDelete", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [Auth(UserRoles.MasterAdmin)]
        [HttpGet]
        public ActionResult DynamicEdit(int id = -1)
        {
            try
            {
                if (id == -1)
                {
                    var model = new DynamicCategory();
                    model.Id = -1;
                    return View(model);
                }
                else
                {
                    var model = categoryService.Find(id);
                    return View(model);
                }
            }
            catch (Exception exc)
            {
                logger.Error("Category.DynamicEdit(get)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.MasterAdmin)]
        [HttpPost]
        public ActionResult DynamicEdit(DynamicCategory c)
        {
            try
            {
                if (c.Id == -1)
                {
                    categoryService.Insert(c);
                }
                else
                {
                    categoryService.Update(c);
                }

                return RedirectToAction("DynamicIndex");
            }
            catch (Exception exc)
            {
                logger.Error("Category.DynamicEdit(post)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public ActionResult AddVisited(int catid)
        {
            try
            {
                categoryService.UpdateVisited(catid);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });

            }
            catch (Exception exc)
            {
                logger.Error("Category.AddVisited", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        public ActionResult SiteMap(int city = 0, int TradeID = 0)
        {
            ViewBag.city = city;
            ViewBag.TradeID = TradeID;
            ViewBag.cityItem = city > 0 ? regionService.Find(city) : null;
            ViewBag.categories = categoryService.GetLinks(
                Advertise.AdvertiseType.All, city, 0, city > 0 ? 20 : 100);
            return View();
        }

        [ResponseCache(Duration = 60 * 15, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult SearchCategory(string search_string = "",
            string Province = "-1", string City = "-1", string Area = "-1")
        {
            ViewBag.Province = Province;
            ViewBag.City = City;
            ViewBag.Area = Area;
            ViewBag.search_string = search_string;
            try
            {
                var regions = regionService.GetBySearchRegion(search_string);
                if (!regions.Any() && search_string != "")
                    ViewBag.showNotFound = true;
                var model = new List<SearchTableDTO>();
                foreach (var item in regions)
                {
                    model.Add(SearchTableDTO.Generate(item, regionService.GetRegionName(item.Type != 2 ? 0 : (int)item.ParentID)));
                }
                return PartialView("_SearchTable", model);
            }
            catch (Exception exc)
            {
                logger.Error("Category.SearchCategory", exc);
                ViewBag.showNotFound = true;
                return PartialView("_SearchTable", null);
            }
        }

        // TODO: resolve cache for this
        //public class LiveOutputCacheAttribute : ResponseCacheAttribute
        //{
        //    public LiveOutputCacheAttribute() : base()
        //    {
        //        var query = Request.Query;
        //        if (query.ContainsKey("empty_range_from") ||
        //            query.ContainsKey("empty_range_to") ||
        //            query.ContainsKey("today_empty_homes") ||
        //            query.ContainsKey("discount_homes") ||
        //            query.ContainsKey(""norouz_special") ||
        //            query.ContainsKey("frompaypernight") ||
        //            query.ContainsKey("topaypernight") ||
        //            query.ContainsKey("instant_reserve") ||
        //            query["ajax"] != false)
        //        {
        //            // clear cache
        //            Location = ResponseCacheLocation.None;
        //        }
        //        else
        //        {
        //            Location = ResponseCacheLocation.Any;
        //        }
        //    }
        //}

        // TODO: uncomment this
        //[LiveOutputCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "*" }, Location = ResponseCacheLocation.Any)]
        public ActionResult Item(int regionType,
            int regionId = 0, string name = "",
            string type = "", int countryDirection = 0,
            int page = 1, string discount_homes = null, string today_empty_homes = null,
            string frompaypernight = null, string topaypernight = null,
            string fromMetrazh = null, string toMetrazh = null,
            string region = null, string capacity = null,
            string room = null, string elevator = null, string pool = null,
            string empty_range_from = null, string empty_range_to = null,
            string norouz_special = null, string instant_reserve = null,
            int t = -1, int priceRangeType = 0, int wcType = -1,
            int wifi = 0, int washingMachine = 0, int jacuzzi = 0,
            int poolTable = 0, int foosball = 0, int teaMaker = 0,
            int rules_pets = 0, int rules_party = 0, int rules_smoking = 0,
            int parking = 0, int sort = 0, string roomList = "",
            string phrase = "", bool ajax = false, string path = null)
        {
            try
            {
                var rawUrl = HttpContext.Request.Path.Value;
                rawUrl = string.IsNullOrEmpty(HttpContext.Request.QueryString.Value) ?
                    rawUrl : rawUrl + "?" + HttpContext.Request.QueryString.Value;
                if (ajax == false)
                {
                    if (rawUrl.Last() == '/')
                    {
                        return RedirectPermanent(rawUrl.Remove(rawUrl.Length - 1));
                    }
                    if (page == 1 && HttpContext.Request.QueryString.Value.Contains("?page=1"))
                    {
                        return RedirectPermanent(HtmlUtility.RemoveFromQueryString(rawUrl, "page", "1"));
                    }
                }
                Region targetLocation = null;
                DynamicCategory category = null;
                DynamicCategory subCategory = null;
                var typeInt = UrlStringToAdvertiseType(type);
                if (typeInt == -1)
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
                        return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                    }
                }
                if (regionType == -2)
                {
                    category = categoryService.GetForItemAction(regionType, (AdvertiseType)typeInt);
                }
                else if (regionType == -1)
                {
                    category = categoryService.GetForItemAction(regionType, (AdvertiseType)typeInt, (CountryDirection)countryDirection);
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
                            return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                        }
                    }
                    if (targetLocation.Type == 0)
                    {
                        if (name != "استان-" + targetLocation.PersianName.Trim().Replace(" ", "-"))
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
                        category = categoryService.GetForItemAction(regionType, (AdvertiseType)typeInt, 0, targetLocation.Id, 0, 0);
                    }
                    else if (targetLocation.Type == 1)
                    {
                        if (name != targetLocation.PersianName.Trim().Replace(" ", "-"))
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
                        category = categoryService.GetForItemAction(regionType, (AdvertiseType)typeInt, 0, 0, targetLocation.Id, 0);
                    }
                    else
                    {
                        if (name != targetLocation.PersianName.Trim().Replace(" ", "-"))
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
                        category = categoryService.GetForItemAction(regionType, (AdvertiseType)typeInt, 0, 0, targetLocation.ParentID == null ? 0 : (int)targetLocation.ParentID, 0);
                        subCategory = categoryService.GetForItemAction(regionType, (AdvertiseType)typeInt, 0, 0, 0, targetLocation.Id);
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
                        return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
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
                var model = categoryService.GetFilteredAdvertises(category.Id,
                    area, frompaypernight, topaypernight, null, region, capacity,
                    room, elevator, pool, norouz_special, today_empty_homes,
                    empty_range_from, empty_range_to, discount_homes, instant_reserve,
                    t, (priceRangeTypes)priceRangeType,
                    wcType, wifi == 1, washingMachine == 1, jacuzzi == 1, poolTable == 1,
                    foosball == 1, teaMaker == 1, rules_pets == 1, rules_party == 1, rules_smoking == 1,
                    parking == 1, sort, deserializedRoomList, phrase.Replace("-", " "));

                ViewBag.raw_url = ajax ? path.Split('?')[0] : HttpContext.Request.Path.Value;
                ViewBag.urlWithParameters = ajax ? path : rawUrl;
                ViewBag.dcategory = category;
                ViewBag.area = area;
                var provinceString = category.Province == null ? "" : category.RegionProvince.PersianName;
                var cityString = category.City == null ? "" : category.RegionCity.PersianName;
                var areaString = category.Area == null ? "" : category.RegionArea.PersianName;
                var countryDirectionString = GetCountryDirectionString(category.CountryDirection);
                ViewBag.provinceString = provinceString;
                ViewBag.cityString = cityString;
                ViewBag.areaString = areaString;
                ViewBag.countryDirectionString = countryDirectionString;
                ViewBag.categoryH1Title = AdvertiseSeoLocalization
                    .GetTitle(category.MostAccType,
                    (int)category.Type, provinceString,
                    cityString, areaString, countryDirectionString);
                ViewBag.phrase = phrase;
                ViewBag.frompaypernight = frompaypernight;
                ViewBag.topaypernight = topaypernight;
                ViewBag.fromMetrazh = fromMetrazh;
                ViewBag.toMetrazh = toMetrazh;
                ViewBag.parking = parking;
                ViewBag.region = region;
                ViewBag.capacity = capacity;
                ViewBag.room = room;
                ViewBag.elevator = elevator;
                ViewBag.pool = pool;
                ViewBag.priceRangeType = priceRangeType;
                ViewBag.wcType = wcType;
                ViewBag.wifi = wifi;
                ViewBag.washingMachine = washingMachine;
                ViewBag.jacuzzi = jacuzzi;
                ViewBag.poolTable = poolTable;
                ViewBag.foosball = foosball;
                ViewBag.teaMaker = teaMaker;
                ViewBag.rules_pets = rules_pets;
                ViewBag.rules_party = rules_party;
                ViewBag.rules_smoking = rules_smoking;
                ViewBag.norouz_special = norouz_special;
                ViewBag.today_empty_homes = today_empty_homes;
                ViewBag.empty_range_from = empty_range_from;
                ViewBag.empty_range_to = empty_range_to;
                ViewBag.discount_homes = discount_homes;
                ViewBag.instant_reserve = instant_reserve;
                ViewBag.roomList = roomList;
                ViewBag.type = category == null ?
                    Advertise.AdvertiseTypeToHeadType(typeInt) :
                    (int)category.ParentAccType;
                ViewBag.t = t;
                ViewBag.sort = sort;
                var areaRegionRelated = area < 1 ? null : regionService.Find(area).Related;
                var areaRegionRelatedIds = string.IsNullOrEmpty(areaRegionRelated) ? null : Array.ConvertAll(
                            areaRegionRelated.Trim(',').Split(','), x => int.Parse(x));
                ViewBag.related_categories = subCategory != null ?
                    categoryService.GetRelatedCategories(subCategory.Id, areaRegionRelatedIds, model.Count())
                    : (category == null ? new List<DynamicCategory>() :
                    categoryService.GetRelatedCategories(category.Id, areaRegionRelatedIds, model.Count()));

                ViewBag.AnyTodayEmpty = today_empty_homes == "1" || model.Any(x => x.TodayIsEmpty || x.Childs.All(y => y.TodayIsEmpty));
                var pages_count = Math.Max(1, Math.Ceiling((float)((float)model.Count() / 12f)));
                ViewBag.pagesCount = pages_count;
                ViewBag.currentPageNumber = page;

                ViewBag.Title = AdvertiseSeoLocalization.GetMetaTitle(category.MostAccType, category.CountAdvertise,
                    (int)category.MinPrice, (int)category.Type, provinceString, cityString, null, countryDirectionString);
                ViewBag.keywords = AdvertiseSeoLocalization.GetKeywords((int)category.Type, provinceString, cityString, areaString, countryDirectionString);
                ViewBag.Description = AdvertiseSeoLocalization.GetDescription(category.MostAccType, (int)category.Type, provinceString, cityString, areaString, countryDirectionString);
                ViewBag.accTypeUrlString = AdvertiseUrlLocalization.AdvertiseTypeToUrlString((int)category.Type);

                ViewBag.categoryFaqTrustQuestion = CategoryFaqLocalization.CategoryFaqTrustQuestion(category);
                ViewBag.categoryFaqTrustAnswer = CategoryFaqLocalization.CategoryFaqTrustAnswer();
                ViewBag.categoryFaqPriceQuestion = CategoryFaqLocalization.CategoryFaqPriceQuestion(category);
                ViewBag.categoryFaqPriceAnswer = CategoryFaqLocalization.CategoryFaqPriceAnswer(category);
                ViewBag.categoryFaqAreasQuestion = CategoryFaqLocalization.CategoryFaqAreasQuestion(category);
                ViewBag.categoryFaqAreasAnswer = CategoryFaqLocalization.CategoryFaqAreasAnswer(category);
                ViewBag.categoryFaqEvidenceQuestion = CategoryFaqLocalization.CategoryFaqEvidenceQuestion(category);
                ViewBag.categoryFaqEvidenceAnswer = CategoryFaqLocalization.CategoryFaqEvidenceAnswer();
                ViewBag.categoryFaqReserveQuestion = CategoryFaqLocalization.CategoryFaqReserveQuestion(category);
                ViewBag.categoryFaqReserveAnswer = CategoryFaqLocalization.CategoryFaqReserveAnswer(category);
                ViewBag.categoryFaqHostQuestion = CategoryFaqLocalization.CategoryFaqHostQuestion();
                ViewBag.categoryFaqHostAnswer = CategoryFaqLocalization.CategoryFaqHostAnswer();

                ViewBag.priceOptions = new int[]
                {
                    30000, 50000, 100000, 150000,200000,
                    250000, 300000, 350000, 400000, 500000,
                    600000, 700000, 800000, 900000, 1000000,
                    1200000, 1400000, 1600000, 1800000,
                    2000000, 2500000, 3500000, 4000000,
                    5000000, 10000000
                };
                ViewBag.monthlyPriceOptions = new int[]
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
                ViewBag.dateString = dateString;

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
                ViewBag.priceMin = priceMin;
                if (topaypernight != null)
                {
                    priceInt = int.Parse(topaypernight);
                    if (priceInt > 0)
                    {
                        //priceMaxString = String.Format("{0:n0}", priceInt) + " تومان";
                        priceMax = priceInt;
                    }
                }
                ViewBag.priceMax = priceMax;

                var positionArray = (Advertise.PositionType[])(Enum.GetValues(typeof(Advertise.PositionType)));
                var positionItems = positionArray.ToList();
                positionItems.Remove(Advertise.PositionType.none);
                ViewBag.positionItems = positionItems;

                ViewBag.regionString = area > 0 ? areaString :
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
                ViewBag.priceString = priceString;

                ViewBag.accTypeString = t > 0 ?
                    AdvertiseMainLocalization.GetAdvertiseTypeUserString((Advertise.AdvertiseType)t) :
                    (category.Type != Advertise.AdvertiseType.All ? category.TypeString : "");

                List<int> roomListIds = null;
                if (!string.IsNullOrEmpty(roomList))
                {
                    roomListIds = Array.ConvertAll(roomList.Split(','), x => int.Parse(x)).ToList();
                }
                ViewBag.roomListIds = roomListIds;

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
                ViewBag.queryString = queryString;

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

                ViewBag.provinces = regionService.GetByType(AdvertiseRegion.Province);
                ViewBag.cities = regionService.GetChildren(category.Province == null ? 0 : (int)category.Province, RegionStatus.HasAdvertise);
                ViewBag.areas = regionService.GetChildren(category.City == null ? 0 : (int)category.City, RegionStatus.HasAdvertise);

                model = model.Skip(12 * (page - 1)).Take(12);
                var user = userAccessor.CurrentUser;
                var userFavorite = user.Id > 0 ? user.Favorite : new List<UserFavorite>();
                List<AccommodationCardDTO> advertiseItemDTOs = new List<AccommodationCardDTO>();
                foreach (var item in model)
                {
                    var dto = (AccommodationCardDTO)item;
                    dto.Favourited = user.Id > 0 && userFavorite.Any(x => x.AdvertiseID == item.Id);
                    advertiseItemDTOs.Add(dto);
                }
                if (ajax)
                {
                    ViewBag.for_advertise_page = true;
                    return PartialView("_AdvertiseListItems", advertiseItemDTOs);
                }
                else
                {
                    return View(advertiseItemDTOs);
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
                    return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                }
            }
        }

        public ActionResult Category(int Type, bool a_m_p_version,
            int province = 0, int city = 0, int area = 0,
            bool norouz_special = false, bool today_empty_homes = false,
            bool discount_homes = false, int country_direction = 0, int page = 1,
            bool instant_reserve = false, int t = -1, int priceRangeType = 0,
            int perWC = 0, int euWC = 0,
            int wifi = 0, int washingMachine = 0, int jacuzzi = 0,
            int poolTable = 0, int foosball = 0, int teaMaker = 0,
            int rules_pets = 0, int rules_party = 0, int rules_smoking = 0,
            int parking = 0, int sort = 0, string roomList = "",
            int frompaypernight = -1, int topaypernight = -1,
            int fromMetrazh = -1, int toMetrazh = -1,
            int region = -1, int capacity = -1,
            int room = -1, int elevator = -1, int pool = -1, 
            string empty_range_from = "", string empty_range_to = "",
            string phrase = "", bool ajax = false)
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
            // TODO: check this
            //var frompaypernight = Request.Query["frompaypernight"];
            //var topaypernight = Request.Query["topaypernight"];
            //var fromMetrazh = Request.Query["fromMetrazh"];
            //var toMetrazh = Request.Query["toMetrazh"];
            //var region = Request.Query["region"];
            //var capacity = Request.Query["capacity"];
            //var room = Request.Query["room"];
            //var elevator = Request.Query["elevator"];
            //var pool = Request.Query["pool"];
            //var empty_range_from = Request.Query["empty_range_from"];
            //var empty_range_to = Request.Query["empty_range_to"];
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
            if (string.IsNullOrEmpty(empty_range_from) == false)
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
            if (url[url.Length - 1] == '/')
            {
                url = url.Remove(url.Length - 1, 1);
            }
            var discountHomes = discount_homes ? 1 : 0;
            var todayEmpty = today_empty_homes ? 1 : 0;
            var instantReserve = instant_reserve ? 1 : 0;
            var norouzSpecial = norouz_special ? 1 : 0;
            var path = url + query_string;
            if (ajax)
            {
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
                    ajax,
                    path
                });
            }
            return Redirect(path);
        }

        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "*" }, Location = ResponseCacheLocation.Any)]
        public ActionResult SearchByRegionPopup(int? province = null,
            int? city = null, int? area = null)
        {
            ViewBag.province = province;
            ViewBag.city = city;
            ViewBag.area = area;
            return PartialView("_SearchByRegion");
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
                    url = "/ایران";
                }
                else
                {
                    var category = categoryService.GetCategoryByCountryDirectionOrRegion(
                        Advertise.AdvertiseType.All, CountryDirection.Unset,
                        region.Id, (Region.AdvertiseRegion)region.Type);
                    url = CategoryUrlLocalization.CategoryToUrl(category);
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
                logger.Error("Category.RegionSearchToUrl", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }
    }
}
