using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class XmlControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: "MainSitemap",
                pattern: "sitemap.xml",
                defaults: new { controller = "xml", action = "sitemap" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "OldSitemap",
                pattern: "old-sitemap.xml",
                defaults: new { controller = "xml", action = "oldsitemap" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "AdvertiseSitemap",
                pattern: "advertises-sitemap.xml",
                defaults: new { controller = "xml", action = "adsitemap" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "ImageSitemap",
                pattern: "image-sitemap.xml",
                defaults: new { controller = "xml", action = "ImageSitemap" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "ProvincesSitemap",
                pattern: "province-sitemap.xml",
                defaults: new { controller = "xml", action = "ProvinceSitemap" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "CitySitemap",
                pattern: "city-sitemap.xml",
                defaults: new { controller = "xml", action = "CitySitemap" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "AreaSitemap",
                pattern: "area-sitemap.xml",
                defaults: new { controller = "xml", action = "AreaSitemap" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
              "RSS", "rss",
               new { controller = "XML", action = "RSS" }
               //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
               );
        }
    }
}
