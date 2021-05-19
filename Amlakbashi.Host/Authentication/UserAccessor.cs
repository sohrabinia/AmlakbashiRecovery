using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Http;
using Amlakbashi.Host.Extensions;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Common.StaticData;

namespace Amlakbashi.Host.Authentication
{
    public class UserAccessor : IUserAccessor
    {
        public User CurrentUser { get; }
        public User DoerUser { get; }
        public UserAccessor(IHttpContextAccessor httpContextAccessor,
            IUserAppService userService)
        {
            var context = httpContextAccessor.HttpContext;
            if (context.User.IsImpersonatedUser() == true)
            {
                var adminIsImpersonated = context.Request.Cookies[ImpersonateData.ImpersonateCookieName];
                if (string.IsNullOrEmpty(adminIsImpersonated) == false && adminIsImpersonated == ImpersonateData.ImpersonateCookieValue)
                {
                    CurrentUser = userService.GetActivatedUserByMainMobile(context.User.Identity.Name);
                    DoerUser = userService.GetActivatedUserByMainMobile(context.User.GetImpersonatedAdminUsername());
                    return;
                }
            }

            if (context.Items["userinfo"] != null)
            {
                CurrentUser = DoerUser = context.Items["userinfo"] as User;
                return;
            }
            if (string.IsNullOrEmpty(context.User.Identity.Name))
            {
                CurrentUser = DoerUser = new User();
                return;
            }
            User user;
            if (PhoneUtility.ValidateInternationalNumber(context.User.Identity.Name))
            {
                //var international_mobile = PhoneUtility.LocalNumberToInternational(
                //        context.User.Identity.Name, 98);
                user = userService.GetActivatedUserByMainMobile(context.User.Identity.Name);
            }
            else
            {
                user = userService.GetActivatedUserByEmail(context.User.Identity.Name);
            }
            if (user == null)
            {
                user = CurrentUser = DoerUser = new User();
            }
            else
            {
                CurrentUser = DoerUser = user;
            }
            context.Items.Add("userinfo", user);
        }
    }
}
