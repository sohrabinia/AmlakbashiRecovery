
using Amlakbashi.Host.Configurations;
using Amlakbashi.Host.Hubs.Admin;
using Amlakbashi.Host.Hubs.Dashboard;
using Amlakbashi.Host.Hubs.Portal;
using Autofac;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("/amlakbashi-7e6b2-firebase-adminsdk-h6gkp-0159f2aab7.json")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<PortalHub>("/PortalHub");
                endpoints.MapHub<ReserveAdminHub>("/ReserveAdminHub");
                endpoints.MapHub<SupportChatAdminHub>("/SupportChatAdminHub");
                endpoints.MapHub<ReserveDashboardHub>("/ReserveDashboardHub");
            });

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            IoCConfig.Config(builder);
        }
    }
}
