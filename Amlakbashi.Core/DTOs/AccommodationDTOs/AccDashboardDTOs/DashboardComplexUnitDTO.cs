using System;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs
{
    [Serializable]
    public class DashboardComplexUnitDTO  : BaseDashboardAccDTO
    {
        public FloorItems floor { get; set; }
        public string floorString { get; set; }
        public int roomCount { get; set; }
        public string previewUrl { get; set; }
        public bool anyComment { get; set; }
        public int newCommentCount { get; set; }
        public float userRating { get; set; }
    }
}
