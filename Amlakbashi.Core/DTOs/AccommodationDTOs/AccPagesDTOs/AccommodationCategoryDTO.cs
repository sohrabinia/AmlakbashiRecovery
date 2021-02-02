using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class AccommodationCategoryDTO
    {
        public bool CountryDirection { get; set; }
        public bool Province { get; set; }
        public bool City { get; set; }
        public int? CityMostAccType { get; set; }
        public int? CityCountAdvertise { get; set; }
        public bool Area { get; set; }
        public string CountryDirectionName { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string CountryDirectionUrl { get; set; }
        public string ProvinceUrl { get; set; }
        public string CityUrl { get; set; }
        public string AreaUrl { get; set; }
    }
}
