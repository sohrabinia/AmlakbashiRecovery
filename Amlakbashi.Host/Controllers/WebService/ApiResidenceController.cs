using Amlakbashi.Application.DTOs;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiResidenceController : ApiBaseController
    {
        private readonly IAdvertiseAppService advertiseService;
        private readonly ICategoryAppService categoryService;
        private readonly IRegionAppService regionService;
        private readonly IUserAppService userService;
        private readonly ICacheManager cacheManager;
        public ApiResidenceController(IAdvertiseAppService advertiseService,
            ICategoryAppService categoryService,
            IRegionAppService regionService,
            IUserAppService userService,
            ICacheManager cacheManager)
        {
            this.advertiseService = advertiseService;
            this.categoryService = categoryService;
            this.regionService = regionService;
            this.userService = userService;
            this.cacheManager = cacheManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public AdvertiseListResponse Get([FromQuery] AdvertiseGetListRequest request)
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
            request.userId = User.GetId();

            var response = advertiseService.Filter(request);

            //if (canUseCache)
            //{
            //    cacheManager.Set(cachedName, categoryItemDTO);
            //}

            return response;
        }

        [AllowAnonymous]
        [HttpGet("{id:long}")]
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
            var hostCreateDate = userService.GetIdentityUser(advertise.User.PhoneNumber).CreateDate;
            if (hostCreateDate != null)
            {
                response.hostCreateDate = StringUtility.EnglishNumberToPersian(DateTimeUtility.ConvertDate(hostCreateDate.Value));
            }
            if (User.Identity.IsAuthenticated && userService.Find(User.GetId()).Favorite.Any(x => x.AdvertiseID == id))
            {
                response.favorite = true;
            }
            return Ok(response);
        }

        [HttpGet("list")]
        public List<AdvertiseBasicInfoReponse> GetUserAdvertises(Advertise.AdvertiseStatus status = Advertise.AdvertiseStatus.Published,
            int page = 1, int pageItemCount = 20)
        {
            var advertises = advertiseService.GetAdvertisesByUserId(User.GetId());
            advertises = advertises.Where(x => x.Status == status).ToList();
            List<AdvertiseBasicInfoReponse> response = new List<AdvertiseBasicInfoReponse>();
            response.AddRange(advertises.Select(x => (AdvertiseBasicInfoReponse)x));
            return response;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(AdvertisePostCreateRequest request)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = User.GetId();
            var result = await advertiseService.CreateAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return CreatedAtAction(nameof(Get), new { id = result.Result });
        }

        [HttpGet("update/basic/{id:long}")]
        public IActionResult GetBasicInfoForUpdate(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            AdvertiseGetBasicInfoForUpdateResponse response = advertise;
            return Ok(response);
        }

        [HttpPost("update/basic")]
        public async Task<IActionResult> UpdateBasicInfo(AdvertisePostBasicInfoRequest request)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = User.GetId();
            var result = await advertiseService.UpdateBasicInfoAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpGet("update/general/{id:long}")]
        public IActionResult GetGeneralInfoForUpdate(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            AdvertiseGetGeneralInfoForUpdateResponse response = advertise;
            return Ok(response);
        }

        [HttpPost("update/general")]
        public async Task<IActionResult> UpdateGeneralInfo(AdvertisePostGeneralInfoRequest request)
        {
            var checkRegionResult = regionService.IsValidRegions(request.province, request.city, request.area);
            if (checkRegionResult.HasError())
            {
                return BadRequest(checkRegionResult.GetErrors());
            }
            request.userId = User.GetId();
            var result = await advertiseService.UpdateGeneralInfoAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpGet("update/supplementary/{id:long}")]
        public IActionResult GetSupplementaryInfoForUpdate(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            return Ok((AdvertiseGetSupplementaryInfoForUpdateResponse)advertise);
        }

        [HttpPost("update/supplementary")]
        public async Task<IActionResult> UpdateSupplementaryInfo(AdvertisePostSupplementaryInfoRequest request)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = User.GetId();
            var result = await advertiseService.UpdateSupplementaryInfoAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpGet("update/final/{id:long}")]
        public IActionResult GetFinalInfoForUpdate(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            return Ok((AdvertiseGetFinalInfoForUpdateResponse)advertise);
        }

        [HttpPost("update/final")]
        public async Task<IActionResult> UpdateFinalInfo(AdvertisePostFinalInfoRequest request)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = User.GetId();
            var result = await advertiseService.UpdateFinalInfoAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpGet("update/hotelroom/{id:long}")]
        public IActionResult GetHotelRoomInfoForUpdate(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null || advertise.Mode != Advertise.AdvertiseMode.Child)
            {
                return NotFound();
            }
            return Ok((AdvertiseGetHotelRoomInfoForUpdateResponse)advertise);
        }

        [HttpPost("update/hotelroom")]
        public async Task<IActionResult> CreateOrUpdateHotelRoomInfo(AdvertisePostHotelRoomInfoRequest request)
        {
            if (request.IsValid(ModelState) == false)
            {
                return BadRequest(ModelState);
            }
            request.userId = User.GetId();
            ServiceResult result = null;
            if (request.unitId > 0)
            {
                result = await advertiseService.UpdateHotelRoomInfoAsync(request);
            }
            else
            {
                result = await advertiseService.CreateHotelRoomAsync(request);
            }
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [AllowAnonymous]
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
                imageUrl = x.GetMainImageUrl()
            }));
            return Ok(response);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [HttpPost("calendar")]
        public async Task<IActionResult> UpdateCalendarData(AdvertiseUpdateCalendarRequest request)
        {
            request.userId = User.GetId();
            request.actionSource = ActionLog.ActionSourceEnum.WebsiteDashboard;
            var result = await advertiseService.UpdateCalendarAsync(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpPost("manualprice")]
        public IActionResult UpdateManualPrice(AdvertiseUpdatePriceRequest request,
            [FromServices] IPriceTableAppService priceTableService)
        {
            var result = priceTableService.UpdateAdvertiseManualPrices(request);
            if (result.HasError())
            {
                return BadRequest(result.GetErrors());
            }
            return Ok();
        }

        [HttpGet("favorite")]
        public AdvertiseListResponse GetFavorites(int page = 1)
        {
            var favoriteAdvertises = advertiseService.GetUserFavoriteAdvertises(User.GetId(), page);
            return favoriteAdvertises;
        }

        [HttpPost("favorite/{id:long}")]
        public IActionResult AddFavorite(long id)
        {
            var advertise = advertiseService.Find(id);
            if (advertise == null)
            {
                return NotFound();
            }
            userService.AddFavorite(User.GetId(), id);
            return CreatedAtAction(nameof(GetFavorites), null);
        }

        [HttpDelete("favorite/{id:long}")]
        public IActionResult DeleteFavorite(long id)
        {
            if (userService.DeleteFavorite(User.GetId(), id))
            {
                return NoContent();
            }
            return BadRequest("advertise id is incorrect");
        }

        [AllowAnonymous]
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

        [HttpPost("instantreserve/permanent")]
        public async Task<IActionResult> UpdatePermanentInstantReserve(UpdatePermanentInstantReserveRequest request)
        {
            var result = await advertiseService.UpdateInstantReserveStatus(request.residenceId,
                request.active ? Advertise.InstantReserveStatusEnum.Permanent : Advertise.InstantReserveStatusEnum.Calendar);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("instantreserve")]
        public IActionResult GetInstantReserveDates(long residenceId)
        {
            var residence = advertiseService.Find(residenceId);
            if (residence == null || residence.UserID != User.GetId())
            {
                return BadRequest("not allowed");
            }
            var instantReserveDates = residence.InstantReserveDates.Select(x => DateTimeUtility.DateValueOfJS(x.Date)).ToList();
            return Ok(SerializeUtility.SerializeToJS(instantReserveDates));
        }

        [HttpPost("instantreserve")]
        public async Task<IActionResult> AddInstantReserveDates(UpdateInstantReserveDatesRequest request)
        {
            var result = await advertiseService.AddInstantReserveDates(request.residenceId, request.fromDate, request.toDate, User.GetId());
            return result.HasError() ? BadRequest(result.GetErrors()) : Ok(SerializeUtility.SerializeToJS(result.Result));
        }

        [HttpDelete("instantreserve")]
        public async Task<IActionResult> DeleteInstantReserveDates(UpdateInstantReserveDatesRequest request)
        {
            var result = await advertiseService.DeleteInstantReserveDates(request.residenceId, request.fromDate, request.toDate, User.GetId());
            return result.HasError() ? BadRequest(result.GetErrors()) : Ok(SerializeUtility.SerializeToJS(result.Result));
        }
    }
}
