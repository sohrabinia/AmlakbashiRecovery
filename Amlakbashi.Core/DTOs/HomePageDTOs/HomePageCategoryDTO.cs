using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.HomePageDTOs
{
    public class HomePageCategoryDTO
    {
        public int CategoryID;
        public int CountAdvertise;
        public string Title;
        public string URL;
        public List<HomePageAdvertiseDTO> Advertises;
        public DynamicCategory category;
        public string categoryUrl;
        public string categoryH1Title;
        public int RegionId { get; set; }
        public List<AccommodationCardDTO> AdvertiseItems { get; set; }
    }
}
