using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Utilities
{
    public class PropertyCopier<TFrom, TTo> where TFrom : class
                                                where TTo : class
    {
        public static void Copy(TFrom from, TTo to)
        {
            if (from == null || to == null)
            {
                return;
            }

            var sourceProperties = from.GetType().GetProperties();
            var targetProperties = to.GetType().GetProperties();

            foreach (var parentProperty in sourceProperties)
            {
                foreach (var childProperty in targetProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        childProperty.SetValue(to, parentProperty.GetValue(from));
                        break;
                    }
                }
            }
        }

        public static void CopyWithoutCheckType(TFrom from, TTo to)
        {
            if (from == null || to == null)
            {
                return;
            }

            var sourceProperties = from.GetType().GetProperties();
            var targetProperties = to.GetType().GetProperties();

            foreach (var parentProperty in sourceProperties)
            {
                foreach (var childProperty in targetProperties)
                {
                    if (parentProperty.Name == childProperty.Name)
                    {
                        if (parentProperty.GetValue(from) != null)
                        {
                            childProperty.SetValue(to, parentProperty.GetValue(from));
                            break;
                        }
                    }
                }
            }
        }

        public static void CopyInsensetive(TFrom from, TTo to)
        {
            var sourceProperties = from.GetType().GetProperties();
            var targetProperties = to.GetType().GetProperties();

            foreach (var parentProperty in sourceProperties)
            {
                foreach (var childProperty in targetProperties)
                {
                    //var propertiesAreTheSame = parentProperty.Name.ToLower() == childProperty.Name.ToLower();
                    //if (propertiesAreTheSame)
                    //{
                    //    propertiesAreTheSame = parentProperty.PropertyType == childProperty.PropertyType;
                    //    propertiesAreTheSame = propertiesAreTheSame ||
                    //        Nullable.GetUnderlyingType(parentProperty.PropertyType) == childProperty.PropertyType ||
                    //        Nullable.GetUnderlyingType(childProperty.PropertyType) == parentProperty.PropertyType;
                    //}
                    //if (propertiesAreTheSame)
                    //{
                    //    childProperty.SetValue(to, parentProperty.GetValue(from));
                    //    break;
                    //}

                    if (parentProperty.Name.ToLower() == childProperty.Name.ToLower() &&
                        (parentProperty.PropertyType == childProperty.PropertyType ||
                        Nullable.GetUnderlyingType(parentProperty.PropertyType) == childProperty.PropertyType ||
                        Nullable.GetUnderlyingType(childProperty.PropertyType) == parentProperty.PropertyType))
                    {
                        childProperty.SetValue(to, parentProperty.GetValue(from));
                        break;
                    }
                }
            }
        }
    }
}
