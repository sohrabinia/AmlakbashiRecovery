using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiRegionDTO
    {
        public int id { get; set; }
        public string persianName { get; set; }
        public List<ApiRegionDTO> children { get; set; }
    }
}
