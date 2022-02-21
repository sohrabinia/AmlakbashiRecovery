using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.BlogPostDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using AutoMapper;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using X.PagedList;
using static Amlakbashi.Core.Entities.BlogPost;

namespace Amlakbashi.Host.Controllers
{
    public class BlogPostController : BaseController
    {
        private readonly IBlogPostAppService blogPostService;
        private readonly IUserAppService userService;
        private readonly IRegionAppService regionService;
        private readonly IUserAccessor userAccessor;
        private readonly IMapper mapper;
        private readonly ILog logger;
        public BlogPostController(IBlogPostAppService blogPostService,
            IMapper mapper,
            IUserAppService userService,
            IUserAccessor userAccessor,
            IRegionAppService regionService,
            ILog logger)
        {
            this.blogPostService = blogPostService;
            this.userService = userService;
            this.userAccessor = userAccessor;
            this.regionService = regionService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [Authorize(Policy = Policies.Post_View)]
        public ActionResult Index(int? page, int id = 0,
            int sortOrder = 0, int status = -1,
            int showingPlace = -1, string postTitle = null,
            int Province = 0, int City = 0, int Area = 0)
        {
            try
            {
                var model = blogPostService.Filter(id,
                    (SortOrdersEnum)sortOrder,
                    (BlogPostStatus)status,
                    (PlaceEnum)showingPlace,
                    postTitle, Province,
                    City, Area);
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 10);

                ViewBag.id = id;
                ViewBag.status = status;
                ViewBag.showingPlace = showingPlace;
                ViewBag.postTitle = postTitle;
                ViewBag.Province = Province;
                ViewBag.City = City;
                ViewBag.Area = Area;
                ViewBag.sortOrder = sortOrder;
                ViewBag.RowIndexStart = (PageNumber * 10) - 10;

                List<BlogPostIndexDTO> blogPostDTOs = new List<BlogPostIndexDTO>();
                foreach (var item in onePageOfModel)
                {
                    var dto = new BlogPostIndexDTO()
                    {
                        BlogPost = item,
                        UserFullName = userService.Find(item.UserID).FullName,
                        LastModifyUserFullName = userService.Find(item.LastModifyUserID).FullName,
                        ShowingPlace = (item.ShowingPlace == BlogPost.PlaceEnum.HomePage ? "-" :
                            (item.Area > 0 ? (regionService.GetRegionName(item.City) + "/" +
                            regionService.GetRegionName(item.Area)) :
                            (item.City > 0 ? regionService.GetRegionName(item.City) : "-")))
                    };
                    blogPostDTOs.Add(dto);
                }
                ViewBag.dto = blogPostDTOs;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return Redirect(Request.Headers["Referrer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Post_Add)]
        public ActionResult AddBlogPostPopup()
        {
            return PartialView("_AddBlogPost");
        }

        [Authorize(Policy = Policies.Post_Edit)]
        public ActionResult EditBlogPostPopup(int id)
        {
            try
            {
                var model = blogPostService.Find(id);
                return PartialView("_AddBlogPost", model);
            }
            catch (Exception exc)
            {
                logger.Error("BlogPost.EditBlogPostPopup", exc);
                return PartialView("_AddBlogPost", null);
            }
        }

        [Authorize(Policy = Policies.Post_Add)]
        public ActionResult AddEdit(int redirectStatus, int id = 0)
        {
            var model = blogPostService.Find(id);
            ViewBag.redirectStatus = redirectStatus;
            return View(model);
        }

        [Authorize(Policy = Policies.Post_Add)]
        //[HttpPost]
        public JsonResult AddEditBlogPost(BlogPost data)
        {
            try
            {
                var exists = data.Id > 0;
                string[] errorMessages;
                var validated = blogPostService.Validate(data, out errorMessages);
                if (validated)
                {
                    if (exists)
                        blogPostService.Update(data, userAccessor.CurrentUser.Id);
                    else
                        blogPostService.Insert(data, userAccessor.CurrentUser.Id);
                }
                return GenerateJsonResult(new {
                    status = validated ? 1 : 0,
                    errorMessages = errorMessages,
                    msg = validated ? (exists ? "ویرایش با موفقیت انجام شد" :
                        "پست با موفقیت ایجاد شد") : ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    errorMessages = new string[] { "عملیات با خطای فنی مواجه شد" }
                });
            }
        }

        [Authorize(Policy = Policies.Post_Publish)]
        public JsonResult ToRecycleBin(int id)
        {
            try
            {
                blogPostService.Scrap(id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "به زباله دان انتقال یافت"
                });
            }
            catch (Exception exc)
            {
                logger.Error("BlogPost.ToRecycleBin", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Post_Publish)]
        public JsonResult RecycleItem(int id)
        {
            try
            {
                blogPostService.Recycle(id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "پست مورد نظر بازیابی شد و به پیش نویس ها اضافه شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("BlogPost.RecycleItem", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Authorize(Policy = Policies.Post_Publish)]
        public JsonResult Remove(int id)
        {
            try
            {
                blogPostService.Delete(id);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "پست مورد نظر به طور کلی حذف شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("BlogPost.Remove", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }
    }
}
