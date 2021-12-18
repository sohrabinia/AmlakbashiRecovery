using Amlakbashi.Host.Configurations.RouteConfigurations;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations
{
    public class RouteConfig
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            AreaRouteConfig.Config(endpointRouteBuilder);
            ControllerRouteConfig.Config(endpointRouteBuilder);
            HubRouteConfig.Config(endpointRouteBuilder);
        }
    }
}
