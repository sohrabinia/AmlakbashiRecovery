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
            var instantReserveList = context.Advertises.Where(x => (int)x.InstantReserveStatus == 1);
            foreach (var item in instantReserveList)
            {
                item.InstantReserveStatus = Core.Entities.Advertise.InstantReserveStatusEnum.Calendar;
            }

            instantReserveList = context.Advertises.Where(x => (int)x.InstantReserveStatus == 2);
            foreach (var item in instantReserveList)
            {
                if (item.Status == Core.Entities.Advertise.AdvertiseStatus.Published)
                {
                    item.InstantReserveStatus = Core.Entities.Advertise.InstantReserveStatusEnum.Permanent;
                }
                else
                {
                    item.InstantReserveStatus = Core.Entities.Advertise.InstantReserveStatusEnum.Calendar;
                }
            }

            context.SaveChanges();
        }
    }
}
