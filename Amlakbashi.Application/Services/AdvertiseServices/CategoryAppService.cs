using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;
using Amlakbashi.Core.Infrastructure.FilterHelpers.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using MediatR;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Microsoft.EntityFrameworkCore;

namespace Amlakbashi.Application.Services.Category
{
    internal class CategoryAppService : AppServiceBase<DynamicCategory, int>, ICategoryAppService
    {
        private readonly IAdvertiseFilterHelper advertiseFilter;
        private readonly IMediator mediator;
        public CategoryAppService(IMediator mediator,
            IAdvertiseFilterHelper advertiseFilter,
            IRepository<DynamicCategory, int> repository,
            ICacheManager<DynamicCategory> cache) : base(repository, cache)
        {
            this.advertiseFilter = advertiseFilter;
            this.mediator = mediator;
        }

        public IList<DynamicCategory> Filter(AdvertiseType Type, int Province, int City, int Area, string sort, string query)
        {
            var model = Repository.Query(q => q.Where(w => (int)w.Type != 84 && (int)w.Type != 85 && (int)w.Type != 86));

            if (Type > 0)
                model = model.Where(c => c.Type == Type);
            if (Province > 0)
                model = model.Where(c => c.Province == Province);
            if (City > 0)
                model = model.Where(c => c.City == City);
            if (Area > 0)
                model = model.Where(c => c.Area == Area);

            if (!string.IsNullOrEmpty(query))
            {
                var q_list = query.Split(' ').ToList();
                foreach (var qitem in q_list)
                {
                    if (string.IsNullOrEmpty(qitem))
                        continue;
                    model = model.Where(c => c.Title.Contains(qitem));
                }
            }
            if (sort == "viewcount")
                model = model.OrderByDescending(c => c.CountView).ThenByDescending(c => c.CountAdvertise);
            else if (sort == "adcount")
                model = model.OrderByDescending(c => c.CountAdvertise).ThenByDescending(c => c.CountView);

            return model.ToList();
        }

        public IQueryable<Advertise> GetFilteredAdvertises(int categoryId,
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
          string phrase = ""
      )
        {
            bool OccupiedTablesincluded = false;
            bool DiscountTablesincluded = false;
            bool Childrenincluded = false;
            IQueryable<Advertise> advertises;
            var category = Repository.Find(categoryId);
            var advertiseIds = category.Advertises.Select(s => s.Id).ToList();
            advertises = Repository.Query<Advertise, long>(q =>
                q.Where(w => advertiseIds.Contains(w.Id)));
            //if (includeOccupiedTables || includeDiscountTables || includeChildren)
            //{
            //    var advertiseIds = category.Advertises.Select(s => s.Id).ToList();
            //    advertises = Repository.Query<Advertise, long>(q =>
            //        q.Where(w => advertiseIds.Contains(w.Id)));
            //    if (includeOccupiedTables)
            //    {
            //        advertises = advertises.Include(i => i.OccupiedTables);
            //    }
            //    if (includeDiscountTables)
            //    {
            //        advertises = advertises.Include(i => i.DiscountTables);
            //    }
            //    if (includeChildren)
            //    {
            //        advertises = advertises.Include(i => i.Childs);
            //    }
            //}
            //else
            //{
            //    advertises = category.Advertises.AsQueryable();
            //}

            //if ((!string.IsNullOrEmpty(empty_range_from) &&
            //    !string.IsNullOrEmpty(empty_range_to)) ||
            //    (today_empty_homes != null &&
            //    today_empty_homes == "1"))
            //{
            //    var advertiseIds = category.Advertises.Select(s => s.Id).ToList();
            //    if (includeRelations)
            //    {
            //        advertises = Repository.Query<Advertise, long>(q =>
            //            q.Include(i => i.OccupiedTables)
            //            .Include(i => i.DiscountTables)
            //            .Include(i => i.Childs).Where(f =>
            //            advertiseIds.Contains(f.Id)));
            //    }
            //    else
            //    {
            //        if (today_empty_homes != null && today_empty_homes == "1")
            //        {
            //            advertises = Repository.Query<Advertise, long>(q =>
            //                q.Include(i => i.OccupiedTables)
            //                .Include(i => i.Childs)
            //                .Where(f =>
            //                advertiseIds.Contains(f.Id)));
            //        }
            //        advertises = Repository.Query<Advertise, long>(q =>
            //            q.Include(i => i.OccupiedTables).Where(f =>
            //            advertiseIds.Contains(f.Id)));
            //    }
            //}
            //else
            //{
            //    if (includeRelations)
            //    {
            //        var advertiseIds = category.Advertises.Select(s => s.Id).ToList();
            //        advertises = Repository.Query<Advertise, long>(q =>
            //            q.Include(i => i.DiscountTables)
            //            .Include(i => i.Childs)
            //            .Include(i => i.OccupiedTables)
            //            .Where(f =>
            //            advertiseIds.Contains(f.Id)));
            //    }
            //    else
            //    {
            //        advertises = category.Advertises.AsQueryable();
            //    }
            //}

            //filter by phrase or area
            advertises = advertiseFilter.FilterPhrase(advertises, phrase);
            if (string.IsNullOrEmpty(phrase) && area > 0)
                advertises = advertises.Where(x => x.Area == area);

            // filter by accommodation's position 

            if (position != null && position != "-1")
            {
                int positionInt = 0;
                if (int.TryParse(position, out positionInt) && positionInt > 0)
                    advertises = advertises.Where(a => a.Position == (PositionType)positionInt);
            }
            //filter by parking
            advertises = advertiseFilter.FilterParking(advertises, parking, hasParking);

            //filter by capacity
            int capacity_int = 0;
            if (capacity != null && capacity != "-1")
            {
                if (int.TryParse(capacity, out capacity_int) && capacity_int > 0)
                {
                    advertises = advertises.Where(a => a.Capacity >= capacity_int ||
                        a.Capacity + a.MoreThanCapacity >= capacity_int);
                }
            }
            //filter by room
            advertises = advertiseFilter.FilterRoom(advertises, room, roomList);

            //filter by elevator
            if (elevator != null && elevator != "-1")
            {
                var request_elevator = elevator == "1";
                advertises = advertises.Where(a => a.Elevator == request_elevator);
            }
            //filter by pool
            if (pool != null && pool != "-1")
            {
                var request_pool = pool == "1";
                advertises = advertises.Where(a => a.Pool == request_pool);
            }
            //filter by wifi
            if (wifi)
            {
                advertises = advertises.Where(a => (bool)a.Wifi);
            }
            //filter by washing machine
            if (washingMachine)
            {
                advertises = advertises.Where(a => (bool)a.WashingMachine);
            }
            //filter by jacuzzi
            if (jacuzzi)
            {
                advertises = advertises.Where(a => (bool)a.Jacuzzi);
            }
            //filter by pool table
            if (poolTable)
            {
                advertises = advertises.Where(a => (bool)a.PoolTable);
            }
            //filter by foosball
            if (foosball)
            {
                advertises = advertises.Where(a => (bool)a.Foosball);
            }
            //filter by tea maker
            if (teaMaker)
            {
                advertises = advertises.Where(a => (bool)a.TeaMaker);
            }
            //filter by allow pets rule
            if (rules_pets)
            {
                advertises = advertises.Where(a => (bool)a.AllowPets);
            }
            //filter by allow party rule
            if (rules_party)
            {
                advertises = advertises.Where(a => (bool)a.AllowParty);
            }
            //filter by allow smoking rule
            if (rules_smoking)
            {
                advertises = advertises.Where(a => (bool)a.AllowSmoking);
            }
            //filter by wc type
            if (wcType != -1)
            {
                if (wcType == 128)
                {
                    advertises = advertises.Where(a => a.WC == WCItems.EuropianAndPersian);
                }
                else if (wcType == 129)
                {
                    advertises = advertises.Where(a => a.WC == WCItems.Europian || a.WC == WCItems.EuropianAndPersian);
                }
                else if (wcType == 127)
                {
                    advertises = advertises.Where(a => a.WC == WCItems.Persian || a.WC == WCItems.EuropianAndPersian);
                }
            }
            //filter by norouz special
            if (norouz_special != null && norouz_special == "1")
            {
                advertises = advertises.Where(a => a.NorouzPrice > 0 ||
                    (a.Childs.Any() && a.Childs.All(x => x.NorouzPrice > 0)));
            }
            //filter instant reserve accommodations
            if (instant_reserve != null && instant_reserve == "1")
            {
                advertises = advertises.Where(a => a.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed);
            }

            //filter by price
            int frompaypernight_int = 0;
            int topaypernight_int = 0;
            var hasFromPrice = (!string.IsNullOrEmpty(frompaypernight) &&
                int.TryParse(frompaypernight, out frompaypernight_int) &&
                frompaypernight_int > 0);
            var hasToPrice = !string.IsNullOrEmpty(topaypernight) &&
                int.TryParse(topaypernight, out topaypernight_int) &&
                topaypernight_int > 0;
            if (hasFromPrice || hasToPrice)
            {
                if (!Childrenincluded)
                {
                    advertises = advertises.Include(i => i.Childs);
                    Childrenincluded = true;
                }
                advertises = advertiseFilter.FilterPrice(advertises, priceRangeType,
                    frompaypernight_int, topaypernight_int);
            }

            //filter discounted accommodations
            if (discount_homes != null && discount_homes == "1")
            {
                if (!DiscountTablesincluded)
                {
                    advertises = advertises.Include(i => i.DiscountTables);
                    DiscountTablesincluded = true;
                }
                var today = DateTime.Now.Date;
                advertises = advertises.Where(w => w.DiscountTables.Any(
                    a => a.To > today && a.Percent > 2));
            }
            //filter by empty accommodation in date range
            if ((!string.IsNullOrEmpty(empty_range_from) &&
                !string.IsNullOrEmpty(empty_range_to)) ||
                (today_empty_homes != null &&
                today_empty_homes == "1"))
            {
                var test = advertises.ToList();
                if (today_empty_homes != null && today_empty_homes == "1" &&
                    (string.IsNullOrEmpty(empty_range_from) ||
                    string.IsNullOrEmpty(empty_range_to)))
                {
                    empty_range_from = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date);
                    empty_range_to = DateTimeUtility.GregorianToPersianDate(DateTime.Now.AddDays(1).Date);
                }
                var from = StringUtility.PersianNumberToEnglish(empty_range_from).Replace("/", ",");
                var to = StringUtility.PersianNumberToEnglish(empty_range_to).Replace("/", ",");

                var range = DateTimeUtility.PersianDateRangeToList(from, to, true, false)
                    .Select(s => DateTimeUtility.PersianDateToGregorian(s)).ToList();
                if (!OccupiedTablesincluded)
                {
                    advertises = advertises.Include(i => i.OccupiedTables);
                    OccupiedTablesincluded = true;
                }
                advertises = advertiseFilter.FilterEmptyInRange(advertises, range);
                test = advertises.ToList();
            }
            IOrderedQueryable<Advertise> model_output;
            if (capacity_int > 0)
            {
                if (priorType > 0)
                {
                    if (today_empty_homes != null &&
                        today_empty_homes == "1")
                    {
                        model_output = advertises
                            .OrderByDescending(x => x.TodayIsEmpty /*|| x.Childs.Any(y => y.TodayIsEmpty)*/)
                            .ThenByDescending(x => x.TypeID == (AdvertiseType)priorType)
                            .ThenByDescending(x => x.Capacity == capacity_int ? 1 : 0)
                            .ThenByDescending(x => x.Capacity > capacity_int ? 1 : 0)
                            .ThenBy(x => x.Capacity > capacity_int ? x.Capacity : -x.Capacity);
                    }
                    else
                    {
                        model_output = advertises
                            .OrderByDescending(x => x.TypeID == (AdvertiseType)priorType)
                            .ThenByDescending(x => x.Capacity == capacity_int ? 1 : 0)
                            .ThenByDescending(x => x.Capacity > capacity_int ? 1 : 0)
                            .ThenBy(x => x.Capacity > capacity_int ? x.Capacity : -x.Capacity);
                    }
                }
                else
                {
                    if (today_empty_homes != null &&
                        today_empty_homes == "1")
                    {
                        model_output = advertises
                            .OrderByDescending(x => x.TodayIsEmpty /*|| x.Childs.Any(y => y.TodayIsEmpty)*/)
                            .ThenByDescending(x => x.Capacity == capacity_int ? 1 : 0)
                            .ThenByDescending(x => x.Capacity > capacity_int ? 1 : 0)
                            .ThenBy(x => x.Capacity > capacity_int ? x.Capacity : -x.Capacity);
                    }
                    else
                    {
                        model_output = advertises
                            .OrderByDescending(x => x.Capacity == capacity_int ? 1 : 0)
                            .ThenByDescending(x => x.Capacity > capacity_int ? 1 : 0)
                            .ThenBy(x => x.Capacity > capacity_int ? x.Capacity : -x.Capacity);
                    }
                }
            }
            //else if (!string.IsNullOrEmpty(norouz_special) && norouz_special == "1")
            //{
            //    model_output = tmpResult.OrderBy(x => x.unixNorouzMinRequestDate > todayUnix ? 1 : 0);
            //}
            else
            {
                if (priorType > 0)
                {
                    if (today_empty_homes != null &&
                        today_empty_homes == "1")
                    {
                        model_output = advertises
                            .OrderByDescending(x => x.TodayIsEmpty /*|| x.Childs.Any(y => y.TodayIsEmpty)*/)
                            .ThenByDescending(x => x.TypeID == (AdvertiseType)priorType);
                    }
                    else
                    {
                        model_output = advertises
                            .OrderByDescending(x => x.TypeID == (AdvertiseType)priorType);
                    }
                }
                else
                {
                    if (today_empty_homes != null &&
                        today_empty_homes == "1")
                    {
                        model_output = advertises
                            .OrderByDescending(x => x.TodayIsEmpty /*|| x.Childs.Any(y => y.TodayIsEmpty)*/);
                    }
                    else
                    {
                        model_output = advertises
                            .OrderBy(x => 0);
                    }
                }
            }
            switch ((SortOrder)sort)
            {
                case SortOrder.MoreExpensive:
                    switch (priceRangeType)
                    {
                        case priceRangeTypes.Holiday:
                            model_output = model_output.ThenByDescending(a => a.HolidayPrice);
                            break;
                        case priceRangeTypes.HolidayPeak:
                            model_output = model_output.ThenByDescending(a => a.HolidayPikePrice);
                            break;
                        case priceRangeTypes.Monthly:
                            model_output = model_output.ThenByDescending(a => a.RentPrice);
                            break;
                        default:
                            model_output = model_output.ThenByDescending(a => a.BasePrice);
                            break;
                    }
                    break;
                case SortOrder.Cheaper:
                    switch (priceRangeType)
                    {
                        case priceRangeTypes.Holiday:
                            model_output = model_output.ThenBy(a => a.HolidayPrice);
                            break;
                        case priceRangeTypes.HolidayPeak:
                            model_output = model_output.ThenBy(a => a.HolidayPikePrice);
                            break;
                        case priceRangeTypes.Monthly:
                            model_output = model_output.ThenBy(a => a.RentPrice);
                            break;
                        default:
                            model_output = model_output.ThenBy(a => a.BasePrice);
                            break;
                    }
                    break;
                case SortOrder.UserRate:
                    model_output = model_output.ThenByDescending(a => a.AverageUserRating);
                    break;
                case SortOrder.Clean:
                    model_output = model_output.ThenByDescending(a => a.TidinessUserRating);
                    break;
                default:
                    model_output = model_output.ThenByDescending(a => a.AdvertiseScore);
                    break;
            }
            return model_output;
        }

        public IList<DynamicCategory> GetProvincesForXML(bool old)
        {
            if (old)
            {
                return Repository.Query(q => q.Where(w => w.CountAdvertise > 0 && (w.Province != null ||
                (w.CountryDirection == CountryDirection.North && w.Province == null)) && w.City == null)
                .Where(w => w.Province == 1 || w.Province == 352 || w.Province == 1555 || w.Province == null)
                .OrderByDescending(o => o.CountAdvertise).ToList());
            }

            return Repository.Query(q => q.Where(w => w.CountAdvertise > 0 && (w.Province != null ||
                (w.CountryDirection == CountryDirection.North && w.Province == null)) && w.City == null)
                .Where(w => w.ParentAccType == w.Type)
                .Where(w => w.Province == 1 || w.Province == 352 || w.Province == 1555 || w.Province == null)
                .OrderByDescending(o => o.CountAdvertise).ToList());
        }

        public IList<DynamicCategory> GetCitiesForXML(bool old)
        {
            if (old)
            {
                return Repository.Query(q => q.Where(cd => cd.CountAdvertise > 0 && cd.City != null && cd.Area == null)
                    .OrderByDescending(cd => cd.CountAdvertise).ToList());
            }

            return Repository.Query(q => q.Where(cd => cd.CountAdvertise > 0 && cd.City != null && cd.Area == null)
                .Where(x => x.ParentAccType == x.Type)
                .OrderByDescending(cd => cd.CountAdvertise).ToList());
        }

        public IList<DynamicCategory> GetAreasForXML(bool old)
        {
            if (old)
            {
                return Repository.Query(q => q.Where(cd => cd.CountAdvertise > 0 && cd.Area != null)
                    .OrderByDescending(cd => cd.CountAdvertise).ToList());
            }

            return Repository.Query(q => q.Where(cd => cd.CountAdvertise > 0 && cd.Area != null && cd.ParentAccType == cd.Type)
                .OrderByDescending(cd => cd.CountAdvertise).ToList());
        }

        public DynamicCategory GetByUrl(string url)
        {
            return Repository.Query(q => q.FirstOrDefault(w => w.URL == url));
        }

        public DynamicCategory Find(int id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public DynamicCategory Find(AdvertiseType type, CountryDirection countryDirection, int province, int city, int area)
        {
            if (type == AdvertiseType.None)
                type = AdvertiseType.All;
            var categories = Repository.Query(q => q.Where(w => w.Type == type));
            if (city > 0)
            {
                return categories.FirstOrDefault(f => f.City == city && f.Area == null);
            }
            if (province > 0)
            {
                return categories.FirstOrDefault(f => f.Province == province && f.City == null);
            }
            if (countryDirection > 0)
            {
                return categories.FirstOrDefault(f => f.CountryDirection == countryDirection && f.Province == null);
            }
            return categories.FirstOrDefault(f => f.CountryDirection == CountryDirection.Unset && f.Province == null);
        }

        public DynamicCategory GetByProvinceCity(AdvertiseType type, int province, int city)
        {
            return Repository.Query(q => q.FirstOrDefault(x => x.Type == type &&
                      x.Province == province && x.City == city && x.Area == null));
        }

        public DynamicCategory GetCategoryByCountryDirectionOrRegion(AdvertiseType type, CountryDirection countryDirection,
            int regionId, Region.AdvertiseRegion regionType)
        {
            var categories = Repository.Query(q => q);
            categories = categories.Where(x => x.Type == type);
            if (countryDirection > 0)
            {
                return categories.First(x => x.CountryDirection == countryDirection);
            }
            else
            {
                switch (regionType)
                {
                    case Region.AdvertiseRegion.Province:
                        return categories.First(x => x.Province == regionId);
                    case Region.AdvertiseRegion.City:
                        return categories.First(x => x.City == regionId);
                    case Region.AdvertiseRegion.Area:
                        return categories.First(x => x.Area == regionId);
                    default:
                        return null;
                }
            }
        }

        public DynamicCategory GetForItemAction(int regionType,
            AdvertiseType type, CountryDirection countryDirection = CountryDirection.Unset,
            int province = 0, int city = 0, int area = 0)
        {
            if (regionType == -2)
            {
                return Repository.Query(q => q.FirstOrDefault(f => f.CountryDirection == CountryDirection.Unset
                    && f.Province == null && f.Type == type));
            }
            else if (regionType == -1)
            {
                return Repository.Query(q => q.FirstOrDefault(f => f.CountryDirection == countryDirection
                    && f.Province == null && f.Type == type));
            }
            else
            {
                if (province > 0)
                {
                    return Repository.Query(q => q.FirstOrDefault(f => f.Province == province &&
                            f.City == null && f.Type == type));
                }
                else if (city > 0)
                {
                    return Repository.Query(q => q.FirstOrDefault(f => f.City == city &&
                            f.Area == null && f.Type == type));
                }
                else
                {
                    return Repository.Query(q => q.FirstOrDefault(f => f.Area == area && f.Type == type));
                }
            }
        }

        public List<DynamicCategory> GetAccItemLinks(int? province,
            int? city, int? area, AdvertiseType Type = AdvertiseType.None)
        {
            return mediator.Send(new GetCategoriesFilterCommand(Type, CountryDirection.Unset, province, city, area, false)).Result;
        }

        public void Insert(DynamicCategory newCategory)
        {
            Repository.Insert(newCategory);
            Repository.Save();
        }

        public void Update(DynamicCategory editedCategory)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedCategory.Id));
            data.Title = editedCategory.Title;
            data.Province = editedCategory.Province;
            data.City = editedCategory.City;
            data.Area = editedCategory.Area;
            data.Type = editedCategory.Type;
            data.URL = editedCategory.URL;
            data.Description = editedCategory.Description;
            data.DescriptionH1 = editedCategory.DescriptionH1;
            data.ShowDescription = editedCategory.ShowDescription;
            data.CustomUrlTitle = editedCategory.CustomUrlTitle;
            data.RelatedCategoryIds = editedCategory.RelatedCategoryIds;
            Repository.Update(editedCategory);
            Repository.Save();
        }

        public void UpdateVisited(int id)
        {
            var cat = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            cat.CountView += 1;
            Repository.Update(cat);
            Repository.Save();
        }

        public void Delete(int id)
        {
            Repository.Delete(id);
            Repository.Save();
        }

        public IList<DynamicCategory> GetLinks(AdvertiseType Type = AdvertiseType.None,
           int City = -1, int Area = -1, int count = 20)
        {
            IQueryable<DynamicCategory> categories;
            if (Type > 0)
                categories = Repository.Query(q => q.Where(w => w.CountAdvertise > 0 && w.Type == Type));
            else
                categories = Repository.Query(q => q.Where(w => w.CountAdvertise > 0 && w.Type == AdvertiseType.All));
            if (Area > 0)
                categories = categories.Where(c => c.City == City);
            else if (City > 0)
                categories = categories.Where(c => c.Area == Area);
            else
                categories = categories.Where(c => c.City == null);

            categories = categories.OrderByDescending(c => c.CountAdvertise);
            return categories.Take(count).ToList();
        }

        public IList<DynamicCategory> GetRelatedCategories(int id,
            int[] relatedRegionIds, int found_count, int count = 6)
        {
            try
            {
                var category = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
                var relatedCategories = Repository.Query(q => q);
                relatedCategories = relatedCategories.Where(x =>
                    x.ParentAccType == x.Type &&
                    x.CountAdvertise > 0 &&
                    x.Id != category.Id);
                if (!string.IsNullOrEmpty(category.RelatedCategoryIds))
                {
                    var related_ids = Array.ConvertAll(category.RelatedCategoryIds.Split(','),
                        x => int.Parse(x)).ToList();
                    return relatedCategories.Where(x => related_ids.Contains(x.Id)).ToList();
                }
                if (category.Province != null)
                {
                    if (category.CountryDirection != CountryDirection.North)
                    {
                        if (category.Type == Advertise.AdvertiseType.All)
                        {
                            return relatedCategories.Where(x => x.Province == null &&
                                x.CountryDirection != CountryDirection.North &&
                                x.Type != Advertise.AdvertiseType.All).
                                OrderByDescending(x => x.CountAdvertise).ToList();
                        }
                        else
                        {
                            return relatedCategories.Where(x => x.Province != null &&
                                x.City == null &&
                                x.Type == category.ParentAccType).
                                OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                        }
                    }
                    else
                    {
                        if (category.Type == AdvertiseType.All)
                        {
                            return relatedCategories.Where(x =>
                                (x.Province == 1029 || x.Province == 1393 || x.Province == 1555) &&
                                x.City == null &&
                                x.Type != AdvertiseType.All).
                                OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                        }
                        else
                        {
                            return relatedCategories.Where(x =>
                                (x.Province == 1029 || x.Province == 1393 || x.Province == 1555) &&
                                x.City == null &&
                                x.Type == category.ParentAccType).
                                OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                        }
                    }
                }
                if (category.City == null)
                {
                    if (category.Type == AdvertiseType.All)
                    {
                        return relatedCategories.Where(x =>
                            x.Province == category.Province &&
                            x.City == null &&
                            x.Type != AdvertiseType.All).
                            OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                    }
                    else
                    {
                        return relatedCategories.Where(x =>
                            x.Province == category.Province &&
                            x.City != null &&
                            x.Area == null &&
                            x.Type == category.ParentAccType).
                            OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                    }
                }
                if (category.Area == null)
                {
                    if (found_count < 1)
                    {
                        return relatedCategories.Where(x =>
                        x.City == category.City &&
                        x.Area == null).
                        OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                    }
                    else
                    {
                        return relatedCategories.Where(x =>
                            x.Province == category.Province &&
                            x.City != null &&
                            x.City != category.City &&
                            x.Area == null &&
                            x.Type == AdvertiseType.All).
                            OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                    }
                }
                if (category.Area != null)
                {
                    if (relatedRegionIds != null &&
                        relatedRegionIds.Length > 0)
                    {
                        if (relatedRegionIds.Length > 0)
                        {
                            return
                                relatedCategories.Where(x =>
                                x.Type == AdvertiseType.All &&
                                (
                                (x.Area == null &&
                                x.City == category.City)
                                ||
                                relatedRegionIds.Contains(x.Area == null ? 0 : (int)x.Area))
                                ).
                                OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                        }
                    }
                    else
                    {
                        return
                            relatedCategories.Where(x =>
                            x.Type == AdvertiseType.All &&
                            (
                                (x.Area == null &&
                                x.City == category.City)
                                ||
                            (x.City == category.City &&
                            x.Area != null &&
                            x.Area != category.Area)
                            )
                            ).OrderByDescending(x => x.CountAdvertise).Take(count).ToList();
                    }
                }
                return new List<DynamicCategory>();
            }
            catch
            {
                // TODO logger
                return new List<DynamicCategory>();
            }
        }

        public IList<DynamicCategory> GetListByIds(IList<int> ids)
        {
            return Repository.Query(q => q.Where(w => ids.Contains(w.Id)).ToList());
        }
    }
}
