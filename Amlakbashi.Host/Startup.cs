using Amlakbashi.Application;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Data.Identity;
using Amlakbashi.Host.Authentication;
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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                    var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        RequireSignedTokens = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true,
                        NameClaimType = "name",
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
                    var refererUrl = context.Request.Path;
                    context.Response.Redirect("/errors/accessdenied?originUrl=" + refererUrl);
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
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                //FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
                //Set the content-type without restriction. This setting can download all types of files, but it is not recommended because it is not safe.
                //ServeUnknownFileTypes = true 
                //The following settings can download files of type apk
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
                {
                    { ".apk","application/vnd.android.package-archive"}
                })
            });
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();

            UrlRewriteConfig.Config(app);
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

            DatabaseInitializer.SeedData(app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope());
        }
    }
}
