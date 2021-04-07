using Amlakbashi.Core.Identity;
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
            foreach (var item in PolicyData.AllPolicies)
            {
                options.AddPolicy(item.Key, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(item.Value);
                });
            }
        }
    }
}
