using Amlakbashi.Core.Common.StaticData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Extensions
{
    public static class UserExtensions
    {
        public static bool IsImpersonatedUser(this ClaimsPrincipal userPrincipal)
        {
            var adminIsImpersonated = new HttpContextAccessor().HttpContext.Request.Cookies[ImpersonateData.ImpersonateCookieName];
            if (string.IsNullOrEmpty(adminIsImpersonated) == false && adminIsImpersonated == ImpersonateData.ImpersonateCookieValue)
            {
                return true;
            }
            return false;
        }

        public static string GetImpersonatedAdminUsername(this ClaimsPrincipal userPrincipal)
        {
            var claim = userPrincipal.FindFirst("ImpersonateAdminUsername");
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }

        public static string GetImpersonateExpireTime(this ClaimsPrincipal userPrincipal)
        {
            var claim = userPrincipal.FindFirst("ImpersonateExpireTime");
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }

        public static string GetGuid(this ClaimsPrincipal userPrincipal)
        {
            return userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetRefreshToken(this ClaimsPrincipal userPrincipal)
        {
            return userPrincipal?.FindFirst("refreshToken")?.Value;
        }
    }
}
