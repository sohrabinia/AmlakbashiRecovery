using System;
using System.Linq;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using log4net;
using AutoMapper;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.DTOs;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Amlakbashi.Host.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly ILog logger;
        private readonly IMapper mapper;
        private readonly IServiceAppService serviceService;
        public ServiceController(
            ILog logger,
            IMapper mapper,
            IServiceAppService serviceService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.serviceService = serviceService;
        }

        [Auth(UserRoles.Admin)]
        public JsonResult Delete(int id)
        {
            try
            {
                serviceService.Delete(id);
                return GenerateJsonResult(new { status = 1, val = "" } );
            }
            catch (Exception exc)
            {
                logger.Error("service deletion failed", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Auth(UserRoles.Admin)]
        [HttpGet]
        public ActionResult Edit(int id = -1)
        {
            try
            {
                var servicesRaw = serviceService.GetRoots();
                var services = servicesRaw.Select(s => mapper.Map<ServiceDTO>(s)).ToList();
                foreach (var item in services)
                {
                    item.AddChildren(
                        serviceService.GetChildren(item.Id).
                        Select(s => mapper.Map<ServiceDTO>(s)).ToList());
                }
                ViewBag.Services = services;
                ViewBag.msg = TempData["msg"];
                if (id == -1)
                {
                    Service model = new Service();
                    model.Id = -1;
                    return View(model);
                }
                else
                {
                    Service model = serviceService.Find(id);
                    return View(model);
                }
            }
            catch (Exception exc)
            {
                logger.Error("service edit page failed to load", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        [HttpPost]
        public ActionResult Edit(Service Service)
        {
            try
            {
                if (Service.Id == -1)
                {
                    serviceService.Insert(Service);
                }
                else
                {
                    if (serviceService.Validate(Service) == false)
                    {
                        TempData["msg"] = "والد سرویس نمیتواند از نسل خودش باشد.";
                        return RedirectToAction("edit", new { id=Service.Id});
                    }
                    serviceService.Update(Service);
                }

                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                logger.Error("service update failed", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        public ActionResult Index(int? page)
        {
            try
            {
                var model = serviceService.GetAll().
                    Select(s => mapper.Map<ServiceDTO>(s)).ToList();
                foreach (var item in model)
                {
                    if (item.ParentId != -1)
                        item.Parent = serviceService.Find(item.ParentId);
                }
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 15);
                ViewBag.RowIndexStart = (PageNumber * 15) - 15;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("service index page failed to load", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }
    }
}

