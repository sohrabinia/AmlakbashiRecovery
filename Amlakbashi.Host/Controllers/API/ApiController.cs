using System;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using log4net;
using Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Accounting;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.ReserveSupportManager;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Amlakbashi.Host.Hubs.Dashboard.HubServers;
using Amlakbashi.Host.Hubs.Admin.HubServers;
using Microsoft.AspNetCore.Identity;
using Amlakbashi.Data.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Amlakbashi.Core.Identity.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : Controller
    {
        private string client_id = "7e1dff94-4f78-4eba-af9f-e54605925e5c";
        private const string bearerScheme = JwtBearerDefaults.AuthenticationScheme;
        private readonly IReportItemAppService reportItemService;
        private readonly ICommentAppService commentService;
        private readonly IUserAppService userService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IBankCardAppService bankCardService;
        private readonly IRegionAppService regionService;
        private readonly IDiscountTableAppService discountTableService;
        private readonly IPriceTableAppService priceTableService;
        private readonly IAdvertiseReportAppService advertiseReportService;
        private readonly IFileAppService fileService;
        private readonly ICategoryAppService categoryService;
        private readonly IChatAppService chatService;
        private readonly IReserveAppService reserveService;
        private readonly IExtrinsicReserveAppService extrinsicReserveService;
        private readonly IAccountingFacade accounting;
        private readonly IInstantReserveAutoCancelAppService instantReserveAutoCancelService;
        private readonly IUserContactFacade userContact;
        private readonly IReserveSupportManager reserveSupportManager;
        private readonly IReserveAutoCancelAppService reserveAutoCancelService;
        private readonly ILog logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IReserveDashboardHubServer reserveDashboardHubServer;
        private readonly IReserveAdminHubServer reserveAdminHubServer;
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        public ApiController(ICommentAppService commentService,
            IReportItemAppService reportItemService,
            IBankCardAppService bankCardService,
            IUserAppService userService,
            IRegionAppService regionService,
            IDiscountTableAppService discountTableService,
            IPriceTableAppService priceTableService,
            IAdvertiseReportAppService advertiseReportService,
            IFileAppService fileService,
            ICategoryAppService categoryService,
            IAdvertiseAppService advertiseService,
            IChatAppService chatService,
            IReserveAppService reserveService,
            IExtrinsicReserveAppService extrinsicReserveService,
            IAccountingFacade accounting,
            IInstantReserveAutoCancelAppService instantReserveAutoCancelService,
            IUserContactFacade userContact,
            IReserveSupportManager reserveSupportManager,
            IReserveAutoCancelAppService reserveAutoCancelService,
            IWebHostEnvironment webHostEnvironment,
            IReserveDashboardHubServer reserveDashboardHubServer,
            IReserveAdminHubServer reserveAdminHubServer,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager,
            ILog logger)
        {
            this.accounting = accounting;
            this.reportItemService = reportItemService;
            this.commentService = commentService;
            this.userService = userService;
            this.bankCardService = bankCardService;
            this.regionService = regionService;
            this.discountTableService = discountTableService;
            this.priceTableService = priceTableService;
            this.advertiseReportService = advertiseReportService;
            this.fileService = fileService;
            this.categoryService = categoryService;
            this.advertiseService = advertiseService;
            this.chatService = chatService;
            this.reserveService = reserveService;
            this.extrinsicReserveService = extrinsicReserveService;
            this.instantReserveAutoCancelService = instantReserveAutoCancelService;
            this.userContact = userContact;
            this.reserveSupportManager = reserveSupportManager;
            this.reserveAutoCancelService = reserveAutoCancelService;
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.reserveDashboardHubServer = reserveDashboardHubServer;
            this.reserveAdminHubServer = reserveAdminHubServer;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        private bool ClientAuthenticate(string client_id)
        {
            return this.client_id == client_id;
        }

        public JsonResult CheckAndroidAppVersion(string cid, string version, int buildNumber)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                int AndroidApp_MinBuildNumber = 58;
                int AndroidApp_LastBuildNumber = 58;

                //var newFeatures = new string[] {
                //    "تسهیل ورود به حساب کاربری"
                //};
                var newFeatures = new string[] { };

                var forceUpdate = buildNumber < AndroidApp_MinBuildNumber;
                var updateSuggestion = buildNumber < AndroidApp_LastBuildNumber;
                return GenerateJsonResult(new
                {
                    done = true,
                    forceUpdate = forceUpdate,
                    updateSuggestion = updateSuggestion,
                    newFeatures = newFeatures
                    //,
                    //customBlock = true,
                    //customBlockTitle = "تست عنوان",
                    //customBlockDesc = "تست متن",
                    //customBlockUrls = new dynamic[] {
                    //    new {
                    //        id = 0,
                    //        title = "دانلود از گوگل پلی",
                    //        url = "https://cafebazaar.ir/app/com.amlakbashi.app"
                    //    },
                    //    new {
                    //        id = 1,
                    //        title = "دانلود از کافه بازار",
                    //        url = "https://cafebazaar.ir/app/com.amlakbashi.app"
                    //    }
                    //}
                });
            }
            catch (Exception exc)
            {
                logger.Error("CheckAndroidAppVersion", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    forceUpdate = false,
                    updateSuggestion = false
                });
            }
        }

        public JsonResult CheckIOSAppVersion(string cid, string version, int buildNumber)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                int IOSApp_MinBuildNumber = 9;
                int IOSApp_LastBuildNumber = 9;

                var newFeatures = new string[] {
                    "تسهیل ورود به حساب کاربری"
                };

                var forceUpdate = buildNumber < IOSApp_MinBuildNumber;
                var updateSuggestion = buildNumber < IOSApp_LastBuildNumber;
                return GenerateJsonResult(new
                {
                    done = true,
                    forceUpdate = forceUpdate,
                    updateSuggestion = updateSuggestion,
                    newFeatures = newFeatures
                });
            }
            catch (Exception exc)
            {
                logger.Error("CheckIOSAppVersion", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    forceUpdate = false,
                    updateSuggestion = false
                });
            }
        }

        private User GetUser()
        {
            var auth = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(auth) || auth == "null")
            {
                return new User();
            }
            auth = auth.Remove(0, 7);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(auth);
            var tokenS = jsonToken as JwtSecurityToken;
            var mainMobile = tokenS.Claims.First(claim => claim.Type == "name").Value;
            if (string.IsNullOrEmpty(mainMobile))
            {
                return new User();
            }
            return userService.GetByMainMobile(mainMobile);
        }

        private JsonResult GenerateJsonResult(dynamic obj)
        {
            var auth = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(auth) == false && auth != "null")
            {
                auth = auth.Remove(0, 7);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(auth);
                var tokenS = jsonToken as JwtSecurityToken;
                var mainMobile = tokenS.Claims.First(claim => claim.Type == "name").Value;
                var securityStamp = tokenS.Claims.FirstOrDefault(claim => claim.Type == "AspNet.Identity.SecurityStamp").Value;
                var identityUser = userService.GetIdentityUser(mainMobile);
                var objDict = new RouteValueDictionary(obj);
                var dict = new Dictionary<string, object>();
                foreach (var key in objDict.Keys)
                {
                    dict.Add(key, objDict[key]);
                }
                dict.Add("securityStampMismatch", identityUser.SecurityStamp != securityStamp);
                return new JsonResult(dict);
            }
            return new JsonResult(obj);
        }
    }
}

