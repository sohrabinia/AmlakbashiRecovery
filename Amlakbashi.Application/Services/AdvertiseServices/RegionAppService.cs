using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Region;
using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.DTOs.WebService.Responses.Regions;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class RegionAppService : BaseAppService<Region, int>, IRegionAppService
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

        public IList<RegionListDTO> GetList(int regionId, int type, bool withSubRegions)
        {
            var regions = Repository.Query(q => q);
            if (regionId > 0)
            {
                regions = regions.Where(x => x.Id == regionId);
            }
            else
            {
                regions = regions.Where(x => x.Type == type);
            }
            var result = new List<RegionListDTO>();

            if (withSubRegions)
            {
                foreach (var item in regions)
                {
                    result.Add(GetSubRegions(item));
                }
            }
            else
            {
                foreach (var item in regions)
                {
                    result.Add(new RegionListDTO()
                    {
                        regionId = item.Id,
                        name = item.PersianName
                    });
                }
            }
            return result;
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

        private RegionListDTO GetSubRegions(Region region)
        {
            var dto = new RegionListDTO()
            {
                regionId = region.Id,
                name = region.PersianName
            };
            foreach (var item in region.Childs)
            {
                dto.subRegions.Add(GetSubRegions(item));
            }
            return dto;
        }
    }
}
