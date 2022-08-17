using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Regions
{
    public class RegionListDTO
    {
        public int regionId { get; set; }
        public string name { get; set; }
        public IList<RegionListDTO> subRegions { get; set; } = new List<RegionListDTO>();
    }
}
