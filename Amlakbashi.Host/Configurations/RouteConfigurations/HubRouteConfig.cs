using Amlakbashi.Host.Hubs.Admin;
using Amlakbashi.Host.Hubs.Dashboard;
using Amlakbashi.Host.Hubs.Portal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations
{
    public class HubRouteConfig
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<PortalHub>("/portalhub");
            endpointRouteBuilder.MapHub<ReserveAdminHub>("/reserveadminhub");
            endpointRouteBuilder.MapHub<SupportChatAdminHub>("/supportchatadminhub");
            endpointRouteBuilder.MapHub<ReserveDashboardHub>("/reservedashboardhub");
        }
    }
}
