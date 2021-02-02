using Amlakbashi.Core.Entities;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs
{
    [Serializable]
    public class DashboardHotelRoomDTO : BaseDashboardAccDTO
    {
        public string roomUnitString { get; set; }
        public bool anyComment { get; set; }
        public int newCommentCount { get; set; }
    }
}
