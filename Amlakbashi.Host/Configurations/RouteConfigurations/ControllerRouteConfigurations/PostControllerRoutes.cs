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
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "contact",
                "contact",
                new { controller = "post", action = "contact" }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "help",
                "help",
                new { controller = "post", action = "public", sid = 8 }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "comment-complain",
                "comment-complain",
                new { controller = "post", action = "public", sid = 24 }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "rules",
                "rules",
                new { controller = "post", action = "public", sid = 25 }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "contact-amp",
                "amp/contact",
                new { controller = "post", action = "public", sid = 6, amp_version = true }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "help-amp",
                "amp/help",
                new { controller = "post", action = "public", sid = 8, amp_version = true }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "comment-complain-amp",
                "amp/comment-complain",
                new { controller = "post", action = "public", sid = 24, amp_version = true }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "rules-amp",
                "amp/rules",
                new { controller = "post", action = "public", sid = 25, amp_version = true }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );

            endpointRouteBuilder.MapControllerRoute(
                "جستجوی-اشتباه",
                "s/{regionId}/",
                    new
                    {
                        controller = "Post",
                        action = "Http404",
                    }
            );

            endpointRouteBuilder.MapControllerRoute(
                "املاک",
                "املاک/{url}",
                new
                {
                    controller = "Post",
                    action = "Http404",
                }
                //,constraints: new { subdomain = new SubdomainRouteConstraint("www") }
            );
        }
    }
}
