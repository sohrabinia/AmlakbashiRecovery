using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class TagControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                "تگ",
                "residences/tag/{urlTitle}",
                new
                {
                    controller = "Tag",
                    action = "GetResidences"
                }
            );
        }
    }
}
