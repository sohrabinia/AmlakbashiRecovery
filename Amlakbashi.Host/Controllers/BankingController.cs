using Amlakbashi.Accounting;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        //[Authorize(Roles = Roles.TechnicalManager)]
        public async Task<JsonResult> VerifySheba(string sheba)
        {
            var result = await accountingFacade.VerifySheba(sheba);
            return GenerateJsonResult(result);
        }
    }
}
