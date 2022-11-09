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

            endpointRouteBuilder.MapControllerRoute(
                "advertise-image-thumb-large",
                "عکس-آگهی-بزرگ/{slug}",
                 defaults: new { controller = "File", action = "AdvertiseImageThumbLarge" },
                 constraints: new { slug = new AdvertiseItemConstraint() }
            );

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
