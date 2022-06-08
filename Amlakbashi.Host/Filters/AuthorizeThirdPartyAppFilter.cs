using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Filters
{
    public class AuthorizeThirdPartyAppFilter : IAuthorizationFilter
    {
        private readonly IConfiguration configuration;
        public AuthorizeThirdPartyAppFilter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string tpk = null;
            if (context.HttpContext.Request.Headers.ContainsKey("tpk"))
            {
                tpk = context.HttpContext.Request.Headers["tpk"];
            }
            var thirdPartyAppsKeys = configuration.GetSection("ThirdPartyAppsKeys").AsEnumerable();
            var keys = thirdPartyAppsKeys.Select(x => x.Value).Where(x => string.IsNullOrEmpty(x) == false);
            if (keys.Contains(tpk) == false)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = (int)System.Net.HttpStatusCode.Unauthorized,
                    Content = "unauthorized third party app"
                };
            }
        }
    }

    public class AuthorizeThirdPartyAppAttribute : TypeFilterAttribute
    {
        public AuthorizeThirdPartyAppAttribute() : base(typeof(AuthorizeThirdPartyAppFilter))
        {
            
        }
    }
}
