using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using log4net;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.Region;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using Amlakbashi.Core.Common.Utilities;
using static Amlakbashi.Core.Entities.User;
using Amlakbashi.Application.Services.Category.Interfaces;
using static Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs.CheckUnsetOccupiedDTO;
using static Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs.CheckSetOccupiedDTO;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using static Amlakbashi.Core.Entities.ActionLog;
using AccDashboardDTOs = Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using Amlakbashi.Host.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;

namespace Amlakbashi.Host.Controllers
{
    public class AccomodationController : BaseController
    {
        private readonly IAdvertiseAppService advertiseService;
        private readonly ICategoryAppService categoryService;
        private readonly IReportItemAppService reportItemService;
        private readonly IBlogPostAppService blogpostService;
        private readonly IRegionAppService regionService;
        private readonly IDiscountTableAppService discountTableService;
        private readonly IPriceTableAppService priceTableService;
        private readonly IExtrinsicReserveAppService extrinsicReserveService;
        private readonly IReserveAppService reserveService;
        private readonly IUserAppService userService;
        private readonly ILog logger;
        private readonly IUserAccessor userAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccomodationController(ILog logger,
            IAdvertiseAppService advertiseService,
            IExtrinsicReserveAppService extrinsicReserveService,
            ICategoryAppService categoryService,
            IBlogPostAppService blogpostService,
            IReportItemAppService reportItemService,
            IRegionAppService regionService,
            IDiscountTableAppService discountTableService,
            IPriceTableAppService priceTableService,
            IReserveAppService reserveService,
            IUserAppService userService,
            IUserAccessor userAccessor,
            IWebHostEnvironment webHostEnvironment
            )
        {
            this.logger = logger;
            this.advertiseService = advertiseService;
            this.extrinsicReserveService = extrinsicReserveService;
            this.categoryService = categoryService;
            this.blogpostService = blogpostService;
            this.reportItemService = reportItemService;
            this.regionService = regionService;
            this.discountTableService = discountTableService;
            this.priceTableService = priceTableService;
            this.reserveService = reserveService;
            this.userService = userService;
            this.userAccessor = userAccessor;
            this.webHostEnvironment = webHostEnvironment;
        }

        #region Admin Add/Edit Accommodation

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpGet]
        public ActionResult AdminBasicForm(long id)
        {
            try
            {
                var urlReferrer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(urlReferrer))
                {
                    HttpContext.Session.SetObjectAsJson("urlReferrer", urlReferrer);
                }
                AdvertiseType parentType;
                AdvertiseStatus status;
                var director = advertiseService.GetAdminForm(id, DirectorType.Basic, out parentType, out status);
                var model = BasicFormDTO.Generate(director, id);
                ViewBag.type = director.AdvertiseType;
                ViewBag.haveChild = advertiseService.GetAccChilds(id).Any();
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AdminBasicForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public ActionResult AdminBasicForm(Advertise data, bool forceSave = false, int tab = 0)
        {
            try
            {
                Dictionary<string, string> errors;
                List<string> groupErrors = new List<string>();
                var director = advertiseService.SubmitAdminBasicForm(data, out errors,
                    out groupErrors, userAccessor.CurrentUser.Id);
                if (groupErrors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    ViewBag.errors = groupErrors;
                    ViewBag.haveChild = advertiseService.GetAccChilds(data.Id).Any();
                    ViewBag.tab = tab;
                    return View(BasicFormDTO.Generate(director, data.Id));
                }
                switch (tab)
                {
                    case 2:
                        return RedirectToAction(nameof(AdminGeneralForm), new
                        {
                            id = data.Id
                        });
                    case 3:
                        return RedirectToAction(nameof(AdminExtraForm), new
                        {
                            id = data.Id
                        });
                    case 4:
                        if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(AdminComplexForm), new
                            {
                                parentId = data.Id
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(AdminHotelForm), new
                            {
                                parentId = data.Id
                            });
                        }
                    case 5:
                        return RedirectToAction(nameof(AdminStatusForm), new
                        {
                            id = data.Id
                        });
                    default:
                        return RedirectToAction(nameof(AdminGeneralForm), new
                        {
                            id = data.Id
                        });
                }
            }
            catch (Exception exc)
            {
                logger.Error("AdminBasicForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpGet]
        public ActionResult AdminGeneralForm(long id)
        {
            try
            {
                AdvertiseType parentType;
                AdvertiseStatus status;
                var director = advertiseService.GetAdminForm(id, DirectorType.General, out parentType, out status);
                var model = GeneralFormDTO.Generate(director, id);
                ViewBag.type = director.AdvertiseType;
                ViewBag.status = status;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AdminGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public ActionResult AdminGeneralForm(Advertise data, bool forceSave = false, int tab = 0)
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
                List<string> groupErrors = new List<string>();
                AdvertiseType parentType;
                AdvertiseStatus status;
                var director = advertiseService.SubmitAdminForm(data, out errors, out groupErrors,
                    forceSave, DirectorType.General, userAccessor.CurrentUser.Id, out parentType, out status,
                    webHostEnvironment.WebRootPath);
                var hasImportantError = errors.ContainsKey("Province") || errors.ContainsKey("City");
                if (hasImportantError || (forceSave == false && groupErrors.Any()))
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    if (hasImportantError)
                    {
                        ViewBag.hasImportantError = true;
                    }
                    ViewBag.errors = groupErrors;
                    ViewBag.tab = tab;
                    ViewBag.status = status;
                    return View(GeneralFormDTO.Generate(director, data.Id));
                }
                switch (tab)
                {
                    case 1:
                        return RedirectToAction(nameof(AdminBasicForm), new
                        {
                            id = data.Id
                        });
                    case 3:
                        return RedirectToAction(nameof(AdminExtraForm), new
                        {
                            id = data.Id
                        });
                    case 4:
                        if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(AdminComplexForm), new
                            {
                                parentId = data.Id
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(AdminHotelForm), new
                            {
                                parentId = data.Id
                            });
                        }
                    case 5:
                        return RedirectToAction(nameof(AdminStatusForm), new
                        {
                            id = data.Id
                        });
                    default:
                        return RedirectToAction(nameof(AdminExtraForm), new
                        {
                            id = data.Id
                        });
                }
            }
            catch (Exception exc)
            {
                logger.Error("AdminExtraForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpGet]
        public ActionResult AdminExtraForm(long id)
        {
            try
            {
                AdvertiseType parentType;
                AdvertiseStatus status;
                var director = advertiseService.GetAdminForm(id, DirectorType.Extra, out parentType, out status);
                var model = ExtraFormDTO.Generate(director, id);
                ViewBag.type = director.AdvertiseType;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AdminExtraForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public ActionResult AdminExtraForm(Advertise data, PoolInputDTO poolDTO, bool forceSave = false, int tab = 0)
        {
            try
            {
                Dictionary<string, string> errors;
                List<string> groupErrors = new List<string>();
                AdvertiseType parentType;
                AdvertiseStatus status;
                if (data.Pool == true)
                {
                    data.PoolFeatures = poolDTO.ConvertToEnum();
                }
                else
                {
                    data.PoolFeatures = PoolFeaturesEnum.None;
                }
                var director = advertiseService.SubmitAdminForm(data, out errors, out groupErrors, forceSave,
                    DirectorType.Extra, userAccessor.CurrentUser.Id, out parentType, out status);
                if (forceSave == false && groupErrors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    ViewBag.errors = groupErrors;
                    ViewBag.tab = tab;
                    return View(ExtraFormDTO.Generate(director, data.Id));
                }
                if (tab > 0)
                {
                    switch (tab)
                    {
                        case 1:
                            return RedirectToAction(nameof(AdminBasicForm), new
                            {
                                id = data.Id
                            });
                        case 2:
                            return RedirectToAction(nameof(AdminGeneralForm), new
                            {
                                id = data.Id
                            });
                        case 5:
                            return RedirectToAction(nameof(AdminStatusForm), new
                            {
                                id = data.Id
                            });
                        case 4:
                            if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                            {
                                return RedirectToAction(nameof(AdminComplexForm), new
                                {
                                    parentId = data.Id
                                });
                            }
                            else
                            {
                                return RedirectToAction(nameof(AdminHotelForm), new
                                {
                                    parentId = data.Id
                                });
                            }
                    }
                }
                if (director.Mode == AdvertiseMode.Parent)
                {
                    if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                    {
                        return RedirectToAction(nameof(AdminComplexForm), new
                        {
                            parentId = data.Id
                        });
                    }
                    else
                    {
                        return RedirectToAction(nameof(AdminHotelForm), new
                        {
                            parentId = data.Id
                        });
                    }
                }
                return RedirectToAction(nameof(AdminStatusForm), new
                {
                    id = data.Id
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdminExtraForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpGet]
        public ActionResult AdminComplexForm(long parentId, long id = 0)
        {
            try
            {
                var childs = advertiseService.GetAccChilds(parentId);
                if (id == 0)
                {
                    var ids = childs.Values.SelectMany(m => m.Keys.Select(s => s)).ToList();
                    if (ids.Count > 0)
                    {
                        id = ids.First();
                    }
                }
                ComplexUnitFormDTO model = null;
                AdvertiseStatus status = AdvertiseStatus.Unset;
                if (id > 0)
                {
                    AdvertiseType parentType;
                    var director = advertiseService.GetAdminForm(id, DirectorType.ComplexUnit, out parentType, out status);
                    model = ComplexUnitFormDTO.Generate(director, id, parentId, parentType);
                }
                else
                {
                    model = new ComplexUnitFormDTO(AdvertiseType.None);
                    model.ParentId = parentId;
                }
                ViewBag.childs = childs;
                ViewBag.parentId = parentId;
                ViewBag.status = status;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AdminGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public ActionResult AdminComplexForm(Advertise data, PoolInputDTO poolDTO, bool forceSave = false, int tab = 0)
        {
            try
            {
                if (data.PhotoID < 1)
                {
                    data.PhotoID = null;
                }
                Dictionary<string, string> errors;
                List<string> groupErrors;
                AdvertiseType parentType;
                AdvertiseStatus status;
                if (data.Pool == true)
                {
                    data.PoolFeatures = poolDTO.ConvertToEnum();
                }
                else
                {
                    data.PoolFeatures = PoolFeaturesEnum.None;
                }
                var childs = advertiseService.GetAccChilds((long)data.ParentId);
                var director = advertiseService.SubmitAdminForm(data, out errors, out groupErrors, forceSave,
                    DirectorType.ComplexUnit, userAccessor.CurrentUser.Id, out parentType, out status, webHostEnvironment.WebRootPath);
                if (forceSave == false && groupErrors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    ViewBag.errors = groupErrors;
                    ViewBag.childs = childs;
                    ViewBag.tab = tab;
                    ViewBag.status = status;
                    return View(ComplexUnitFormDTO.Generate(director, data.Id, (long)data.ParentId, AdvertiseType.Complex));
                }

                if (tab > 0)
                {
                    switch (tab)
                    {
                        case 1:
                            return RedirectToAction(nameof(AdminBasicForm), new
                            {
                                id = data.ParentId
                            });
                        case 2:
                            return RedirectToAction(nameof(AdminGeneralForm), new
                            {
                                id = data.ParentId
                            });
                        case 3:
                            return RedirectToAction(nameof(AdminExtraForm), new
                            {
                                id = data.ParentId
                            });
                        case 5:
                            return RedirectToAction(nameof(AdminStatusForm), new
                            {
                                id = data.ParentId
                            });
                        default:
                            return RedirectToAction(nameof(AdminComplexForm), new
                            {
                                id = tab,
                                parentId = data.ParentId
                            });
                    }
                }

                var ids = childs.Values.SelectMany(m => m.Keys.Select(s => s)).ToList();
                var nextIndex = ids.IndexOf(data.Id) + 1;

                if (nextIndex < ids.Count)
                {
                    return RedirectToAction(nameof(AdminComplexForm), new
                    {
                        id = ids[nextIndex],
                        parentId = data.ParentId
                    });

                }
                return RedirectToAction(nameof(AdminStatusForm), new
                {
                    id = data.ParentId
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdminGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpGet]
        public ActionResult AdminHotelForm(long parentId, long id = 0)
        {
            try
            {
                var childs = advertiseService.GetAccChilds(parentId);
                if (id == 0)
                {
                    var ids = childs.Values.SelectMany(m => m.Keys.Select(s => s)).ToList();
                    if (ids.Count > 0)
                    {
                        id = ids.First();
                    }
                }
                HotelUnitFormDTO model = null;
                AdvertiseStatus status = AdvertiseStatus.Unset;
                if (id > 0)
                {
                    AdvertiseType parentType;
                    var director = advertiseService.GetAdminForm(id, DirectorType.HotelUnit, out parentType, out status);
                    model = HotelUnitFormDTO.Generate(director, id, parentId);
                }
                else
                {
                    model = new HotelUnitFormDTO();
                    model.ParentId = parentId;
                }
                ViewBag.childs = childs;
                ViewBag.parentId = parentId;
                ViewBag.status = status;
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("AdminGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public ActionResult AdminHotelForm(Advertise data, bool forceSave = false, int tab = 0)
        {
            try
            {
                data.TypeID = AdvertiseType.Hotel;
                var childs = advertiseService.GetAccChilds((long)data.ParentId);
                Dictionary<string, string> errors;
                List<string> groupErrors;
                AdvertiseType parentType;
                AdvertiseStatus status;
                var director = advertiseService.SubmitAdminForm(data, out errors, out groupErrors, forceSave,
                    DirectorType.HotelUnit, userAccessor.CurrentUser.Id, out parentType, out status);
                if (forceSave == false && groupErrors.Any())
                {
                    ModelState.Clear();
                    foreach (var item in errors)
                    {
                        ModelState.AddModelError(item.Key, item.Value == null ? "" : item.Value);
                    }
                    ViewBag.errors = groupErrors;
                    ViewBag.childs = childs;
                    ViewBag.tab = tab;
                    ViewBag.status = status;
                    return View(HotelUnitFormDTO.Generate(director, data.Id, (long)data.ParentId));
                }

                if (tab > 0)
                {
                    switch (tab)
                    {
                        case 1:
                            return RedirectToAction(nameof(AdminBasicForm), new
                            {
                                id = data.ParentId
                            });
                        case 2:
                            return RedirectToAction(nameof(AdminGeneralForm), new
                            {
                                id = data.ParentId
                            });
                        case 3:
                            return RedirectToAction(nameof(AdminExtraForm), new
                            {
                                id = data.ParentId
                            });
                        case 5:
                            return RedirectToAction(nameof(AdminStatusForm), new
                            {
                                id = data.ParentId
                            });
                        default:
                            return RedirectToAction(nameof(AdminHotelForm), new
                            {
                                id = tab,
                                parentId = data.ParentId
                            });
                    }
                }

                var ids = childs.Values.SelectMany(m => m.Keys.Select(s => s)).ToList();
                var nextIndex = ids.IndexOf(data.Id) + 1;

                if (nextIndex < ids.Count)
                {
                    return RedirectToAction(nameof(AdminHotelForm), new
                    {
                        id = ids[nextIndex],
                        parentId = data.ParentId
                    });

                }
                return RedirectToAction(nameof(AdminStatusForm), new
                {
                    id = data.ParentId
                });
            }
            catch (Exception exc)
            {
                logger.Error("AdminGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpGet]
        public ActionResult AdminStatusForm(long id)
        {
            try
            {
                var acc = advertiseService.Find(id);
                ViewBag.type = acc.TypeID;
                ViewBag.mode = acc.Mode;
                ViewBag.errors = TempData["prevErrors"];
                return View(acc);
            }
            catch (Exception exc)
            {
                logger.Error("AdminGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Publish)]
        [HttpPost]
        public ActionResult AdminStatusForm(long id, bool status, List<NotVerifyReasonsEnum> notVerifyReasons)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (status)
                {
                    advertiseService.Publish(id, userAccessor.CurrentUser.Id, ActionSourceEnum.AdminPanel);
                }
                else
                {
                    advertiseService.NotVerify(id, userAccessor.CurrentUser.Id);
                    advertiseService.SetNotVerifyReasons(id, notVerifyReasons);
                }
                ViewBag.type = acc.TypeID;
                ViewBag.mode = acc.Mode;
                ViewBag.errors = TempData["prevErrors"];
                var urlReferrer = HttpContext.Session.GetObjectFromJson<string>("urlReferrer");
                if (string.IsNullOrEmpty(urlReferrer))
                    urlReferrer = "/advertise/index";
                return Redirect(urlReferrer);
            }
            catch (Exception exc)
            {
                logger.Error("AdminGeneralForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        #endregion

        #region Add/Edit Accomodation

        [Authorize]
        [HttpGet]
        public ActionResult AddOrEditAccomodation()
        {
            return RedirectPermanent("/accomodation/accbasicform");
        }

        [Authorize]
        [HttpGet]
        public ActionResult AccBasicForm(long id = -1)
        {
            try
            {
                bool isEdit;
                int level;
                var director = advertiseService.GetBasicForm(id, out isEdit, out level);
                var model = new BasicFormDTO();
                model = BasicFormDTO.Generate(director, id);
                if (director.Mode == AdvertiseMode.Parent && isEdit)
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
        public ActionResult AccBasicForm(Advertise data, bool isEdit = false, int tab = 0)
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
                        return RedirectToAction(nameof(AccGeneralForm), new
                        {
                            id = data.Id
                        });
                    case 3:
                        return RedirectToAction(nameof(AccExtraForm), new
                        {
                            id = data.Id
                        });
                    case 4:
                        if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(AccComplexForm), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(AccHotelForm), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                    default:
                        return RedirectToAction(nameof(AccGeneralForm), new
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
        public ActionResult AccGeneralForm(long id)
        {
            try
            {
                bool isEdit;
                int level;
                var director = advertiseService.GetGeneralForm(id, out isEdit, out level);
                var model = new GeneralFormDTO();
                model = GeneralFormDTO.Generate(director, id);
                if (director.Mode == AdvertiseMode.Parent && isEdit)
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
        public ActionResult AccGeneralForm(Advertise data, bool isEdit = false, int tab = 0)
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
                        return RedirectToAction(nameof(AccBasicForm), new
                        {
                            id = data.Id
                        });
                    case 3:
                        return RedirectToAction(nameof(AccExtraForm), new
                        {
                            id = data.Id
                        });
                    case 4:
                        if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(AccComplexForm), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(AccHotelForm), new
                            {
                                parentId = data.Id,
                                id = -1
                            });
                        }
                    default:
                        return RedirectToAction(nameof(AccExtraForm), new
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
        public ActionResult AccExtraForm(long id)
        {
            try
            {
                bool isEdit;
                int level;
                var director = advertiseService.GetExtraForm(id, out isEdit, out level);
                var model = new ExtraFormDTO();
                model = ExtraFormDTO.Generate(director, id);
                if (director.Mode == AdvertiseMode.Parent && isEdit)
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
        public ActionResult AccExtraForm(Advertise data, PoolInputDTO poolDTO, bool isEdit = false, int tab = 0)
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
                    data.PoolFeatures = PoolFeaturesEnum.None;
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
                var isAdd = data.Status == AdvertiseStatus.NotCompleted;
                if (tab > 0)
                {
                    switch (tab)
                    {
                        case 1:
                            return RedirectToAction(nameof(AccBasicForm), new
                            {
                                id = data.Id
                            });
                        case 2:
                            return RedirectToAction(nameof(AccGeneralForm), new
                            {
                                id = data.Id
                            });
                        case 4:
                            if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                            {
                                return RedirectToAction(nameof(AccComplexForm), new
                                {
                                    parentId = data.Id,
                                    id = -1
                                });
                            }
                            else
                            {
                                return RedirectToAction(nameof(AccHotelForm), new
                                {
                                    parentId = data.Id,
                                    id = -1
                                });
                            }
                    }
                }
                switch ((AdvertiseMode)director.Mode)
                {
                    case AdvertiseMode.Parent:
                        if (director.AdvertiseType == AdvertiseType.Complex || director.AdvertiseType == AdvertiseType.HotelApartment)
                        {
                            return RedirectToAction(nameof(AccComplexForm), new
                            {
                                parentId = data.Id
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(AccHotelForm), new
                            {
                                parentId = data.Id
                            });
                        }
                    default:
                        var user = userAccessor.CurrentUser;
                        if (string.IsNullOrEmpty(user.Mobile2) ||
                            string.IsNullOrEmpty(user.Tell) ||
                            string.IsNullOrEmpty(user.ThirdPersonTell))
                        {
                            var success_str = "آگهی شما با موفقیت " + (isAdd ? "ثبت" : "ویرایش") + " و پس از تایید کارشناس " + (isAdd ? "" : "دوباره ") + "نمایش داده میشود، لطفا اطلاعات مورد نیاز را تکمیل کنید";
                            TempData["alert_success"] = success_str;
                            return RedirectToAction("ProfileManager", "User", new { UserID = user.Id });
                        }
                        else
                        {
                            var success_str = "آگهی شما با موفقیت " + (isAdd ? "ثبت" : "ویرایش") + " و پس از تایید کارشناس " + (isAdd ? "" : "دوباره ") + "نمایش داده میشود . \n";
                            TempData["alert"] = success_str;
                            return Redirect("/dashboard");
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
        public ActionResult AccHotelForm(long parentId, long id = 0)
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
                if (director.AdvertiseType > AdvertiseType.None)
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
        public ActionResult AccHotelForm(Advertise data, bool isEdit = false, bool saveAndNewRoom = false, int tab = -1)
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
                            return RedirectToAction(nameof(AccBasicForm), new
                            {
                                id = data.ParentId
                            });
                        case 2:
                            return RedirectToAction(nameof(AccGeneralForm), new
                            {
                                id = data.ParentId
                            });
                        case 3:
                            return RedirectToAction(nameof(AccExtraForm), new
                            {
                                id = data.ParentId
                            });
                        default:
                            return RedirectToAction(nameof(AccHotelForm), new
                            {
                                id = tab,
                                parentId = data.ParentId
                            });
                    }
                }
                if (saveAndNewRoom)
                {
                    return RedirectToAction(nameof(AccHotelForm), new
                    {
                        parentId = data.ParentId
                    });
                }
                return Redirect("/dashboard");

            }
            catch (Exception exc)
            {
                logger.Error("AccHotelForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult AccComplexForm(long parentId, long id = 0)
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
                AdvertiseType parentType;
                var director = advertiseService.GetComplexForm(id, parentId, out parentType, out isEdit);
                var model = new ComplexUnitFormDTO(parentType);
                if (director.AdvertiseType > AdvertiseType.None)
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
        public ActionResult AccComplexForm(Advertise data, PoolInputDTO poolDTO, bool isEdit = false, bool saveAndNewRoom = false, int tab = -1)
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
                    data.PoolFeatures = PoolFeaturesEnum.None;
                }
                Dictionary<string, string> errors;
                List<string> groupErrors;
                AdvertiseType parentType;
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
                            return RedirectToAction(nameof(AccBasicForm), new
                            {
                                id = data.ParentId
                            });
                        case 2:
                            return RedirectToAction(nameof(AccGeneralForm), new
                            {
                                id = data.ParentId
                            });
                        case 3:
                            return RedirectToAction(nameof(AccExtraForm), new
                            {
                                id = data.ParentId
                            });
                        default:
                            return RedirectToAction(nameof(AccComplexForm), new
                            {
                                id = tab,
                                parentId = data.ParentId
                            });
                    }
                }
                if (saveAndNewRoom)
                {
                    return RedirectToAction(nameof(AccComplexForm), new
                    {
                        parentId = data.ParentId
                    });
                }
                return Redirect("/dashboard");

            }
            catch (Exception exc)
            {
                logger.Error("AccComplexForm", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult AccComplexTypeForm(long parentId, int childType)
        {
            var paradigmAdvertise = new Advertise()
            {
                TypeID = (AdvertiseType)childType,
                Floor = FloorItems.Unset
            };
            var childs = advertiseService.GetAccChilds(parentId);
            if (childs != null && childs.Count > 0 && childs.Keys.Contains((AdvertiseType)childType))
            {
                var paradigmId = childs[(AdvertiseType)childType].Keys.Last();
                paradigmAdvertise = advertiseService.Find(paradigmId);
            }
            var director = new AdvertiseDirector(paradigmAdvertise, DirectorType.ComplexUnit);
            var model = ComplexUnitFormDTO.Generate(director, 0, parentId, AdvertiseType.Complex);
            if (model.floor != null)
            {
                model.floor.Floor = FloorItems.Unset;
            }
            model.titleAndDesc = new Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs.TitleDescInputDTO(false);
            return PartialView("_AccComplexTypeForm", model);
        }
        #endregion

        [Authorize]
        public JsonResult SetPriceForDateRange(long advertise_id,
            string from_date, string to_date, int price)
        {
            try
            {
                var advertise = advertiseService.Find(advertise_id);
                if (advertise.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new { status = 0, msg = "شما مجوز این کار را ندارید" });
                }
                string msg;
                var done = priceTableService.SetAccommodationPriceInDate(
                    advertise_id, from_date, to_date, price, out msg);
                var priceDict = done ?
                    advertiseService.GetAccPriceDatesInfo(advertise_id) : null;
                return GenerateJsonResult(new { status = done ? 1 : 0, msg = msg, priceDict = priceDict });
            }
            catch (Exception exc)
            {
                logger.Error("SetPriceForDateRange", exc);
                return GenerateJsonResult(new { status = 0, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }

        public JsonResult GetPriceDict(long id)
        {
            try
            {
                var priceDict = advertiseService.GetAccPriceDatesInfo(id);
                return GenerateJsonResult(new { status = 1, priceDict = priceDict });
            }
            catch (Exception exc)
            {
                logger.Error("GetPriceDict", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize]
        public JsonResult CheckSetAsOccupiedForDateRange(long advertise_id,
            string from_date, string to_date, bool forRemove = false)
        {
            try
            {
                var acc = advertiseService.Find(advertise_id);
                if (acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new { status = 0, msg = "شما مجوز این کار را ندارید" });
                }
                if (forRemove)
                {
                    var result = advertiseService.CheckUnsetOccupiedDateRange(
                        advertise_id, from_date, to_date);
                    return GenerateJsonResult(new
                    {
                        status = result.Result == CheckUnsetOccupiedResult.OK ? 1 : 0,
                        msg = result.ToString()
                    });
                }
                else
                {
                    var result = advertiseService.CheckSetAsOccupiedDateRange(
                        advertise_id, from_date, to_date);
                    return GenerateJsonResult(new
                    {
                        status = result.Result == CheckSetOccupiedResult.OK ||
                        result.Result == CheckSetOccupiedResult.ContainsReserveRequest ? 1 : 0,
                        msg = result.ToString()
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("CheckSetAsOccupiedForDateRange", exc);
                return GenerateJsonResult(new { status = 0, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }

        [Authorize]
        public JsonResult GetOccupiedDates(long id)
        {
            try
            {
                var acc = advertiseService.Find(id);
                var occupiedList = acc.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s));
                var extrinsicList = acc.ExtrinsicReserves.Select(s => DateTimeUtility.DateValueOfJS(s.StartDate)).ToList();
                return GenerateJsonResult(new 
                { 
                    status = 1, 
                    occupiedList = occupiedList,
                    extrinsicList = extrinsicList
                });
            }
            catch (Exception exc)
            {
                logger.Error("GetOccupiedDates", exc);
                return GenerateJsonResult(new { status = 0, msg = "متاسفانه عملیات با خطا مواجه شد" });
            }
        }

        [Authorize]
        public JsonResult SetAsOccupiedForDateRange(long advertise_id,
            string from_date, string to_date)
        {
            try
            {
                var acc = advertiseService.Find(advertise_id);
                if (acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new { status = 0, msg = "شما مجوز این کار را ندارید" });
                }
                var checkResult = advertiseService.CheckSetAsOccupiedDateRange(advertise_id,
                    from_date, to_date);
                if (checkResult.Result == CheckSetOccupiedResult.OK ||
                    checkResult.Result == CheckSetOccupiedResult.ContainsReserveRequest)
                {
                    extrinsicReserveService.Insert(advertise_id, from_date, to_date, ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id, acc.Count);
                    var todayPersian = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date);
                    bool changeToday = false;
                    if (todayPersian == from_date)
                    {
                        advertiseService.UnsetTodayEmpty(advertise_id);
                        changeToday = true;
                    }
                    acc = advertiseService.Find(acc.Id, true);
                    var occupiedList = acc.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s)).ToList();
                    var extrinsicList = acc.ExtrinsicReserves.Select(s => DateTimeUtility.DateValueOfJS(s.StartDate)).ToList();
                    return GenerateJsonResult(new { 
                        status = 1, 
                        msg = "محدوده انتخاب شده به عنوان روز های پر ثبت شد",
                        occupiedList = occupiedList,
                        extrinsicList = extrinsicList,
                        changeToday = changeToday
                    });
                }
                else
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = checkResult.ToString(),
                        occupiedList = new List<long>()
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("SetAsOccupiedForDateRange", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public JsonResult RemoveFromOccupiedForDateRange(long advertise_id,
            string from_date, string to_date)
        {
            try
            {
                var advertise = advertiseService.Find(advertise_id);
                if (advertise.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                var checkResult = advertiseService.CheckUnsetOccupiedDateRange(
                    advertise_id, from_date, to_date);
                if (checkResult.Result == CheckUnsetOccupiedResult.OK)
                {
                    advertiseService.DeleteExtrinsicReserves(advertise_id, from_date, to_date);
                    advertise = advertiseService.Find(advertise_id, true);
                    var todayPersian = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date);
                    bool changeToday = false;
                    if (todayPersian == from_date)
                    {
                        advertiseService.SetAsTodayEmpty(advertise_id);
                        changeToday = true;
                    }
                    var occupiedList = advertise.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s)).ToList();
                    var extrinsicList = advertise.ExtrinsicReserves.Select(s => DateTimeUtility.DateValueOfJS(s.StartDate)).ToList();
                    return GenerateJsonResult(new
                    {
                        status = 1,
                        msg = checkResult.ToString(),
                        occupiedList = occupiedList,
                        extrinsicList = extrinsicList,
                        changeToday = changeToday
                    });
                }
                else
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = checkResult.ToString(),
                        occupiedList = new List<long>()
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("RemoveFromOccupiedForDateRange", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public JsonResult SetNorouzMinReserveDate(long id,
            long dateUnix)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                advertiseService.SetNorouzMinReserveDate(id, dateUnix);
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("SetNorouzMinReserveDate", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult Available(long id, bool isAvailable)
        {
            try
            {
                advertiseService.SetAvailable(id, isAvailable);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Available", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = ""
                });
            }
        }

        [Authorize(Policy = Policies.Advertise_Publish)]
        public JsonResult Publish(long id)
        {
            try
            {
                advertiseService.Publish(id, userAccessor.CurrentUser.Id, ActionSourceEnum.AdminPanel);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Publish", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = ""
                });
            }
        }

        [Authorize(Policy = Policies.Advertise_Publish)]
        public JsonResult Suspend(long id)
        {
            try
            {
                advertiseService.Suspend(id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Suspend", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = ""
                });
            }
        }

        [Authorize]
        public JsonResult ToggleActive(long id, bool? active)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز انجام این کار را ندارید"
                    });
                }
                var allowedStates = new List<int>() {
                    (int)AdvertiseStatus.Published,
                    (int)AdvertiseStatus.Archived,
                };
                if (!allowedStates.Contains((int)acc.Status))
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "آگهی شما هنوز منتشر نشده است"
                    });
                }
                var newStatus = 0;
                if (active != null)
                {
                    if ((bool)active && acc.Status != AdvertiseStatus.Archived)
                    {
                        return GenerateJsonResult(new
                        {
                            status = -1
                        });
                    }
                    if ((bool)!active && acc.Status != AdvertiseStatus.Published)
                    {
                        return GenerateJsonResult(new
                        {
                            status = -1
                        });
                    }
                    if ((bool)active)
                    {
                        advertiseService.Publish(id, userAccessor.CurrentUser.Id, ActionSourceEnum.WebsiteDashboard);
                        newStatus = (int)AdvertiseStatus.Published;
                    }
                    else
                    {
                        advertiseService.Suspend(id);
                        newStatus = (int)AdvertiseStatus.Archived;
                    }
                }
                else
                {
                    newStatus = (int)advertiseService.ToggleSuspension(id);
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    newValue = newStatus == (int)AdvertiseStatus.Published ? 1 : 0,
                    statusString = AdvertiseMainLocalization.GetAdvertiseStatusString((int)newStatus),
                    statusColor = AdvertiseStyleHelper.GetAdvertiseStatusColor((int)newStatus)
                });
            }
            catch (Exception exc)
            {
                logger.Error("ToggleActive", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [Authorize]
        public JsonResult AddDiscount(long id, string from, string to, int discount)
        {
            try
            {
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                from = StringUtility.PersianNumberToEnglish(from).Replace("/", ",");
                to = StringUtility.PersianNumberToEnglish(to).Replace("/", ",");
                var from_gregorian = DateTimeUtility.PersianDateToGregorian(from);
                var to_gregorian = DateTimeUtility.PersianDateToGregorian(to);
                List<string> errorList;
                var done = discountTableService.Insert(id, from_gregorian, to_gregorian, discount, out errorList);
                var priceDict = advertiseService.GetAccPriceDatesInfo(id);
                return GenerateJsonResult(new
                {
                    status = done ? 1 : 0,
                    msg = done ? "تخفیف مورد نظر با موفقیت اعمال شد" : string.Join("\n", errorList),
                    priceDict = priceDict
                });
            }
            catch (Exception exc)
            {
                logger.Error("AddDiscount", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [ResponseCache(Duration = 60 * 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult Item(string slug, string capacity = null,
            string empty_range_from = null, string empty_range_to = null)
        {
            try
            {
                if (HttpContext.Request.Path.Value.Last() == '/')
                {
                    return RedirectPermanent(HtmlUtility.EncodeUrlForRedirect(HttpContext.Request.Path.Value.Remove(HttpContext.Request.Path.Value.Length - 1)));
                }
                var id = long.Parse(slug.Split('-')[0]);
                var model = advertiseService.FindIncludingDeleted(id);
                if (model.Slug.ToLower() != slug.ToLower())
                {
                    return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                }
                advertiseService.UpdateAccView(id);

                #region Initialize DTO
                var advertiseIds = advertiseService.GetAdvertiseIdsByUserId(model.UserID);
                var allUserReportItems = reportItemService.GetByAccList(advertiseIds);
                Dictionary<AdvertiseType, IList<AdvertiseDirector>> childDirectors;
                var director = advertiseService.GetAdvertisePageData(id, out childDirectors);
                var accDTO = AccommodationItemDTO.Generate(userAccessor.CurrentUser, model,
                    director, childDirectors, allUserReportItems);
                accDTO.RawUrl = HttpContext.Request.Path.Value.Split('?')[0];
                accDTO.EmptyRangeFrom = empty_range_from;
                accDTO.EmptyRangeTo = empty_range_to;
                accDTO.RelatedLinkCapacity = capacity;
                accDTO.IsPreview = false;
                #endregion

                if (accDTO.CanPublish == false)
                {
                    var regionIds = regionService.GetParentIdsByCityId(model.City == null ? 0 : (int)model.City);
                    accDTO.RelatedCategories = new List<DynamicCategory>();
                    accDTO.RelatedCategories.Add(categoryService.GetAccItemLinks(model.Province, model.City, model.Area, model.TypeID).Last());
                }
                ViewBag.amp_version = false;
                return View(accDTO);
            }
            catch (Exception exc)
            {
                logger.Error("Accommodation.Item", exc);
                return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
            }
        }

        [Authorize]
        public ActionResult Preview(long id, string capacity = null,
            string empty_range_from = null, string empty_range_to = null)
        {
            try
            {
                var acc = advertiseService.FindIncludingDeleted(id);
                if (userAccessor.CurrentUser.Id != acc.UserID)
                {
                    return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                }
                advertiseService.UpdateAccView(id);
                ViewBag.amp_version = false;

                #region Initial DTO
                var advertiseIds = advertiseService.GetAdvertiseIdsByUserId(acc.UserID);
                var allUserReportItems = reportItemService.GetByAccList(advertiseIds);
                Dictionary<AdvertiseType, IList<AdvertiseDirector>> childDirectors;
                var director = advertiseService.GetAdvertisePageData(id, out childDirectors);
                var accDTO = AccommodationItemDTO.Generate(userAccessor.CurrentUser, acc,
                    director, childDirectors, allUserReportItems);
                accDTO.RawUrl = HttpContext.Request.Path.Value.Split('?')[0];
                accDTO.EmptyRangeFrom = empty_range_from;
                accDTO.EmptyRangeTo = empty_range_to;
                accDTO.RelatedLinkCapacity = capacity;
                accDTO.IsPreview = false;
                accDTO.CanPublish = true;
                #endregion

                return View("Item", accDTO);
            }
            catch (Exception exc)
            {
                logger.Error("Accommodation.Preview", exc);
                return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
            }
        }

        [Authorize(Policy = Policies.Advertise_View)]
        public ActionResult AdminPreview(long id, string capacity = null,
            string empty_range_from = null, string empty_range_to = null)
        {
            try
            {
                var acc = advertiseService.FindIncludingDeleted(id);
                advertiseService.UpdateAccView(id);
                ViewBag.amp_version = false;

                #region Initial DTO
                var advertiseIds = advertiseService.GetAdvertiseIdsByUserId(acc.UserID);
                var allUserReportItems = reportItemService.GetByAccList(advertiseIds);
                Dictionary<AdvertiseType, IList<AdvertiseDirector>> childDirectors;
                var director = advertiseService.GetAdvertisePageData(id, out childDirectors);
                var accDTO = AccommodationItemDTO.Generate(userAccessor.CurrentUser,
                    acc, director, childDirectors, allUserReportItems);
                accDTO.RawUrl = HttpContext.Request.Path.Value.Split('?')[0];
                accDTO.EmptyRangeFrom = empty_range_from;
                accDTO.EmptyRangeTo = empty_range_to;
                accDTO.RelatedLinkCapacity = capacity;
                accDTO.IsPreview = false;
                accDTO.CanPublish = true;
                #endregion

                return View("Item", accDTO);
            }
            catch (Exception exc)
            {
                logger.Error("Accomodation.AdminPreview", exc);
                return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
            }
        }

        [Authorize]
        public JsonResult GetDiscounts(long id)
        {
            try
            {
                var advertise = advertiseService.Find(id);
                if (advertise.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                var model = discountTableService.GetDiscountsOfAccommodation(id);
                return GenerateJsonResult(new
                {
                    status = model.Any() ? 1 : 2,
                    discounts = model.Select(s => (AccDashboardDTOs.DiscountDTO)s)
                });
            }
            catch (Exception exc)
            {
                logger.Error("GetDiscounts", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public JsonResult RemoveDiscount(int discount_id)
        {
            try
            {
                var discount = discountTableService.Find(discount_id);
                var acc = advertiseService.Find(discount.AdvertiseID);
                if (acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز این کار را ندارید"
                    });
                }
                discountTableService.Delete(discount_id);
                var priceDict = advertiseService.GetAccPriceDatesInfo(acc.Id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    priceDict = priceDict
                });
            }
            catch (Exception exc)
            {
                logger.Error("RemoveDiscount", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public JsonResult SetAsTodayEmpty(long id)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز این کار را ندارید"
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
                            status = 0,
                            msg = "واحد شما برای امروز رزرو شده است"
                        });
                    }
                }
                advertiseService.SetAsTodayEmpty(id);
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("SetAsTodayEmpty", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public JsonResult UnsetTodayEmpty(long id)
        {
            var acc = advertiseService.Find(id);
            if (acc.UserID != userAccessor.CurrentUser.Id)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "شما مجوز این کار را ندارید"
                });
            }
            var today = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date);
            var tomorrow = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date.AddDays(1).Date);
            var checkResult = advertiseService.CheckSetAsOccupiedDateRange(
                id, today, tomorrow);
            if (checkResult.Result == CheckSetOccupiedResult.OK ||
                checkResult.Result == CheckSetOccupiedResult.ContainsReserveRequest)
            {
                extrinsicReserveService.Insert(id, DateTime.Now.Date, ActionSourceEnum.WebsiteDashboard, userAccessor.DoerUser.Id, acc.Count);
            }
            else
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = checkResult.ToString()
                });
            }
            advertiseService.UnsetTodayEmpty(id);
            return GenerateJsonResult(new
            {
                status = 1
            });
        }

        public JsonResult GetAccomodationUrl(long id)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (acc == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = -1
                    });
                }
                var url = AdvertiseUrlLocalization.SlugToAdvertiseUrl(acc.Slug);
                url = GeneralData.WebsiteUrl + url;
                return GenerateJsonResult(new
                {
                    status = 1,
                    url
                });
            }
            catch (Exception exc)
            {
                logger.Error("GetAccomodationUrl", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }

        public PartialViewResult GetReservePopup(long accomodationId)
        {
            var model = advertiseService.Find(accomodationId);
            var dto = new ReservePopupDTO(model.Capacity,
                model.MoreThanCapacity,
                model.GetFirstDiscountData(false, true));
            ViewBag.forceHeaderShown = true;
            return PartialView("_Reserve", dto);
        }

        [ResponseCache(Duration = 60 * 15, VaryByQueryKeys = new string[] { "*" })]
        public JsonResult GetRegionChildrenItems(int region_type, int? parent_id, int status = -1)
        {
            try
            {
                var region_string = RegionLocalization.GetAdvertiseRegionString(region_type);
                var Items = regionService.Filter(
                    (AdvertiseRegion)region_type, parent_id == null ? 0 : (int)parent_id,
                    (RegionStatus)status);
                if (Items.Count() > 1)
                {
                    string ret = "<option value='-1' style='color:#ccc;'>" + region_string + " را انتخاب کنید</option>";
                    foreach (var item in Items)
                    {
                        ret += "<option value='" + item.Id + "'>" + item.PersianName + "</option>";
                    }
                    return GenerateJsonResult(new
                    {
                        status = 1,
                        val = ret
                    });
                }
                else if (Items.Count() == 1)
                {
                    var item = Items[0];
                    var ret = "<option value='" + item.Id + "'>" + item.PersianName + "</option>";
                    return GenerateJsonResult(new
                    {
                        status = 1,
                        val = ret
                    });
                }
                else
                {
                    string ret = "";
                    switch ((AdvertiseRegion)region_type)
                    {
                        case AdvertiseRegion.City:
                            ret = "<option value='-1' style='color:#ccc;'>این استان شهر ندارد</option>";
                            break;
                        case AdvertiseRegion.Area:
                            ret = "<option value='-1' style='color:#ccc;'>این شهر منطقه ندارد</option>";
                            break;
                        default:
                            ret = "<option value='-1' style='color:#ccc;'>گزینه ای وجود ندارد.</option>";
                            break;
                    }
                    return GenerateJsonResult(new
                    {
                        status = 1,
                        val = ret
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error("GetRegionChildrenItems", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [ResponseCache(Duration = 60 * 15, VaryByQueryKeys = new string[] { "*" })]
        public JsonResult GetProvinces(int status = -1)
        {
            var provinces = regionService.Filter(AdvertiseRegion.Province, 0, (RegionStatus)status);
            return GenerateJsonResult(new
            {
                provinces = provinces
            });
        }

        [ResponseCache(Duration = 60 * 15, VaryByQueryKeys = new string[] { "*" })]
        public JsonResult GetCities(int province_id, int status = -1)
        {
            if (province_id > 0)
            {
                var cities = regionService.Filter(AdvertiseRegion.City,
                    province_id, (RegionStatus)status);
                return GenerateJsonResult(new
                {
                    cities = cities
                });
            }
            else
            {
                return GenerateJsonResult(new
                {
                    cities = new List<Region>()
                });
            }
        }

        [ResponseCache(Duration = 60 * 15, VaryByQueryKeys = new string[] { "*" })]
        public JsonResult GetAreas(int city_id, int status = -1)
        {
            if (city_id > 0)
            {
                var areas = regionService.Filter(
                    AdvertiseRegion.Area, city_id, (RegionStatus)status);
                return GenerateJsonResult(new
                {
                    areas = areas
                });
            }
            else
            {
                return GenerateJsonResult(new
                {
                    areas = new List<Region>()
                });
            }
        }

        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetRelatedAccommodations(long id, string capacity = null,
            string empty_range_from = null, string empty_range_to = null)
        {
            try
            {
                var model = advertiseService.GetAdvertiseRelatedItems(id);
                ViewBag.capacity = capacity;
                ViewBag.empty_range_from = empty_range_from;
                ViewBag.empty_range_to = empty_range_to;
                var user = userAccessor.CurrentUser;
                List<AccommodationCardDTO> advertiseItemDTOs = new List<AccommodationCardDTO>();
                foreach (var item in model)
                {
                    var dto = (AccommodationCardDTO)item;
                    dto.Favourited = user.Id > 0 && user.Favorite.Any(x => x.AdvertiseID == item.Id);
                    advertiseItemDTOs.Add(dto);
                }
                return PartialView("_AdvertiseList", advertiseItemDTOs);
            }
            catch (Exception exc)
            {
                logger.Error("Accomodation.GetRelatedAccommodations", exc);
                return PartialView("_AdvertiseList", null);
            }
        }

        public JsonResult GetAccommodationDynamicViewBag(long id)
        {
            var advertise = advertiseService.FindIncludingDeleted(id);
            var price_property_dict = new Dictionary<string, string>();
            object price_temp;
            string price_str;
            var priceProperties = new Property[] {
                Property.DailyPrice,
                Property.HolidayPrice,
                Property.HolidayPikePrice,
                Property.MoreThanCapacityPrice,
                Property.RentPrice,
                Property.NorouzPrice
            };
            foreach (var pr in priceProperties)
            {
                if (advertise.Childs == null || advertise.Childs.Count == 0)
                {
                    price_temp = advertise.GetPropertyValue(pr);
                }
                else
                {
                    price_temp = advertise.Childs.ElementAt(0).GetPropertyValue(pr);
                }
                price_str = string.Format("{0:n0}", price_temp) + " تومان";
                price_property_dict.Add(pr.ToString(), price_str);
            }
            if (advertise.NorouzOverCapacityPrice > 0)
            {
                price_property_dict.Add("NorouzOverCapacityPrice",
                    string.Format("{0:n0}", advertise.NorouzOverCapacityPrice) + " تومان");
            }

            var advertise_rules = advertiseService.GetRulesDictionary(advertise.Id);
            var rules_string = "";
            var short_rules_string = "";
            var ii = 0;
            foreach (var item in advertise_rules)
            {
                if (ii < 2)
                {
                    short_rules_string += "<br/>" + item.Key + ": " + item.Value;
                }
                rules_string += "<br/>" + item.Key + ": " + item.Value;
                ii++;
            }

            var currentUser = userAccessor.CurrentUser;
            bool verifyEmail = false;
            bool isNumberForIran = false;
            string userEmailAddress = "";
            if (currentUser != null && string.IsNullOrEmpty(currentUser.MainMobile) == false)
            {
                var identityUser = userService.GetIdentityUser(currentUser.MainMobile);
                verifyEmail = identityUser.EmailConfirmed;
                isNumberForIran = PhoneUtility.IsNumberForIran(currentUser.MainMobile);
                userEmailAddress = identityUser.Email;
            }
            var is_favourited = currentUser.Id > 0 &&
                currentUser.Favorite != null &&
                currentUser.Favorite.Any(f => f.AdvertiseID == id);
            var user_is_autenticated = User.Identity.IsAuthenticated;
            //List<long> occupiedList;
            //Dictionary<string, DatePriceDTO> priceDict;
            var occupiedList = advertise.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s));
            var priceDict = advertiseService.GetAccPriceDatesInfo(id);
            var maxInstantReserveDate = DateTime.Now.Date.AddDays(advertise.MaxInstantReserveStart);
            var data = new
            {
                is_favourited = is_favourited,
                user_is_autenticated = user_is_autenticated,
                rules_string = rules_string,
                short_rules_string = short_rules_string,
                price_property_dict = price_property_dict,
                instantReserveAvailable = advertise.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed,
                instantReserveMaxStart = advertise.MaxInstantReserveStart,
                maxReserveStartDate = DateTimeUtility.GregorianToPersianDate(maxInstantReserveDate).Replace(",", "/").Substring(2),
                maxInstantReserveStartUnix = DateTimeUtility.DateValueOfJS(maxInstantReserveDate),
                occupiedList = occupiedList,
                priceDict = priceDict,
                verifyEmail = verifyEmail,
                isNumberForIran = isNumberForIran,
                userEmailAddress = userEmailAddress
            };
            advertiseService.AddToAdvertiseVisit(id);
            //return GenerateJsonResult(new
            //{
            //    Data = data
            //});
            return GenerateJsonResult(data);
        }

        public JsonResult GetAccListDynamicViewBag(string ids)
        {
            var idList = string.IsNullOrEmpty(ids) || ids == "," ? new List<long>() :
                Array.ConvertAll(ids.Split(','), x => long.Parse(x)).ToList();
            var price_dict = advertiseService.GetAdvertiseListPrices(idList);
            return GenerateJsonResult(new
            {
                price_dict = price_dict
            });
        }

        [Authorize]
        public JsonResult InstantReserveRequest(long id,
            bool ignoreMsg, int userId)
        {
            var acc = advertiseService.Find(id);
            if (acc.UserID != userId)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "شما مجوز این کار را ندارید"
                });
            }
            if (userAccessor.CurrentUser.InstantReserveAccess == InstantReserveAccessEnum.Banned)
            {
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "این امکان برای شما غیر فعال شده است"
                });
            }
            bool needMsg;
            advertiseService.RequestInstantReserve(id, ignoreMsg, userId,
                userAccessor.DoerUser.Id, ActionLog.ActionSourceEnum.WebsiteDashboard,
                userAccessor.CurrentUser.InstantReserveAccess, out needMsg);
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
                        banned = userAccessor.CurrentUser.InstantReserveAccess == InstantReserveAccessEnum.Banned,
                        buttonTitle = AdvertiseMainLocalization.GetInstantReserveButtonTitle(acc.InstantReserveStatus, userAccessor.CurrentUser.InstantReserveAccess == InstantReserveAccessEnum.Banned)
                    }
                };
            }
            return GenerateJsonResult(result);
        }

        [Authorize]
        public JsonResult InstantReserveCancel(long id, int userId)
        {
            var acc = advertiseService.Find(id);
            if (acc.UserID != userId)
            {
                return GenerateJsonResult(new InstantReserveRequestResultDTO()
                {
                    status = 0,
                    msg = "شما مجوز این کار را ندارید"
                } as dynamic);
            }
            if (userAccessor.CurrentUser.InstantReserveAccess == InstantReserveAccessEnum.Banned)
            {
                return GenerateJsonResult(new InstantReserveRequestResultDTO()
                {
                    status = 0,
                    msg = "این امکان برای شما غیر فعال شده است"
                });
            }
            advertiseService.CancelInstantReserve(id, userId, userAccessor.DoerUser.Id,
                ActionLog.ActionSourceEnum.WebsiteDashboard);
            var result = new InstantReserveRequestResultDTO()
            {
                status = 1,
                newData = new InstantReserveDetailDTO()
                {
                    status = acc.InstantReserveStatus,
                    statusString = AdvertiseMainLocalization.GetInstantReserveStatusString(acc.InstantReserveStatus),
                    statusColor = AdvertiseStyleHelper.GetInstantReserveStatusColor(acc.InstantReserveStatus),
                    banned = userAccessor.CurrentUser.InstantReserveAccess == InstantReserveAccessEnum.Banned,
                    buttonTitle = AdvertiseMainLocalization.GetInstantReserveButtonTitle(acc.InstantReserveStatus, userAccessor.CurrentUser.InstantReserveAccess == InstantReserveAccessEnum.Banned)
                }
            };
            return GenerateJsonResult(result);
        }

        public string GetInstnatReserveBanReason(long id)
        {
            return advertiseService.GetInstantReserveBanReason(id);
        }

        public ActionResult UserRatingDetailPopup(long id, int userid)
        {
            return PartialView("_UserRatingDetail", reportItemService.GetAccUserRatings(id, userid));
        }

        [Authorize]
        public JsonResult GetStayDuration(long id)
        {
            try
            {
                var acc = advertiseService.Find(id);
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
                logger.Error("GetStayDuration", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }

        [Authorize]
        public JsonResult SetStayDuration(long id,
            string minStr = "0", string maxStr = "0")
        {
            try
            {
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
                    msg = "حداقل اقامت " +
                        (min == 0 ? "بدون محدودیت" : min + " شب") +
                        " و حداکثر اقامت " +
                        (max == 0 ? "بدون محدودیت" : max + " شب") +
                        " تعیین شد.",
                    status = 1,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("SetStayDuration", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public JsonResult SetNorouzPrice(long id,
            int price, int overCapacityPrice = 0)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (userAccessor.CurrentUser.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز انجام این کار را ندارید"
                    });
                }
                advertiseService.SetNorouzPrice(id, price, overCapacityPrice);
                return GenerateJsonResult(new
                {
                    status = 1,
                    norouzPrice = price,
                    overCapacityPrice = overCapacityPrice
                });
            }
            catch (Exception exc)
            {
                logger.Error("SetNorouzPrice", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public JsonResult GetInstantReserveStart(long id)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (userAccessor.CurrentUser.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز انجام این کار را ندارید"
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
                logger.Error("GetInstantReserveStart", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }

        [Authorize]
        public JsonResult SetInstantReserveStart(long id, int maxStart)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (userAccessor.CurrentUser.Id != acc.UserID)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز انجام این کار را ندارید"
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
                    msg = "حداکثر شروع سفر تا " +
                        maxStart + " روز" +
                        " تعیین شد.",
                    status = 1,
                    data = data
                });
            }
            catch (Exception exc)
            {
                logger.Error("SetInstantReserveStart", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [Authorize]
        public ActionResult GetSetPricePopup(long id)
        {
            var user = userAccessor.CurrentUser;
            var acc = advertiseService.Find(id);
            if (acc.UserID != user.Id)
            {
                return PartialView("_AccSetPrice");
            }
            var priceDict = advertiseService.GetAccPriceDatesInfo(id);
            ViewBag.priceDict = SerializeUtility.SerializeToJS(priceDict);
            return PartialView("_AccSetPrice", acc);
        }

        [Authorize]
        public ActionResult GetSetMinNorouzReservePopup(long id)
        {
            var user = userAccessor.CurrentUser;
            var acc = advertiseService.Find(id);
            if (acc.UserID != user.Id)
            {
                return PartialView("_AccSetMinNorouzReserve");
            }
            return PartialView("_AccSetMinNorouzReserve", acc);
        }

        [Authorize]
        public IActionResult GetSetOccupiedPopup(long id)
        {
            var user = userAccessor.CurrentUser;
            var acc = advertiseService.Find(id);
            if (acc == null || acc.UserID != user.Id)
            {
                return PartialView("_AccSetOccupied");
            }
            var occupiedList = acc.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s)).ToList();
            var extrinsicList = acc.ExtrinsicReserves.Select(s => DateTimeUtility.DateValueOfJS(s.StartDate)).ToList();
            ViewBag.occupiedList = SerializeUtility.SerializeToJS(occupiedList);
            ViewBag.extrinsicList = SerializeUtility.SerializeToJS(extrinsicList);
            return PartialView("_AccSetOccupied", acc);
        }

        public IActionResult GetAccUrlById(string id)
        {
            try
            {
                id = StringUtility.PersianNumberToEnglish(id);
                var idLong = long.Parse(id);
                string slug = "";
                var acc = advertiseService.Find(idLong);
                if (acc == null || acc.Status != AdvertiseStatus.Published || !acc.Available)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0
                    });
                }
                if (acc.Mode == AdvertiseMode.Child && acc.Count > 0)
                {
                    slug = acc.Parent.Slug;
                }
                else
                {
                    slug = acc.Slug;
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    url = AdvertiseUrlLocalization.SlugToAdvertiseUrl(slug)
                });
            }
            catch (Exception exc)
            {
                logger.Error("Accomodation.GetAccUrlById", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }

        [Authorize]
        public JsonResult Delete(long id)
        {
            try
            {
                Advertise acc = advertiseService.Find(id);
                if (acc == null || acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شما مجوز حذف این آگهی را ندارید"
                    });
                }
                var status = advertiseService.Delete(id);
                return GenerateJsonResult(new
                {
                    status = status,
                    msg = status ? "" : "این آگهی دارای درخواست رزرو فعال است"
                });
            }
            catch (Exception exc)
            {
                logger.Error("Accomodation.Delete", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }

        public ActionResult AccBlogNews(long id)
        {
            var acc = advertiseService.FindIncludingDeleted(id);
            var model = blogpostService.GetAccommodationNewItems(
                acc.City == null ? 0 : (int)acc.City, acc.Area == null ? 0 : (int)acc.Area, (int)acc.TypeID,
                (int)acc.Position, acc.Pool == null ? false : (bool)acc.Pool, 2);
            return PartialView("_AccBlogNews", model);
        }

        [Authorize]
        public JsonResult SetHygieneProtocol(long id, HygieneProtocolStatus value)
        {
            try
            {
                var acc = advertiseService.Find(id);
                if (acc.UserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new { status = 0 });
                }
                advertiseService.SetHygieneProtocol(id, value);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Accomodation.SetHygieneProtocol", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        public JsonResult SetHygieneProtocolAdmin(long id, HygieneProtocolStatus value)
        {
            try
            {
                advertiseService.SetHygieneProtocol(id, value);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("Accomodation.SetHygieneProtocolAdmin", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        public JsonResult MustShowHygieneProtocolPopup()
        {
            bool result = false;
            try
            {
                var user = userAccessor.CurrentUser;
                if (user.Id > 0 && user.UserGeneralType > 0)
                {
                    var userAccs = user.Advertises;
                    if (userAccs != null && userAccs.Count == 1 && userAccs.FirstOrDefault().HygieneProtocol == null)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Accomodation.MustShowHygieneProtocolPopup", exc);
                result = false;
            }
            return GenerateJsonResult(new { result = result });
        }
    }
}