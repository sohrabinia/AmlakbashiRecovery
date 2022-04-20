using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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

        public static User.UserGeneralTypeEnum GetUserPanelType(this ClaimsPrincipal userPrincipal)
        {
            var type = userPrincipal?.FindFirst("panel")?.Value;
            if (type == "host")
            {
                return User.UserGeneralTypeEnum.Host;
            }
            else
            {
                return User.UserGeneralTypeEnum.Guest;
            }
        }
    }
}
