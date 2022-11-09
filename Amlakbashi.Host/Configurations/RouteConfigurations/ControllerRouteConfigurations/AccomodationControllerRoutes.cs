using Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class AccomodationControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                "اقامتگاه",
                "اجاره-روزانه/{slug}",
                new { controller = "Accomodation", action = "Item" }
                , constraints: new { url = new AdvertiseItemConstraint() }
            );
        }
    }
}
