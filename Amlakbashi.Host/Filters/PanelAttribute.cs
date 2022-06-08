using Amlakbashi.Host.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Amlakbashi.Host.Filters
{
    public class PanelAttribute : ActionFilterAttribute
    {
        private readonly User.UserGeneralTypeEnum Type;
        public PanelAttribute(User.UserGeneralTypeEnum type)
        {
            this.Type = type;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.GetUserPanelType() != Type)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = (int)System.Net.HttpStatusCode.Forbidden,
                    Content = "forbidden for current panel"
                };
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
