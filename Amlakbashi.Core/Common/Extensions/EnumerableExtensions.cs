using Amlakbashi.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> list, int page, int pageItemCount)
        {
            page = page > 0 ? page : 1;
            pageItemCount = pageItemCount > 0 ? pageItemCount : 20;
            return new PagedList<T>()
            {
                PagingInfo = new PagingInfo(list.Count(), page, pageItemCount),
                List = list.Skip((page - 1) * pageItemCount).Take(pageItemCount).ToList()
            };
        }

        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> list, int page, int pageItemCount)
        {
            page = page > 0 ? page : 1;
            pageItemCount = pageItemCount > 0 ? pageItemCount : 20;
            return new PagedList<T>()
            {
                PagingInfo = new PagingInfo(list.Count(), page, pageItemCount),
                List = await list.Skip((page - 1) * pageItemCount).Take(pageItemCount).ToListAsync()
            };
        }
    }
}
