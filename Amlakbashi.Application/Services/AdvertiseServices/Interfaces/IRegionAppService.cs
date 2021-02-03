using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs;
using Amlakbashi.Core.Entities;
using ServiceStack.Redis.Support.Locking;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IRegionAppService : IAppService<Region, int>
    {
        Region Find(int id);
        IList<Region> GetAll();
        IList<Region> Filter(AdvertiseRegion type, int parentId = 0,
            RegionStatus status = RegionStatus.All, RegionSortOrder sortOrder = RegionSortOrder.Default);
        IList<Region> GetByType(AdvertiseRegion type);
        IList<Region> GetChildren(int id, Region.RegionStatus status = RegionStatus.All);
        IList<int> GetParentIdsByCityId(int city);
        string GetCityName(int cityId, int areaId, CountryDirection countryDirection);
        string GetLocationString(int cityId, int areaId, CountryDirection countryDirection);
        string GetLocationUrl(int province, int city, int area);
        IList<Region> SearchLocationForApp(string searchString);
        string GetCityName(int cityId, int areaId, int countryDirection);
        string GetRegionName(int id);
        Dictionary<DynamicCategory, string[]> GetRegionPersianNamesByCategoryList(IList<DynamicCategory> categoryList);
        ApiRegionTotalDTO GetRegionHierarchy();
    }
}
