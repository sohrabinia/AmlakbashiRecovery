using Amlakbashi.Application.Services.ActionLogServices.Interfaces;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using X.PagedList;

namespace Amlakbashi.Host.Controllers
{
    public class ActionLogController : BaseController
    {
        private readonly ILog logger;
        private readonly IActionLogAppService actionLogService;
        public ActionLogController(ILog logger,
            IActionLogAppService actionLogService)
        {
            this.logger = logger;
            this.actionLogService = actionLogService;
        }

        [Authorize(Policy = Policies.ActionLog_View)]
        public ActionResult Index(int? page, int user_id = -1, int action_type = -1,
            int action_source = -1, long related_id = -1)
        {
            try
            {
                var model = actionLogService.Filter(user_id, action_type, action_source, related_id);

                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 10);

                ViewBag.user_id = user_id;
                ViewBag.action_type = action_type;
                ViewBag.action_source = action_source;
                ViewBag.related_id = related_id;

                ViewBag.RowIndexStart = (PageNumber * 10) - 10;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("ActionLog.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.ActionLog_View)]
        public ActionResult Detail(long id)
        {
            try
            {
                var model = actionLogService.Find(id);
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("ActionLog.Detail", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}
