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
            "09191613134",
            "09356172126",
            "09102600350",
            "09121197156",
            "09360263804",
            "09198155019",
            "09216813826",
            "09354086894",
            "09196218216",
            "09912097905",
            "09052932348",
            "09199075074",
            "09365966647",
            "09107447535"
        };

        public static List<string> MasterAdminMobiles = new List<string>()
        {
            "09191613134", // رسول شاه حسینی
            "09356172126", // رضا نجمی
            "09102600350", // مهدیه دین پژوه
            "09121197156", // آقای سهرابی نیا
            "09198155019", // درسا سهرابی نیا
            "09199075074", // مهسا سهرابی نیا
            "09216813826" // لیلا فرهنگیان
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