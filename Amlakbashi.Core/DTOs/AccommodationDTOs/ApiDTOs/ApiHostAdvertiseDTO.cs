using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiHostAdvertiseDTO
    {
        public long id { get; set; }
        public int status { get; set; }
        public string statusTitle { get; set; }
        public string statusColor { get; set; }
        public string title { get; set; }
        public long image { get; set; }
        public bool todayEmpty { get; set; }
        public bool hasComment { get; set; }
        public int newCommentCount { get; set; }
        public int adType { get; set; }
        public bool isHotel { get; set; }
        public bool isComplex { get; set; }
        public List<ApiComplexChildDTO> apartmentChildren { get; set; }
        public List<ApiComplexChildDTO> suitChildren { get; set; }
        public List<ApiComplexChildDTO> villaChildren { get; set; }
        public List<ApiComplexChildDTO> houseChildren { get; set; }
        public List<ApiComplexChildDTO> hutChildren { get; set; }
        public List<ApiHotelChildDTO> hotelChildren { get; set; }
        public List<string> notVerifyReasons { get; set; }
        public string hotelUnitTitle { get; set; }
        public InstantReserveDetailDTO instantReserveDetail { get; set; }
        public StayDurationDTO stayDuration { get; set; }
        public int maxInstantReserveStart { get; set; }
        public long norouzPrice { get; set; }
        public string norouzPriceString { get; set; }
        public string norouzMinReserveDateString { get; set; }
        public int norouzOverCapacityPrice { get; set; }
    }
}
