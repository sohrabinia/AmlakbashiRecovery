using Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class CategoryControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
              "RSS", "rss",
               new { controller = "XML", action = "RSS" }
               );

            endpointRouteBuilder.MapControllerRoute(
                "جستجوی-عبارت",
                "s/p/{phrase}",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = -2,
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "جستجوی-عبارت-نوع",
                "s/p/{phrase}/{type}",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = -2,
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "ایران",
                "ایران",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = -2,
                }
            );


            endpointRouteBuilder.MapControllerRoute(
                "ایران-نوع",
                "ایران/{type}",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = -2,
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "شمال",
                "شمال",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = -1,
                    countryDirection = 1
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "شمال-با-نوع",
                "شمال/{type}",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = -1,
                    countryDirection = 1
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "جستجوی-موقعیت",
                "s/{regionId}/{name}",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = 0,
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "جستجوی-موقعیت-نوع",
                "s/{regionId}/{name}/{type}",
                new
                {
                    controller = "Category",
                    action = "Item",
                    regionType = 0,
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "نقشه-سایت",
                "نقشه-سایت/{city}/{TradeID}",
                new { controller = "Category", action = "SiteMap", city = 0, TradeId = 0 }
            );

            endpointRouteBuilder.MapControllerRoute(
                "سوالات-متداول",
                "سوالات-متداول",
                new { controller = "Post", action = "FrequentlyQuestions" }
                );

            endpointRouteBuilder.MapControllerRoute(
                "درباره-ما",
                "درباره-ما",
                new { controller = "post", action = "public", sid = 4 }
                );

            endpointRouteBuilder.MapControllerRoute(
                "سوالات-متداول-amp",
                "amp/سوالات-متداول",
                new { controller = "Post", action = "FrequentlyQuestions", amp_version = true }
                );

            endpointRouteBuilder.MapControllerRoute(
                "download-app-amp",
                "amp/downloadapp",
                new { controller = "Post", action = "DownloadApp", amp_version = true }
                );

            endpointRouteBuilder.MapControllerRoute(
                "اخبار-و-مقالات",
                "اخبار-و-مقالات/{title}-{id}",
                defaults: new { controller = "post", action = "newsitem" },
                constraints: new { id = new PostNewsConstraint()}
            );

            endpointRouteBuilder.MapControllerRoute(
                "اخبار-و-مقالات-amp",
                "amp/اخبار-و-مقالات/{title}-{id}",
                defaults: new { controller = "post", action = "newsitem", amp_version = true },
                constraints: new { id = new PostNewsConstraint()}
            );

            endpointRouteBuilder.MapControllerRoute(
                "اخبار-و-مقالات-لیست",
                "اخبار-و-مقالات",
                new { controller = "post", action = "news" }
            );

            endpointRouteBuilder.MapControllerRoute(
                "اخبار-و-مقالات-لیست-amp",
                "amp/اخبار-و-مقالات",
                new { controller = "post", action = "news", amp_version = true }
            );

            endpointRouteBuilder.MapControllerRoute(
                "home_page_amp",
                "amp",
                new
                {
                    controller = "Post",
                    action = "Page",
                    amp_version = true
                }
            );

            endpointRouteBuilder.MapControllerRoute(
                "home_page_mobile",
                "mobile",
                new
                {
                    controller = "Post",
                    action = "DownloadApp",
                    fromApp = true
                }
            );
        }
    }
}
