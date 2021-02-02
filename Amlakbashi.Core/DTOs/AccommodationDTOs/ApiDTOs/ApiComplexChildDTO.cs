using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiComplexChildDTO
    {
        public long id { get; set; }
        public int number { get; set; }
        public string title { get; set; }
        public bool available { get; set; }
        public long image { get; set; }
        public int floor { get; set; }
        public int minCapacity { get; set; }
        public int maxCapacity { get; set; }
        public bool todayEmpty { get; set; }
        public int norouzPrice { get; set; }
        public int norouzOverCapacityPrice { get; set; }

        public static implicit operator ApiComplexChildDTO(Advertise advertise)
        {
            var dto = new ApiComplexChildDTO();
            dto.id = advertise.Id;
            dto.available = advertise.Available;
            dto.todayEmpty = advertise.TodayIsEmpty;
            dto.floor = (int)advertise.Floor;
            dto.image = advertise.PhotoID == null ? 0 : (int)advertise.PhotoID;
            dto.title = advertise.Title;
            dto.minCapacity = advertise.Capacity;
            dto.maxCapacity = advertise.Capacity + advertise.MoreThanCapacity;
            //norouzPrice = advertise.NorouzPrice;
            //norouzOverCapacityPrice = advertise.NorouzOverCapacityPric;
            dto.norouzPrice = 0;
            dto.norouzOverCapacityPrice = 0;
            return dto;
        }
    }
}
