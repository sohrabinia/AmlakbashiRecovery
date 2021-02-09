using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints
{
    public class AdvertiseItemConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var slug = values["slug"].ToString();
            return int.TryParse(slug.Split('-')[0], out _);
        }
    }
}
