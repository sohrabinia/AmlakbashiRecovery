using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiPositionDTO
    {
        public static implicit operator ApiPositionDTO(Advertise advertise)
        {
            var dto = new ApiPositionDTO();
            dto.id = advertise.Id;
            dto.position = advertise.Position;
            dto.address = advertise.Address;
            dto.province = advertise.Province == null ? 0 : (int)advertise.Province;
            dto.city = advertise.City == null ? 0 : (int)advertise.City;
            dto.area = advertise.Area == null ? 0 : (int)advertise.Area;
            dto.latitude = advertise.Latitude;
            dto.longitude = advertise.Longitude;
            return dto;
        }

        public long id { get; set; }
        public PositionType position { get; set; }
        public int province { get; set; }
        public int city { get; set; }
        public int area { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public List<SelectItem> positionSelectItem { get; set; }
    }
}
