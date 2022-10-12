using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Amlakbashi.Host.Controllers
{
    public class CancelDiscussionController : BaseController
    {
        private readonly ILog logger;
        private readonly IReserveAppService reserveService;
        private readonly IUserAccessor userAccessor;
        public CancelDiscussionController(ILog logger,
            IReserveAppService reserveService,
            IUserAccessor userAccessor)
        {
            this.logger = logger;
            this.reserveService = reserveService;
            this.userAccessor = userAccessor;
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public ActionResult Index(long reserve_id)
        {
            try
            {
                return View(reserveService.Find(reserve_id).GetCancelDiscussionList());
            }
            catch (Exception exc)
            {
                logger.Error("CancelDiscussion.Index", exc);
                return Redirect(Request.Headers["Referrer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public ActionResult GetReserveCancelDiscussion(long reserve_id)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                return PartialView("_ReserveCancelDiscussion", reserve);
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return Content("");
            }
        }

        [Authorize]
        public ActionResult GuestCancelDiscussionPopup(long reserve_id)
        {
            var reserve = reserveService.Find(reserve_id);
            if (userAccessor.CurrentUser.Id != reserve.UserID)
                return null;
            return CancelDiscussionPopup(reserve_id);
        }

        [Authorize]
        public ActionResult HostCancelDiscussionPopup(long reserve_id)
        {
            var reserve = reserveService.Find(reserve_id);
            if (userAccessor.CurrentUser.Id != reserve.HostUserID)
                return null;
            return CancelDiscussionPopup(reserve_id);
        }

        public ActionResult CancelDiscussionPopup(long reserve_id)
        {
            return PartialView("_CancelDiscussionPopup", reserveService.Find(reserve_id).GetCancelDiscussionList());
        }

        public JsonResult SendText(long reserve_id, string text)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var user_id = userAccessor.CurrentUser.Id;
                var guest_user_id = reserve.UserID;
                var host_user_id = reserve.HostUserID;
                var is_guest = user_id == guest_user_id;

                if (user_id != guest_user_id && user_id != host_user_id)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0
                    });
                }
                reserveService.UpdateCanselDiscussion(reserve.Id, text, userAccessor.CurrentUser);
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("CancelDiscussion.SendText", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }
    }
}
