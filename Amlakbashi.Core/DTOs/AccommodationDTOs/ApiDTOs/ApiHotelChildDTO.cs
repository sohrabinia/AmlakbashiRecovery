using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiHotelChildDTO
    {
        public long id { get; set; }
        public int number { get; set; }
        public string title { get; set; }
        public bool available { get; set; }
        public int count { get; set; }
        public int status { get; set; }
        public bool todayEmpty { get; set; }
        public long norouzPrice { get; set; }
        public int norouzOverCapacityPrice { get; set; }

        public static implicit operator ApiHotelChildDTO(Advertise advertise)
        {
            var dto = new ApiHotelChildDTO();
            dto.id = advertise.Id;
            //dto.number = number;
            dto.title = advertise.Title;
            dto.count = advertise.Count;
            dto.available = advertise.Available;
            dto.todayEmpty = advertise.TodayIsEmpty;
            dto.status = (int)advertise.Status;
            //norouzPrice = advertise.NorouzPrice;
            //norouzOverCapacityPrice = advertise.NorouzOverCapacityPric;
            dto.norouzPrice = 0;
            dto.norouzOverCapacityPrice = 0;
            return dto;
        }

        //public static List<HotelChild> GenerateFromAdvertise(IEnumerable<Advertise> advertises)
        //{
        //    var output = new List<HotelChild>();
        //    int number = 0;
        //    foreach (var advertise in advertises)
        //    {
        //        number++;
        //        output.Add(new HotelChild()
        //        {
        //            id = advertise.Id,
        //            number = number,
        //            title = advertise.Title,
        //            count = advertise.Count,
        //            available = advertise.Available,
        //            todayEmpty = advertise.TodayIsEmpty,
        //            status = (int)advertise.Status,
        //            //norouzPrice = advertise.NorouzPrice,
        //            //norouzOverCapacityPrice = advertise.NorouzOverCapacityPrice
        //            norouzPrice = 0,
        //            norouzOverCapacityPrice = 0
        //        });
        //    }

        //    return output;
        //}
    }
}
