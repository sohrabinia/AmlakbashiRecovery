using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints
{
    public class ImageDimensionConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var w = values["w"];
            var h = values["h"];
            if (w == null || h == null)
                return false;
            return int.TryParse(w.ToString(), out _) && int.TryParse(h.ToString(), out _);
        }
    }
}
