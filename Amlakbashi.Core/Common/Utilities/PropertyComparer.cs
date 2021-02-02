using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class PropertyComparer<TFrom, TTo> where TFrom : class
                                                where TTo : class
    {
        public static bool Compare(TFrom from, TTo to)
        {
            if (from == null || to == null)
            {
                return false;
            }
            var sourceProperties = from.GetType().GetProperties();
            var targetProperties = to.GetType().GetProperties();
            foreach (var parentProperty in sourceProperties)
            {
                foreach (var childProperty in targetProperties)
                {
                    if (parentProperty.Name == childProperty.Name &&
                        parentProperty.PropertyType == childProperty.PropertyType &&
                        parentProperty.GetValue(from) == childProperty.GetValue(to))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
