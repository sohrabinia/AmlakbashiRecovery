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
            var value = userPrincipal.FindFirst("Impersonate");
            if (value != null && value.Value == "true")
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
    }
}
