using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Amlakbashi.Data.Identity
{
    public static class IdentityDbInitializer
    {
        public static void Initialize(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<IdentityDB>();
            if (context.Database.GetPendingMigrations().Any() == false)
            {
                return;
            }

            context.Database.Migrate();
        }
    }
}
