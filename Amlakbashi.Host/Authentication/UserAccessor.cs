using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Http;
using Amlakbashi.Host.Extensions;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;

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
            if (context.Session.GetString("impersonateUser") != null)
            {
                CurrentUser = context.Session.GetObjectFromJson<User>("impersonateUser");
                DoerUser = context.Session.GetObjectFromJson<User>("impersonateAdmin");
                return;
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
            if (PhoneUtility.ValidateLocalNumber(context.User.Identity.Name))
            {
                var international_mobile = PhoneUtility.LocalNumberToInternational(
                        context.User.Identity.Name, 98);
                user = userService.GetActivatedUserByMainMobile(international_mobile);
            }
            else
            {
                user = userService.GetActivatedUserByEmail(context.User.Identity.Name);
            }
            if (user == null)
            {
                CurrentUser = DoerUser = new User();
            }
            context.Items.Add("userinfo", user);
            CurrentUser = DoerUser = user;
        }
    }
}
