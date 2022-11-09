using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class DefaultControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: "default",
                pattern: "{controller=Post}/{action=Page}");
        }
    }
}
