using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints
{
    public class PostNewsConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var id = values["id"].ToString();
            var title = values["title"].ToString();
            return int.TryParse(id, out _) && !string.IsNullOrEmpty(title);
        }
    }
}
