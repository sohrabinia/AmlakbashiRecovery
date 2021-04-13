using Amlakbashi.Application;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Data.Identity;
using Amlakbashi.Host.Configurations;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
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
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
            });

            services.AddDbContext<IdentityDB>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityDB")));
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "+ 0123456789";
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddRoles<AppRole>().AddEntityFrameworkStores<IdentityDB>().AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };
                });

            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/errors/accessdenied";
                options.LoginPath = string.Empty;
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Redirect("/errors/accessdenied");
                    return Task.CompletedTask;
                };
            });

            services.AddAuthorization(options =>
            {
                PolicyConfig.Config(options);
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

            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Arabic }));
            services.AddResponseCaching();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
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
            app.UseCookiePolicy();
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

            DatabaseInitializer.SeedData(app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope());
        }
    }
}
