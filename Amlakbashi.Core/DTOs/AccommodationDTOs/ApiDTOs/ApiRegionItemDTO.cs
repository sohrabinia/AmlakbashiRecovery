using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiRegionItemDTO
    {
        public int id { get; set; }
        public string persianName { get; set; }

        public static implicit operator ApiRegionItemDTO(Region region)
        {
            var dto = new ApiRegionItemDTO();
            dto.id = region.Id;
            dto.persianName = region.PersianName;
            return dto;
        }
    }
}
