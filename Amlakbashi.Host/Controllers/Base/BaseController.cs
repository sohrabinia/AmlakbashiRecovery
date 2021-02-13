using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        public JsonResult GenerateJsonResult(object data)
        {
            return new JsonResult(data);
        }
    }
}
