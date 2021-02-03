using Amlakbashi.Core.Common.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Amlakbashi.Core.Common.Extensions
{
    public static class SoftDeleteQueryExtension
    {
        public static void AddSoftDeleteQueryFilter(
        this IMutableEntityType entityData)
        {
            var methodToCall = typeof(SoftDeleteQueryExtension)
                .GetMethod(nameof(GetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(entityData.ClrType);
            var filter = methodToCall.Invoke(null, new object[] { });
            entityData.SetQueryFilter((LambdaExpression)filter);
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
            return filter;
        }
    }
}
