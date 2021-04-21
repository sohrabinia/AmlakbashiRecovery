using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Hosting;
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
            if (host.HasValue && host.Value.ToLower().Contains("webapi.") && path.Value.StartsWith("/api/") == false)
            {
                var domainSections = host.Value.Split('.');
                var mainDomain = domainSections.ElementAt(domainSections.Length - 2) + "." + domainSections.Last();
                var redirectUrl = $"{request.Scheme}://{mainDomain}/errors/http404";
                HttpResponse response = context.HttpContext.Response;
                response.Redirect(redirectUrl);
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}
