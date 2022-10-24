using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.AdvertiseDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;
using Entities = Amlakbashi.Core.Entities;

namespace Amlakbashi.Host.Controllers
{
    public class AdvertiseController : BaseController
    {
        private readonly IAdvertiseAppService advertiseService;
        private readonly IReportItemAppService reportItemService;
        private readonly IUserAppService userService;
        private readonly IRegionAppService regionService;
        private readonly ICategoryAppService categoryService;
        private readonly IDiscountTableAppService discountTableService;
        private readonly IReserveAppService reserveService;
        private readonly IUserContactFacade userContact;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        public AdvertiseController(ILog logger,
            IAdvertiseAppService advertiseService,
            IReportItemAppService reportItemService,
            IUserAppService userService,
            IRegionAppService regionService,
            ICategoryAppService dynamicCategoryService,
            IDiscountTableAppService discountTableService,
            IReserveAppService reserveService,
            IUserContactFacade userContact,
            IUserAccessor userAccessor
            )
        {
            this.advertiseService = advertiseService;
            this.reportItemService = reportItemService;
            this.regionService = regionService;
            this.userService = userService;
            this.categoryService = dynamicCategoryService;
            this.discountTableService = discountTableService;
            this.reserveService = reserveService;
            this.userContact = userContact;
            this.logger = logger;
            this.userAccessor = userAccessor;
        }

        [Authorize(Policy = Policies.Advertise_View)]
        public ActionResult Admin()
        {
            return View();
        }

        [Authorize(Policy = Policies.Advertise_View)]
        public IActionResult NewIndex(AdvertiseIndexDTO dto)
        {
            try
            {
                advertiseService.FilterNew(dto);
                return View(dto);
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.NewIndex", exc);
                return View();
            }
        }

        [Authorize(Policy = Policies.Advertise_View)]
        public IActionResult GetAdvertiseIndexDetails(long advertiseId)
        {
            try
            {
                var advertise = advertiseService.Find(advertiseId);
                if (advertise == null)
                {
                    return PartialView("_AdvertiseIndexDetailInfo");
                }
                AdvertiseIndexDetailDTO dto = advertise;
                if (advertise.User != null)
                {
                    dto.UserFullName = advertise.User.FullName;
                }
                if (advertise.RegionCity != null)
                {
                    dto.CityPersianName = advertise.RegionCity.PersianName;
                }
                return PartialView("_AdvertiseIndexDetailInfo", dto);
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.GetAdvertiseIndexDetails", exc);
                return PartialView("_AdvertiseIndexDetailInfo");
            }
        }

        [Authorize(Policy = Policies.Advertise_View)]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            try
            {
                ViewBag.msg = TempData["msg"];
                var model = advertiseService.FindIncludingDeleted(id);
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.Edit", exc);
                return Redirect(Request.Headers["Referrer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public ActionResult Edit(Advertise ad)
        {
            try
            {
                var objad = advertiseService.FindIncludingDeleted(ad.Id);
                if (objad.UserId != ad.UserId)
                {
                    var host_user = userService.Find(ad.UserId);
                    if (host_user.Type < 1)
                    {
                        userService.UpdateUserGeneralType(host_user.Id, Entities.User.UserGeneralTypeEnum.Host);
                    }
                }
                advertiseService.Edit(ad, userAccessor.CurrentUser.Id);
                return RedirectToAction(nameof(NewIndex));
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.Edit", exc);
                return Redirect(Request.Headers["Referrer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Publish)]
        public JsonResult Delete(long id)
        {
            try
            {
                var status = advertiseService.Delete(id);
                return GenerateJsonResult(new { status = status, msg = status ? "" : "این آگهی دارای درخواست رزرو ثبت شده است" });
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.Delete", exc);
                return GenerateJsonResult(new { status = 0, msg = "عملیات با خطا مواجه شد" });
            }
        }

        [Authorize(Policy = Policies.Advertise_Publish)]
        public JsonResult NotVerify(long id)
        {
            try
            {
                advertiseService.NotVerify(id);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("NotVerify", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize(Policy = Policies.Advertise_Publish)]
        public async Task<IActionResult> UpdateActivity(long residenceId)
        {
            var result = await advertiseService.UpdateActivityAsync(residenceId);
            return GenerateJsonResult(new
            {
                status = result.HasError() ? 0 : 1,
                active = result.Result == AdvertiseStatus.Archived ? false : true
            });
        }

        public ActionResult AdvertisePage(string url, string area_str = null, bool amp_version = false,
            int page = 1, string discount_homes = null, string today_empty_homes = null,
            string frompaypernight = null, string topaypernight = null,
            string fromMetrazh = null, string toMetrazh = null,
            string parking = null, string region = null, string capacity = null,
            string room = null, string elevator = null, string pool = null,
            string empty_range_from = null, string empty_range_to = null,
            string norouz_special = null, string instant_reserve = null
        )
        {
            try
            {
                if (HttpContext.Request.Path.Value.Last() == '/')
                {
                    return RedirectPermanent(HtmlUtility.EncodeUrlForRedirect(HttpContext.Request.Path.Value.Remove(HttpContext.Request.Path.Value.Length - 1)));
                }
                if (amp_version)
                {
                    ViewBag.raw_url = HttpContext.Request.Path.Value.Replace("/amp", "");
                }
                else
                {
                    ViewBag.raw_url = HttpContext.Request.Path.Value;
                }
                ViewBag.amp_version = amp_version;
                var category = categoryService.GetByUrl(url);
                if (category != null)
                {
                    if (category.Province > 0)
                    {
                        var targetUrl = CategoryUrlLocalization.CategoryToUrl(category);
                        return RedirectPermanent(HtmlUtility.EncodeUrlForRedirect(targetUrl));
                    }
                    else
                    {
                        if (category.CountryDirection == CountryDirection.North)
                        {
                            return RedirectPermanent(HtmlUtility.EncodeUrlForRedirect("/شمال"));
                        }
                        else
                        {
                            return RedirectPermanent("/");
                        }
                    }
                }
                else
                {
                    return NotFound("صفحه ی مورد نظر موجود نمی باشد .");
                }
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.AdvertisePage", exc);
                return Redirect(Request.Headers["Referrer"].ToString());
            }
        }

        public JsonResult TggleFavorite(long id, bool flag)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    TempData["msg"] = "برای افزودن آگهی به علاقه مندی ها ابتدا عضو شوید و در صورتی که عضو هستید وارد شوید .";
                    return GenerateJsonResult(new { status = 2, val = "" });
                }
                var objUser = userAccessor.CurrentUser;
                if (objUser.Favorite == null || objUser.Favorite.Count == 0)
                {
                    objUser.Favorite = new List<UserFavorite>();
                }

                if (flag)
                {
                    userService.DeleteFavorite(objUser.Id, id);
                }
                else
                {
                    userService.AddFavorite(objUser.Id, id);
                }
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.ToggleFavourite", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        public ActionResult Add()
        {
            return RedirectPermanent("/accomodation/accbasicform");
        }

        public ActionResult Item()
        {
            Response.StatusCode = 404;
            return View("/views/errors/http404.cshtml");
        }

        public ActionResult DailyRentPage(bool amp_version = false)
        {
            return AdvertisePage("اجاره-ویلا-سوئیت-آپارتمان", "", amp_version);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddComment(long advertiseID, string text, int user_id = 0, long parentID = 0)
        {
            try
            {
                if (user_id == 0)
                    user_id = userAccessor.CurrentUser.Id;
                string cannotAddReason;
                var canAdd = advertiseService.AddAdvertiseComment(user_id,
                    advertiseID, text, out cannotAddReason,
                    userAccessor.CurrentUser.Id == user_id ? (int?)null : userAccessor.CurrentUser.Id);
                if (canAdd)
                {
                    return GenerateJsonResult(new
                    {
                        status = 1
                    });
                }
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = cannotAddReason
                });
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.AddComment", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = "خطایی در سیستم رخ داده است.لطفا بعدا امتحان کنید."
                });
            }
        }

        [Authorize]
        public JsonResult AddHostReplyComment(long advertiseID, string text, int user_id, long parentID = 0)
        {
            try
            {
                var acc = advertiseService.Find(advertiseID);
                if (acc.UserId != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        val = "شما مجوز انجام این کار را ندارید."
                    });
                }
                advertiseService.AddAdvertiseHostReplyComment(user_id, advertiseID, text);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.AddHostReplyComment", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = "خطایی در سیستم رخ داده است.لطفا بعدا امتحان کنید."
                });
            }
        }

        [Authorize]
        public JsonResult AddScore(long advertiseID, int ReportID, int value, int user_id = 0)
        {
            try
            {
                if (user_id > 0 && user_id != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        val = "شما مجوز انجام این کار را ندارید."
                    });
                }
                reportItemService.SubmitAdvertiseScore(userAccessor.CurrentUser.Id,
                    advertiseID, ReportID, value, 0);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.AddScore", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = "خطایی در سیتمم  اتفاق افتاده است.لطفا بعدا امتحان کنید."
                });
            }
        }
        [Authorize(Policy = Policies.Reserve_Support_Actions)]
        public JsonResult AddScoreAdmin(long advertiseID, int ReportID, int value, int user_id = 0)
        {
            try
            {
                if (user_id < 1 || userService.Find(user_id) == null)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        val = "کد کاربری نامعتبر"
                    });
                }
                reportItemService.SubmitAdvertiseScore(user_id,
                    advertiseID, ReportID, value, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("Advertise.AddScoreAdmin", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = "خطایی در سیتمم  اتفاق افتاده است.لطفا بعدا امتحان کنید."
                });
            }
        }

        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult MostViewAdvertiseByCity(int city_id, int province_id, int type_id, int count)
        {
            try
            {
                var model = advertiseService.GetMostViewedAdvertisesInCity(city_id, province_id, type_id, count).ToList();
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
                logger.Error("Advertise.MostViewAdvertiseByCity", exc);
                return PartialView("_AdvertiseList", null);
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        public JsonResult AddSupporterInfoToAdvertise(long advertise_id, string text)
        {
            try
            {
                advertiseService.AddSupporterInfo(advertise_id, text, userAccessor.CurrentUser);
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("AddSupporterInfoToAdvertise", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }

        [Authorize(Policy = Policies.Advertise_View)]
        public ActionResult GetAdvertiseSupporterInfo(long advertise_id)
        {
            try
            {
                var advertise = advertiseService.Find(advertise_id);
                return PartialView("_AdvertiseSupporterInfo", advertise);
            }
            catch (Exception exc)
            {
                logger.Error("GetAdvertiseSupporterInfo", exc);
                return Content("");
            }
        }
    }
}
