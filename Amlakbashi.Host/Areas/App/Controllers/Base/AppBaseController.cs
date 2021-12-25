using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Area.App.Controllers.Base
{
    public abstract class AppBaseController : Controller
    {
        public JsonResult GenerateJsonResult(object data)
        {
            return new JsonResult(data);
        }
    }
}
