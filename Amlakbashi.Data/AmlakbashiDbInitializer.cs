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
            //foreach (var item in context.Files)
            //{
            //    if (item.FilePath is null)
            //    {
            //        item.Type = Core.Entities.File.FileTypeEnum.Unset;
            //    }
            //    else if (item.FilePath.Contains("content/users/"))
            //    {
            //        item.Type = Core.Entities.File.FileTypeEnum.UserImage;
            //    }
            //    else if (item.FilePath.Contains("content/licenses/"))
            //    {
            //        item.Type = Core.Entities.File.FileTypeEnum.ResidenceLicense;
            //    }
            //    else if (item.FilePath.Contains("content/advertise/"))
            //    {
            //        item.Type = Core.Entities.File.FileTypeEnum.ResidenceImage;
            //    }
            //    else if (item.FilePath.Contains("content/blogpost/"))
            //    {
            //        item.Type = Core.Entities.File.FileTypeEnum.BlogPostImage;
            //    }
            //    else
            //    {
            //        item.Type = Core.Entities.File.FileTypeEnum.Unset;
            //    }
            //}
            //context.UpdateRange(context.Files);
            //context.SaveChanges();
        }
    }
}
