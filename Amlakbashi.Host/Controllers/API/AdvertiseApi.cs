using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Amlakbashi.Core.Entities;
using Entities = Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.Region;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using static Amlakbashi.Core.Entities.User;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs;
using static Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs.CheckSetOccupiedDTO;
using static Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs.CheckUnsetOccupiedDTO;
using static Amlakbashi.Core.Entities.ActionLog;
using System.Drawing;
using Amlakbashi.Core.DTOs.FileDTOs;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : BaseController
    {
        private int[] categories = new int[]
            { 55593, 55574, 74214, 55894, 55944, 55962, 55957, 55861, 55953, 55952, 55961, 55960 };

        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "*" }, Location = ResponseCacheLocation.Any)]
        public JsonResult GetHomePageCarousels(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var items = new List<ApiHomePageCarouselDTO>();
                List<ApiAdvertiseItemDTO> dto = new List<ApiAdvertiseItemDTO>();
                var accList = discountTableService.GetMostDiscountAdvertises(8);
                foreach (var item in accList)
                {
                    dto.Add(item);
                }
                items.Add(new ApiHomePageCarouselDTO()
                {
                    title = "اقامتگاه های تخفیف دار",
                    cid = 0,
                    type = 1,
                    items = dto
                });
                var category_items = new List<DynamicCategory>();
                foreach (var category in categories)
                {
                    category_items.Add(categoryService.Find(category));
                }

                foreach (var category_item in category_items)
                {
                    IEnumerable<Advertise> advertise_list;

                    if (category_item.City != null)
                    {
                        advertise_list = advertiseService.GetMostViewedAdvertisesInCity(
                            category_item.City == null ? 0 : (int)category_item.City, 0, (int)category_item.Type, 8);
                    }
                    else if (category_item.Province != null)
                    {
                        advertise_list = advertiseService.GetMostViewedAdvertisesInCity(
                            0, category_item.Province == null ? 0 : (int)category_item.Province, (int)category_item.Type, 8);
                    }
                    else
                    {
                        advertise_list = advertiseService.GetMostViewedAdvertisesByType(
                            (int)category_item.Type, 8);
                    }
                    dto = new List<ApiAdvertiseItemDTO>();
                    foreach (var item in advertise_list)
                    {
                        dto.Add(item);
                    }
                    items.Add(new ApiHomePageCarouselDTO()
                    {
                        title = category_item.Title,
                        cid = category_item.Id,
                        type = 0,
                        items = dto
                    });
                }
                //var norouzItems = new List<HomePageCarousel>();
                //var norouzCarouselItem = new HomePageCarousel()
                //{
                //    title = "اقامتگاه های ویژه نوروز ۹۹",
                //    cid = 0,
                //    type = 2,
                //    items = AdvertiseItem.GenerateFromAdvertiseList(
                //        advertiseService.GetMostViewedNorouzAdvertises(8),
                //        all_discounts, now)
                //};
                //if (!norouzCarouselItem.items.Any())
                //{
                //    norouzCarouselItem = null;
                //}
                //return GenerateJsonResult(new { items = items, norouzItem = norouzCarouselItem });
                return GenerateJsonResult(new { items = items, norouzItem = (ApiHomePageCarouselDTO)null });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseAPI.GetHomePageCarousels", exc);
                return GenerateJsonResult(new { });
            }
        }

        //[OutputCache(Duration = 60 * 60, Location = System.Web.UI.OutputCacheLocation.Server, VaryByParam = "*")]
        public JsonResult GetFilteredAdvertises(string cid, int region_id, string start_date = null,
            string end_date = null)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                start_date = start_date == "null" ? null : start_date;
                end_date = end_date == "null" ? null : end_date;
                var all_regions = regionService.GetAll();
                var region = all_regions.FirstOrDefault(x => x.Id == region_id);
                var area = (region != null && region.Type ==
                    (int)AdvertiseRegion.Area) ? region : null;
                var city = area != null ?
                    all_regions.FirstOrDefault(x => x.Id == area.ParentID) :
                    (region != null && region.Type ==
                        (int)AdvertiseRegion.City ? region : null);
                var province = city != null ?
                    all_regions.FirstOrDefault(x => x.Id == city.ParentID) :
                    (region != null && region.Type ==
                        (int)AdvertiseRegion.Province ? region : null);
                var area_id = area != null ? area.Id : 0;
                var city_id = city != null ? city.Id : 0;
                var province_id = province != null ? province.Id : 0;
                var category = categoryService.GetByProvinceCity(AdvertiseType.All, province_id, city_id);
                IEnumerable<Advertise> advertises;
                if (category == null)
                {
                    advertises = new List<Advertise>();
                }
                else
                {
                    advertises = categoryService.GetFilteredAdvertises(category.Id,
                        area_id, null, null, null, null, null, null, null, null, null, null,
                        start_date, end_date, null, null);
                }

                //AdvertiseDepend.FilterResult filter_result;
                //string redirect_string;
                //DynamicCategory category;
                //var advertises = AdvertiseDepend.GetFilteredAdvertises(null, null,
                //    out filter_result, out redirect_string, out category,
                //    null, null, null,
                //    null, null, null,
                //    null, null, null,
                //    null, null, null,
                //    start_date, end_date, null, category_id);
                List<ApiAdvertiseItemDTO> result = new List<ApiAdvertiseItemDTO>();
                foreach (var item in advertises)
                {
                    result.Add(item);
                }
                var url = result.Any() ? GeneralData.WebsiteUrl + CategoryUrlLocalization.CategoryToUrl(category) : null;
                return GenerateJsonResult(new { items = result, websiteUrl = url });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseAPI.GetFilteredAdvertises", exc);
                return GenerateJsonResult(new { });
            }
        }

        public JsonResult GetFavouriteAdvertises(string cid, string token)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise_ids = user.Favorite.OrderByDescending(f => f.SetDate).Select(f => f.AdvertiseID).Take(100).ToList();
                var advertises = advertiseService.GetAccListByIds(advertise_ids, AdvertiseStatus.Published);
                var now = DateTime.Now.Date;
                List<ApiAdvertiseItemDTO> result = new List<ApiAdvertiseItemDTO>();
                foreach (var item in advertises)
                {
                    result.Add(item);
                }
                return GenerateJsonResult(new { done = true, data = result });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { done = false, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }

        public JsonResult GetHostAdvertises(string cid, string token)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertises = advertiseService.GetNotChildAdvertisesByUserId(user.Id);
                var output = new List<ApiHostAdvertiseDTO>();
                bool _isComplex;
                bool _isHotel;
                bool _hasComment;
                int[] not_verify_ids;
                List<string> _notVerifyReasons;
                foreach (var advertise in advertises)
                {
                    var hotelChilds = new List<ApiHotelChildDTO>();
                    var hotelChild = new ApiHotelChildDTO();
                    var apartmentChilds = new List<ApiComplexChildDTO>();
                    var hutChilds = new List<ApiComplexChildDTO>();
                    var houseChilds = new List<ApiComplexChildDTO>();
                    var villaChilds = new List<ApiComplexChildDTO>();
                    var suitChilds = new List<ApiComplexChildDTO>();
                    var complexChild = new ApiComplexChildDTO();
                    _isComplex = advertise.Childs.Count > 0 &&
                        advertise.Childs.ElementAt(0).Count == 0;
                    _isHotel = !_isComplex && advertise.Childs.Count > 0;
                    not_verify_ids = string.IsNullOrEmpty(advertise.NotVerifyReasons) ?
                        new int[0] :
                        Array.ConvertAll(advertise.NotVerifyReasons.Split(','), str => int.Parse(str));
                    _notVerifyReasons = Array.ConvertAll(not_verify_ids,
                        id => AdvertiseMainLocalization.GetNotVerifyReasonTitle(id)).ToList();
                    _hasComment = commentService.AnyComment(advertise.Id);

                    var number = 1;
                    foreach (var item in advertise.Childs)
                    {
                        hotelChild = item;
                        hotelChild.number = number;
                        hotelChilds.Add(hotelChild);
                        number++;
                    }

                    number = 1;
                    foreach (var item in advertise.Childs.Where(w => w.TypeID == AdvertiseType.Apartment))
                    {
                        complexChild = item;
                        complexChild.number = number;
                        apartmentChilds.Add(complexChild);
                        number++;
                    }

                    number = 1;
                    foreach (var item in advertise.Childs.Where(w => w.TypeID == AdvertiseType.House))
                    {
                        complexChild = item;
                        complexChild.number = number;
                        houseChilds.Add(complexChild);
                        number++;
                    }

                    number = 1;
                    foreach (var item in advertise.Childs.Where(w => w.TypeID == AdvertiseType.Hut))
                    {
                        complexChild = item;
                        complexChild.number = number;
                        hutChilds.Add(complexChild);
                        number++;
                    }

                    number = 1;
                    foreach (var item in advertise.Childs.Where(w => w.TypeID == AdvertiseType.SuitAndRoom))
                    {
                        complexChild = item;
                        complexChild.number = number;
                        suitChilds.Add(complexChild);
                        number++;
                    }

                    number = 1;
                    foreach (var item in advertise.Childs.Where(w => w.TypeID == AdvertiseType.Villa))
                    {
                        complexChild = item;
                        complexChild.number = number;
                        villaChilds.Add(complexChild);
                        number++;
                    }

                    output.Add(new ApiHostAdvertiseDTO()
                    {
                        id = advertise.Id,
                        title = advertise.Title,
                        image = advertise.PhotoID == null ? 0 : (long)advertise.PhotoID,
                        adType = (int)advertise.TypeID,
                        status = (int)advertise.Status,
                        statusTitle = AdvertiseMainLocalization.GetAdvertiseStatusString((int)advertise.Status),
                        statusColor = AdvertiseStyleHelper.GetAdvertiseStatusColor((int)advertise.Status),
                        todayEmpty = advertise.TodayIsEmpty,
                        hasComment = _hasComment,
                        newCommentCount = 0,
                        notVerifyReasons = _notVerifyReasons,
                        isComplex = _isComplex,
                        isHotel = _isHotel,
                        hotelChildren = hotelChilds,
                        apartmentChildren = apartmentChilds,
                        suitChildren = suitChilds,
                        villaChildren = villaChilds,
                        houseChildren = houseChilds,
                        hutChildren = hutChilds,
                        hotelUnitTitle = _isHotel ? AdvertiseMainLocalization.GetHotelUnitTitle(advertise.TypeID) : "",
                        instantReserveDetail = new InstantReserveDetailDTO()
                        {
                            banned = user.InstantReserveAccess == InstantReserveAccessEnum.Banned,
                            status = advertise.InstantReserveStatus,
                            statusString = AdvertiseMainLocalization.GetInstantReserveStatusString(advertise.InstantReserveStatus),
                            statusColor = AdvertiseStyleHelper.GetInstantReserveStatusColor(advertise.InstantReserveStatus),
                            buttonTitle = AdvertiseMainLocalization.GetInstantReserveButtonTitle(advertise.InstantReserveStatus, user.InstantReserveAccess == InstantReserveAccessEnum.Banned)
                        },
                        stayDuration = new StayDurationDTO() { id = advertise.Id, min = advertise.MinReserveDays, max = advertise.MaxReserveDays },
                        maxInstantReserveStart = advertise.MaxInstantReserveStart,
                        //norouzPrice = advertise.NorouzPrice,
                        norouzPrice = 0,
                        //norouzPriceString = string.Format("{0:n0}", advertise.NorouzPrice),
                        norouzPriceString = "",
                        //norouzMinReserveDateString = advertise.unixNorouzMinRequestDate < 1 ? null : DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(advertise.unixNorouzMinRequestDate)).Replace(",", "/"),
                        norouzMinReserveDateString = "",
                        //norouzOverCapacityPrice = advertise.NorouzOverCapacityPrice
                        norouzOverCapacityPrice = 0
                    });
                }
                var result = output;
                return GenerateJsonResult(new { done = true, data = result });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetHostAdvertises", exc);
                return GenerateJsonResult(new { done = false, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }
        [ResponseCache(Duration = 60 * 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "*" })]
        public JsonResult GetCategoryAdvertises(int id, string cid,
            string start_date = null, string end_date = null)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                start_date = start_date == "null" ? null : start_date;
                end_date = end_date == "null" ? null : end_date;

                var category = categoryService.Find(id);

                var advertises = categoryService.GetFilteredAdvertises(category.Id,
                    category.Area == null ? 0 : (int)category.Area, null, null, null, null,
                        null, null, null, null, null, null,
                        start_date, end_date, null, null);

                //AdvertiseDepend.FilterResult filter_result;
                //string redirect_string;
                //DynamicCategory category;
                //var advertises = AdvertiseDepend.GetFilteredAdvertises(null, null,
                //    out filter_result, out redirect_string, out category,
                //    null, null, null,
                //    null, null, null,
                //    null, null, null,
                //    null, null, null,
                //    start_date, end_date, null, id);
                List<ApiAdvertiseItemDTO> result = new List<ApiAdvertiseItemDTO>();
                foreach (var item in advertises)
                {
                    result.Add(item);
                }
                var url = result.Any() ? GeneralData.WebsiteUrl + CategoryUrlLocalization.CategoryToUrl(category) : null;
                return GenerateJsonResult(new { items = result, websiteUrl = url });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetCategoryAdvertises", exc);
                return GenerateJsonResult(new { });
            }
        }

        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "*" }, Location = ResponseCacheLocation.Any)]
        public JsonResult GetAllAvailableAdvertises(string cid, string start_date = null,
            string end_date = null, bool for_discount = false, bool for_instant_reserve = false,
            bool for_norouz_special = false)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                start_date = start_date == "null" ? null : start_date;
                end_date = end_date == "null" ? null : end_date;
                //List<ApiAdvertiseItemDTO> result = advertiseService.GetAllAvailableAdvertises(start_date, end_date,
                //    for_discount, for_instant_reserve, for_norouz_special);
                var category = categoryService.Find(AdvertiseType.All,
                    CountryDirection.Unset, 0, 0, 0);
                var result = new List<ApiAdvertiseItemDTO>();
                var advertises = categoryService.GetFilteredAdvertises(
                        categoryId: category.Id, empty_range_from: start_date,
                        empty_range_to: end_date);
                foreach (var advertise in advertises)
                {
                    result.Add(advertise);
                }
                return GenerateJsonResult(new { items = result, websiteUrl = result.Any() ? (GeneralData.WebsiteUrl + "/ایران" + (for_discount ? "?discount_homes=1" : (for_instant_reserve ? "?instant_reserve=1" : ""))) : null });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAllAvailableAdvertises", exc);
                return GenerateJsonResult(new { });
            }
        }

        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "*" }, Location = ResponseCacheLocation.Any)]
        public JsonResult GetVillaShomalAdvertises(string cid, string start_date = null,
            string end_date = null)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                start_date = start_date == "null" ? null : start_date;
                end_date = end_date == "null" ? null : end_date;
                var category = categoryService.Find(AdvertiseType.Villa,
                    CountryDirection.North, 0, 0, 0);
                var result = new List<ApiAdvertiseItemDTO>();
                var advertises = categoryService.GetFilteredAdvertises(
                        categoryId: category.Id, empty_range_from: start_date,
                        empty_range_to: end_date);
                foreach (var advertise in advertises)
                {
                    result.Add(advertise);
                }
                //List<ApiAdvertiseItemDTO> result = advertiseService.GetVillaShomalAdvertises(start_date, end_date);
                return GenerateJsonResult(new { items = result, websiteUrl = result.Any() ? (GeneralData.WebsiteUrl + "/شمال/ویلا") : null });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetVillaShomalAdvertises", exc);
                return GenerateJsonResult(new { });
            }
        }

        public JsonResult GetAdvertiseDetail(long id, string cid, string token, bool getAllComments = false)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                bool favourited = false;
                string commentNotVerifyReason = "";
                var user = GetUser(token);
                IQueryable<Comment> comments = commentService.GetAllAsIQueryable();
                var advertise = advertiseService.FindIncludingDeleted(id);
                if (user.Id > 0)
                {
                    favourited = user.Favorite.Any(x => x.AdvertiseID == id);
                    commentNotVerifyReason = commentService.GetNotVerifyReasonIfExists(advertise.Id, advertise.UserID, user.Id);
                }
                var result = ApiAdvertiseDetailDTO.Generate(user.Id, advertise,
                    false, favourited, getAllComments, commentNotVerifyReason);
                return GenerateJsonResult(new { items = result });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertiseDetail", exc);
                return GenerateJsonResult(new { });
            }
        }

        public JsonResult GetAdvertiseDatesInfo(long id, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var advertise = advertiseService.FindIncludingDeleted(id);
                var occDatesFrom = new List<string>();
                var occDatesTo = new List<string>();
                var occDatesOrig = advertise.OccupiedDates;
                if (occDatesOrig.Any())
                {
                    var orderedOccDates = occDatesOrig.Distinct()
                        .OrderBy(o => o.Date).ToList();
                    var count = orderedOccDates.Count;
                    var indexInSequence = 0;
                    for (int i = 0; i < count; i++)
                    {
                        var dateKey = orderedOccDates[i].ToString("yyyy-MM-dd");
                        var nextOccDate = (i == count - 1) ? (DateTime?)null : orderedOccDates[i + 1];
                        var nextInSequenceExists = nextOccDate != null && nextOccDate == orderedOccDates[i].AddDays(1);
                        if (indexInSequence == 0)
                        {
                            occDatesFrom.Add(dateKey);
                            indexInSequence++;
                        }
                        else if (nextInSequenceExists)
                        {
                            Console.WriteLine("This is in the middle");
                            occDatesFrom.Add(dateKey);
                            occDatesTo.Add(dateKey);
                            indexInSequence++;
                        }
                        else
                        {
                            Console.WriteLine("This is the end of the sequence");
                            occDatesTo.Add(dateKey);
                        }
                        if (nextInSequenceExists == false)
                        {
                            indexInSequence = 0;
                        }
                    }
                }

                var holidays = DateTimeUtility.GetHolidaysInGregorian();
                var priceList = advertiseService.GetAccPriceDatesInfo(id)
                    .ToDictionary(k => DateTimeUtility.GregorianToPersianDate(
                        DateTimeUtility.JSValueToDate(long.Parse(k.Key))),
                        v => v.Value.price);

                return GenerateJsonResult(new
                {
                    occDatesFrom = occDatesFrom,
                    occDatesTo = occDatesTo,
                    holidays = holidays,
                    priceList = priceList
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertiseDatesInfo", exc);
                var emptyArr = new List<string>();
                return GenerateJsonResult(new { occDatesFrom = emptyArr, occDatesTo = emptyArr, holidays = emptyArr });
            }
        }

        public JsonResult AdvertiseToggleFavourite(long id, string cid, string token)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new { status = 2 });
                }
                if (user.Favorite == null || user.Favorite.Count == 0)
                {
                    user.Favorite = new List<UserFavorite>();
                }
                var favouriteItem = user.Favorite.FirstOrDefault(x => x.AdvertiseID == id);
                var favourited = favouriteItem != null;
                if (favourited)
                {
                    userService.DeleteFavorite(user.Id, id);
                }
                else
                {
                    userService.AddFavorite(user.Id, id);
                }

                return GenerateJsonResult(new { status = 1, favourited = !favourited });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.AdvertiseToggleFavourite", exc);
                return GenerateJsonResult(new { status = 0, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }

        [Serializable]
        public class PriceListHelper
        {
            public string price { get; set; }
        }

        public JsonResult SearchByAdvertiseId(long id, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var advertise = advertiseService.Find(id, 3);
                if (advertise != null)
                {
                    return GenerateJsonResult(new
                    {
                        done = true,
                        title = advertise.Title
                    });
                }
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "آگهی مورد نظر یافت نشد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه خطایی رخ داده است"
                });
            }
        }

        public JsonResult GetHolidays(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var holidays = DateTimeUtility.GetHolidaysInGregorian();
                return GenerateJsonResult(new { items = holidays });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new { items = new List<string>() });
            }
        }

        public JsonResult GetAllProvinces(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var regions = regionService.Filter(AdvertiseRegion.Province);
                var result = new List<ApiRegionItemDTO>();
                result.AddRange(regions.Select(s => (ApiRegionItemDTO)s).ToList());
                return GenerateJsonResult(new { items = result });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAllProvinces", exc);
                return GenerateJsonResult(new { items = new List<ApiRegionItemDTO>() });
            }
        }

        public JsonResult GetCities(int province_id, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                List<ApiRegionItemDTO> regions = new List<ApiRegionItemDTO>();
                if (province_id > 0)
                {
                    var regionList = regionService.Filter(AdvertiseRegion.City, province_id);
                    regions.AddRange(regionList.Select(s => (ApiRegionItemDTO)s).ToList());
                }
                return GenerateJsonResult(new { items = regions });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetCities", exc);
                return GenerateJsonResult(new { items = new List<ApiRegionItemDTO>() });
            }
        }

        public JsonResult GetAreas(int city_id, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                List<ApiRegionItemDTO> regions = new List<ApiRegionItemDTO>();
                if (city_id > 0)
                {
                    var regionList = regionService.Filter(AdvertiseRegion.Area, city_id);
                    regions.AddRange(regionList.Select(s => (ApiRegionItemDTO)s).ToList());
                }
                return GenerateJsonResult(new { items = regions });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAreas", exc);
                return GenerateJsonResult(new { items = new List<ApiRegionItemDTO>() });
            }
        }

        [ResponseCache(Duration = 60 * 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "text" })]
        public JsonResult SearchRegion(string text, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var foundRegions = regionService.SearchLocationForApp(text);
                var regions = new List<ApiRegionItemDTO>();
                regions.AddRange(foundRegions.Select(s => (ApiRegionItemDTO)s).ToList());
                return GenerateJsonResult(new { items = regions });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.SearchRegion", exc);
                return GenerateJsonResult(new { items = new List<ApiRegionItemDTO>() });
            }
        }

        public JsonResult GetAdvertiseRatingInfo(string cid, string token, long advertise_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.FindIncludingDeleted(advertise_id);
                var currentComment = commentService.GetByAccSenderUser(advertise_id, user.Id);
                var userRatings = new ApiUserRatingItemDTO()
                {
                    tidiness = reportItemService.GetAdvertiseRatingOfUser(user.Id, advertise_id, 1),
                    hostBehaviour = reportItemService.GetAdvertiseRatingOfUser(user.Id, advertise_id, 2),
                    position = reportItemService.GetAdvertiseRatingOfUser(user.Id, advertise_id, 3),
                    infoCorrectness = reportItemService.GetAdvertiseRatingOfUser(user.Id, advertise_id, 4),
                    safety = reportItemService.GetAdvertiseRatingOfUser(user.Id, advertise_id, 5),
                    priceWorth = reportItemService.GetAdvertiseRatingOfUser(user.Id, advertise_id, 6)
                };
                return GenerateJsonResult(new
                {
                    done = true,
                    currentComment = currentComment != null ?
                        currentComment.Text : "",
                    userRatings = userRatings,
                    title = "سفر به " + 
                    regionService.Find(advertise.City == null ? 0 : (int)advertise.City).PersianName + " - " + advertise.Title
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertiseRatingInfo", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SendAdvertiseComment(string cid, string token, long advertise_id, string text)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                string cannotAddReason;
                var canAdd = advertiseService.AddAdvertiseComment(user.Id,
                    advertise_id, text, out cannotAddReason);
                return GenerateJsonResult(new
                {
                    done = canAdd,
                    msg = cannotAddReason
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SubmitAdvertiseScore(string cid, string token,
            long advertise_id, int report_id, int score)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                reportItemService.SubmitAdvertiseScore(user.Id,
                    advertise_id, report_id, score);
                return GenerateJsonResult(new
                {
                    done = true
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.SubmitAdvertiseScore", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult GetAdvertisePrices(string cid, string token,
            long id, bool is_group, int group_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                var data = advertiseService.GetPrices(id);
                data.id = (int)id;
                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertisePrices", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        public JsonResult EditAdvertisePrice(string cid, string token,
            PriceInputDTO data, int targetStatus = -1)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var acc = advertiseService.Find(data.id);
                if (acc.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                Dictionary<string, string> errors;
                var done = advertiseService.SetPrices(data.id, data, out errors);
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors.Values.ToList()
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.EditAdvertisePrice", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetAdvertisePhotos(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                int accUserId = 0;
                var data = advertiseService.GetPhotoDTO(id, out accUserId);
                if (data != null && accUserId != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                if (data == null)
                {
                    data.album = new List<long>();
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertisePhotos", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        public JsonResult EditAdvertisePhotos(string cid, string token,
            ApiPhotoDTO data, int targetStatus = -1)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(data.id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                List<string> errors = new List<string>();
                var done = advertiseService.UpdatePhotos(data, webHostEnvironment.WebRootPath);
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.EditAdvertisePhotos", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetAdvertiseAmenities(string cid, string token,
            long id, bool is_group, int group_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                int userId = 0;
                var data = advertiseService.GetAmenitiesDTO(id, out userId);
                if (userId != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }

                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertiseAmenities", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        public JsonResult EditAdvertiseAmenities(string cid = null, string token = null,
            ApiAmenitiesDTO data = null, int targetStatus = -1)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(data.id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                Dictionary<string, string> errors;
                string msg;
                var done = advertiseService.UpdateAmenities(data, out errors, out msg);
                advertiseService.UpdateExtraBlanketCount(data.id, data.extraBlanketCount);
                advertiseService.UpdateElevator(data.id, data.elevator);
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = done ? new List<string>() : new List<string>() { msg }
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.EditAdvertiseAmenities", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetAdvertisePosition(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                int userId = 0;
                var data = advertiseService.GetPositionDTO(id, out userId);
                if (userId != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertisePosition", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        [HttpPost]
        public JsonResult EditAdvertisePosition(string cid, string token,
            string data_string, int targetStatus = -1, int buildNumber = 0)
        {
            var data = JsonConvert.DeserializeObject<ApiPositionDTO>(data_string);
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(data.id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                Dictionary<string, string> errors = null;
                bool done = false;
                if (buildNumber > 0)
                {
                    if (data.latitude == 0 || data.longitude == 0)
                    {
                        done = false;
                        errors = new Dictionary<string, string>();
                        errors.Add("", "لطفا موقعیت دقیق اقامتگاه را با کلیک بر روی نقشه انتخاب کنید");
                    }
                    else
                    {
                        done = advertiseService.UpdatePositionDTO(data, out errors);
                    }
                }
                else
                {
                    done = advertiseService.UpdatePositionDTO(data, out errors);
                }
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors.Values.ToList()
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetAdvertiseRules(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                int userId = 0;
                var data = advertiseService.GetRulesDTO(id, out userId);
                if (userId != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertiseRules", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }
        [HttpPost]
        public JsonResult EditAdvertiseRules(string cid, string token,
            string data_string, int targetStatus = -1)
        {
            var data = JsonConvert.DeserializeObject<ApiRulesDTO>(data_string);
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(data.id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                List<string> errors = new List<string>();
                var done = advertiseService.UpdateRulesDTO(data);
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.EditAdvertiseRules", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetAdvertiseSpecifics(string cid, string token,
            long id, bool is_group, int group_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                int userId = 0;
                var data = advertiseService.GetSpecificDTO(id, out userId);
                if (userId != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertiseSpecifics", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        public JsonResult EditAdvertiseSpecifics(string cid = null, string token = null,
            string data_string = null, int targetStatus = -1)
        {
            var data = JsonConvert.DeserializeObject<ApiSpecificDTO>(data_string);
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(data.id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                List<string> errors;
                var done = advertiseService.UpdateSpecificDTO(data,
                    advertise.Childs != null && advertise.Childs.Any(), out errors);
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.EditAdvertiseSpecifics", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetAdvertiseDiscounts(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }

                var discountList = discountTableService.GetDiscountsOfAccommodation(id);
                var dto = new DiscountTableDTO();
                dto.id = id;
                dto.discounts = new List<DiscountItemDTO>();
                foreach (var item in discountList)
                {
                    dto.discounts.Add(new DiscountItemDTO()
                    {
                        id = item.Id,
                        fromDate = DateTimeUtility.GregorianToPersianDate(item.From),
                        toDate = DateTimeUtility.GregorianToPersianDate(item.To),
                        percent = item.Percent
                    });
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    data = dto
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetAdvertiseDiscounts", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        [HttpPost]
        public JsonResult EditAdvertiseDiscounts(string cid, string token,
            DiscountTableDTO data)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(data.id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                List<string> errors;
                var dataEntity = new List<DiscountTable>();
                if (data.discounts == null)
                    data.discounts = new List<DiscountItemDTO>();
                foreach (var item in data.discounts)
                {
                    dataEntity.Add(new DiscountTable()
                    {
                        Id = item.id,
                        AdvertiseID = data.id,
                        From = DateTimeUtility.PersianDateToGregorian(item.fromDate),
                        To = DateTimeUtility.PersianDateToGregorian(item.toDate),
                        Percent = item.percent
                    });
                }
                var done = discountTableService.Update(data.id, dataEntity, out errors);
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetHotelUnitData(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                int userId = 0;
                var data = advertiseService.GetHotelUnitDTO(id, out userId);
                if (userId != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetHotelUnitData", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        public JsonResult EditHotelUnitData(string cid = null, string token = null,
            string data_string = null)
        {
            var data = JsonConvert.DeserializeObject<ApiHotelUnitDTO>(data_string);
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }
                var advertise = advertiseService.Find(data.id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = new List<string>() { "این آگهی متعطلق به شما نیست" }
                    });
                }
                List<string> errors;
                var done = advertiseService.UpdateHotelUnitDTO(data, out errors);
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.EditHotelUnitData", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult GetAllRegions(string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var data = regionService.GetRegionHierarchy();
                return GenerateJsonResult(new
                {
                    done = true,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public JsonResult CheckSetAsOccupiedDateRange(string cid, string token,
           long id, string from_date, string to_date)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                var checkResult = advertiseService.CheckSetAsOccupiedDateRange(id, from_date, to_date);
                return GenerateJsonResult(new
                {
                    done = checkResult.Result == CheckSetOccupiedResult.OK ||
                        checkResult.Result == CheckSetOccupiedResult.ContainsReserveRequest,
                    msg = checkResult.ToString()
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SetAsOccupiedDateRange(string cid, string token,
           long id, string from_date, string to_date)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                var checkResult = advertiseService.CheckSetAsOccupiedDateRange(id,
                    from_date, to_date);
                if (checkResult.Result == CheckSetOccupiedResult.OK ||
                    checkResult.Result == CheckSetOccupiedResult.ContainsReserveRequest)
                {
                    extrinsicReserveService.Insert(id, from_date, to_date, ActionSourceEnum.Application, user.Id, advertise.Count);
                    return GenerateJsonResult(new
                    {
                        done = true,
                        msg = checkResult.ToString()
                    });
                }
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = checkResult.ToString()
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult RemoveFromOccupiedDateRange(string cid, string token,
            long id, string from_date, string to_date)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                var checkResult = advertiseService.CheckUnsetOccupiedDateRange(id, from_date, to_date);
                if (checkResult.Result == CheckUnsetOccupiedResult.OK)
                {
                    advertiseService.DeleteExtrinsicReserves(id, from_date, to_date);
                    return GenerateJsonResult(new
                    {
                        done = true,
                        msg = "محدوده انتخاب شده از روز های پر حذف شد"
                    });
                }
                else
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = checkResult.ToString()
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SetPriceForDateRange(string cid, string token,
            long id, string from_date, string to_date, int price)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                string msg;
                var done = priceTableService.SetAccommodationPriceInDate(id, from_date, to_date, price, out msg);
                return GenerateJsonResult(new
                {
                    done = done,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult AdvertiseSuspendToggle(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                var target_status = advertiseService.ToggleSuspension(id);
                return GenerateJsonResult(new
                {
                    done = true,
                    status = (int)target_status,
                    statusTitle = AdvertiseMainLocalization.GetAdvertiseStatusString(
                        (int)advertise.Status),
                    statusColor = AdvertiseStyleHelper.GetAdvertiseStatusColor(
                        (int)advertise.Status)
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }
        public JsonResult AdvertiseSetAsTodayEmpty(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                var todayPersian = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date);
                var tommorowPersian = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date.AddDays(1));
                if (advertiseService.GetOccupiedDatesInRange(id, todayPersian, tommorowPersian).Any())
                {
                    var checkResult = advertiseService.CheckUnsetOccupiedDateRange(id, todayPersian, tommorowPersian);
                    if (checkResult.Result == CheckUnsetOccupiedResult.OK)
                    {
                        advertiseService.DeleteExtrinsicReserves(id, todayPersian, tommorowPersian);
                    }
                    else
                    {
                        return GenerateJsonResult(new
                        {
                            done = false,
                            msg = "واحد شما برای امروز رزرو شده است"
                        });
                    }
                }
                advertiseService.SetAsTodayEmpty(id);
                return GenerateJsonResult(new
                {
                    done = true,
                    msg = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult AdvertiseUnsetTodayEmpty(string cid, string token,
            long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != user.Id)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "این آگهی متعطلق به شما نیست"
                    });
                }
                advertiseService.UnsetTodayEmpty(id);
                return GenerateJsonResult(new
                {
                    done = true,
                    msg = "این اقامتگاه برای امروز به عنوان پر ثبت شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult InstantReserveRequest(string cid, string token,
            long id, bool ignoreMsg)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = false,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            var acc = advertiseService.Find(id);
            if (acc.UserID != user.Id)
            {
                return GenerateJsonResult(new InstantReserveRequestResultDTO()
                {
                    status = 0,
                    msg = "شما مجوز این کار را ندارید"
                });
            }
            if (user.InstantReserveAccess == InstantReserveAccessEnum.Banned)
            {
                return GenerateJsonResult(new InstantReserveRequestResultDTO()
                {
                    status = 0,
                    msg = "این امکان برای شما غیر فعال شده است"
                });
            }
            bool needMsg;
            advertiseService.RequestInstantReserve(id, ignoreMsg, user.Id,
                user.Id, ActionLog.ActionSourceEnum.Application,
                user.InstantReserveAccess, out needMsg);
            acc = advertiseService.Find(id);
            InstantReserveRequestResultDTO result;
            if (needMsg)
            {
                result = new InstantReserveRequestResultDTO()
                {
                    status = 1,
                    needMsg = true
                };
            }
            else
            {
                result = new InstantReserveRequestResultDTO()
                {
                    status = 1,
                    needMsg = false,
                    msg = acc.InstantReserveStatus == InstantReserveStatusEnum.Requested ?
                          "درخواست فعال سازی شما ارسال شد و بعد از تایید کارشناس این امکان برای این اقامتگاه فعال میشود" :
                          "امکان رزرو آنی برای این اقامتگاه فعال شد",
                    newData = new InstantReserveDetailDTO()
                    {
                        status = acc.InstantReserveStatus,
                        statusString = AdvertiseMainLocalization.GetInstantReserveStatusString(acc.InstantReserveStatus),
                        statusColor = AdvertiseStyleHelper.GetInstantReserveStatusColor(acc.InstantReserveStatus),
                        banned = user.InstantReserveAccess == InstantReserveAccessEnum.Banned,
                        buttonTitle = AdvertiseMainLocalization.GetInstantReserveButtonTitle(acc.InstantReserveStatus, user.InstantReserveAccess == InstantReserveAccessEnum.Banned)
                    }
                };
            }
            return GenerateJsonResult(result);
        }

        public JsonResult InstantReserveCancel(string cid,
            string token, long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = false,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            var acc = advertiseService.Find(id);
            if (acc.UserID != user.Id)
            {
                return GenerateJsonResult(new InstantReserveRequestResultDTO()
                {
                    status = 0,
                    msg = "شما مجوز این کار را ندارید"
                });
            }
            if (user.InstantReserveAccess == InstantReserveAccessEnum.Banned)
            {
                return GenerateJsonResult(new InstantReserveRequestResultDTO()
                {
                    status = 0,
                    msg = "این امکان برای شما غیر فعال شده است"
                });
            }
            advertiseService.CancelInstantReserve(id, user.Id, user.Id,
                            ActionLog.ActionSourceEnum.Application);
            var result = new InstantReserveRequestResultDTO()
            {
                status = 1,
                newData = new InstantReserveDetailDTO()
                {
                    status = acc.InstantReserveStatus,
                    statusString = AdvertiseMainLocalization.GetInstantReserveStatusString(acc.InstantReserveStatus),
                    statusColor = AdvertiseStyleHelper.GetInstantReserveStatusColor(acc.InstantReserveStatus),
                    banned = user.InstantReserveAccess == InstantReserveAccessEnum.Banned,
                    buttonTitle = AdvertiseMainLocalization.GetInstantReserveButtonTitle(acc.InstantReserveStatus, user.InstantReserveAccess == InstantReserveAccessEnum.Banned)
                }
            };
            return GenerateJsonResult(result);
        }

        public JsonResult GetInstnatReserveBanReason(string cid,
            string token, long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = false,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            return GenerateJsonResult(
                advertiseService.
                GetInstantReserveBanReason(id));
        }

        public JsonResult GetStayDuration(string cid, string token, long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                var acc = advertiseService.Find(id);
                if (user.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                var data = new StayDurationDTO()
                {
                    id = acc.Id,
                    min = acc.MinReserveDays,
                    max = acc.MaxReserveDays
                };
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SetStayDuration(string cid, string token,
            long id, string minStr = "0", string maxStr = "0")
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                var acc = advertiseService.Find(id);
                if (user.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                if (string.IsNullOrEmpty(minStr) ||
                    minStr == "undefined")
                {
                    minStr = "0";
                }
                if (string.IsNullOrEmpty(maxStr) ||
                    maxStr == "undefined")
                {
                    maxStr = "0";
                }
                int min, max;
                if (!int.TryParse(minStr, out min) ||
                    !int.TryParse(maxStr, out max))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "لطفا عدد وارد کنید"
                    });
                }
                if (min == 1)
                {
                    min = 0;
                }
                if (max > 0 && max < min)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "حداکثر مدت رزرو نباید از حداقل کمتر باشد. لطفا بررسی کنید."
                    });
                }
                advertiseService.SetStayDuration(id, min, max);
                var data = new StayDurationDTO()
                {
                    id = id,
                    min = min,
                    max = max
                };
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = new
                    {
                        msg = "حداقل اقامت " +
                        (min == 0 ? "بدون محدودیت" : min + " شب") +
                        " و حداکثر اقامت " +
                        (max == 0 ? "بدون محدودیت" : max + " شب") +
                        " تعیین شد.",
                        status = 1,
                        data = data
                    },
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult GetInstantReserveStart(string cid, string token, long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                var acc = advertiseService.Find(id);
                if (user.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                var data = new InstantReserveMaxStartDTO()
                {
                    id = acc.Id,
                    maxStart = acc.MaxInstantReserveStart
                };
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SetInstantReserveStart(string cid, string token,
            long id, int maxStart)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                var acc = advertiseService.Find(id);
                if (user.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                advertiseService.SetMaxInstantReserveStart(id, maxStart);
                var data = new InstantReserveMaxStartDTO()
                {
                    id = id,
                    maxStart = maxStart
                };
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = new
                    {
                        msg = "حداکثر شروع سفر تا " +
                        maxStart + " روز" +
                        " تعیین شد.",
                        status = 1,
                        data = data
                    }
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult GetNorouzPrice(string cid, string token, long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                int userId = 0;
                var data = advertiseService.GetNorouzPriceDTO(id, out userId);
                if (user.Id != userId)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetNorouzPrice", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SetNorouzPrice(string cid, string token,
            long id, int norouzPrice, int overCapacityPrice = 0,
            int buildNumber = 0)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                var acc = advertiseService.Find(id);
                if (user.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                var data = new ApiNorouzPriceDTO();
                data.SetNorouzPrice(norouzPrice);
                if (buildNumber > 0)
                {
                    data.SetNorouzOverCapacityPrice(overCapacityPrice);
                }
                List<string> errors = new List<string>();
                if (data.Validate(out errors))
                {
                    advertiseService.SetNorouzPrice(id, norouzPrice, overCapacityPrice, buildNumber);
                }
                var priceString = norouzPrice > 0 ? string.Format("{0:n0}", norouzPrice) + " تومان" : "بدون تفاوت با روز های عادی";
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = new
                    {
                        msg = "قیمت روز های نوروز " +
                        priceString +
                        " تعیین شد.",
                        status = 1,
                        data = data
                    }
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.SetNorouzPrice", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult GetNorouzMinReserve(string cid, string token, long id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                var acc = advertiseService.Find(id);
                if (user.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                var data = new ApiNorouzMinReserveDateDTO();
                data.id = id;
                data.data = acc.unixNorouzMinRequestDate < 1 ? null :
                    DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(acc.unixNorouzMinRequestDate))
                    .Replace(",", "/");
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdvertiseApi.GetNorouzMinReserve", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        public JsonResult SetNorouzMinReserve(string cid, string token,
            long id, string minReserveDate)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            var user = GetUser(token);
            if (user.Id < 1)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "ابتدا با حساب کاربری خود وارد شوید"
                });
            }
            try
            {
                var acc = advertiseService.Find(id);
                if (user.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این اقامتگاه متعلق به شما نمیباشد"
                    });
                }
                var data = new ApiNorouzMinReserveDateDTO();
                data.data = string.IsNullOrEmpty(minReserveDate) || minReserveDate == "null" ? null : minReserveDate.Replace(",", "/");
                long unix = 0;
                if (!string.IsNullOrEmpty(data.data))
                {
                    var persianDate = data.data.Replace("/", ",");
                    var gregorianDate = DateTimeUtility.PersianDateToGregorian(persianDate).Date;
                    unix = string.IsNullOrEmpty(data.data) ? 0 :
                        DateTimeUtility.DateValueOfJS(gregorianDate);
                }
                advertiseService.SetNorouzMinReserveDate(id, unix);
                var dateString = !string.IsNullOrEmpty(data.data) ?
                    "تاریخ " + data.data : "بدون محدودیت";
                return GenerateJsonResult(new
                {
                    status = 1,
                    data = new
                    {
                        msg = "حداقل درخواست رزرو برای نوروز " +
                        dateString +
                        " تعیین شد.",
                        status = 1,
                        data = data
                    }
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        public JsonResult AddAdvertiseReport(string cid, string token, AdvertiseReportDTO data)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser(token);
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        errors = new List<string>() { "ابتدا با حساب کاربری خود وارد شوید" }
                    });
                }

                var acc = advertiseService.Find(data.accId);
                List<string> errors;
                bool done;
                var dataEntity = new AdvertiseReport()
                {
                    Id = data.id,
                    AdvertiseID = data.accId,
                    Reason = data.reason,
                    ReasonString = data.reasonString
                };
                if (data.id > 0)
                {
                    done = advertiseReportService.Update(dataEntity, out errors);
                }
                else
                {
                    done = advertiseReportService.Insert(dataEntity, out errors);
                }
                return GenerateJsonResult(new
                {
                    done = done,
                    errors = errors,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    errors = new List<string>() { "متاسفانه عملیات با خطا مواجه شد" }
                });
            }
        }

        public static string GetRulesText(out string[] paragraphs)
        {
            paragraphs = new string[4];
            paragraphs[0] = "کنسل نمودن رزرو توسط مهمان تا ۷۲ ساعت مانده به شروع اقامت: کسر ۱۰٪ از مبلغ کل رزرو و بازگشت باقی‌مانده مبلغ.";
            paragraphs[1] = "کنسل نمودن رزرو توسط مهمان کمتر از ۷۲ ساعت مانده به شروع اقامت: کسر مبلغ اولین شب رزرو و بازگشت باقی‌مانده مبلغ.";
            paragraphs[2] = "کنسل نمودن رزرو توسط مهمان در روز شروع اقامت: کسر مبلغ ۲ شب اول رزرو و بازگشت باقی‌مانده مبلغ";
            paragraphs[3] = "در ایام پیک تعطیلات، بازه‌ی ۷۲ ساعت، ۱ هفته محاسبه می‌شود و امکان کنسلی وجود ندارد";
            return "قوانین کنسلی توسط مهمان:";
        }
    }
}