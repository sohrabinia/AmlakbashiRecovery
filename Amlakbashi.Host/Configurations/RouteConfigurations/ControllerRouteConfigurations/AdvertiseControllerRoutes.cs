using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class AdvertiseControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                "اجاره-روزانه-با-منطقه",
                "اجاره-روزانه/{url}/{area_str}",
                new
                {
                    controller = "Advertise",
                    action = "AdvertisePage",
                    cat = "اجاره-روزانه"
                }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "اجاره-روزانه",
                "اجاره-روزانه/{url}",
                new
                {
                    controller = "Advertise",
                    action = "AdvertisePage",
                    cat = "اجاره-روزانه",
                    area_str = ""
                }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "اجاره-روزانه-همه",
                "اجاره-روزانه",
                new
                {
                    controller = "Advertise",
                    action = "DailyRentPage",
                }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "اجاره-روزانه-با-منطقه-amp",
                "amp/اجاره-روزانه/{url}/{area_str}",
                new
                {
                    controller = "Advertise",
                    action = "AdvertisePage",
                    cat = "اجاره-روزانه",
                    amp_version = true
                }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "اجاره-روزانه-amp",
                "amp/اجاره-روزانه/{url}",
                new
                {
                    controller = "Advertise",
                    action = "AdvertisePage",
                    cat = "اجاره-روزانه",
                    area_str = "",
                    amp_version = true
                }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "اجاره-روزانه-همه-amp",
                "amp/اجاره-روزانه",
                new
                {
                    controller = "Advertise",
                    action = "DailyRentPage",
                    amp_version = true
                }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );
        }
    }
}
