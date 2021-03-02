using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Net.Http.Headers;

namespace Amlakbashi.Host.Configurations.UrlRewriteRules
{
    public class RedirectLowerCaseRule : IRule
    {
        public int StatusCode { get; } = StatusCodes.Status301MovedPermanently;

        public void ApplyRule(RewriteContext context)
        {
            HttpRequest request = context.HttpContext.Request;
            PathString path = context.HttpContext.Request.Path;
            HostString host = context.HttpContext.Request.Host;

            if ((path.HasValue && path.Value.Any(char.IsUpper) || host.HasValue && host.Value.Any(char.IsUpper))
                && request.Method != "POST")
            {
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = StatusCode;
                response.Headers[HeaderNames.Location] = (request.Scheme + "://" + host.Value + request.PathBase + request.Path).ToLower() + request.QueryString;
                context.Result = RuleResult.EndResponse;
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}
