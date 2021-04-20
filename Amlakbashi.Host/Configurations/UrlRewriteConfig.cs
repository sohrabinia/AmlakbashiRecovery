using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Amlakbashi.Host.Configurations.UrlRewriteRules;

namespace Amlakbashi.Host.Configurations
{
    public class UrlRewriteConfig
    {
        public static void Config(IApplicationBuilder app)
        {
            var options = new RewriteOptions();
            options.AddRedirectToWww();
            options.AddRedirectToLowerCase();
            options.AddWebapiSubdomain();
            app.UseRewriter(options);
        }
    }
}
