using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class UserLocalization
    {
        public static string GetIdentityPasswordErrorString(string passwordErrorCode, string englishDescription)
        {
            //TODO: Write persian localization for each error code
            switch (passwordErrorCode)
            {
                case "PasswordTooShort":
                    return "رمز عبور باید حداقل شامل 5 کاراکتر باشد";
                case "PasswordMismatch":
                    return "رمز عبور فعلی اشتباه است";
                default:
                    return "اشکال در رمز عبور";
            }
            return englishDescription;
        }
    }
}
