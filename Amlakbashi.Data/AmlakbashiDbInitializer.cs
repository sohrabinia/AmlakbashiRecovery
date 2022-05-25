using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Data
{
    public static class AmlakbashiDbInitializer
    {
        public static void Initialize(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<AmlakbashiDB>();
            if (context.Database.GetPendingMigrations().Any() == false)
            {
                return;
            }
            context.Database.Migrate();
            SeedData(context);
        }

        private static void SeedData(AmlakbashiDB context)
        {
            var users = context.Users.Where(x => string.IsNullOrEmpty(x.PhoneNumber2) == false);
            foreach (var item in users)
            {
                item.NoticesPhoneNumber = Core.Entities.User.NoticesPhoneNumberEnum.PhoneNumber2;
                context.Update(item);
            }
            context.SaveChanges();
        }
    }
}
