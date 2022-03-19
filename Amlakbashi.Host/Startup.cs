using Amlakbashi.Application;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Data;
using Amlakbashi.Data.Identity;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Configurations;
using Amlakbashi.Host.Extensions;
using AntiXssMiddleware.Middleware;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
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

            services.AddDbContext<IdentityDB>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityDB")), ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "+ 0123456789";
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
            }).AddPasswordValidator<CustomPasswordValidator>()
            .AddRoles<AppRole>().AddEntityFrameworkStores<IdentityDB>().AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = 
                        TokenUtility.GetTokenValidationParameters(Configuration["JwtConfig:Secret"]);
                });

            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/errors/accessdenied";
                options.LoginPath = string.Empty;
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.Events.OnRedirectToLogin = context =>
                {
                    var refererUrl = context.Request.Path;
                    context.Response.Redirect("/errors/accessdenied?originUrl=" + refererUrl);
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.IsAjaxRequest())
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                    else
                    {
                        context.Response.Redirect("/errors/accessdenied");
                    }
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

            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
            services.AddResponseCaching();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSignalR();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = 20971520;
                x.MultipartBodyLengthLimit = 20971520;
            });

            var redisConfigString = $"{Configuration.GetValue<string>("Redis:Server")}:{Configuration.GetValue<int>("Redis:Port")},allowAdmin=true,abortConnect=false";
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfigString;
            });
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfigString));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowCrossOrigins", policy =>
                {
                    var crossOrigins = new List<string>();
                    crossOrigins.Add("http://localhost:3000");
                    crossOrigins.Add("https://localhost:3000");
                    crossOrigins.Add("http://next.amlakbashi.com");
                    crossOrigins.Add("https://next.amlakbashi.com");
                    policy.WithOrigins(crossOrigins.ToArray()).AllowAnyHeader().AllowAnyMethod();
                });
            });
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
                app.UseExceptionHandler("/errors/http500");
                app.UseStatusCodePagesWithReExecute("/errors/http404");
            }

            app.UseAntiXssMiddleware();
            app.UseCors("AllowCrossOrigins");
            app.UseResponseCaching();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (ctx.File.Name.EndsWith(".css") || ctx.File.Name.EndsWith(".js"))
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + 60 * 60 * 24 * 365;
                    }
                }
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
                {
                    { ".apk","application/vnd.android.package-archive"}
                })
            });
            app.UseRouting();
            UrlRewriteConfig.Config(app);

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints => RouteConfig.Config(endpoints));

            // If, for some reason, you need a reference to the built container, you
            // can use the convenience extension method GetAutofacRoot.
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            // hangfire
            GlobalConfiguration.Configuration.UseAutofacActivator(AutofacContainer, false)
                .UseSerializerSettings(new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects
                });

            backgroundStartup.Startup();

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(env.ContentRootPath + "/amlakbashi-7e6b2-firebase-adminsdk-h6gkp-0159f2aab7.json")
            });

            //IdentityDbInitializer.Initialize(app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope());
            AmlakbashiDbInitializer.Initialize(app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope());
        }
    }
}
