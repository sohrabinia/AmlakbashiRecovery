using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity;

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

        public static string GetRolePersianTitle(string roleTitle)
        {
            switch (roleTitle)
            {
                case Roles.AdvertiseSenior:
                    return "کارشناس ارشد آگهی";
                case Roles.Admin:
                    return "مدیر";
                case Roles.ReserveManager:
                    return "مدیر رزرو";
                case Roles.FinanceJunior:
                    return "کارشناس مالی";
                case Roles.TechnicalManager:
                    return "مدیر فنی";
                case Roles.AdvertiseJunior:
                    return "کارشناس آگهی";
                case Roles.ContentManager:
                    return "مدیر محتوا";
                case Roles.CommunicationSenior:
                    return "کارشناس ارشد ارتباطات";
                case Roles.UserSenior:
                    return "کارشناس ارشد کاربران";
                case Roles.UserJunior:
                    return "کارشناس کاربران";
                case Roles.ReserveSenior:
                    return "کارشناس ارشد رزرو";
                case Roles.ContentSenior:
                    return "کارشناس ارشد محتوا";
                case Roles.UserManager:
                    return "مدیر کاربران";
                case Roles.ReserveJunior:
                    return "کارشناس رزرو";
                case Roles.CommunicationManager:
                    return "مدیر ارتباطات";
                case Roles.FinanceSenior:
                    return "کارشناس ارشد مالی";
                case Roles.FinanceManager:
                    return "مدیر مالی";
                case Roles.TechnicalEmployee:
                    return "برنامه نویس";
                case Roles.AdvertiseManager:
                    return "مدیر آگهی";
                case Roles.SuperAdmin:
                    return "مدیر کل";
                default:
                    return "پیش فرض";
            }
        }
    }
}
