using Amlakbashi.Accounting;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers
{
    public class BankingController : BaseController
    {
        private readonly ILog logger;
        private readonly IAccountingFacade accountingFacade;

        public BankingController(ILog logger,
            IAccountingFacade accountingFacade)
        {
            this.logger = logger;
            this.accountingFacade = accountingFacade;
        }

        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public async Task<IActionResult> VerifySheba(string sheba)
        {
            try
            {
                var result = await accountingFacade.VerifySheba("IR" + sheba);
                return PartialView("_VerifySheba", result);
            }
            catch (Exception exc)
            {
                logger.Error("Banking.VerifySheba", exc);
                return PartialView("_VerifySheba");
            }
        }
    }
}
