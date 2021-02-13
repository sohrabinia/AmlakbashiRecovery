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
            DefaultControllerRoutes.Config(endpointRouteBuilder);
            FileControllerRoutes.Config(endpointRouteBuilder);
            AdminControllerRoutes.Config(endpointRouteBuilder);
            PostControllerRoutes.Config(endpointRouteBuilder);
        }
    }
}
