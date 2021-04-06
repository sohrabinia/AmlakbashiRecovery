using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Configurations
{
    public class PolicyConfig
    {
        public static void Config(AuthorizationOptions options)
        {
            options.AddPolicy("AllAdmins", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin", "SuperAdmin");
            });
            options.AddPolicy("SuperAdmins", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("SuperAdmin");
            });
        }
    }
}
