using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs
{
    [Serializable]
    public class DashboardAccDTO : BaseDashboardAccDTO
    {
        public AdvertiseStatus status { get; set; }
        public string statusString { get; set; }
        public string statusColor { get; set; }
        public string previewUrl { get; set; }
        public string editUrl { get; set; }
        public string capacityString { get; set; }
        public int minReserveDays { get; set; }
        public int maxReserveDays { get; set; }
        public bool anyComment { get; set; }
        public int newCommentCount { get; set; }
        public float userRating { get; set; }
        public InstantReserveStatusEnum instantReserveStatus { get; set; }
        public int maxInstantReserveStart { get; set; }
        public AccMainType mainType { get; set; }
        public string typeString { get; set; }
        public string locationString { get; set; }
        public List<string> notVerifyReasons { get; set; }
        public List<DashboardHotelRoomDTO> hotelRooms { get; set; }
        public List<DashboardComplexUnitDTO> apartmentUnits { get; set; }
        public List<DashboardComplexUnitDTO> suitUnits { get; set; }
        public List<DashboardComplexUnitDTO> villaUnits { get; set; }
        public List<DashboardComplexUnitDTO> houseUnits { get; set; }
        public List<DashboardComplexUnitDTO> hutUnits { get; set; }
    }
}
