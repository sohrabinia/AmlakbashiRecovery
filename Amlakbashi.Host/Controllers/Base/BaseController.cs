using Amlakbashi.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        public JsonResult GenerateJsonResult(object data)
        {
            return new JsonResult(data);
        }

        protected JsonResult SuccessJsonResult(string message = null)
        {
            return GenerateJsonResult(new
            {
                status = 1,
                message
            });
        }

        protected JsonResult ErrorJsonResult(string message = null)
        {
            return GenerateJsonResult(new
            {
                status = 0,
                message
            });
        }

        protected IActionResult GenerateResult(ServiceResult result, object okResponse = null)
        {
            if (result.CheckHasError)
            {
                return BadRequest(result.Errors);
            }
            return okResponse is null ? Ok() : Ok(okResponse);
        }
    }
}
