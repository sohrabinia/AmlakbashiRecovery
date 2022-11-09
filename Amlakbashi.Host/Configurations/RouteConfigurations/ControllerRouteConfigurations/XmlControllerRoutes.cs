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
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "OldSitemap",
                pattern: "old-sitemap.xml",
                defaults: new { controller = "xml", action = "oldsitemap" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "AdvertiseSitemap",
                pattern: "advertises-sitemap.xml",
                defaults: new { controller = "xml", action = "adsitemap" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "ImageSitemap",
                pattern: "image-sitemap.xml",
                defaults: new { controller = "xml", action = "ImageSitemap" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "ProvincesSitemap",
                pattern: "province-sitemap.xml",
                defaults: new { controller = "xml", action = "ProvinceSitemap" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "CitySitemap",
                pattern: "city-sitemap.xml",
                defaults: new { controller = "xml", action = "CitySitemap" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "AreaSitemap",
                pattern: "area-sitemap.xml",
                defaults: new { controller = "xml", action = "AreaSitemap" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "TagSitemap",
                pattern: "tag-sitemap.xml",
                defaults: new { controller = "xml", action = "TagSitemap" }
            );

            endpointRouteBuilder.MapControllerRoute(
              "RSS", "rss",
               new { controller = "XML", action = "RSS" }
            );
        }
    }
}
