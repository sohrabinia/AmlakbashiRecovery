using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Host.Authentication;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILog logger;
        public AppAdvertiseController(IAdvertiseAppService advertiseService,
            ICategoryAppService categoryService,
            IRegionAppService regionService,
            IReportItemAppService reportItemService,
            IUserAccessor userAccessor,
            ICacheManager cacheManager,
            IWebHostEnvironment webHostEnvironment,
            ILog logger)
        {
            this.advertiseService = advertiseService;
            this.categoryService = categoryService;
            this.regionService = regionService;
            this.reportItemService = reportItemService;
            this.userAccessor = userAccessor;
            this.cacheManager = cacheManager;
            this.webHostEnvironment = webHostEnvironment;
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

        [Authorize]
        [HttpGet]
        public ActionResult UpdateBasic(long id = -1)
        {
            try
            {
                bool isEdit;
                int level;
                var director = advertiseService.GetBasicForm(id, out isEdit, out level);
                var model = new BasicFormDTO();
                model = BasicFormDTO.Generate(director, id);
                if (director.Mode == Advertise.AdvertiseMode.Parent && isEdit)
                {
                    ViewBag.childs = advertiseService.GetAccChilds(id);
                }
                ViewBag.isEdit = isEdit;
                ViewBag.type = director.AdvertiseType;
                ViewBag.level = level;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AccBasicForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateBasic(Advertise data, bool isEdit = false, int tab = 0)
        {
            try
            {
                Dictionary<string, string> errors;
                List<string> groupErrors;
                int level;
                var director = advertiseService.SubmitBasicForm(data, userAccessor.CurrentUser.Id, out errors, out groupErrors, out level);
                if (errors.Any() || groupErrors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    if (isEdit)
                    {
                        ViewBag.childs = advertiseService.GetAccChilds(data.Id);
                    }
                    ViewBag.errors = groupErrors;
                    ViewBag.isEdit = isEdit;
                    ViewBag.type = director.AdvertiseType;
                    ViewBag.level = level;
                    return View(BasicFormDTO.Generate(director, data.Id));
                }
                switch (tab)
                {
                    case 2:
                        return RedirectToAction(nameof(UpdateGeneral), new
                        {
                            id = data.Id
                        });
                    case 3:
                        return RedirectToAction(nameof(UpdateExtra), new
                        {
                            id = data.Id
                        });
                    case 4:
                        if (director.AdvertiseType == Advertise.AdvertiseType.Complex || director.AdvertiseType == Advertise.AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(UpdateComplexUnit), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(UpdateHotelRoom), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                    default:
                        return RedirectToAction(nameof(UpdateGeneral), new
                        {
                            id = data.Id
                        });
                }
            }
            catch (Exception exc)
            {
                logger.Error("AccBasicForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult UpdateGeneral(long id)
        {
            try
            {
                bool isEdit;
                int level;
                var director = advertiseService.GetGeneralForm(id, out isEdit, out level);
                var model = new GeneralFormDTO();
                model = GeneralFormDTO.Generate(director, id);
                if (director.Mode == Advertise.AdvertiseMode.Parent && isEdit)
                {
                    ViewBag.childs = advertiseService.GetAccChilds(id);
                }
                ViewBag.isEdit = isEdit;
                ViewBag.type = director.AdvertiseType;
                ViewBag.level = level;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AccGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateGeneral(Advertise data, bool isEdit = false, int tab = 0)
        {
            try
            {
                if (data.Area < 1)
                {
                    data.Area = null;
                }
                if (data.PhotoID < 1)
                {
                    data.PhotoID = null;
                }
                Dictionary<string, string> errors;
                List<string> groupErrors;
                int level;
                var director = advertiseService.SubmitGeneralForm(data, out errors, out groupErrors, out level,
                    webHostEnvironment.WebRootPath, isEdit);
                if (errors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    if (isEdit)
                    {
                        ViewBag.childs = advertiseService.GetAccChilds(data.Id);
                    }
                    ViewBag.errors = groupErrors;
                    ViewBag.isEdit = isEdit;
                    ViewBag.type = director.AdvertiseType;
                    ViewBag.level = level;
                    return View(GeneralFormDTO.Generate(director, data.Id));
                }
                switch (tab)
                {
                    case 1:
                        return RedirectToAction(nameof(UpdateBasic), new
                        {
                            id = data.Id
                        });
                    case 3:
                        return RedirectToAction(nameof(UpdateExtra), new
                        {
                            id = data.Id
                        });
                    case 4:
                        if (director.AdvertiseType == Advertise.AdvertiseType.Complex || director.AdvertiseType == Advertise.AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(UpdateComplexUnit), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(UpdateHotelRoom), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                    default:
                        return RedirectToAction(nameof(UpdateExtra), new
                        {
                            id = data.Id
                        });
                }
            }
            catch (Exception exc)
            {
                logger.Error("AccGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult UpdateExtra(long id)
        {
            try
            {
                bool isEdit;
                int level;
                var director = advertiseService.GetExtraForm(id, out isEdit, out level);
                var model = new ExtraFormDTO();
                model = ExtraFormDTO.Generate(director, id);
                if (director.Mode == Advertise.AdvertiseMode.Parent && isEdit)
                {
                    ViewBag.childs = advertiseService.GetAccChilds(id);
                }
                ViewBag.isEdit = isEdit;
                ViewBag.type = director.AdvertiseType;
                ViewBag.level = level;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AccExtraForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateExtra(Advertise data, PoolInputDTO poolDTO, bool isEdit = false, int tab = 0)
        {
            try
            {
                Dictionary<string, string> errors;
                List<string> groupErrors;
                int level;
                if (data.Pool == true)
                {
                    data.PoolFeatures = poolDTO.ConvertToEnum();
                }
                else
                {
                    data.PoolFeatures = Advertise.PoolFeaturesEnum.None;
                }
                var director = advertiseService.SubmitExtraForm(data, out errors, out groupErrors, out level, isEdit);
                if (errors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    if (isEdit)
                    {
                        ViewBag.childs = advertiseService.GetAccChilds(data.Id);
                    }
                    ViewBag.isEdit = isEdit;
                    ViewBag.errors = groupErrors;
                    ViewBag.type = director.AdvertiseType;
                    ViewBag.level = level;
                    return View(ExtraFormDTO.Generate(director, data.Id));
                }

                var isAdd = data.Status == Advertise.AdvertiseStatus.NotCompleted;
                switch (tab)
                {
                    case 1:
                        return RedirectToAction(nameof(UpdateBasic), new
                        {
                            id = data.Id
                        });
                    case 2:
                        return RedirectToAction(nameof(UpdateGeneral), new
                        {
                            id = data.Id
                        });
                    case 4:
                        if (director.AdvertiseType == Advertise.AdvertiseType.Complex || director.AdvertiseType == Advertise.AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(UpdateComplexUnit), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(UpdateHotelRoom), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                    default:
                        if (director.Mode == Advertise.AdvertiseMode.Parent)
                        {
                            if (director.AdvertiseType == Advertise.AdvertiseType.Complex || director.AdvertiseType == Advertise.AdvertiseType.HotelApartment)
                            {
                                return RedirectToAction(nameof(UpdateComplexUnit), new
                                {
                                    parentId = data.Id
                                });
                            }
                            else
                            {
                                return RedirectToAction(nameof(UpdateHotelRoom), new
                                {
                                    parentId = data.Id
                                });
                            }
                        }
                        else
                        {
                            var user = userAccessor.CurrentUser;
                            var success_str = "آگهی شما با موفقیت " + (isAdd ? "ثبت" : "ویرایش") + " و پس از تایید کارشناس " + (isAdd ? "" : "دوباره ") + "نمایش داده میشود . \n";
                            TempData["alert"] = success_str;
                            return Redirect("/app/advertise/list");
                        }
                }
            }
            catch (Exception exc)
            {
                logger.Error("AccExtraForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult UpdateHotelRoom(long parentId, long id = 0)
        {
            try
            {
                var childs = advertiseService.GetAccChilds(parentId);
                if (id == -1)
                {
                    var ids = childs.Values.SelectMany(m => m.Keys.Select(s => s)).ToList();
                    if (ids.Count > 0)
                    {
                        id = ids.First();
                    }
                    else
                    {
                        id = 0;
                    }
                }
                bool isEdit;
                var director = advertiseService.GetHotelForm(id, parentId, out isEdit);
                var model = new HotelUnitFormDTO();
                if (director.AdvertiseType > Advertise.AdvertiseType.None)
                {
                    model = HotelUnitFormDTO.Generate(director, id, parentId);
                }
                else
                {
                    model.Id = id;
                    model.ParentId = parentId;
                    model.Type = director.AdvertiseType;
                }
                ViewBag.isEdit = isEdit;
                ViewBag.childs = childs;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AccHotelForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateHotelRoom(Advertise data, bool isEdit = false, bool saveAndNewRoom = false, int tab = -1)
        {
            try
            {
                if (tab >= 0)
                {
                    saveAndNewRoom = true;
                }
                Dictionary<string, string> errors;
                List<string> groupErrors;
                var director = advertiseService.SubmitHotelForm(data, userAccessor.CurrentUser.Id, out errors, out groupErrors, saveAndNewRoom);
                if (errors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    ViewBag.isEdit = isEdit;
                    ViewBag.errors = groupErrors;
                    ViewBag.childs = advertiseService.GetAccChilds((long)data.ParentId);
                    return View(HotelUnitFormDTO.Generate(director, data.Id, (long)data.ParentId));
                }
                if (tab >= 0)
                {
                    switch (tab)
                    {
                        case 1:
                            return RedirectToAction(nameof(UpdateBasic), new
                            {
                                id = data.ParentId
                            });
                        case 2:
                            return RedirectToAction(nameof(UpdateGeneral), new
                            {
                                id = data.ParentId
                            });
                        case 3:
                            return RedirectToAction(nameof(UpdateExtra), new
                            {
                                id = data.ParentId
                            });
                        default:
                            return RedirectToAction(nameof(UpdateHotelRoom), new
                            {
                                id = tab,
                                parentId = data.ParentId
                            });
                    }
                }
                if (saveAndNewRoom)
                {
                    return RedirectToAction(nameof(UpdateHotelRoom), new
                    {
                        parentId = data.ParentId
                    });
                }
                return Redirect("/app/advertise/list");

            }
            catch (Exception exc)
            {
                logger.Error("AccHotelForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult UpdateComplexUnit(long parentId, long id = 0)
        {
            try
            {
                var childs = advertiseService.GetAccChilds(parentId);
                if (id == -1)
                {
                    var ids = childs.Values.SelectMany(m => m.Keys.Select(s => s)).ToList();
                    if (ids.Count > 0)
                    {
                        id = ids.First();
                    }
                    else
                    {
                        id = 0;
                    }
                }
                bool isEdit;
                Advertise.AdvertiseType parentType;
                var director = advertiseService.GetComplexForm(id, parentId, out parentType, out isEdit);
                var model = new ComplexUnitFormDTO(parentType);
                if (director.AdvertiseType > Advertise.AdvertiseType.None)
                {
                    model = ComplexUnitFormDTO.Generate(director, id, parentId, parentType);
                }
                else
                {
                    model.Id = id;
                    model.ParentId = parentId;
                    model.ParentType = parentType;
                }
                ViewBag.isEdit = isEdit;
                ViewBag.post = false;
                ViewBag.childs = childs;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AccComplexForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateComplexUnit(Advertise data, PoolInputDTO poolDTO, bool isEdit = false, bool saveAndNewRoom = false, int tab = -1)
        {
            try
            {
                if (tab >= 0)
                {
                    saveAndNewRoom = true;
                }
                if (data.PhotoID < 1)
                {
                    data.PhotoID = null;
                }
                if (data.Pool == true)
                {
                    data.PoolFeatures = poolDTO.ConvertToEnum();
                }
                else
                {
                    data.PoolFeatures = Advertise.PoolFeaturesEnum.None;
                }
                Dictionary<string, string> errors;
                List<string> groupErrors;
                Advertise.AdvertiseType parentType;
                var director = advertiseService.SubmitComplexForm(data, userAccessor.CurrentUser.Id, out errors, out groupErrors, saveAndNewRoom, out parentType,
                    webHostEnvironment.WebRootPath);
                if (errors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    ViewBag.isEdit = isEdit;
                    ViewBag.errors = groupErrors;
                    ViewBag.post = true;
                    ViewBag.childs = advertiseService.GetAccChilds((long)data.ParentId);
                    return View(ComplexUnitFormDTO.Generate(director, data.Id, (long)data.ParentId, parentType));
                }
                if (tab >= 0)
                {
                    switch (tab)
                    {
                        case 1:
                            return RedirectToAction(nameof(UpdateBasic), new
                            {
                                id = data.ParentId
                            });
                        case 2:
                            return RedirectToAction(nameof(UpdateGeneral), new
                            {
                                id = data.ParentId
                            });
                        case 3:
                            return RedirectToAction(nameof(UpdateExtra), new
                            {
                                id = data.ParentId
                            });
                        default:
                            return RedirectToAction(nameof(UpdateComplexUnit), new
                            {
                                id = tab,
                                parentId = data.ParentId
                            });
                    }
                }
                if (saveAndNewRoom)
                {
                    return RedirectToAction(nameof(UpdateComplexUnit), new
                    {
                        parentId = data.ParentId
                    });
                }
                return Redirect("/app/advertise/list");
            }
            catch (Exception exc)
            {
                logger.Error("AccComplexForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult SelectComplexUnitType(long parentId, int childType)
        {
            var paradigmAdvertise = new Advertise()
            {
                TypeID = (Advertise.AdvertiseType)childType,
                Floor = Advertise.FloorItems.Unset
            };
            var childs = advertiseService.GetAccChilds(parentId);
            if (childs != null && childs.Count > 0 && childs.Keys.Contains((Advertise.AdvertiseType)childType))
            {
                var paradigmId = childs[(Advertise.AdvertiseType)childType].Keys.Last();
                paradigmAdvertise = advertiseService.Find(paradigmId);
            }
            var director = new AdvertiseDirector(paradigmAdvertise, DirectorType.ComplexUnit);
            var model = ComplexUnitFormDTO.Generate(director, 0, parentId, Advertise.AdvertiseType.Complex);
            if (model.floor != null)
            {
                model.floor.Floor = Advertise.FloorItems.Unset;
            }
            model.titleAndDesc = new TitleDescInputDTO(false);
            return PartialView("~/Views/Accomodation/_AccComplexTypeForm.cshtml", model);
        }
    }
}
