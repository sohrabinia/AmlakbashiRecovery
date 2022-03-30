using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
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
        public AdvertiseListResponse Get(AdvertisesRequest request)
        {
            bool isArea = false;
            DynamicCategory category = categoryService.GetByRegion(request.regionId, request.advertiseType, out isArea);

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

    }
}
