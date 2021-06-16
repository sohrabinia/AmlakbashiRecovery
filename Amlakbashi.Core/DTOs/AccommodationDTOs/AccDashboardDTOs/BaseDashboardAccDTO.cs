using System.Collections.Generic;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs
{
    public abstract class BaseDashboardAccDTO
    {
        public long id { get; set; }
        public string title { get; set; }
        public bool isVerified { get; set; }
        public bool isActivated { get; set; }
        public string photoUrl { get; set; }
        public List<DiscountDTO> discounts { get; set; }
        public List<long> unavailableDates { get; set; }
        public List<long> extrinsicDates { get; set; }
        public bool todayIsEmpty { get; set; }
        public bool todayIsFull { get; set; }
    }
}
