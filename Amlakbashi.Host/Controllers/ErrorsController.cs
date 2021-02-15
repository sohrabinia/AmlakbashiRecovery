using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Amlakbashi.Host.Controllers
{
    public class ErrorsController : BaseController
    {
        private readonly ILog logger;
        private readonly ICategoryAppService categoryService;
        private readonly IAdvertiseAppService advertiseService;
        public ErrorsController(ILog logger,
            ICategoryAppService categoryService,
            IAdvertiseAppService advertiseService)
        {
            this.logger = logger;
            this.categoryService = categoryService;
            this.advertiseService = advertiseService;
        }

        [HttpGet]
        public ActionResult Http404()
        {
            try
            {
                Advertise acc = null;
                var url = Request.Path.Value.Split('/').Last();
                long id;
                if (long.TryParse(url.Split('-')[0], out id))
                {
                    acc = advertiseService.Find(id);
                }
                if (acc != null)
                {
                    ViewBag.isAccommodation = true;
                    ViewBag.relatedCategories = categoryService.GetLinks(
                        acc.TypeID, acc.City == null ? 0 : (int)acc.City, -1, 1);
                }
                Response.StatusCode = 404;
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Errors.Http404", exc);
                return null;
            }

        }

        [HttpGet]
        public ActionResult Http500()
        {
            try
            {
                Response.StatusCode = 500;
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Errors.Http500", exc);
                return null;
            }

        }

        public ActionResult AccessDenied(bool is_login_banned = false, string originUrl = "")
        {
            try
            {
                ViewBag.is_login_banned = is_login_banned;
                ViewBag.originUrl = originUrl;
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Errors.AccessDenied", exc);
                return null;
            }

        }
    }
}
