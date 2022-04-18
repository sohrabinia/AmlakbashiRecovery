using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/advertise")]
    public class ApiAdvertiseController : ApiBaseController
    {
        private readonly IAdvertiseAppService advertiseService;
        private readonly ICategoryAppService categoryService;
        private readonly IUserAppService userService;
        private readonly ICacheManager cacheManager;
        private readonly IUserAccessor userAccessor;
        public ApiAdvertiseController(IAdvertiseAppService advertiseService,
            ICategoryAppService categoryService,
            IUserAppService userService,
            IUserAccessor userAccessor,
            ICacheManager cacheManager)
        {
            this.advertiseService = advertiseService;
            this.categoryService = categoryService;
            this.userService = userService;
            this.cacheManager = cacheManager;
            this.userAccessor = userAccessor;
        }

        [HttpGet]
        public AdvertiseListResponse Get([FromQuery] AdvertiseListRequest request)
        {
            bool isArea = false;
            DynamicCategory category = categoryService.GetByRegion(request.regionId, request.residencyType, out isArea);

            // read from redis cache
            //bool canUseCache = string.IsNullOrEmpty(request.phrase) && area < 1;
            //var cachedName = $"{CacheNames.Category_Item_}{category.Id}";
            //if (canUseCache)
            //{
            //    var cachedData = cacheManager.Get<SearchResponse>(cachedName);
            //    if (cachedData != null)
            //    {
            //        return cachedData;
            //    }
            //}

            if (isArea)
            {
                request.area = request.regionId;
            }
            request.categoryId = category.Id;
            request.UserFavorites = userAccessor.CurrentUser.Id > 0 ?
                userAccessor.CurrentUser.Favorite : new List<UserFavorite>();

            var response = advertiseService.Filter(request);

            //if (canUseCache)
            //{
            //    cacheManager.Set(cachedName, categoryItemDTO);
            //}

            return response;
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            advertiseService.UpdateAccView(id);
            var response = new AdvertiseResponse();
            response = advertise;
            var hostCreateDate = userService.GetIdentityUser(advertise.User.MainMobile).CreateDate;
            if (hostCreateDate != null)
            {
                response.hostCreateDate = StringUtility.EnglishNumberToPersian(DateTimeUtility.ConvertDate(hostCreateDate.Value));
            }
            if (userAccessor.CurrentUser.Id > 0 && userAccessor.CurrentUser.Favorite.Any(x => x.AdvertiseID == id))
            {
                response.favorite = true;
            }
            return Ok(response);
        }

        [HttpGet("searchid")]
        public IActionResult SearchId(string id)
        {
            var advertises = advertiseService.Filter(id);
            if (advertises.Any() == false)
            {
                return NotFound();
            }
            var response = new List<AdvertiseSearchIdResponse>();
            response.AddRange(advertises.Select(x => new AdvertiseSearchIdResponse()
            {
                id = x.Id,
                title = x.Title,
                roomCount = x.Room,
                typeTitle = AdvertiseMainLocalization.GetAdvertiseTypePersianNameForAdminPanel(x.TypeID),
                provinceName = x.RegionProvince.PersianName,
                cityName = x.RegionCity.PersianName,
                imageUrl = $"/file/accthumbxxxlarge?accid={x.Id}&fileid={x.MainPhoto.Id}"
            }));
            return Ok(response);
        }

        [HttpGet("types")]
        public IList<AdvertiseTypesResponse> GetAdvertiseTypes()
        {
            var response = new List<AdvertiseTypesResponse>();
            var advertiseTypesList = Enum.GetValues<Advertise.AdvertiseType>().ToList();
            advertiseTypesList.Remove(Advertise.AdvertiseType.None);
            response.AddRange(advertiseTypesList.Select(x => new AdvertiseTypesResponse()
            {
                name = AdvertiseMainLocalization.GetAdvertiseTypePersianNameForAdminPanel(x),
                value = (int)x
            }));
            return response;
        }

        [HttpGet("calendar/{id:long}")]
        public IActionResult GetCalendarData(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            var response = new AdvertiseCalendarResponse()
            {
                occupiedDates = advertise.OccupiedDates().Select(x => DateTimeUtility.DateValueOfJS(x)).ToList(),
                prices = advertiseService.GetAccPriceDatesInfo(id).Select(x => new AdvertiseCalendarPriceItemResponse()
                {
                    date = x.Key,
                    price = x.Value.price,
                    discount = x.Value.off
                }).ToList()
            };
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("favorite")]
        public AdvertiseListResponse GetFavorites(int page = 1)
        {
            var favoriteAdvertises = advertiseService.GetUserFavoriteAdvertises(userAccessor.CurrentUser.Id, page);
            return favoriteAdvertises;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("favorite/{id:long}")]
        public IActionResult AddFavorite(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            userService.AddFavorite(userAccessor.CurrentUser.Id, id);
            return CreatedAtAction(nameof(GetFavorites), null);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("favorite/{id:long}")]
        public IActionResult DeleteFavorite(long id)
        {
            if (userService.DeleteFavorite(userAccessor.CurrentUser.Id, id))
            {
                return NoContent();
            }
            return BadRequest("advertise id is incorrect");
        }

        [HttpGet("rules/{id:long}")]
        public IActionResult GetRules(long id)
        {
            var advertise = advertiseService.Find(id);
            return Ok(new
            {
                party = advertise.AllowParty,
                pets = advertise.AllowPets,
                smoking = advertise.AllowSmoking,
                otherRules = advertise.OtherRules,
                requiredEvidences = advertise.EvidenceRequired,
            });
        }
    }
}
