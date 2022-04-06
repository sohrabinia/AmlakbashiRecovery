using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Regions
{
    public class RegionListResponse
    {
        public int regionId { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public int residencyCount { get; set; }
    }
}
