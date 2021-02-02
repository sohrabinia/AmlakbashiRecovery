using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs
{
    [Serializable]
    public class DiscountDTO
    {
        public long id { get; set; }
        public string dateString { get; set; }
        public int percent { get; set; }

        public static implicit operator DiscountDTO(DiscountTable discountTable)
        {
            var result = new DiscountDTO();
            result.id = discountTable.Id;
            result.percent = discountTable.Percent;
            var fromString = DateTimeUtility.GregorianToPersianDate(discountTable.From).Replace(",", "/");
            var toString = DateTimeUtility.GregorianToPersianDate(discountTable.To).Replace(",", "/");
            result.dateString = fromString + " تا " + toString;
            return result;
        }
    }
}
