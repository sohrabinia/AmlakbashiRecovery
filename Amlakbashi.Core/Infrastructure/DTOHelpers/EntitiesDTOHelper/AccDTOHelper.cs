using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.DTOHelpers.EntitiesDTOHelper
{
    public static class AccDTOHelper
    {
        public static List<DTOSelectItem> GenerateAccSelectList<T>() where T : Enum
        {
            var arr = (Enum.GetValues(typeof(T))
                as T[]).OrderBy(x => (int)Enum.Parse(typeof(T), x.ToString())).ToArray();
            var result = new List<DTOSelectItem>();
            foreach (var item in arr)
            {
                result.Add(new DTOSelectItem((int)Enum.Parse(typeof(T), item.ToString()),
                    AdvertiseMainLocalization.GetEnumPersianName(item)));
            }
            return result;
        }

        public static DTOCheckbox GenerateAccCheckbox(Property property, bool? value)
        {
            return new DTOCheckbox()
            {
                name = property.ToString(),
                value = value,
                title = AdvertiseMainLocalization.GetPropertyTitle(property)
            };
        }
    }
}
