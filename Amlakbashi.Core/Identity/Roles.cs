using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Identity
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string TechnicalManager = "TechnicalManager";
        public const string TechnicalEmployee = "TechnicalEmployee";
        public const string ReserveManager = "ReserveManager";
        public const string ReserveSenior = "ReserveSenior";
        public const string ReserveJunior = "ReserveJunior";
        public const string AdvertiseManager = "AdvertiseManager";
        public const string AdvertiseSenior = "AdvertiseSenior";
        public const string AdvertiseJunior = "AdvertiseJunior";
        public const string ContentManager = "ContentManager";
        public const string ContentSenior = "ContentSenior";
        public const string CommunicationManager = "CommunicationManager";
        public const string CommunicationSenior = "CommunicationSenior";
        public const string FinanceManager = "FinanceManager";
        public const string FinanceSenior = "FinanceSenior";
        public const string FinanceJunior = "FinanceJunior";
        public const string UserManager = "UserManager";
        public const string UserSenior = "UserSenior";
        public const string UserJunior = "UserJunior";

        public static string[] AllEmployeeRoles =
            new string[] {
                    SuperAdmin,
                    Admin,
                    TechnicalManager,
                    TechnicalEmployee,
                    ReserveManager,
                    ReserveSenior,
                    ReserveJunior,
                    AdvertiseManager,
                    AdvertiseSenior,
                    AdvertiseJunior,
                    ContentManager,
                    ContentSenior,
                    CommunicationManager,
                    CommunicationSenior,
                    FinanceManager,
                    FinanceSenior,
                    FinanceJunior,
                    UserManager,
                    UserSenior,
                    UserJunior
                };

        public static string[] SupportRoles =
            new string[]
            {
                ReserveManager,
                ReserveSenior,
                ReserveJunior,
                AdvertiseJunior,
                UserJunior
            };
        public static Dictionary<string, string> InitialUserRoles
        {
            get
            {
#if DEBUG
                return new Dictionary<string, string>() {
                    { "+98 9121197156", SuperAdmin },
                    { "+98 9191613134", SuperAdmin },
                    { "+98 9356172126", SuperAdmin },
                    { "+98 9212085439", Admin },
                    { "+98 9107447535", Admin }
                };
#else
                return new Dictionary<string, string>() {
                    { "+98 9121197156", SuperAdmin },
                    { "+98 9191613134", SuperAdmin }
                };
#endif
            }
        }
    }
}
