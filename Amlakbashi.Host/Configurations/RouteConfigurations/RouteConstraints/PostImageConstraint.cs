using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints
{
    public class PostImageConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var w = values["w"].ToString();
            var h = values["h"].ToString();
            var post_id = values["PostID"].ToString();
            return int.TryParse(post_id, out _) && int.TryParse(w, out _)
                && int.TryParse(h, out _);
        }
    }
}
