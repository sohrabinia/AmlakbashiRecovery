using Amlakbashi.Core.Common.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Filters
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cacheManager = context.HttpContext.RequestServices.GetService(typeof(ICacheManager)) as ICacheManager;
            if (string.IsNullOrEmpty(context.HttpContext.Request.QueryString.Value))
            {
                var regionId = context.HttpContext.Request.RouteValues["regionId"];
                var cachedData = cacheManager.Get<ViewResult>($"category.item:{regionId}");
                if (cachedData != null)
                {
                    context.Result = cachedData;
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var cacheManager = context.HttpContext.RequestServices.GetService(typeof(ICacheManager)) as ICacheManager;
            if (string.IsNullOrEmpty(context.HttpContext.Request.QueryString.Value))
            {
                var regionId = context.HttpContext.Request.RouteValues["regionId"];
                cacheManager.Set($"category.item:{regionId}", context.Result as ViewResult);
            }
        }
    }
}
