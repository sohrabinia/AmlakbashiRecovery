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
            var dto = new DiscountDTO();
            dto.id = discountTable.Id;
            dto.percent = discountTable.Percent;
            var fromString = DateTimeUtility.GregorianToPersianDate(discountTable.From).Replace(",", "/");
            var toString = DateTimeUtility.GregorianToPersianDate(discountTable.To).Replace(",", "/");
            dto.dateString = fromString + " تا " + toString;
            return dto;
        }
    }
}
