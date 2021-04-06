using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Extensions;

namespace Amlakbashi.Host.Authentication
{
    public static class TempRoles
    {
        public static List<string> AdminMobiles = new List<string>()
        {
            "+98 9191613134",
            "+98 9356172126",
            "+98 9102600350",
            "+98 9121197156",
            "+98 9360263804",
            "+98 9198155019",
            "+98 9216813826",
            "+98 9354086894",
            "+98 9196218216",
            "+98 9052932348",
            "+98 9199075074",
            "+98 9365966647",
            "+98 9107447535"
        };

        public static List<string> MasterAdminMobiles = new List<string>()
        {
            "+98 9191613134", // رسول شاه حسینی
            "+98 9356172126", // رضا نجمی
            "+98 9102600350", // مهدیه دین پژوه
            "+98 9121197156", // آقای سهرابی نیا
            "+98 9198155019", // درسا سهرابی نیا
            "+98 9199075074", // مهسا سهرابی نیا
            "+98 9216813826" // لیلا فرهنگیان
        };
    }

    public class Auth : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly UserRoles[] allowedRoles;

        public Auth(params UserRoles[] roles)
        {
            this.allowedRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(user))
            {
                context.Result = GenerateUnathorizedResult(context);
            }

            foreach (var role in allowedRoles)
            {
                if (context.HttpContext.Session.Get("impersonateUser") != null)
                {
                    context.Result = GenerateUnathorizedResult(context);
                }
                switch (role)
                {
                    case UserRoles.Admin:
                        if (TempRoles.AdminMobiles.Contains(user) == false)
                        {
                            context.Result = GenerateUnathorizedResult(context);
                        }
                        break;
                    case UserRoles.MasterAdmin:
                        if (TempRoles.MasterAdminMobiles.Contains(user) == false)
                        {
                            context.Result = GenerateUnathorizedResult(context);
                        }
                        break;
                }
            }
        }

        private IActionResult GenerateUnathorizedResult(AuthorizationFilterContext context)
        {
            var isAjax = context.HttpContext.Request
                .Headers["X-Requested-With"] == "XMLHttpRequest";//TODO: Check if works correctly
            if (context.HttpContext.Request.Path.Value.StartsWith("/api/") || isAjax)
            {
                return new JsonResult(new { status = 0, msg = "Access Denied" });
            }
            else
            {
                return new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "AccessDenied",
                    controller = "Errors",
                    originUrl = UriHelper.GetDisplayUrl(context.HttpContext.Request)
                }));
            }
        }
    }

    public enum UserRoles
    {
        User = 0,
        Admin = 1,
        MasterAdmin = 2
    }
}