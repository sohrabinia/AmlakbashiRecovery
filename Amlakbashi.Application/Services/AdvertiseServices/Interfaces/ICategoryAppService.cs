using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Application.Services.Category.Interfaces
{
    public interface ICategoryAppService : IAppService<DynamicCategory, int>
    {
        IList<DynamicCategory> Filter(AdvertiseType Type, int Province, int City, int Area, string sort, string query);
        IQueryable<Advertise> GetFilteredAdvertises(int categoryId,
          int area = 0,
          string frompaypernight = null, string topaypernight = null,
          string parking = null, string position = null,
          string capacity = null, string room = null,
          string elevator = null, string pool = null,
          string norouz_special = null,
          string today_empty_homes = null,
          string empty_range_from = null,
          string empty_range_to = null,
          string discount_homes = null,
          string instant_reserve = null,
          int priorType = -1,
          priceRangeTypes priceRangeType = priceRangeTypes.Daily,
          int wcType = -1,
          bool wifi = false,
          bool washingMachine = false,
          bool jacuzzi = false,
          bool poolTable = false,
          bool foosball = false,
          bool teaMaker = false,
          bool rules_pets = false,
          bool rules_party = false,
          bool rules_smoking = false,
          bool hasParking = false,
          int sort = 0,
          List<int> roomList = null,
          string phrase = "",
          bool forceIncludeChildren = false,
          bool forceIncludeDiscounts = false
      );
        IList<DynamicCategory> GetProvincesForXML(bool old);
        IList<DynamicCategory> GetCitiesForXML(bool old);
        IList<DynamicCategory> GetAreasForXML(bool old);
        DynamicCategory GetByUrl(string url);
        DynamicCategory Find(int id);
        DynamicCategory Find(AdvertiseType type, CountryDirection countryDirection, int province, int city, int area);
        DynamicCategory GetByProvinceCity(AdvertiseType type, int province, int city);
        DynamicCategory GetForItemAction(int regionType, AdvertiseType type,
            CountryDirection countryDirection = CountryDirection.Unset,
            int province = 0, int city = 0, int area = 0);
        DynamicCategory GetCategoryByCountryDirectionOrRegion(AdvertiseType type, CountryDirection countryDirection,
            int regionId, Region.AdvertiseRegion regionType);
        List<DynamicCategory> GetAccItemLinks(int? province,
            int? city, int? area, AdvertiseType Type = AdvertiseType.None);
        void Insert(DynamicCategory newCategory);
        void Update(DynamicCategory editedCategory);
        void UpdateVisited(int id);
        void Delete(int id);
        IList<DynamicCategory> GetLinks(AdvertiseType Type = AdvertiseType.None, int City = -1, int Area = -1, int count = 20);
        IList<DynamicCategory> GetRelatedCategories(int id, int[] relatedRegionIds, int found_count, int count = 6);
        IList<DynamicCategory> GetListByIds(IList<int> ids);
    }
}
