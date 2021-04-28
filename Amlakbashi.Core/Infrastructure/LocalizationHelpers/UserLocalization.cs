using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class UserLocalization
    {
        public static string GetIdentityPasswordErrorString(string passwordErrorCode, string englishDescription)
        {
            switch (passwordErrorCode)
            {
                case "PasswordTooShort":
                    return "رمز عبور باید حداقل شامل 5 کاراکتر باشد";
                case "PasswordMismatch":
                    return "رمز عبور فعلی اشتباه است";
                case "PasswordRequiresDigit":
                    return "در رمز عبور باید حداقل از یک عدد استفاده کنید";
                case "PasswordRequiresLower":
                    return "در رمز عبور باید حداقل از یک حرف انگلیسی استفاده کنید";
                default:
                    return "اشکال در رمز عبور";
            }
            return englishDescription;
        }
    }
}
