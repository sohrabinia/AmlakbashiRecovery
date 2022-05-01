using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Region;
using Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs;
using Amlakbashi.Application.DTOs;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class RegionAppService : AppServiceBase<Region, int>, IRegionAppService
    {
        public RegionAppService(IRepository<Region, int> repository) : base(repository)
        {
        }

        public IList<Region> Filter(AdvertiseRegion type, int parentId = 0,
            RegionStatus status = RegionStatus.All,
            RegionSortOrder sortOrder = RegionSortOrder.Default)
        {
            var regions = Repository.Query(q => q);
            if (parentId > 0)
            {
                regions = regions.Where(w =>
                    w.ParentID == parentId);
            }
            switch (status)
            {
                case RegionStatus.Empty:
                    regions = regions.Where(w => w.CountAdvertise < 1);
                    break;
                case RegionStatus.HasAdvertise:
                    regions = regions.Where(w => w.CountAdvertise > 0);
                    break;
            }
            var type_int = (int)type;
            var result = regions.Where(w => w.Type == type_int);
            switch (sortOrder)
            {
                case RegionSortOrder.PersianName:
                    result = result.OrderBy(o => o.PersianName);
                    break;
            }
            return result.ToList();
        }

        public Region Find(int id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public IList<Region> GetAll()
        {
            return Repository.Query(q => q).ToList();
        }

        public IList<Region> GetByType(AdvertiseRegion type)
        {
            var typeInt = (int)type;
            return Repository.Query(q => q.Where(w => w.Type == typeInt)).ToList();
        }

        public IList<Region> GetChildren(int id, Region.RegionStatus status = RegionStatus.All)
        {
            switch (status)
            {
                case RegionStatus.Empty:
                    return Repository.Query(q => q.Where(w => w.ParentID == id && w.CountAdvertise < 1)).ToList();
                case RegionStatus.HasAdvertise:
                    return Repository.Query(q => q.Where(w => w.ParentID == id && w.CountAdvertise > 0)).ToList();
                default:
                    return Repository.Query(q => q.Where(w => w.ParentID == id)).ToList();
            }
        }

        public IList<int> GetParentIdsByCityId(int city)
        {
            return Repository.Query(q => q.Where(x => x.ParentID == city).Select(x => x.Id)).ToList();
        }

        public IList<Region> SearchLocationForApp(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return new List<Region>();
            }
            searchString = searchString.Replace("ي", "ی");
            if (searchString.Contains("شمال") || searchString == "شم" || searchString == "شما")
            {
                var result = new List<Region>();
                result.Add(new Region() { Id = -1, PersianName = "شمال" });
                result.Add(new Region() { Id = 1555, PersianName = "استان مازندران" });
                result.Add(new Region() { Id = 1029, PersianName = "استان گیلان" });
                result.Add(new Region() { Id = 1393, PersianName = "استان گلستان" });
                return result;
            }
            IQueryable<Region> all_regions = Repository.Query(q => q);
            all_regions = all_regions.Where(x => x.CountAdvertise > 0);
            const int type_area = (int)AdvertiseRegion.Area;
            const int type_city = (int)AdvertiseRegion.City;
            const int type_province = (int)AdvertiseRegion.Province;
            IEnumerable<Region> foundRegions;
            var first_is_alef = searchString.First() == 'ا';
            if (first_is_alef)
            {
                var kolah = "آ" + searchString.Remove(0, 1);

                List<Region> areas = all_regions.Where(w => w.Type == type_area &&
                w.PersianName.Contains(searchString) || w.PersianName.Contains(kolah)).ToList();
                List<Region> cities = all_regions.Where(w => w.Type == type_city &&
                    w.PersianName.Contains(searchString) || w.PersianName.Contains(kolah)).
                    OrderBy(w => Math.Abs(w.PersianName.Length - searchString.Length)).ToList();
                List<Region> provinces = all_regions.Where(w => w.Type == type_province &&
                    w.PersianName.Contains(searchString) || w.PersianName.Contains(kolah)).
                    OrderBy(w => Math.Abs(w.PersianName.Length - searchString.Length)).
                    OrderBy(w => Math.Abs(w.PersianName.Length - searchString.Length)).ToList();
                foundRegions = cities.Concat(provinces).Concat(areas).
                    OrderBy(w => Math.Abs(w.PersianName.Length - searchString.Length)).Take(3);
            }
            else
            {
                List<Region> areas = all_regions.Where(w => w.Type == type_area &&
                w.PersianName.Contains(searchString)).ToList();
                List<Region> cities = all_regions.Where(w => w.Type == type_city &&
                    w.PersianName.Contains(searchString)).
                    OrderBy(x => Math.Abs(x.PersianName.Length - searchString.Length)).ToList();
                List<Region> provinces = all_regions.Where(w => w.Type == type_province &&
                    w.PersianName.Contains(searchString)).
                    OrderBy(o => Math.Abs(o.PersianName.Length - searchString.Length)).
                    OrderBy(o => Math.Abs(o.PersianName.Length - searchString.Length)).ToList();
                foundRegions = cities.Concat(provinces).Concat(areas).
                    OrderBy(o => Math.Abs(o.PersianName.Length - searchString.Length)).Take(3);
            }
            foreach (var region in foundRegions)
            {
                switch (region.Type)
                {
                    case type_province:
                        region.PersianName = "استان " + region.PersianName;
                        break;
                    case type_area:
                        region.PersianName = region.PersianName + " (" +
                        all_regions.First(x => x.Id == region.ParentID).PersianName
                        + ")";
                        break;
                }
            }
            return foundRegions.ToList();
        }

        public string GetRegionName(int id)
        {
            var region = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            return region == null ? "" : region.PersianName;
        }

        public Dictionary<DynamicCategory, string[]> GetRegionPersianNamesByCategoryList(IList<DynamicCategory> categoryList)
        {
            var regionsList = Repository.Query(q => q);
            var dic = new Dictionary<DynamicCategory, string[]>();
            var provinceName = "";
            var cityName = "";
            var areaName = "";
            foreach (var item in categoryList)
            {
                provinceName = item.Province != null ? regionsList.FirstOrDefault(w => w.Id == item.Province).PersianName : "";
                cityName = item.City != null ? regionsList.FirstOrDefault(w => w.Id == item.City).PersianName : "";
                areaName = item.Area != null ? regionsList.FirstOrDefault(w => w.Id == item.Area).PersianName : "";
                dic.Add(item, new string[] { provinceName, cityName, areaName });
            }
            return dic;
        }

        public ApiRegionTotalDTO GetRegionHierarchy()
        {
            var allRegions = Repository.Query(q => q);
            return ApiRegionTotalDTO.Generate(allRegions);
        }

        public IList<Region> GetBySearchRegion(string search_string)
        {
            IQueryable<Region> result = Repository.Query(q => q);
            if (string.IsNullOrEmpty(search_string))
            {
                return new List<Region>();
            }
            result = result.Where(x => x.CountAdvertise > 0);

            search_string = search_string.Replace("ي", "ی");
            var first_is_alef = search_string.First() == 'ا';
            if (first_is_alef)
            {
                var kolah = "آ" + search_string.Remove(0, 1);
                result = result.Where(x => x.PersianName.Contains(search_string) ||
                    x.PersianName.Contains(kolah));
            }
            else
            {
                result = result.Where(x => x.PersianName.Contains(search_string));
            }

            return result.OrderByDescending(x => x.PersianName == search_string)
                .ThenByDescending(x => x.CountAdvertise).Take(5).ToList();
        }

        public ServiceResult IsValidRegions(int provinceId, int cityId, int areaId)
        {
            var serviceResult = new ServiceResult();
            var province = Repository.Find(provinceId);
            if (province == null || province.Type != 0)
            {
                serviceResult.AddError("province is incorrect");
            }
            else if (province.Childs.Any(x => x.Id == cityId) == false)
            {
                serviceResult.AddError("city is incorrect");
            }
            else if (areaId > 0)
            {
                var city = province.Childs.FirstOrDefault(x => x.Id == cityId);
                if (city.Childs.Any(x => x.Id == areaId) == false)
                {
                    serviceResult.AddError("area is incorrect");
                }
            }
            return serviceResult;
        }
    }
}
