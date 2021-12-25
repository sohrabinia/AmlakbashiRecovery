using Amlakbashi.Accounting;
using Amlakbashi.Host.Area.App.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/wallet/[action]")]
    public class AppWalletController : AppBaseController
    {
        private readonly IUserAccessor userAccessor;
        private readonly IAccountingFacade accounting;
        public AppWalletController(IUserAccessor userAccessor,
            IAccountingFacade accounting)
        {
            this.userAccessor = userAccessor;
            this.accounting = accounting;
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserWalletManager()
        {
            var user = userAccessor.CurrentUser;
            ViewBag.Credit = user.Credit;
            ViewBag.UserID = user.Id;
            var model = accounting.GetCreditListByUserId(user.Id);
            return View(model);
        }

        [Authorize]
        public IActionResult Prize()
        {
            var user = userAccessor.CurrentUser;
            var presentorCode = user.Id;
            ViewBag.presentorCode = presentorCode == 0 ? "" : presentorCode.ToString();
            ViewBag.refreshOnLogin = presentorCode == 0;
            return View();
        }
    }
}
