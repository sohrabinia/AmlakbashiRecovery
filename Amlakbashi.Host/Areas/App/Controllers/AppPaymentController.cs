using Amlakbashi.Accounting;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Area.App.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/payment/[action]")]
    public class AppPaymentController : AppBaseController
    {
        private readonly IUserAccessor userAccessor;
        private readonly IAccountingFacade accounting;
        public AppPaymentController(IUserAccessor userAccessor,
            IAccountingFacade accounting)
        {
            this.userAccessor = userAccessor;
            this.accounting = accounting;
        }
    }
}
