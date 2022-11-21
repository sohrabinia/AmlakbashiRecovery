using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.DTOs.WebService.Responses.Regions;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IRegionAppService
    {
        Region Find(int id);
        IList<RegionListDTO> GetList(int regionId, int type, bool withSubRegions);
        IList<Region> Filter(Region.AdvertiseRegion type, int parentId = 0,
            Region.RegionStatus status = Region.RegionStatus.All, Region.RegionSortOrder sortOrder = Region.RegionSortOrder.Default);
        IList<Region> GetByType(Region.AdvertiseRegion type);
        IList<Region> GetChildren(int id, Region.RegionStatus status = Region.RegionStatus.All);
        IList<int> GetParentIdsByCityId(int city);
        string GetRegionName(int id);
        Dictionary<DynamicCategory, string[]> GetRegionPersianNamesByCategoryList(IList<DynamicCategory> categoryList);
        IList<Region> GetBySearchRegion(string search_string);
        ServiceResult IsValidRegions(int provinceId, int cityId, int areaId);
    }
}
