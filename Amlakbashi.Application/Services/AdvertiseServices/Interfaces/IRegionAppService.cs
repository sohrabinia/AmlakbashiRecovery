using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.DTOs.WebService.Responses.Regions;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IRegionAppService : IAppService<Region, int>
    {
        Region Find(int id);
        IList<RegionListDTO> GetList(int regionId, int type, bool withSubRegions);
        IList<Region> Filter(AdvertiseRegion type, int parentId = 0,
            RegionStatus status = RegionStatus.All, RegionSortOrder sortOrder = RegionSortOrder.Default);
        IList<Region> GetByType(AdvertiseRegion type);
        IList<Region> GetChildren(int id, Region.RegionStatus status = RegionStatus.All);
        IList<int> GetParentIdsByCityId(int city);
        string GetRegionName(int id);
        Dictionary<DynamicCategory, string[]> GetRegionPersianNamesByCategoryList(IList<DynamicCategory> categoryList);
        IList<Region> GetBySearchRegion(string search_string);
        ServiceResult IsValidRegions(int provinceId, int cityId, int areaId);
    }
}
