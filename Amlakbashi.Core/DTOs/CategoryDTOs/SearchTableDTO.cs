using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;

namespace Amlakbashi.Core.DTOs.CategoryDTOs
{
    public class SearchTableDTO
    {
        public long categoryId { get; set; }
        public string regionString { get; set; }
        public string typeString { get; set; }
        public string title { get; set; }
        public string areaString { get; set; }
        public string cityString { get; set; }
        public int countAdvertise { get; set; }
        public string link { get; set; }

        public static SearchTableDTO GenerateForApp(Region region, string cityName)
        {
            var dto = new SearchTableDTO();
            dto.regionString = region.PersianName;
            dto.title = region.Type == 0 ? "استان " + region.PersianName : region.PersianName;
            dto.cityString = cityName;
            dto.countAdvertise = region.CountAdvertise;
            dto.link = $"/app/category/item?regionid={region.Id}";
            return dto;
        }

        public static SearchTableDTO Generate(Region region, string cityName)
        {
            var dto = new SearchTableDTO();
            dto.regionString = region.PersianName;
            dto.title = region.Type == 0 ? "استان " + region.PersianName : region.PersianName;
            dto.cityString = cityName;
            dto.countAdvertise = region.CountAdvertise;
            dto.link = RegionLocalization.GetLocationUrl(region);
            return dto;
        }
    }
}
