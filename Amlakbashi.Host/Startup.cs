using Amlakbashi.Application;
using Amlakbashi.Host.Configurations;
using Amlakbashi.Host.Hubs.Admin;
using Amlakbashi.Host.Hubs.Dashboard;
using Amlakbashi.Host.Hubs.Portal;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();
                Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }


        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the ConfigureContainer method, below.
        public void ConfigureServices(IServiceCollection services)
        {

            //Added for session state
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
            });

            // TODO: match authentication with previous system
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options=> {
                options.Cookie.MaxAge = new TimeSpan(60, 0, 0, 0);
            });

            services.AddHangfire(configuration => configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(Configuration.GetConnectionString("JobDb"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));
            services.AddHangfireServer();

            services.AddResponseCaching();
            services.AddControllersWithViews();
            services.AddSignalR();
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            IoCConfig.Config(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BackgroundStartup backgroundStartup)
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
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();

            UrlRewriteConfig.Config(app);
            app.UseEndpoints(endpoints => RouteConfig.Config(endpoints));

            // If, for some reason, you need a reference to the built container, you
            // can use the convenience extension method GetAutofacRoot.
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            // hangfire
            GlobalConfiguration.Configuration.UseAutofacActivator(AutofacContainer)
                .UseSerializerSettings(new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects
                });

            backgroundStartup.Startup();

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(env.ContentRootPath + "/amlakbashi-7e6b2-firebase-adminsdk-h6gkp-0159f2aab7.json")
            });
        }
    }
}
