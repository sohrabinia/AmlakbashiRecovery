using Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations;
using Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations
{
    public class ControllerRouteConfig
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            AccomodationControllerRoutes.Config(endpointRouteBuilder);
            AdminControllerRoutes.Config(endpointRouteBuilder);
            AdvertiseControllerRoutes.Config(endpointRouteBuilder);
            CategoryControllerRoutes.Config(endpointRouteBuilder);
            FileControllerRoutes.Config(endpointRouteBuilder);
            PostControllerRoutes.Config(endpointRouteBuilder);
            XmlControllerRoutes.Config(endpointRouteBuilder);
            DefaultControllerRoutes.Config(endpointRouteBuilder);
        }
    }
}
