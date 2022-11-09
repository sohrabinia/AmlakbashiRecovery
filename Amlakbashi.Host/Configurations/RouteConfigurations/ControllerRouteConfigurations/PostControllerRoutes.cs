using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class PostControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                "dashboard",
                "dashboard",
                new { controller = "post", action = "Dashboard" }
            );

            endpointRouteBuilder.MapControllerRoute(
                "contact",
                "contact",
                new { controller = "post", action = "contact" }
            );

            endpointRouteBuilder.MapControllerRoute(
                "help",
                "help",
                new { controller = "post", action = "public", sid = 8 }
            );

            endpointRouteBuilder.MapControllerRoute(
                "comment-complain",
                "comment-complain",
                new { controller = "post", action = "public", sid = 24 }
            );

            endpointRouteBuilder.MapControllerRoute(
                "rules",
                "rules",
                new { controller = "post", action = "public", sid = 25 }
            );

            endpointRouteBuilder.MapControllerRoute(
                "contact-amp",
                "amp/contact",
                new { controller = "post", action = "public", sid = 6, amp_version = true }
            );

            endpointRouteBuilder.MapControllerRoute(
                "help-amp",
                "amp/help",
                new { controller = "post", action = "public", sid = 8, amp_version = true }
            );

            endpointRouteBuilder.MapControllerRoute(
                "comment-complain-amp",
                "amp/comment-complain",
                new { controller = "post", action = "public", sid = 24, amp_version = true }
            );

            endpointRouteBuilder.MapControllerRoute(
                "rules-amp",
                "amp/rules",
                new { controller = "post", action = "public", sid = 25, amp_version = true }
            );

            endpointRouteBuilder.MapControllerRoute(
                "جستجوی-اشتباه",
                "s/{regionId}/",
                new { controller = "Post", action = "Http404" }
            );

            endpointRouteBuilder.MapControllerRoute(
                "املاک",
                "املاک/{url}",
                new { controller = "Post", action = "Http404" }
            );
        }
    }
}
