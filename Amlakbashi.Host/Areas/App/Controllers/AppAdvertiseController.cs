using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Host.Authentication;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/advertise/[action]")]
    public class AppAdvertiseController : Controller
    {
        private readonly IAdvertiseAppService advertiseService;
        private readonly ICategoryAppService categoryService;
        private readonly IRegionAppService regionService;
        private readonly IReportItemAppService reportItemService;
        private readonly IUserAccessor userAccessor;
        private readonly ICacheManager cacheManager;
        private readonly ILog logger;
        public AppAdvertiseController(IAdvertiseAppService advertiseService,
            ICategoryAppService categoryService,
            IRegionAppService regionService,
            IReportItemAppService reportItemService,
            IUserAccessor userAccessor,
            ICacheManager cacheManager,
            ILog logger)
        {
            this.advertiseService = advertiseService;
            this.categoryService = categoryService;
            this.regionService = regionService;
            this.reportItemService = reportItemService;
            this.userAccessor = userAccessor;
            this.cacheManager = cacheManager;
            this.logger = logger;
        }

        [HttpGet("{advertiseId}")]
        public ActionResult Item(long advertiseId, string capacity = null,
            string empty_range_from = null, string empty_range_to = null)
        {
            try
            {
                var model = advertiseService.FindIncludingDeleted((long)advertiseId);
                if (model == null)
                {
                    return RedirectToRoute(new
                    {
                        controller = "errors",
                        action = "Http404"
                    });
                }
                advertiseService.UpdateAccView((long)advertiseId);
                ViewBag.amp_version = false;

                // get from redis cache
                var canUseCache = capacity == null && empty_range_from == null && empty_range_to == null;
                var cacheName = $"{CacheNames.Advertise_}{model.Id}";
                if (canUseCache)
                {
                    var cachedData = cacheManager.Get<AccommodationItemDTO>(cacheName);
                    if (cachedData != null)
                    {
                        return View(cachedData);
                    }
                }

                #region Initialize DTO
                var advertiseIds = advertiseService.GetAdvertiseIdsByUserId(model.UserID);
                var allUserReportItems = reportItemService.GetByAccList(advertiseIds);
                Dictionary<Advertise.AdvertiseType, IList<AdvertiseDirector>> childDirectors;
                var director = advertiseService.GetAdvertisePageData((long)advertiseId, out childDirectors);
                var accDTO = AccommodationItemDTO.Generate(userAccessor.CurrentUser, model,
                    director, childDirectors, allUserReportItems);
                accDTO.RawUrl = HttpContext.Request.Path.Value.Split('?')[0];
                accDTO.EmptyRangeFrom = empty_range_from;
                accDTO.EmptyRangeTo = empty_range_to;
                accDTO.RelatedLinkCapacity = capacity;
                accDTO.IsPreview = false;

                if (accDTO.CanPublish == false)
                {
                    var regionIds = regionService.GetParentIdsByCityId(model.City == null ? 0 : (int)model.City);
                    accDTO.RelatedCategories = new List<DynamicCategory>();
                    accDTO.RelatedCategories.Add(categoryService.GetAccItemLinks(model.Province, model.City, model.Area, model.TypeID).Last());
                }
                #endregion

                // set into redis cache
                if (canUseCache)
                {
                    cacheManager.Set(cacheName, accDTO);
                }
                return View(accDTO);
            }
            catch (Exception exc)
            {
                logger.Error("AppAdvertise.Item", exc);
                return NotFound("صفحه ی مورد نظر موجود نمی باشد.");
            }
        }

        [Authorize]
        public ActionResult List(int? page, string type = "all", string id = "-1")
        {
            int UserID = userAccessor.CurrentUser.Id;
            var id_long = string.IsNullOrEmpty(id) ? 0 : long.Parse(StringUtility.PersianNumberToEnglish(id));
            var model = advertiseService.Filter(type, UserID, id_long);
            var result = model
                .OrderBy(x => x.Status == 0 ? 0 : (x.Status == Advertise.AdvertiseStatus.FirstReady ?
                1 : (x.Status == Advertise.AdvertiseStatus.Published ? 2 : 3)))
                .ThenByDescending(x => x.CreateDate);
            var PageNumber = page ?? 1;
            var onePageOfModel = result.ToPagedList(PageNumber, 5);
            var pageCount = (int)Math.Ceiling(result.Count() / 5f);
            if (pageCount > 1 && PageNumber > pageCount)
                return Redirect("/errors/Http404");
            var finalModel = AccommodationManagerDTO.Generate(userAccessor.CurrentUser, onePageOfModel.ToList());
            var insertCount = (PageNumber - 1) * 5;
            for (int i = 0; i < insertCount; i++)
            {
                (finalModel.accList as List<DashboardAccDTO>).Insert(0, null);
            }
            var addCount = result.Count() - finalModel.accList.Count();
            for (int i = 0; i < addCount; i++)
            {
                (finalModel.accList as List<DashboardAccDTO>).Add(null);
            }
            finalModel.accList = finalModel.accList.ToPagedList(PageNumber, 5);

            ViewBag.alert_success = TempData["alert_success"];
            ViewBag.alert_error = TempData["alert_error"];
            ViewBag.EditMode = true;
            ViewBag.UserID = UserID;
            ViewBag.type = type;
            ViewBag.id = id_long;
            return View(finalModel);
        }

        [Authorize]
        public ActionResult Favorites()
        {
            try
            {
                var user = userAccessor.CurrentUser;
                var advertise_ids = user.Favorite.OrderByDescending(f => f.SetDate).
                    Select(f => f.AdvertiseID).Take(100).ToList();
                var model = advertiseService.GetAccListByIds(advertise_ids);
                List<AccommodationCardDTO> advertiseItemDTOs = new List<AccommodationCardDTO>();
                foreach (var item in model)
                {
                    var dto = (AccommodationCardDTO)item;
                    dto.Favourited = user.Id > 0 && user.Favorite.Any(x => x.AdvertiseID == item.Id);
                    advertiseItemDTOs.Add(dto);
                }
                ViewBag.userId = user.Id;
                return View(advertiseItemDTOs);
            }
            catch (Exception exc)
            {
                logger.Error("Post.FavoriteManager", exc);
                return Redirect("/errors/http404");
            }
        }
    }
}
