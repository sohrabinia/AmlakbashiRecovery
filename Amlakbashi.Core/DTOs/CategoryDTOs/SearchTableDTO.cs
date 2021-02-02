using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;

namespace Amlakbashi.Core.DTOs.CategoryDTOs
{
    public class SearchTableDTO
    {
        public long categoryId { get; set; }
        public string regionString { get; set; }
        public string typeString { get; set; }
        public string areaString { get; set; }
        public string cityString { get; set; }
        public int countAdvertise { get; set; }
        public string link { get; set; }

        public static SearchTableDTO Generate(DynamicCategory category, string cityName)
        {
            var dto = new SearchTableDTO();
            dto.categoryId = category.Id;
            dto.regionString = category.RegionString;
            dto.typeString = category.TypeString;
            dto.areaString = category.AreaStr;
            dto.cityString = cityName;
            dto.countAdvertise = category.CountAdvertise;
            dto.link = CategoryUrlLocalization.CategoryToUrl(category);
            return dto;
        }
    }
}
