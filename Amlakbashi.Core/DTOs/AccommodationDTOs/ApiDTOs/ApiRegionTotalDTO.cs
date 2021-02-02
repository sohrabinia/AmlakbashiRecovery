using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiRegionTotalDTO
    {
        public List<ApiRegionDTO> provinces { get; set; }

        public static ApiRegionTotalDTO Generate (IQueryable<Region> allRegions)
        {
            var result = new ApiRegionTotalDTO();
            IQueryable<Region> allProvinces = allRegions.
                Where(x => x.Type == (int)AdvertiseRegion.Province);
            IQueryable<Region> all_cities = allRegions.
                Where(x => x.Type == (int)AdvertiseRegion.City);
            IQueryable<Region> all_areas = allRegions.
                Where(x => x.Type == (int)AdvertiseRegion.Area);
            result.provinces = new List<ApiRegionDTO>();
            IQueryable<Region> data_city_children;
            IQueryable<Region> data_area_children;
            List<ApiRegionDTO> city_children;
            List<ApiRegionDTO> area_children;
            foreach (var province in allProvinces)
            {
                data_city_children = all_cities.Where(x => x.ParentID == province.Id);
                city_children = new List<ApiRegionDTO>();
                foreach (var city in data_city_children)
                {
                    data_area_children = all_areas.Where(x => x.ParentID == city.Id);
                    area_children = new List<ApiRegionDTO>();
                    foreach (var area in data_area_children)
                    {
                        area_children.Add(new ApiRegionDTO(){ id = area.Id, persianName = area.PersianName });
                    }
                    city_children.Add(new ApiRegionDTO() { id = city.Id, persianName = city.PersianName, children = area_children });
                }
                result.provinces.Add(new ApiRegionDTO() { id = province.Id, persianName = province.PersianName, children = city_children });
            }
            return result;
        }
    }
}
