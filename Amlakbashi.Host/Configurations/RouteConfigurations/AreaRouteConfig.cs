using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Configurations.RouteConfigurations
{
    public class AreaRouteConfig
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapAreaControllerRoute(
                name: "ApplicationArea",
                areaName: "App",
                pattern: "app/{controller}/{action}");
        }
    }
}
