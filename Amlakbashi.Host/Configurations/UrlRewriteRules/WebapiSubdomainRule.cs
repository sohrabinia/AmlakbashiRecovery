using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Configurations.UrlRewriteRules
{
    public class WebapiSubdomainRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            HttpRequest request = context.HttpContext.Request;
            PathString path = request.Path;
            HostString host = request.Host;
            var routes = context.HttpContext.Request.RouteValues;
            if (host.HasValue && (host.Value.ToLower().Contains("webapi.")))
            {
                HttpResponse response = context.HttpContext.Response;
                //context.HttpContext.Request.RouteValues["controller"] = "api";
                context.Result = RuleResult.ContinueRules;
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}
