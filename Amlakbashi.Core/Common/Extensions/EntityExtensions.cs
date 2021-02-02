using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Extensions
{
    public static class EntityExtensions
    {
        public static IQueryable<T> WhereArchived<T>(
            this IQueryable<T> src)
            where T : IArchiveEntity
        {
            if (typeof(T) is IRecycleEntity)
            {
                return src.Where(w => (w as IRecycleEntity).IsRecycled == false &&
                    w.IsArchived == true);
            }
            else
            {
                return src.Where(w => w.IsArchived == true);
            }
        }
        public static IQueryable<T> WhereRecycled<T>(
            this IQueryable<T> src) where T : IRecycleEntity
        {
            return src.Where(w => w.IsRecycled == true);
        }
        public static IQueryable<T> WhereWorking<T>(
            this IQueryable<T> src)
        {
            if (typeof(T) is IRecycleEntity &&
                typeof(T) is IArchiveEntity)
            {
                return src.Where(w =>
                    (w as IRecycleEntity).IsRecycled == false &&
                    (w as IArchiveEntity).IsArchived == false);
            }
            else if (typeof(T) is IArchiveEntity)
            {
                return src.Where(w => (w as IArchiveEntity).IsArchived == false);
            }
            else if (typeof(T) is IRecycleEntity)
            {
                return src.Where(w => (w as IRecycleEntity).IsRecycled == false);
            }
            else
            {
                return src;
            }
        }
    }
}
