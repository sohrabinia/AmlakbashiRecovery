using Amlakbashi.Host.Configurations.RouteConfigurations.RouteConstraints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Amlakbashi.Host.Configurations.RouteConfigurations.ControllerRouteConfigurations
{
    public class FileControllerRoutes
    {
        public static void Config(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                "post-image-thumb",
                "عکس-پست/{slug}-{PostID}-{w}-{h}",
                defaults: new { controller = "File", action = "PostImageThumb" },
                constraints: new { slug = new PostImageConstraint()}
            );

            endpointRouteBuilder.MapControllerRoute(
                "advertise-image-thumb",
                "عکس-آگهی/{slug}",
                 defaults: new { controller = "File", action = "AdvertiseImageThumb" },
                 constraints: new { slug = new AdvertiseItemConstraint() }
            );

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-wide-image-xsmall",
            //    "عکس-آگهی-عریض-خیلی-کوچک/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumb", w = 146, h = 82 },
            //    constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-wide-image-small",
            //    "عکس-آگهی-عریض-کوچک/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumb", w = 213, h = 120 },
            //    constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-wide-image-medium",
            //    "عکس-آگهی-عریض-متوسط/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumb", w = 249, h = 140 },
            //    constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-wide-image-large",
            //    "عکس-آگهی-عریض-بزرگ/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumb", w = 331, h = 186 },
            //    constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-wide-image-xlarge",
            //    "عکس-آگهی-عریض-بزرگتر/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumb", w = 450, h = 253 },
            //    constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-wide-image-xxlarge",
            //    "عکس-آگهی-عریض-خیلی-بزرگ/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumb", w = 600, h = 338 },
            //    constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-wide-image-xxxlarge",
            //    "عکس-آگهی-عریض-بزرگترین/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumb", w = 700, h = 394 },
            //    constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            endpointRouteBuilder.MapControllerRoute(
                "advertise-image-thumb-large",
                "عکس-آگهی-بزرگ/{slug}",
                 defaults: new { controller = "File", action = "AdvertiseImageThumbLarge" },
                 constraints: new { slug = new AdvertiseItemConstraint() }
            );

            //endpointRouteBuilder.MapControllerRoute(
            //    "advertise-image-thumb-small",
            //    "عکس-آگهی-کوچک/{slug}",
            //     defaults: new { controller = "File", action = "AdvertiseImageThumbSmall" },
            //     constraints: new { slug = new AdvertiseItemConstraint()/*, subdomain = new SubdomainRouteConstraint("www")*/ }
            //);

            endpointRouteBuilder.MapControllerRoute(
                "user-profile-image",
                "عکس-پروفایل-{FileID}",
                defaults: new { controller = "File", action = "UserImageThumb" }
                );

            endpointRouteBuilder.MapControllerRoute(
                "user-profile-image-small",
                "عکس-پروفایل_کوچک-{FileID}",
                defaults: new
                {
                    controller = "File",
                    action = "UserImageThumb",
                    w = 40,
                    h = 40
                }
                );

            endpointRouteBuilder.MapControllerRoute(
                "image-not-found",
                "عکس-یافت-نشد-{w}-{h}",
                defaults: new { controller = "File", action = "ImageNotFound" },
                constraints: new { slug = new ImageDimensionConstraint() }
            );
        }
    }
}
