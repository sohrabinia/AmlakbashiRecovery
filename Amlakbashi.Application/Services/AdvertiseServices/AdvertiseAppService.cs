using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Common.Utilities;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using System;
using Amlakbashi.Core.Infrastructure.PriceHelpers.Interfaces;
using MediatR;
using Amlakbashi.Mediator.Events.AdvertiseEvents;
using System.Transactions;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using static Amlakbashi.Core.Entities.Reserve;
using Amlakbashi.Mediator.Events.ReserveEvents;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Infrastructure.UserContact;
using static Amlakbashi.Core.Entities.ActionLog;
using Amlakbashi.Mediator.Commands.UserCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Mediator.Commands.FileCommands;
using Amlakbashi.Core.DTOs.AdvertiseDTOs;
using Amlakbashi.Mediator.Commands.CategoryCommands;
using Microsoft.AspNetCore.Http;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.Infrastructure.FilterHelpers.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;
using Amlakbashi.Application.DTOs;
using System.Threading.Tasks;
using static Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs.CheckSetOccupiedDTO;
using static Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs.CheckUnsetOccupiedDTO;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class AdvertiseAppService : AppServiceBase<Advertise, long>, IAdvertiseAppService
    {
        private readonly IPriceCalculator priceCalculator;
        private readonly IAdvertiseFilterHelper advertiseFilter;
        private readonly UserManager<AppUser> userManager;
        private readonly IMediator mediator;
        public AdvertiseAppService(IRepository<Advertise, long> repository,
            IMediator mediator, IPriceCalculator priceCalculator,
            UserManager<AppUser> userManager, IAdvertiseFilterHelper advertiseFilter) : base(repository)
        {
            this.mediator = mediator;
            this.priceCalculator = priceCalculator;
            this.userManager = userManager;
            this.advertiseFilter = advertiseFilter;
        }

        public IQueryable<Advertise> GetAllAsIQueriable()
        {
            return Repository.Query(q => q);
        }

        public AdvertiseListResponse Filter(AdvertiseGetListRequest request)
        {
            var category = Repository.Find<DynamicCategory, int>(request.categoryId);
            var advertises = category.Advertises.AsQueryable();

            advertises = advertiseFilter.FilterPhrase(advertises, request.phrase);
            if (request.area > 0)
            {
                advertises = advertises.Where(x => x.AreaId == request.area);
            }
            if (request.locationTypes != null && request.locationTypes.Any())
            {
                advertises = advertises.Where(a => request.locationTypes.Contains(a.LocationType));
            }
            if (request.parking)
            {
                advertises = advertises.Where(a => a.Parking != ParkingItems.NoParking);
            }
            if (request.capacity > 0)
            {
                advertises = advertises.Where(a => a.Capacity >= request.capacity ||
                    a.Capacity + a.ExtraCapacity >= request.capacity);
            }
            if (request.roomCount > 0)
            {
                advertises = advertises.Where(w => w.RoomCount == request.roomCount);
            }
            if (request.bedCount > 0)
            {
                advertises = advertises.Where(x => x.SingleBedCount + (x.DoubleBedCount * 2) == request.bedCount);
            }
            if (request.elevator)
            {
                advertises = advertises.Where(a => a.Elevator == true);
            }
            if (request.pool)
            {
                advertises = advertises.Where(x => x.Pool == true);
            }
            if (request.wifi)
            {
                advertises = advertises.Where(x => x.Wifi == true);
            }
            if (request.washingMachine)
            {
                advertises = advertises.Where(a => a.WashingMachine == true);
            }
            if (request.jacuzzi)
            {
                advertises = advertises.Where(a => a.Jacuzzi == true);
            }
            if (request.poolTable)
            {
                advertises = advertises.Where(a => a.PoolTable == true);
            }
            if (request.foosball)
            {
                advertises = advertises.Where(a => a.Foosball == true);
            }
            if (request.teaMaker)
            {
                advertises = advertises.Where(a => a.TeaMaker == true);
            }
            if (request.pets)
            {
                advertises = advertises.Where(a => a.Pets == true);
            }
            if (request.party)
            {
                advertises = advertises.Where(a => a.Party == true);
            }
            if (request.smoking)
            {
                advertises = advertises.Where(a => a.Smoking == true);
            }
            if (request.wcType != WCItems.Unset)
            {
                if (request.wcType == WCItems.EuropianAndPersian)
                {
                    advertises = advertises.Where(a => a.WC == WCItems.EuropianAndPersian);
                }
                else if (request.wcType == WCItems.Europian)
                {
                    advertises = advertises.Where(a => a.WC == WCItems.Europian || a.WC == WCItems.EuropianAndPersian);
                }
                else
                {
                    advertises = advertises.Where(a => a.WC == WCItems.Persian || a.WC == WCItems.EuropianAndPersian);
                }
            }
            if (request.norouz)
            {
                advertises = advertises.Where(a => a.NowruzPrice > 0);
            }
            if (request.instantReserve)
            {
                advertises = advertises.Where(a => a.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Permanent);
            }
            if (request.minPrice > 0 || request.maxPrice > 0)
            {
                advertises = advertiseFilter.FilterPrice(advertises, request.priceType,
                    request.minPrice, request.maxPrice);
            }
            if ((string.IsNullOrEmpty(request.fromDate) == false &&
                string.IsNullOrEmpty(request.toDate) == false) || request.emptyTonight)
            {
                if (request.emptyTonight)
                {
                    request.fromDate = DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date);
                    request.toDate = DateTimeUtility.GregorianToPersianDate(DateTime.Now.AddDays(1).Date);
                }
                var from = StringUtility.PersianNumberToEnglish(request.fromDate).Replace("/", ",");
                var to = StringUtility.PersianNumberToEnglish(request.toDate).Replace("/", ",");
                var range = DateTimeUtility.PersianDateRangeToList(from, to, true, false)
                    .Select(s => DateTimeUtility.PersianDateToGregorian(s)).ToList();
                advertises = advertises.Where(w => w.OccupiedTables.Any(a => range.Select(s => s).Contains(a.Date)) == false);
            }

            IOrderedQueryable<Advertise> orderedAdvertiseList = advertises.OrderBy(x => true);
            if (request.emptyTonight)
            {
                orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(x => x.EmptyTonight);
            }
            if (request.residencyType != AdvertiseType.None && request.residencyType != AdvertiseType.All)
            {
                orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(x => x.TypeID == request.residencyType);
            }
            if (request.capacity > 0)
            {
                orderedAdvertiseList = orderedAdvertiseList.ThenBy(x => x.Capacity);
            }
            if (request.instantReserve)
            {
                orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(x => 
                    x.InstantReserveStatus == InstantReserveStatusEnum.Permanent ||
                    x.InstantReserveDates.Any(a => a.Date == DateTime.Now.Date));
            }
            switch (request.sort)
            {
                case SortOrder.MoreExpensive:
                    switch (request.priceType)
                    {
                        case priceRangeTypes.Holiday:
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.HolidayPrice);
                            break;
                        case priceRangeTypes.HolidayPeak:
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.PeakHolidayPrice);
                            break;
                        case priceRangeTypes.Monthly:
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.MonthlyPrice);
                            break;
                        case priceRangeTypes.Norouz:
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.NowruzPrice);
                            break;
                        default:
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.BasePrice);
                            break;
                    }
                    break;
                case SortOrder.Cheaper:
                    switch (request.priceType)
                    {
                        case priceRangeTypes.Holiday:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.HolidayPrice);
                            break;
                        case priceRangeTypes.HolidayPeak:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.PeakHolidayPrice);
                            break;
                        case priceRangeTypes.Monthly:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.MonthlyPrice);
                            break;
                        case priceRangeTypes.Norouz:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.NowruzPrice);
                            break;
                        default:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.BasePrice);
                            break;
                    }
                    break;
                case SortOrder.UserRate:
                    orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.AverageUsersScore);
                    break;
                case SortOrder.Clean:
                    orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.CleaningScore);
                    break;
                default:
                    orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.ResidenceScore);
                    break;
            }

            var pagedList = orderedAdvertiseList.ToPagedList(request.page, request.pageItemCount);
            var user = Repository.Find<User, int>(request.userId);

            AdvertiseListResponse response = new AdvertiseListResponse()
            {
                pagingInfo = pagedList.PagingInfo,
                categoryTitle = AdvertiseSeoLocalization.GetTitle(category.MostAccType, (int)category.Type,
                    category.Province == null ? "" : category.RegionProvince.PersianName,
                    category.City == null ? "" : category.RegionCity.PersianName,
                    category.Area == null ? "" : category.RegionArea.PersianName,
                    Region.GetCountryDirectionString(category.CountryDirection))
            };
            foreach (var item in pagedList.List)
            {
                var itemResponse = (AdvertiseListItemResponse)item;
                itemResponse.favourited = user?.Favorite.Any(x => x.AdvertiseID == item.Id) ?? false;
                response.advertiseList.Add(itemResponse);
            }
            return response;
        }

        public IList<Advertise> Filter(string id)
        {
            return Repository.Query(q => q.Where(x => x.Id.ToString().Contains(id) &&
                x.Status == AdvertiseStatus.Published && x.Active)).OrderByDescending(x => x.AmlakbashiScore).Take(5).ToList();
        }

        public IList<Advertise> GetAdvertisesByUserId(int userId, bool includeCommentsAndReports = false)
        {
            if (includeCommentsAndReports)
            {
                return Repository.Query(q => q.Include(i => i.Parent).Include(i => i.Comments).Include(i => i.ReportItems)
                    .Where(w => w.UserId == userId && w.Comments.Any(a => a.Status == Comment.CommentStatus.publish)).ToList());
            }
            return Repository.Query(q => q.Where(w => w.UserId == userId).ToList());
        }

        public IList<long> GetAdvertiseIdsByUserId(int userId)
        {
            return Repository.Query(q => q.Where(w => w.UserId == userId).Select(s => s.Id).ToList());
        }

        public IList<Advertise> GetAdvertisesByStatus(AdvertiseStatus status, bool haveSlug = false)
        {
            if (haveSlug)
            {
                return Repository.Query(q => q.Where(a => a.Status == AdvertiseStatus.Published &&
                    !string.IsNullOrEmpty(a.Slug) && a.ProvinceId > 0).ToList());
            }
            return Repository.Query(q => q.Where(w => w.Status == status).ToList());
        }

        public IList<Advertise> GetAccListByIds(IList<long> ids, AdvertiseStatus status = AdvertiseStatus.Unset)
        {
            if (status == AdvertiseStatus.Unset)
            {
                return Repository.Query(q => q.Where(w => ids.Contains(w.Id)).ToList());
            }
            return Repository.Query(q => q.Where(w => ids.Contains(w.Id) && w.Status == status).ToList());
        }

        public IList<Advertise> GetMostLiked(int count, bool beInstantReserve = false)
        {
            if (beInstantReserve)
            {
                return Repository.Query(q => q.Where(w => w.InstantReserveStatus == InstantReserveStatusEnum.Permanent)
                    .OrderByDescending(o => o.AverageUsersScore).Take(count)).ToList();
            }
            return Repository.Query(q => q.OrderByDescending(o => o.AverageUsersScore).Take(count)).ToList();
        }

        public List<string> GetAdvertiseTags(Advertise advertise)
        {
            var tags = new List<string>();
            tags.Add(AdvertiseMainLocalization.GetAdvertiseTypePersianNameForUser(advertise.TypeID));
            if (advertise.RoomCount > 0)
            {
                tags.Add($"{advertise.RoomCount} خوابه");
            }
            tags.Add(advertise.RegionCity.PersianName);
            tags.Add(advertise.RegionProvince.PersianName);
            return tags;
        }

        public AdvertiseListResponse GetUserFavoriteAdvertises(int userId, int page = 1, int pageItemCount = 20)
        {
            var user = Repository.Find<User, int>(userId);
            var pagedIdList = user.Favorite.OrderByDescending(x => x.SetDate)
                .Select(x => x.AdvertiseID).ToPagedList(page, pageItemCount);
            var advertises = GetAccListByIds(pagedIdList.List);
            var response = new AdvertiseListResponse()
            {
                pagingInfo = pagedIdList.PagingInfo,
                categoryTitle = "علاقه مندی ها"
            };
            foreach (var item in advertises)
            {
                var itemResponse = (AdvertiseListItemResponse)item;
                itemResponse.favourited = true;
                response.advertiseList.Add(itemResponse);
            }
            return response;
        }

        public void AddSupporterInfo(long id, string text, User supporter)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.AddSupportInfo(text, supporter);
            Repository.Update(acc);
            Repository.Save();
        }

        public Advertise Find(long id, bool includeOccupiedTables = false)
        {
            if (includeOccupiedTables)
            {
                return Repository.Query(q => q.Include(i => i.OccupiedTables)
                .FirstOrDefault(f => f.Id == id && f.Status != AdvertiseStatus.Deleted));
            }
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id &&
            f.Status != AdvertiseStatus.Deleted));
        }

        public Advertise FindIncludingDeleted(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public bool Delete(long id)
        {
            var acc = Repository.Find(id);
            if (acc == null)
            {
                return false;
            }
            bool canDelete = false;
            if (acc.Mode != AdvertiseMode.Parent)
            {
                canDelete = acc.Reserves.Any() == false;
            }
            else
            {
                if (acc.Childs == null || acc.Childs.Count == 0)
                {
                    canDelete = true;
                }
                else
                {
                    canDelete = acc.Childs.All(x => x.Reserves.Any() == false);
                }
            }
            if (canDelete)
            {
                var prevAcc = acc.ShallowCopy();
                acc.Status = AdvertiseStatus.Deleted;
                Repository.Update(acc);
                Repository.Save();
                mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
                mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
                return true;
            }
            return false;
        }

        public void FilterNew(AdvertiseIndexDTO dto)
        {
            IQueryable<Advertise> model = Repository.Query(q => q);
            if (dto.Status != AdvertiseStatus.Unset)
            {
                model = model.Where(a => a.Status == dto.Status);
            }
            else
            {
                model = model.Where(a => a.Status != AdvertiseStatus.Deleted);
            }
            if (dto.Id > 0)
            {
                model = model.Where(w => w.Id == dto.Id);
            }
            if (dto.UserId != -1)
            {
                model = model.Where(w => w.UserId == dto.UserId);
            }
            if (dto.Type != Advertise.AdvertiseType.All)
            {
                model = model.Where(a => a.TypeID == dto.Type);
            }
            if (dto.HygieneProtocolStatus > -1)
            {
                var st = (Advertise.HygieneProtocolStatus)dto.HygieneProtocolStatus;
                model = model.Where(w => w.HygieneProtocol == st);
            }
            if (dto.InstantReserveStatus > -1)
            {
                model = model.Where(x => x.InstantReserveStatus == (Advertise.InstantReserveStatusEnum)dto.InstantReserveStatus);
            }
            if (dto.VideoStatus > -1)
            {
                model = model.Where(x => x.VideoStatus == (Advertise.VideoStatusEnum)dto.VideoStatus);
            }
            if (string.IsNullOrEmpty(dto.MinReserveNorouzFromDate) == false)
            {
                var gregorianDate = DateTimeUtility.PersianDateToGregorian(
                        StringUtility.PersianNumberToEnglish(dto.MinReserveNorouzFromDate).Replace('/', ','));
                var minReserveNorouzDateUnix = DateTimeUtility.DateValueOfJS(gregorianDate);
                model = model.Where(x => x.MinReserveDateForNowruz >= minReserveNorouzDateUnix);
            }
            if (dto.Area > -1)
            {
                model = model.Where(x => x.AreaId == dto.Area);
            }
            else if (dto.City > -1)
            {
                model = model.Where(x => x.CityId == dto.City);
            }
            else if (dto.Province > -1)
            {
                model = model.Where(x => x.ProvinceId == dto.Province);
            }
            if (dto.ImageCountMin > 0)
            {
                model = model.Where(x => x.Photos.Count > dto.ImageCountMin);
            }
            if (dto.ImageCountMax > 0)
            {
                model = model.Where(x => x.Photos.Count <= dto.ImageCountMax + 1);
            }
            if (dto.Parking != ParkingItems.Unset)
            {
                model = model.Where(x => x.Parking == dto.Parking || x.Childs.Any(a => a.Parking == dto.Parking));
            }
            if (dto.License != null)
            {
                model = model.Where(x => x.License == dto.License);
            }
            if (dto.Sort == "modify")
                model = model.OrderByDescending(a => a.LastModifiedDate).ThenByDescending(a => a.CreateDate);
            else if (dto.Sort == "view")
                model = model.OrderByDescending(a => a.View);
            else if (dto.Sort == "score")
                model = model.OrderByDescending(a => a.ResidenceScore);
            else
                model = model.OrderByDescending(a => a.CreateDate);

            dto.PagingInfo = new Core.DTOs.PagingDTO(dto.Page, model.Count());
            model = model.Skip((dto.Page - 1) * dto.PagingInfo.PageItemCount).Take(dto.PagingInfo.PageItemCount);

            dto.AdvertiseList = new List<AdvertiseIndexItemDTO>();
            foreach (var item in model)
            {
                AdvertiseIndexItemDTO dtoItem = item;
                if (item.User != null)
                {
                    dtoItem.UserFullName = item.User.FullName;
                }
                if (item.RegionCity != null)
                {
                    dtoItem.CityPersianName = item.RegionCity.PersianName;
                }
                dto.AdvertiseList.Add(dtoItem);
            }
        }

        public IList<Advertise> Filter(string statusString, int userid, long id)
        {
            IQueryable<Advertise> model = Repository.Query(q => q);
            model = model.Where(w => w.Mode != AdvertiseMode.Child);
            model = model.Where(w => w.Status != AdvertiseStatus.Deleted);
            if (userid != -1)
                model = model.Where(w => w.UserId == userid);
            if (statusString != null)
            {
                switch (statusString)
                {
                    case "enable":
                        model = model.Where(w => w.Status == AdvertiseStatus.Published);
                        break;
                    case "disable":
                        model = model.Where(w => w.Status == AdvertiseStatus.Archived);
                        break;
                    case "ready":
                        model = model.Where(w =>
                            w.Status == AdvertiseStatus.ReadyToPublish ||
                            w.Status == AdvertiseStatus.FirstReady ||
                            w.Status == AdvertiseStatus.NotCompleted);
                        break;
                }
            }
            if (id > 0)
            {
                model = model.Where(w => w.Id == id || w.Childs.Any(x => x.Id == id));
            }
            return model.ToList();
        }

        public IList<Advertise> FilterAdmin(int province = 0, int city = 0,
            int area = 0, int adtype = 0, bool defaultProvince = false, int adStatus = -1)
        {
            IQueryable<Advertise> query = Repository.Query(q => q.Where(x => x.UserId > 0));
            if (defaultProvince)
            {
                query = query.Where(x => x.ProvinceId == province);
            }
            if (area > 0)
            {
                query = query.Where(x => x.AreaId == area);
            }
            else if (city > 0)
            {
                query = query.Where(x => x.CityId == city);
            }
            else if (province > 0)
            {
                query = query.Where(x => x.ProvinceId == province);
            }

            if (adtype > 0)
            {
                query = query.Where(x => x.TypeID == (AdvertiseType)adtype);
            }

            if (adStatus > -1)
            {
                query = query.Where(x => x.Status == (Advertise.AdvertiseStatus)adStatus);
            }
            return query.ToList();
        }

        public IList<Advertise> FilterAdmin(int province, int city, int area, int adtype,
            DateTime fromDate, DateTime toDate, int userId)
        {
            IQueryable<Advertise> query = Repository.Query(q => q.Where(x => x.UserId > 0));
            if (fromDate != null && toDate != null)
            {
                query = query.Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate);
            }

            if (area > 0)
            {
                query = query.Where(x => x.AreaId == area);
            }
            else if (city > 0)
            {
                query = query.Where(x => x.CityId == city);
            }
            else if (province > 0)
            {
                query = query.Where(x => x.ProvinceId == province);
            }

            if (adtype > 0 && adtype != (int)Advertise.AdvertiseType.All)
            {
                query = query.Where(x => x.TypeID == (Advertise.AdvertiseType)adtype);
            }

            if (userId > 0)
                query = query.Where(x => x.UserId == userId);

            return query.ToList();
        }

        public void Edit(Advertise editedAd, int adminId)
        {
            var advertise = Repository.Find(editedAd.Id);
            var shallowAdvertise = advertise.ShallowCopy();

            advertise.Title = editedAd.Title;
            advertise.MetaTitle = editedAd.MetaTitle;
            advertise.MetaDescription = editedAd.MetaDescription;
            advertise.Slug = editedAd.Slug;
            advertise.UserId = editedAd.UserId;
            if (advertise.Childs != null && advertise.Childs.Any())
            {
                foreach (var child in advertise.Childs)
                {
                    child.UserId = editedAd.UserId;
                }
            }
            advertise.View = editedAd.View;
            if (advertise.AmlakbashiScore != editedAd.AmlakbashiScore)
            {
                advertise.ResidenceScore += (editedAd.AmlakbashiScore - advertise.AmlakbashiScore);
                advertise.AmlakbashiScore = editedAd.AmlakbashiScore;
            }
            advertise.Description = editedAd.Description;
            advertise.LastModifiedDate = DateTime.Now;
            advertise.MinReserveDuration = editedAd.MinReserveDuration;
            advertise.MaxReserveDuration = editedAd.MaxReserveDuration;
            Repository.Update(advertise);
            Repository.Save();
            mediator.Publish(new AdvertiseUpdateEvent(shallowAdvertise, advertise,
                ActionLog.ActionSourceEnum.AdminPanel, adminId));
            mediator.Publish(new ChangeStayDurationEvent(advertise.Id));
            mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
            mediator.Send(new RemoveCategoryItemCacheCommand(advertise.Id));
        }

        public void UpdateAccView(long accId)
        {
            var acc = Repository.Find(accId);
            acc.View += 1;
            //acc.Overview += 1;
            Repository.Update(acc);
            Repository.Save();
        }

        public AdvertiseDirector GetAdvertisePageData(long id,
            out Dictionary<AdvertiseType, IList<AdvertiseDirector>> childrenDirectors)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var director = new AdvertiseDirector(acc, DirectorType.AdvertisePage);
            var hotelTypes = GetHotelTypes();
            childrenDirectors = new Dictionary<AdvertiseType, IList<AdvertiseDirector>>();
            foreach (var item in acc.Childs.Where(w => w.Active == true))
            {
                var childDirector = new AdvertiseDirector(item, DirectorType.AdvertisePageChild);
                var key = hotelTypes.Contains(childDirector.AdvertiseType) ? AdvertiseType.Hotel : childDirector.AdvertiseType;
                if (childrenDirectors.ContainsKey(key))
                {
                    childrenDirectors[key].Add(childDirector);
                }
                else
                {
                    childrenDirectors.Add(key, new List<AdvertiseDirector>() { childDirector });
                }
            }
            return director;
        }

        public async Task<ServiceResult<long>> CreateAsync(AdvertisePostCreateRequest request)
        {
            var serviceResult = new ServiceResult<long>();
            Advertise residence = new Advertise()
            {
                CreateDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Status = AdvertiseStatus.NotCompleted,
                UserId = request.userId,
                Active = true,
                TypeID = request.type,
                LocationType = request.locationType,
                Mode = Advertise.GetModeByType(request.type),
                Floor = FloorItems.Unset,
            };
            Insert(residence);
            await mediator.Publish(new CreateAdvertiseBasicEvent(residence.Id, request.userId));
            serviceResult.Result = residence.Id;
            return serviceResult;
        }

        public async Task<ServiceResult<long>> UpdateBasicInfoAsync(AdvertisePostBasicInfoRequest request)
        {
            var serviceResult = new ServiceResult<long>();
            var residence = Repository.Find(request.residenceId);
            if (residence == null || request.userId != residence.UserId)
            {
                serviceResult.AddError("advertise not found");
                return serviceResult;
            }
            residence.LocationType = request.locationType;
            Repository.Update(residence);
            Repository.Save();
            await mediator.Publish(new ChangeAdvertisePositionEvent(residence.Id));
            serviceResult.Result = residence.Id;
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateGeneralInfoAsync(AdvertisePostGeneralInfoRequest request)
        {
            var serviceResult = new ServiceResult();
            var residence = Repository.Find(request.residenceId);
            if (residence == null || residence.UserId != request.userId)
            {
                serviceResult.AddError("advertise not found");
                return serviceResult;
            }
            if (request.mainPhotoId > 0 && residence.Photos.Any(x => x.Id == request.mainPhotoId) == false)
            {
                serviceResult.AddError("main image id is incorrect");
                return serviceResult;
            }
            var shallowAdvertise = residence.ShallowCopy();
            residence.ProvinceId = request.provinceId;
            residence.CityId = request.cityId;
            if (request.areaId > 0)
            {
                residence.AreaId = request.areaId;
            }
            else
            {
                residence.AreaId = null;
            }
            residence.Address = request.address;
            residence.Title = request.title;
            residence.Description = request.description;
            residence.Longitude = request.longitude;
            residence.Latitude = request.latitude;
            residence.LastModifiedDate = DateTime.Now;
            residence.MainPhotoId = request.mainPhotoId > 0 ? request.mainPhotoId : residence.MainPhotoId;
            residence.UpdateStatusAfterChangeInfo(HasImportantChangeOnUpdate(shallowAdvertise, residence));
            Repository.Update(residence);
            Repository.Save();

            if (residence.Status != shallowAdvertise.Status)
            {
                await mediator.Publish(new ChangeAdvertiseStatusEvent(residence.Id, shallowAdvertise.Status));
                await mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAdvertise, residence));
            }
            await mediator.Publish(new CreateAdvertiseGeneralEvent(residence.Id));
            await mediator.Publish(new ChangeAdvertiseAddressEvent(shallowAdvertise, residence));
            await mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateSupplementaryInfoAsync(AdvertisePostSupplementaryInfoRequest request)
        {
            var serviceResult = new ServiceResult();
            var residence = Repository.Find(request.residenceId);
            if (residence == null || residence.UserId != request.userId)
            {
                serviceResult.AddError("advertise not found");
                return serviceResult;
            }
            if (request.license && residence.LicenseFileId is null)
            {
                serviceResult.AddError("license file not exist");
                return serviceResult;
            }
            var shallowAdvertise = residence.ShallowCopy();
            PropertyCopier<AdvertisePostSupplementaryInfoRequest, Advertise>.CopyInsensetive(request, residence);
            residence.LastModifiedDate = DateTime.Now;
            residence.PoolFeatures = Advertise.GetPoolFeatureFlag(request.poolHotWater,
                request.poolFiltration, request.poolOpen, request.poolCovered);
            residence.UpdateStatusAfterChangeInfo(HasImportantChangeOnUpdate(shallowAdvertise, residence));
            Repository.Update(residence);
            Repository.Save();

            if (shallowAdvertise.Status != residence.Status)
            {
                await mediator.Publish(new ChangeAdvertiseStatusEvent(residence.Id, shallowAdvertise.Status));
                await mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAdvertise, residence));
            }
            if (residence.Mode == AdvertiseMode.Parent)
            {
                await mediator.Publish(new ChangeAdvertiseRulesEvent(residence.Id));
            }
            await mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateFinalInfoAsync(AdvertisePostFinalInfoRequest request)
        {
            var serviceResult = new ServiceResult();
            var residence = Repository.Find(request.residenceId);
            if (residence == null || residence.UserId != request.userId)
            {
                serviceResult.AddError("advertise not found");
                return serviceResult;
            }
            var shallowAdvertise = residence.ShallowCopy();
            PropertyCopier<AdvertisePostFinalInfoRequest, Advertise>.CopyInsensetive(request, residence);
            if (residence.Status == AdvertiseStatus.NotCompleted)
            {
                residence.Status = AdvertiseStatus.FirstReady;
            }
            else
            {
                residence.UpdateStatusAfterChangeInfo(HasImportantChangeOnUpdate(shallowAdvertise, residence));
            }
            Repository.Update(residence);
            Repository.Save();

            if (shallowAdvertise.Status != residence.Status)
            {
                await mediator.Publish(new ChangeAdvertiseStatusEvent(residence.Id, shallowAdvertise.Status));
                await mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAdvertise, residence));
            }
            await mediator.Publish(new ChangeAdvertisePriceEvent(residence.Id, residence.NowruzPrice != shallowAdvertise.NowruzPrice));
            await mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
            return serviceResult;
        }

        public async Task<ServiceResult> CreateHotelRoomAsync(AdvertisePostHotelRoomInfoRequest request)
        {
            var serviceResult = new ServiceResult();
            var parentResidence = Repository.Find(request.parentId);
            if (parentResidence == null || parentResidence.UserId != request.userId)
            {
                serviceResult.AddError("parent advertise not found");
                return serviceResult;
            }
            if (parentResidence.Mode != AdvertiseMode.Parent)
            {
                serviceResult.AddError("parent is incorrect");
                return serviceResult;
            }
            var shallowParentResidence = parentResidence.ShallowCopy();
            var unit = new Advertise()
            {
                UserId = parentResidence.UserId,
                TypeID = parentResidence.TypeID,
                Mode = AdvertiseMode.Child,
                CreateDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Status = parentResidence.Status,
                Capacity = request.capacity,
                ExtraCapacity = request.extraCapacity,
                Title = request.title,
                Description = request.description,
                DailyPrice = request.dailyPrice,
                PeakHolidayPrice = request.peakHolidayPrice,
                HolidayPrice = request.holidayPrice,
                MonthlyPrice = request.monthlyPrice,
                NowruzPrice = request.nowruzPrice,
                ExtraCapacityPrice = request.extraCapacityPrice,
                NowruzExtraCapacityPrice = request.nowruzExtraCapacityPrice,
                BuildingArea = request.buildingArea,
                RoomCount = request.count,
                SingleBedCount = request.singleBedCount,
                DoubleBedCount = request.doubleBedCount,
                BlanketAndMattressCount = request.blanketAndMattressCount,
                ExtraBlanketCount = request.extraBlanketCount
            };
            parentResidence.Childs.Add(unit);
            if (parentResidence.Status == AdvertiseStatus.NotCompleted)
            {
                parentResidence.Status = AdvertiseStatus.FirstReady;
            }
            else
            {
                parentResidence.UpdateStatusAfterChangeInfo(true);
            }
            Repository.Update(parentResidence);
            Repository.Save();

            if (parentResidence.Status != shallowParentResidence.Status)
            {
                await mediator.Publish(new ChangeAdvertiseStatusEvent(parentResidence.Id, shallowParentResidence.Status));
                await mediator.Publish(new ChangeAdvertiseActiveEvent(shallowParentResidence, parentResidence));
            }
            await mediator.Publish(new AddHotelChildEvent(unit.Id, (long)unit.ParentId));
            await mediator.Publish(new ChangeAdvertisePriceEvent(unit.Id, false));
            await mediator.Send(new RemoveAdvertiseCacheCommand(parentResidence.Id));
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateHotelRoomInfoAsync(AdvertisePostHotelRoomInfoRequest request)
        {
            var serviceResult = new ServiceResult();
            var residence = Repository.Find(request.unitId);
            if (residence == null || residence.Mode != AdvertiseMode.Child)
            {
                serviceResult.AddError("advertise not found");
                return serviceResult;
            }
            var shallowAdvertise = residence.ShallowCopy();
            var shallowParentAdvertise = residence.Parent.ShallowCopy();
            PropertyCopier<AdvertisePostHotelRoomInfoRequest, Advertise>.CopyInsensetive(request, residence);
            residence.ExtraCapacity = request.extraCapacity;
            residence.ExtraCapacityPrice = request.extraCapacityPrice;
            residence.MonthlyPrice = request.monthlyPrice;
            residence.NowruzExtraCapacityPrice = request.nowruzExtraCapacityPrice;
            residence.RoomCount = request.count;
            residence.SingleBedCount = request.singleBedCount;
            residence.DoubleBedCount = request.doubleBedCount;
            residence.BlanketAndMattressCount = request.blanketAndMattressCount;
            residence.Parent.UpdateStatusAfterChangeInfo(true);
            Repository.Update(residence);
            Repository.Save();

            if (shallowAdvertise.Status != residence.Status)
            {
                await mediator.Publish(new ChangeAdvertiseStatusEvent(residence.ParentId.Value, shallowParentAdvertise.Status));
                await mediator.Publish(new ChangeAdvertiseActiveEvent(shallowParentAdvertise, residence.Parent));
            }
            await mediator.Publish(new ChangeAdvertisePriceEvent(residence.Id, residence.NowruzPrice != shallowAdvertise.NowruzPrice));
            await mediator.Send(new RemoveAdvertiseCacheCommand(residence.ParentId.Value));
            return serviceResult;
        }

        private bool HasImportantChangeOnUpdate(Advertise residence, Advertise updatedResidence)
        {
            if (residence.Title != updatedResidence.Title ||
                residence.Description != updatedResidence.Description ||
                residence.ProvinceId != updatedResidence.ProvinceId ||
                residence.CityId != updatedResidence.CityId ||
                residence.AreaId != updatedResidence.AreaId ||
                residence.Address != updatedResidence.Address ||
                residence.TypeID != updatedResidence.TypeID ||
                residence.License != updatedResidence.License ||
                residence.LicenseNumber != updatedResidence.LicenseNumber ||
                residence.OwnershipType != updatedResidence.OwnershipType ||
                residence.AlbumPhoto != updatedResidence.AlbumPhoto ||
                residence.LocationType != updatedResidence.LocationType ||
                residence.RequiredEvidence != updatedResidence.RequiredEvidence ||
                residence.OtherRules != updatedResidence.OtherRules)
            {
                return true;
            }
            return false;
        }

        public AdvertiseDirector GetBasicForm(long id, out bool isEdit, out int level)
        {
            Advertise acc = null;
            level = 0;
            if (id > 0)
            {
                acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
                level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : ((int)acc.OwnershipType < 1 ? 3 : 4));
            }
            isEdit = acc != null && !(acc.Status == AdvertiseStatus.NotCompleted || acc.Status == AdvertiseStatus.Unset);
            if (acc == null)
            {
                acc = new Advertise();
                acc.TypeID = AdvertiseType.None;
            }
            var director = new AdvertiseDirector(acc, DirectorType.Basic);
            return director;
        }

        public AdvertiseDirector SubmitBasicForm(Advertise data, int userId, out Dictionary<string, string> errors,
            out List<string> groupErrors, out int level)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == data.Id));
            data.LastModifiedDate = DateTime.Now;
            level = 1;

            if (data.Id > 0)
            {
                level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : ((int)acc.OwnershipType < 1 ? 3 : 4));
                switch (data.TypeID)
                {
                    case AdvertiseType.Hotel:
                    case AdvertiseType.Camp:
                    case AdvertiseType.TourismAccommodation:
                    case AdvertiseType.Inn:
                    case AdvertiseType.Pansion:
                    case AdvertiseType.HotelApartment:
                    case AdvertiseType.Complex:
                        data.Mode = AdvertiseMode.Parent;
                        break;
                    default:
                        data.Mode = AdvertiseMode.Single;
                        break;
                }
            }
            var director = new AdvertiseDirector(data, DirectorType.Basic);
            data.Status = AdvertiseStatus.NotCompleted;
            data.CreateDate = DateTime.Now;
            data.UserId = userId;
            data.Active = true;
            data.Floor = FloorItems.Unset;
            switch (data.TypeID)
            {
                case AdvertiseType.Hotel:
                case AdvertiseType.Camp:
                case AdvertiseType.TourismAccommodation:
                case AdvertiseType.Inn:
                case AdvertiseType.Pansion:
                case AdvertiseType.HotelApartment:
                case AdvertiseType.Complex:
                    data.Mode = AdvertiseMode.Parent;
                    break;
                default:
                    data.Mode = AdvertiseMode.Single;
                    break;
            }
            if (director.Validate(out errors, out groupErrors))
            {
                if (data.Id < 1)
                {
                    data.Id = 0;
                    Repository.Insert(data);
                    Repository.Save();
                    mediator.Publish(new CreateAdvertiseBasicEvent(data.Id, userId));
                }
                else if (data.TypeID == acc.TypeID || ((int)acc.Status > 4 && !acc.Childs.Any()))
                {
                    if (acc.Status == AdvertiseStatus.NotVerified)
                    {
                        acc.Status = AdvertiseStatus.ReadyToPublish;
                    }
                    director.Submit(ref acc);
                    Repository.Update(acc);
                    Repository.Save();
                    if (data.LocationType != acc.LocationType)
                    {
                        mediator.Publish(new ChangeAdvertisePositionEvent(data.Id));
                    }
                    mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
                }
                else
                {
                    errors.Add("TypeID", null);
                    groupErrors.Add("امکان تغییر نوع آگهی وجود ندارد");
                }
            }
            return director;
        }

        public AdvertiseDirector GetGeneralForm(long id, out bool isEdit, out int level)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            isEdit = acc != null && !(acc.Status == AdvertiseStatus.NotCompleted || acc.Status == AdvertiseStatus.Unset);
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : ((int)acc.OwnershipType < 1 ? 3 : 4));
            var director = new AdvertiseDirector(acc, DirectorType.General);
            return director;
        }

        public AdvertiseDirector SubmitGeneralForm(Advertise data, out Dictionary<string, string> errors,
            out List<string> groupErrors, out int level, string rootPath, bool isEdit = false)
        {
            var acc = Repository.Find(data.Id);
            var shallowAcc = acc.ShallowCopy();
            data.TypeID = acc.TypeID;
            data.MetaDescription = acc.MetaDescription;
            data.MetaTitle = acc.MetaTitle;
            data.Slug = acc.Slug;
            data.Mode = acc.Mode;
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : ((int)acc.OwnershipType < 1 ? 3 : 4));
            var director = new AdvertiseDirector(data, DirectorType.General);
            if (director.Validate(out errors, out groupErrors) == false)
                return director;
            if (data.Id < 1)
            {
                return director;
            }
            bool hasImportantChange = false;
            if (isEdit)
            {
                hasImportantChange = director.HasImpotantChange(acc);
            }
            director.Submit(ref acc);

            var removedPhotoIds = new List<long>();
            var photoPart = director.GetAdvertisePart<PhotoPart>();
            var photoIds = photoPart.AlbumPhotosArray;
            if (!acc.Photos.Select(s => s.Id).SequenceEqual(photoIds) || acc.MainPhotoId != photoPart.MainPhotoId)
            {
                if (photoIds != null && photoIds.Count() > 0)
                {
                    removedPhotoIds = acc.Photos.Select(s => s.Id).Except(photoIds).ToList();
                    acc.Photos.Clear();
                    foreach (var photoId in photoIds)
                    {
                        acc.Photos.Add(Repository.Find<File, long>(photoId));
                    }
                }
                else
                {
                    removedPhotoIds = acc.Photos.Select(s => s.Id).ToList();
                    acc.Photos.Clear();
                }
            }

            var prevStatus = acc.Status;
            if (isEdit)
            {
                switch (acc.Status)
                {
                    case AdvertiseStatus.FirstReady:
                    case AdvertiseStatus.NotCompleted:
                        break;
                    default:
                        if (hasImportantChange || acc.Status == AdvertiseStatus.NotVerified)
                        {
                            acc.Status = AdvertiseStatus.ReadyToPublish;
                            mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, prevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAcc, acc));
                        }
                        break;
                }
                mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            }
            acc.LastModifiedDate = DateTime.Now;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new CreateAdvertiseGeneralEvent(acc.Id));
            mediator.Publish(new ChangeAdvertiseAddressEvent(shallowAcc, acc));
            if (removedPhotoIds.Any())
            {
                mediator.Send(new RemovePhotosByFileIdsCommand(acc.Id, removedPhotoIds)).Wait();
            }
            mediator.Send(new RenameAdvertisePhotosCommand(acc.Id)).Wait();
            mediator.Send(new GenerateThumbImageCommand(acc.Id, acc.MainPhotoId,
                    acc.Photos.Select(s => s.Id).ToList())).Wait();
            return director;
        }

        public AdvertiseDirector GetExtraForm(long id, out bool isEdit, out int level)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            isEdit = acc != null && !(acc.Status == AdvertiseStatus.NotCompleted || acc.Status == AdvertiseStatus.Unset);
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : ((int)acc.OwnershipType < 1 ? 3 : 4));
            var director = new AdvertiseDirector(acc, DirectorType.Extra);
            return director;
        }

        public AdvertiseDirector SubmitExtraForm(Advertise data, out Dictionary<string, string> errors,
            out List<string> groupErrors, out int level, IFormFile uploadedLicenseFile, bool isEdit = false)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == data.Id));
            var oldAcc = acc.ShallowCopy();
            data.Mode = acc.Mode;
            data.TypeID = acc.TypeID;
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : ((int)acc.OwnershipType < 1 ? 3 : 4));
            bool hasImportantChange = false;
            var director = new AdvertiseDirector(data, DirectorType.Extra);

            if (director.Validate(out errors, out groupErrors) == false)
            {
                return director;
            }

            var licenseContentType = uploadedLicenseFile?.ContentType.ToLower();
            if (uploadedLicenseFile != null &&
                (licenseContentType == "image/png" ||
                licenseContentType == "image/jpg" ||
                licenseContentType == "image/jpeg") == false)
            {
                errors.Add("LicenseFileId", "فرمت فایل مجوز صحیح نمی باشد");
                groupErrors.Add("فرمت فایل مجوز صحیح نمی باشد");
                return director;
            }

            if (data.License == true && (uploadedLicenseFile == null && data.LicenseFileId == null))
            {
                errors.Add("LicenseFileId", "لطفا فایل مجوز خود را انتخاب کنید");
                groupErrors.Add("لطفا فایل مجوز خود را انتخاب کنید");
                return director;
            }

            if (data.Id < 1)
            {
                return director;
            }
            if (isEdit)
            {
                hasImportantChange = director.HasImpotantChange(acc);
            }
            long? licenseFileId = null;
            if (uploadedLicenseFile != null)
            {
                licenseFileId = mediator.Send(new UpdateAdvertiseLicenseFileCommand(uploadedLicenseFile, data.Id, acc.UserId, data.LicenseFileId)).Result;
                hasImportantChange = true;
            }
            director.Submit(ref acc);
            if (licenseFileId != null)
            {
                acc.LicenseFileId = licenseFileId;
            }
            var prevStatus = acc.Status;
            if (isEdit)
            {
                switch (acc.Status)
                {
                    case AdvertiseStatus.FirstReady:
                    case AdvertiseStatus.NotCompleted:
                        if (acc.Mode == AdvertiseMode.Single)
                        {
                            acc.Status = AdvertiseStatus.FirstReady;
                            mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, prevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(oldAcc, acc));
                        }
                        else if (acc.Childs.Any())
                        {
                            acc.Status = AdvertiseStatus.FirstReady;
                            mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, prevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(oldAcc, acc));
                        }
                        break;
                    default:
                        if (hasImportantChange || acc.Status == AdvertiseStatus.NotVerified)
                        {
                            acc.Status = AdvertiseStatus.ReadyToPublish;
                            mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, prevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(oldAcc, acc));
                        }
                        break;
                }
                mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            }
            else
            {
                if (acc.Mode == AdvertiseMode.Single)
                {
                    acc.Status = AdvertiseStatus.FirstReady;
                    mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, prevStatus));
                    mediator.Publish(new ChangeAdvertiseActiveEvent(oldAcc, acc));
                }
            }
            acc.LastModifiedDate = DateTime.Now;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeAdvertisePriceEvent(acc.Id, acc.NowruzPrice != oldAcc.NowruzPrice));
            if (acc.Mode == AdvertiseMode.Parent)
            {
                mediator.Publish(new ChangeAdvertiseRulesEvent(acc.Id));
            }
            return director;
        }

        public AdvertiseDirector GetHotelForm(long id, long parentId, out bool isEdit)
        {
            Advertise hotel = null;
            var parent = Repository.Query(q => q.FirstOrDefault(f => f.Id == parentId));
            isEdit = !(parent.Status == AdvertiseStatus.NotCompleted || parent.Status == AdvertiseStatus.Unset);
            if (id > 0)
                hotel = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            if (hotel == null)
            {
                hotel = parent.Childs.Any() ?
                    parent.Childs.FirstOrDefault().ShallowCopy() : new Advertise();
                hotel.TypeID = parent.TypeID;
                hotel.Title = null;
                hotel.UnitCount = 0;
            }
            var director = new AdvertiseDirector(hotel, DirectorType.HotelUnit);
            return director;
        }

        public AdvertiseDirector SubmitHotelForm(Advertise data, int userId, out Dictionary<string, string> errors,
            out List<string> groupErrors, bool save)
        {
            var parent = Repository.Query(q => q.FirstOrDefault(f => f.Id == data.ParentId));
            var oldParent = parent.ShallowCopy();
            data.TypeID = parent.TypeID;
            data.Mode = AdvertiseMode.Child;
            var director = new AdvertiseDirector(data, DirectorType.HotelUnit);
            bool changeNorouzPrice = false;
            if (director.Validate(out errors, out groupErrors))
            {
                if (data.Id > 0)
                {
                    var child = Repository.Query(q => q.FirstOrDefault(f => f.Id == data.Id));
                    var oldChild = child.ShallowCopy();
                    director.Submit(ref child);
                    changeNorouzPrice = child.NowruzPrice != oldChild.NowruzPrice;
                    child.LastModifiedDate = DateTime.Now;
                    Repository.Update(child);
                    if (parent.Status == AdvertiseStatus.NotCompleted || parent.Status == AdvertiseStatus.FirstReady)
                    {
                        if (!save)
                        {
                            var parentPrevStatus = parent.Status;
                            parent.Status = AdvertiseStatus.FirstReady;
                            Repository.Update(parent);
                            mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                        }
                    }
                    else
                    {
                        var parentPrevStatus = parent.Status;
                        parent.Status = director.HasImpotantChange(oldChild) || oldChild.Status == AdvertiseStatus.NotVerified ?
                            AdvertiseStatus.ReadyToPublish : parent.Status;
                        Repository.Update(parent);
                        mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                        mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                    }
                    Repository.Save();
                }
                else
                {
                    data.CreateDate = DateTime.Now;
                    data.LastModifiedDate = DateTime.Now;
                    data.UserId = userId;
                    if (parent.Status == AdvertiseStatus.NotCompleted || parent.Status == AdvertiseStatus.FirstReady)
                    {
                        if (save)
                        {
                            data.Status = parent.Status;
                            Repository.Insert(data);
                            Repository.Save();
                        }
                        else
                        {
                            var parentPrevStatus = parent.Status;
                            parent.Status = AdvertiseStatus.FirstReady;
                            Repository.Update(parent);
                            Repository.Insert(data);
                            Repository.Save();
                            mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                        }
                    }
                    else
                    {
                        var parentPrevStatus = parent.Status;
                        parent.Status = AdvertiseStatus.ReadyToPublish;
                        Repository.Update(parent);
                        Repository.Insert(data);
                        Repository.Save();
                        mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                        mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                    }
                    mediator.Publish(new AddHotelChildEvent(data.Id, (long)data.ParentId));
                }
                mediator.Publish(new ChangeAdvertisePriceEvent(data.Id, changeNorouzPrice));
                mediator.Send(new RemoveAdvertiseCacheCommand(parent.Id));
            }
            return director;
        }

        public AdvertiseDirector GetComplexForm(long id, long parentId, out AdvertiseType parentType, out bool isEdit)
        {
            Advertise hotel = null;
            var parent = Repository.Query(q => q.FirstOrDefault(f => f.Id == parentId));
            parentType = parent.TypeID;
            isEdit = !(parent.Status == AdvertiseStatus.NotCompleted || parent.Status == AdvertiseStatus.Unset);
            if (id > 0)
                hotel = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            if (hotel == null)
            {
                hotel = new Advertise();
                hotel.TypeID = AdvertiseType.None;
            }
            var director = new AdvertiseDirector(hotel, DirectorType.ComplexUnit);
            return director;
        }

        public AdvertiseDirector SubmitComplexForm(Advertise data, int userId, out Dictionary<string, string> errors,
            out List<string> groupErrors, bool save, out AdvertiseType parentType, string rootPath)
        {
            var parent = Repository.Query(q => q.FirstOrDefault(f => f.Id == data.ParentId));
            parentType = AdvertiseType.None;
            Advertise oldParent = new Advertise();
            if (parent != null)
            {
                oldParent = parent.ShallowCopy();
                parentType = parent.TypeID;
            }
            data.Mode = AdvertiseMode.Child;
            var director = new AdvertiseDirector(data, DirectorType.ComplexUnit);
            bool changeNorouzPrice = false;
            if (director.Validate(out errors, out groupErrors))
            {
                if (data.Id > 0)
                {
                    var child = Repository.Query(q => q.FirstOrDefault(f => f.Id == data.Id));
                    var oldChild = child.ShallowCopy();
                    director.Submit(ref child);
                    changeNorouzPrice = child.NowruzPrice != oldChild.NowruzPrice;
                    var removedPhotoIds = new List<long>();
                    var photoPart = director.GetAdvertisePart<PhotoPart>();
                    var photoIds = photoPart.AlbumPhotosArray;
                    var hasPhotoChange = !child.Photos.Select(s => s.Id).SequenceEqual(photoIds);
                    if (hasPhotoChange)
                    {
                        if (photoIds != null && photoIds.Count() > 0)
                        {
                            removedPhotoIds = child.Photos.Select(s => s.Id).Except(photoIds).ToList();
                            child.Photos.Clear();
                            foreach (var photoId in photoIds)
                            {
                                child.Photos.Add(Repository.Find<File, long>(photoId));
                            }
                        }
                        else
                        {
                            removedPhotoIds = child.Photos.Select(s => s.Id).ToList();
                            child.Photos.Clear();
                        }
                    }
                    child.LastModifiedDate = DateTime.Now;
                    Repository.Update(child);
                    if (parent.Status == AdvertiseStatus.NotCompleted || parent.Status == AdvertiseStatus.FirstReady)
                    {
                        if (!save)
                        {
                            var parentPrevStatus = parent.Status;
                            parent.Status = AdvertiseStatus.FirstReady;
                            Repository.Update(parent);
                            mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                        }
                        mediator.Publish(new CreateAdvertiseGeneralEvent(child.Id));
                    }
                    else
                    {
                        var parentPrevStatus = parent.Status;
                        parent.Status = director.HasImpotantChange(oldChild) || oldChild.Status == AdvertiseStatus.NotVerified ?
                            AdvertiseStatus.ReadyToPublish : parent.Status;
                        Repository.Update(parent);
                        mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                        mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                    }
                    Repository.Save();
                    if (hasPhotoChange)
                    {
                        if (removedPhotoIds.Any())
                        {
                            mediator.Send(new RemovePhotosByFileIdsCommand(child.Id, removedPhotoIds)).Wait();
                        }
                        mediator.Send(new RenameAdvertisePhotosCommand(child.Id)).Wait();
                        mediator.Send(new GenerateThumbImageCommand(child.Id, child.MainPhotoId,
                            child.Photos.Select(s => s.Id).ToList())).Wait();
                    }
                }
                else
                {
                    data.CreateDate = DateTime.Now;
                    data.LastModifiedDate = DateTime.Now;
                    data.UserId = userId;
                    data.ProvinceId = parent.ProvinceId;
                    data.CityId = parent.CityId;
                    data.AreaId = parent.AreaId;
                    var photoIds = director.GetAdvertisePart<PhotoPart>().AlbumPhotosArray;
                    if (photoIds != null && photoIds.Count() > 0)
                    {
                        data.Photos = new List<File>();
                        foreach (var photoId in photoIds)
                        {
                            data.Photos.Add(Repository.Find<File, long>(photoId));
                        }
                    }
                    if (parent.Status == AdvertiseStatus.NotCompleted || parent.Status == AdvertiseStatus.FirstReady)
                    {
                        if (save)
                        {
                            data.Status = parent.Status;
                            Repository.Insert(data);
                            Repository.Save();
                        }
                        else
                        {
                            var parentPrevStatus = parent.Status;
                            parent.Status = AdvertiseStatus.FirstReady;
                            Repository.Update(parent);
                            Repository.Insert(data);
                            Repository.Save();
                            mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                            mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                        }
                    }
                    else
                    {
                        var parentPrevStatus = parent.Status;
                        parent.Status = AdvertiseStatus.ReadyToPublish;
                        Repository.Update(parent);
                        Repository.Insert(data);
                        Repository.Save();
                        mediator.Publish(new ChangeAdvertiseStatusEvent(parent.Id, parentPrevStatus));
                        mediator.Publish(new ChangeAdvertiseActiveEvent(oldParent, parent));
                    }
                    if (photoIds != null && photoIds.Count() > 0)
                    {
                        mediator.Send(new RenameAdvertisePhotosCommand(data.Id)).Wait();
                        mediator.Send(new GenerateThumbImageCommand(data.Id, data.MainPhotoId,
                            data.Photos.Select(s => s.Id).ToList())).Wait();
                    }
                    mediator.Publish(new AddComplexChildEvent(data.Id, (long)data.ParentId));
                }
                mediator.Publish(new ChangeAdvertisePriceEvent(data.Id, changeNorouzPrice));
                mediator.Send(new RemoveAdvertiseCacheCommand(data.Id));
            }
            return director;
        }

        public AdvertiseDirector GetAdminForm(long id, DirectorType type, out AdvertiseType parentType, out AdvertiseStatus status)
        {
            var acc = Repository.Find(id);
            status = acc.Status;
            parentType = acc.Mode == AdvertiseMode.Child ? acc.Parent.TypeID : acc.TypeID;
            var director = new AdvertiseDirector(acc, type);
            return director;
        }

        public AdvertiseDirector SubmitAdminBasicForm(Advertise data, out Dictionary<string, string> errors,
            out List<string> groupErrors, int currentUserId)
        {
            errors = new Dictionary<string, string>();
            groupErrors = new List<string>();
            switch (data.TypeID)
            {
                case AdvertiseType.Hotel:
                case AdvertiseType.Camp:
                case AdvertiseType.TourismAccommodation:
                case AdvertiseType.Inn:
                case AdvertiseType.Pansion:
                case AdvertiseType.HotelApartment:
                case AdvertiseType.Complex:
                    data.Mode = AdvertiseMode.Parent;
                    break;
                default:
                    data.Mode = AdvertiseMode.Single;
                    break;
            }

            var director = new AdvertiseDirector(data, DirectorType.Basic);
            if (director.Validate(out errors, out groupErrors))
            {
                var acc = Repository.Find(data.Id);
                var shallowData = acc.ShallowCopy();
                if (director.AdvertiseType != acc.TypeID && acc.Childs.Any())
                {
                    foreach (var item in acc.Childs)
                    {
                        item.Status = AdvertiseStatus.Deleted;
                    }
                }
                director.Submit(ref acc);
                Repository.Update(acc);
                Repository.Save();
                mediator.Publish(new AdvertiseUpdateEvent(shallowData, acc, ActionLog.ActionSourceEnum.AdminPanel, currentUserId));
                mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            }
            return director;
        }

        public AdvertiseDirector SubmitAdminForm(Advertise data, out Dictionary<string, string> errors,
            out List<string> groupErrors, bool forceSave, DirectorType type, int currentUserId,
            out AdvertiseType parentType, out AdvertiseStatus status, IFormFile uploadedLicenseFile = null)
        {
            var acc = Repository.Find(data.Id);
            status = acc.Status;
            var shallowData = acc.ShallowCopy();
            data.Mode = acc.Mode;
            if (acc.Mode != AdvertiseMode.Child)
            {
                data.TypeID = acc.TypeID;
            }
            parentType = acc.Mode == AdvertiseMode.Child ? acc.Parent.TypeID : acc.TypeID;
            errors = new Dictionary<string, string>();
            groupErrors = new List<string>();
            var director = new AdvertiseDirector(data, type);

            if (forceSave == false && (type == DirectorType.General || type == DirectorType.ComplexUnit))
            {
                if (string.IsNullOrEmpty(data.MetaTitle) || string.IsNullOrEmpty(data.MetaDescription) || string.IsNullOrEmpty(data.Slug))
                {
                    errors.Add("Slug", null);
                    errors.Add("MetaTitle", null);
                    errors.Add("MetaDescription", null);
                    groupErrors.Add("متای عنوان و توضیحات گوگل را وارد کنید");
                    return director;
                }
            }

            bool validate = director.Validate(out errors, out groupErrors);
            if (forceSave == false && validate == false)
                return director;

            if (type == DirectorType.General && (errors.Keys.Contains("Province") || errors.Keys.Contains("City")))
                return director;

            var licenseContentType = uploadedLicenseFile?.ContentType.ToLower();
            if (uploadedLicenseFile != null &&
                (licenseContentType == "image/png" ||
                licenseContentType == "image/jpg" ||
                licenseContentType == "image/jpeg") == false)
            {
                groupErrors.Add("فرمت فایل مجوز صحیح نمی باشد");
                return director;
            }

            if (data.License == true && (uploadedLicenseFile == null && data.LicenseFileId == null))
            {
                groupErrors.Add("لطفا فایل مجوز خود را انتخاب کنید");
                return director;
            }

            long? licenseFileId = null;
            if (uploadedLicenseFile != null)
            {
                licenseFileId = mediator.Send(new UpdateAdvertiseLicenseFileCommand(uploadedLicenseFile, data.Id, data.UserId, data.LicenseFileId)).Result;
            }
            director.Submit(ref acc);
            if (licenseFileId != null)
            {
                acc.LicenseFileId = licenseFileId;
            }

            if (type == DirectorType.General || type == DirectorType.ComplexUnit)
            {
                var removedPhotoIds = new List<long>();
                var photoPart = director.GetAdvertisePart<PhotoPart>();
                var photoIds = photoPart.AlbumPhotosArray;
                var hasPhotoChange = !acc.Photos.Select(s => s.Id).SequenceEqual(photoIds);
                if (hasPhotoChange)
                {
                    if (photoIds != null && photoIds.Count() > 0)
                    {
                        removedPhotoIds = acc.Photos.Select(s => s.Id).Except(photoIds).ToList();
                        acc.Photos.Clear();
                        foreach (var photoId in photoIds)
                        {
                            acc.Photos.Add(Repository.Find<File, long>(photoId));
                        }
                    }
                    else
                    {
                        removedPhotoIds = acc.Photos.Select(s => s.Id).ToList();
                        acc.Photos.Clear();
                    }
                    if (removedPhotoIds.Any())
                    {
                        mediator.Send(new RemovePhotosByFileIdsCommand(acc.Id, removedPhotoIds)).Wait();
                    }
                    mediator.Send(new RenameAdvertisePhotosCommand(data.Id)).Wait();
                    mediator.Send(new GenerateThumbImageCommand(data.Id, acc.MainPhotoId,
                            acc.Photos.Select(s => s.Id).ToList())).Wait();
                }
                mediator.Publish(new CreateAdvertiseGeneralEvent(acc.Id, true));
            }

            if (type == DirectorType.Extra || type == DirectorType.ComplexUnit ||
                type == DirectorType.HotelUnit)
            {
                mediator.Publish(new ChangeAdvertisePriceEvent(acc.Id, acc.NowruzPrice != shallowData.NowruzPrice));
            }

            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new AdvertiseUpdateEvent(shallowData, acc, ActionLog.ActionSourceEnum.AdminPanel, currentUserId));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return director;
        }

        public Dictionary<AdvertiseType, Dictionary<long, string>> GetAccChilds(long parentId)
        {
            Dictionary<AdvertiseType, Dictionary<long, string>> childsDic = new Dictionary<AdvertiseType, Dictionary<long, string>>();
            Dictionary<long, string> dic = null;
            var data = Repository.Query(q => q.Where(w => w.ParentId == parentId).OrderBy(o => o.TypeID));
            var existTypes = data.Select(s => s.TypeID).Distinct().ToList();
            foreach (var type in existTypes)
            {
                dic = new Dictionary<long, string>();
                var typeChilds = data.Where(w => w.TypeID == type).ToList();
                foreach (var child in typeChilds)
                {
                    dic.Add(child.Id, child.Title);
                }
                childsDic.Add(type, dic);
            }
            return childsDic;
        }

        public Advertise Insert(Advertise advertise)
        {
            Repository.Insert(advertise);
            Repository.Save();
            return advertise;
        }

        public IDictionary<string, DatePriceDTO> GetAccPriceDatesInfo(long id)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            long couponCalculationPrice;
            var priceDict = priceCalculator.CalculateJalaliDatePrices(
                DateTime.Now.Date, DateTime.Now.Date.AddDays(180),
                acc, out couponCalculationPrice);
            return priceDict;
        }

        public void SetNorouzMinReserveDate(long id, long dateUnix)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var prevAcc = acc.ShallowCopy();
            acc.MinReserveDateForNowruz = dateUnix;
            foreach (var child in acc.Childs)
            {
                child.MinReserveDateForNowruz = dateUnix;
            }
            if (acc.Status != AdvertiseStatus.NotCompleted && acc.Status != AdvertiseStatus.FirstReady && dateUnix > 0)
                acc.Status = (int)AdvertiseStatus.ReadyToPublish;
            Repository.Update(acc);
            Repository.Save();
            if (dateUnix > 0)
            {
                mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
            }
        }

        public void SetAvailable(long id, bool isAvailable)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var prevAcc = acc.ShallowCopy();
            acc.Active = isAvailable;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public void Publish(long id, int doerUserId, ActionSourceEnum actionSource)
        {
            var acc = Repository.Find(id);
            var prevAcc = acc.ShallowCopy();
            acc.Status = AdvertiseStatus.Published;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
            mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
            mediator.Publish(new AdvertiseUpdateEvent(prevAcc, acc, actionSource, doerUserId));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public void Suspend(long id)
        {
            var acc = Repository.Find(id);
            var prevAcc = acc.ShallowCopy();
            acc.Status = AdvertiseStatus.Archived;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
            mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public async Task<ServiceResult<AdvertiseStatus>> UpdateActivity(long residenceId)
        {
            var serviceResult = new ServiceResult<AdvertiseStatus>();
            var residence = await Repository.FindAsync(residenceId);
            if (residence == null ||
                (residence.Status == AdvertiseStatus.Published || residence.Status == AdvertiseStatus.Archived) == false)
            {
                serviceResult.AddError("آگهی اشتباه است");
                return serviceResult;
            }

            serviceResult.Result = await UpdateStatus(residence.Id,
                residence.Status == AdvertiseStatus.Published ? AdvertiseStatus.Archived : AdvertiseStatus.Published);
            return serviceResult;

            //if (residence.Status == AdvertiseStatus.Archived)
            //{
            //    residence.Status = AdvertiseStatus.Published;
            //    Repository.Update(residence);
            //    Repository.Save();
            //    mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
            //    mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, residence));
            //}
            //else if (residence.Status == AdvertiseStatus.Published)
            //{
            //    residence.Status = AdvertiseStatus.Archived;
            //    Repository.Update(residence);
            //    Repository.Save();
            //    mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
            //    mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, residence));
            //}
            //mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
            //return residence.Status;
        }

        public void NotVerify(long id, int currentUserId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var prevAcc = acc.ShallowCopy();
            acc.Status = AdvertiseStatus.NotVerified;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
            mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
            if (currentUserId > 0)
            {
                mediator.Publish(new AdvertiseUpdateEvent(prevAcc, acc, ActionLog.ActionSourceEnum.AdminPanel, currentUserId));
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public void SetNotVerifyReasons(long id, List<Advertise.NotVerifyReasonsEnum> reasons)
        {
            if (reasons == null)
            {
                reasons = new List<NotVerifyReasonsEnum>();
            }
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.SetNotVerifyReasons(reasons);
            Repository.Update(acc);
            Repository.Save();
        }

        public void SetAsTodayEmpty(long id)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.EmptyTonight = true;
            Repository.Update(acc);
            Repository.Save();
            mediator.Send(new RemoveCategoryItemCacheCommand(acc.Id));
        }

        public void UnsetTodayEmpty(long id)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.EmptyTonight = false;
            Repository.Update(acc);
            Repository.Save();
            mediator.Send(new RemoveCategoryItemCacheCommand(acc.Id));
        }

        public Dictionary<string, string> GetAdvertiseListPrices(List<long> ids)
        {
            var accs = Repository.Query(q => q.Where(w => ids.Contains(w.Id)));
            var result = new Dictionary<string, string>();
            foreach (var acc in accs)
            {
                var price = acc.BasePrice;
                result.Add(acc.Id.ToString(), string.Format("{0:n0}", price) + " تومان");
            }
            return result;
        }

        public void SetStayDuration(long residenceId, int min, int max)
        {
            var residence = Repository.Find(residenceId);
            residence.MinReserveDuration = min;
            residence.MaxReserveDuration = max;
            Repository.Update(residence);
            Repository.Save();
            mediator.Publish(new ChangeStayDurationEvent(residenceId));
            mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
        }

        public void SetNorouzPrice(long id, int norouzPrice, int overCapacityPrice = 0)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.NowruzPrice = norouzPrice;
            acc.NowruzExtraCapacityPrice = overCapacityPrice;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeNorouzPriceEvent(id));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public void SetMaxInstantReserveStart(long id, int maxInstantReserveStart)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.MaxInstantReserveStartTimeInterval = maxInstantReserveStart;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeMaxInstantReserveStartEvent(id));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public IList<Advertise> GetAdvertiseRelatedItems(long id, int count = 4)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var publishedState = AdvertiseStatus.Published;
            var accs = Repository.Query(q => q.Where(w =>
                w.Status == publishedState && w.Active &&
                w.UnitCount == 0 && w.UserId != acc.UserId));

            var price_max_difference = acc.DailyPrice * 0.25f; //25 percent of difference to the Advertise

            var model = accs.
                OrderByDescending(x => x.AreaId == acc.AreaId).
                ThenByDescending(x => x.CityId == acc.CityId).
                ThenByDescending(x => x.ProvinceId == acc.ProvinceId).
                ThenByDescending(x => x.CountryDirection == acc.CountryDirection).
                ThenByDescending(x => x.TypeID == acc.TypeID). //sort by advertise type
                ThenByDescending(x => Math.Abs(x.DailyPrice - acc.DailyPrice) <= price_max_difference).
                ThenByDescending(x => x.DailyPrice >= acc.DailyPrice).
                ThenBy(x => Math.Abs(x.DailyPrice - acc.DailyPrice) <= price_max_difference ? 0 : x.DailyPrice).
                ThenByDescending(x => x.ResidenceScore).
                Take(count).ToList();
            return model;
        }

        public IEnumerable<Advertise> GetMostViewedAdvertisesInCity(int city_id, int province_id, int type_id, int count)
        {
            var advertises = Repository.Query(q => q);
            if (city_id > 0)
            {
                advertises = advertises.Where(x => x.CityId == city_id && x.Status == AdvertiseStatus.Published &&
                x.Active && x.UnitCount == 0 && !x.HideInSearch);
            }
            else if (province_id > 0)
            {
                advertises = advertises.Where(x => x.ProvinceId == province_id && x.Status == AdvertiseStatus.Published &&
                x.Active && x.UnitCount == 0 && !x.HideInSearch);
            }
            if (type_id != (int)AdvertiseType.All)
            {
                advertises = advertises.Where(x => x.TypeID == (AdvertiseType)type_id);
            }
            return advertises.OrderByDescending(x => x.ResidenceScore).Take(count).ToList();
        }

        // for Norouz - commented at AdvertiseApi.GetHomePageCarousels
        public IList<Advertise> GetMostViewedNorouzAdvertises(int count)
        {
            IQueryable<Advertise> advertises = Repository.Query(q => q.Where(
                x => x.NowruzPrice > 0 &&
                x.Status == AdvertiseStatus.Published &&
                x.Active && x.UnitCount == 0 && !x.HideInSearch));
            return advertises.OrderByDescending(x => x.ResidenceScore)
                .Take(count).ToList();
        }

        public bool IsReserveAvailable(long advertiseId, string fromDate, string toDate,
            int numberOfGuests, out bool isOccupied,
            out bool guestsOutOfRange, out List<string> occupiedDates)
        {
            var advertise = Repository.Query(q => q.FirstOrDefault(f => f.Id == advertiseId));
            List<string> range = DateTimeUtility.PersianDateRangeToList(fromDate, toDate, true, false);
            occupiedDates = advertise.OccupiedDates().Select(
                s => DateTimeUtility.GregorianToPersianDate(s)).Intersect(range).ToList();
            isOccupied = occupiedDates.Any();
            guestsOutOfRange = numberOfGuests > advertise.Capacity + advertise.ExtraCapacity;
            return !isOccupied && !guestsOutOfRange;
        }

        public IList<string> GetOccupiedDatesInRange(long advertiseId, string persianFrom, string persianTo)
        {
            var acc = Repository.Find(advertiseId);
            var occupiedDates = acc.OccupiedDates().Select(s => DateTimeUtility.GregorianToPersianDate(s));
            var intersects = DateTimeUtility.PersianDateRangeToList(persianFrom, persianTo, true, false)
                .Intersect(occupiedDates);
            return intersects.ToList();
        }

        public async Task<bool> UpdateInstantReserveStatus(long residenceId, InstantReserveStatusEnum status)
        {
            var data = await Repository.FindAsync(residenceId);
            data.InstantReserveStatus = status;
            if (data.Mode == AdvertiseMode.Parent)
            {
                foreach (var item in data.Childs)
                {
                    item.InstantReserveStatus = status;
                }
            }
            Repository.Update(data);
            Repository.Save();
            await mediator.Send(new RemoveAdvertiseCacheCommand(residenceId));
            return true;
        }

        public CheckUnsetOccupiedDTO CheckUnsetOccupiedDateRange(long advertiseId,
            string from_date, string to_date)
        {
            var acc = Repository.Find(advertiseId);
            var reservedDates = acc.ReservedDates().Select(s => DateTimeUtility.GregorianToPersianDate(s));
            var reservedIntersects = DateTimeUtility.PersianDateRangeToList(from_date, to_date, true, false)
                .Intersect(reservedDates);
            if (reservedIntersects.Any())
            {
                var dates_string = string.Join(" - ", reservedIntersects);
                return new CheckUnsetOccupiedDTO()
                {
                    Result = CheckUnsetOccupiedDTO.CheckUnsetOccupiedResult.ContainsReserved,
                    FailedDates = reservedIntersects
                };
            }
            return new CheckUnsetOccupiedDTO()
            {
                Result = CheckUnsetOccupiedDTO.CheckUnsetOccupiedResult.OK
            };
        }

        public CheckSetOccupiedDTO CheckSetAsOccupiedDateRange(long advertiseId,
            string from_date, string to_date)
        {
            var acc = Repository.Find(advertiseId);
            var reservedDates = acc.ReservedDates().Select(s => DateTimeUtility.GregorianToPersianDate(s));
            var reservedIntersects = DateTimeUtility.PersianDateRangeToList(from_date, to_date, true, false)
                .Intersect(reservedDates);
            if (reservedIntersects.Any())
            {
                return new CheckSetOccupiedDTO()
                {
                    Result = CheckSetOccupiedDTO.CheckSetOccupiedResult.ContainsReserved,
                    FailedDates = reservedIntersects
                };
            }
            var acceptedReserveDates = acc.AcceptedReserveDates().Select(
                s => DateTimeUtility.GregorianToPersianDate(s));
            var acceptedReservesIntersects = DateTimeUtility.PersianDateRangeToList(from_date, to_date, true, false)
                .Intersect(acceptedReserveDates);
            if (acceptedReservesIntersects.Any())
            {
                return new CheckSetOccupiedDTO()
                {
                    Result = CheckSetOccupiedDTO.CheckSetOccupiedResult.ContainsAcceptedRerserve,
                    FailedDates = acceptedReservesIntersects
                };
            }
            var reserveRequestDates = acc.ReserveRequestDates().Select(
                s => DateTimeUtility.GregorianToPersianDate(s));
            var reserveRequestIntersects = DateTimeUtility.PersianDateRangeToList(
                from_date, to_date, true, false).Intersect(reserveRequestDates);
            if (reserveRequestIntersects.Any())
            {
                return new CheckSetOccupiedDTO()
                {
                    Result = CheckSetOccupiedDTO.CheckSetOccupiedResult.ContainsReserveRequest,
                    FailedDates = reserveRequestIntersects
                };
            }
            return new CheckSetOccupiedDTO()
            {
                Result = CheckSetOccupiedDTO.CheckSetOccupiedResult.OK
            };
        }

        public bool CheckReserve(int currentUserId, long advertiseId, int guestCount,
            string startDate, string endDate, out string msg, out bool isInstantReserve)
        {
            isInstantReserve = false;
            var advertise = Repository.Find(advertiseId);
            var user = Repository.Find<User, int>(currentUserId);
            var haveReservedRequest = false;
            if (user != null && user.Reserves != null)
            {
                haveReservedRequest = user.Reserves.Any(a => a.GetStateCategory() == ReserveCategory.Reserved ||
                    a.GetStateCategory() == ReserveCategory.Finished);
            }
            //if (((advertise.Mode == AdvertiseMode.Child && advertise.Parent.License == false) ||
            //    (advertise.Mode != AdvertiseMode.Child && advertise.License == false)) &&
            //    advertise.IsForbidden && haveReservedRequest == false)
            //{
            //    msg = "کاربر گرامی، طبق دستور قضایی، رزرو اقامتگاه در اصفهان فقط برای اماکن دارای مجوز از سازمان گردشگری امکان پذیر است.";
            //    return false;
            //}
            if (advertise.Status != AdvertiseStatus.Published)
            {
                msg = "متاسفانه این اقامتگاه در حال حاضر از دسترس خارج است. لطفا اقامتگاه دیگری انتخاب نمایید";
                return false;
            }
            if (guestCount < 1)
            {
                msg = "لطفا تعداد نفرات را وارد کنید";
                return false;
            }
            if (string.IsNullOrEmpty(startDate))
            {
                msg = "لطفا تاریخ شروع سفر را انتخاب کنید";
                return false;
            }
            if (string.IsNullOrEmpty(endDate))
            {
                msg = "لطفا تاریخ پایان سفر را انتخاب کنید";
                return false;
            }
            if (startDate == endDate)
            {
                msg = "تاریخ ورود و تاریخ خروج نمیتوانند یکی باشند";
                return false;
            }
            if (DateTimeUtility.PersianDateToGregorian(startDate) > DateTimeUtility.PersianDateToGregorian(endDate))
            {
                msg = "تاریخ ورود نمیتواند از تاریخ خروج بیشتر باشد. لطفا اصلاح کنید";
                return false;
            }
            var days = DateTimeUtility.GetPersianDateRangeDays(startDate, endDate);
            if (advertise.MinReserveDuration > 0 && days < advertise.MinReserveDuration)
            {
                msg = "برای رزرو این اقامتگاه باید حداقل " + advertise.MinReserveDuration + "  شب اقامت کنید. برای اقامت " + days + " شبه میتوانید اقامتگاه های دیگر را رزرو کنید";
                return false;
            }
            if (advertise.MaxReserveDuration > 0 && days > advertise.MaxReserveDuration)
            {
                msg = "شما میتوانید حداکثر " + advertise.MaxReserveDuration + "  شب در این اقامتگاه اقامت کنید. برای اقامت طولانی تر میتوانید اقامتگاه های دیگر را رزرو کنید";
                return false;
            }
            var todayUnix = DateTimeUtility.DateValueOfJS(DateTime.Now.Date);
            if (advertise.MinReserveDateForNowruz > todayUnix &&
                DateTimeUtility.IsNorouz(DateTimeUtility.PersianDateRangeToList(startDate, endDate, true, false)))
            {
                var minDateString = DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(advertise.MinReserveDateForNowruz));
                msg = "برای رزرو نوروزی این اقامتگاه میتوانید از تاریخ " + minDateString + " اقدام کنید و یا اقامتگاه های دیگر را رزرو کنید";
                return false;
            }
            var startDateGregorian = DateTimeUtility.PersianDateToGregorian(startDate);
            var endDateGregorian = DateTimeUtility.PersianDateToGregorian(endDate);
            var minDate = DateTime.Now.TimeOfDay.Hours > 3 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(-1);
            if (startDateGregorian < minDate || endDateGregorian <= minDate)
            {
                msg = "تاریخ ورود و خروج گذشته است. لطفا زمان درست انتخاب کنید";
                return false;
            }
            var occupiedDates = advertise.OccupiedDates().Select(s => DateTimeUtility.GregorianToPersianDate(s));
            var intersects = DateTimeUtility.PersianDateRangeToList(startDate, endDate, true, false)
                .Intersect(occupiedDates);
            if (intersects.Any())
            {
                msg = "متاسفانه بعضی از روز های انتخاب شده پر هستند";
                return false;
            }
            long priceWithoutDiscount, couponCalPrice;
            var total_price = priceCalculator.CalculateReservePrice(advertise,
                startDate, endDate, guestCount, out priceWithoutDiscount,
                out couponCalPrice);
            long depositePrice;
            if (days > 3)
            {
                depositePrice = (long)Math.Round(total_price * 0.3f);
            }
            else
            {
                var deposite = (long)Math.Round((double)total_price / (double)days);
                depositePrice = (long)(Math.Max(Math.Round(deposite / 1000f, 0), 1) * 1000);
            }

            if (currentUserId > 0 && advertise.UnitCount < 1 &&
                Repository.Find<User, int>(currentUserId).
                UserHasSimilarReserve(advertiseId,
                startDateGregorian, endDateGregorian))
            {
                msg = "شما یک درخواست مشابه برای این آگهی دارید، برای درخواست جدید درخواست قبلی را لغو کنید";
                return false;
            }
            isInstantReserve = advertise.IsReserveInstant(startDateGregorian, endDateGregorian);
            msg = "در صورت موافقت روی دکمه ثبت کلیک کنید";
            return true;
        }

        public long GetReservePrice(long advertiseId,
            string startDate, string endDate, int guestCount,
            out long priceWithoutDiscount,
            out long couponCalculationPrice)
        {
            var advertise = Repository.Find(advertiseId);
            return priceCalculator.CalculateReservePrice(advertise,
                startDate, endDate, guestCount, out priceWithoutDiscount,
                out couponCalculationPrice);
        }

        public bool AddAdvertiseComment(int userId, long advertiseId, string text,
            out string cannotAddReason, int? operatorId = null)
        {
            var advertise = Repository.Find(advertiseId);
            var user = Repository.Find<User, int>(userId);
            var found_comment = advertise.Comments.FirstOrDefault(f =>
                  f.SenderUserID == userId &&
                  f.type == Comment.CommentType.advertise);
            Reserve lastReserve = user.Reserves.OrderByDescending(
                    x => x.EndDate).FirstOrDefault(x => x.UserID == userId &&
                    x.AdvertiseID == advertiseId);
            bool canAddOrEdit;
            cannotAddReason = null;
            if (found_comment != null) //edit
            {
                canAddOrEdit = lastReserve != null &&
                    (DateTime.Now - lastReserve.EndDate).TotalDays <= 30;
                if (canAddOrEdit)
                {
                    found_comment.Text = text;
                    found_comment.Status = (int)Comment.CommentStatus.ready;
                    found_comment.LastModifyDate = DateTime.Now;
                    found_comment.LastModifyDatetick = DateTime.Now.Ticks;
                    found_comment.OperatorID = operatorId;
                    advertise.LastModifiedDate = DateTime.Now;
                    Repository.Update(advertise);
                    Repository.Save();
                }
                else
                {
                    cannotAddReason = "بیشتر از یک ماه از آخرین رزرو شما از این اقامتگاه گذشته و شما نمیتوانید نظر را ویرایش کنید";
                }
            }
            else //add
            {
                canAddOrEdit = lastReserve != null &&
                    (DateTime.Now - lastReserve.EndDate).TotalDays <= 7;
                if (canAddOrEdit)
                {
                    var comment = new Comment();
                    comment.SenderUserID = userId;
                    comment.Status = Comment.CommentStatus.ready;
                    comment.type = Comment.CommentType.advertise;
                    comment.Text = text;
                    comment.CreateDate = DateTime.Now;
                    comment.LastModifyDate = DateTime.Now;
                    comment.LastModifyDatetick = DateTime.Now.Ticks;
                    comment.OperatorID = operatorId;
                    advertise.Comments.Add(comment);
                    advertise.LastModifiedDate = DateTime.Now;
                    Repository.Update(advertise);
                    Repository.Save();
                }
                else
                {
                    cannotAddReason = "بیشتر از یک هفته از آخرین رزرو شما از این اقامتگاه گذشته و شما نمیتوانید در مورد این اقامنگاه نظر ثبت کنید";
                }
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
            return canAddOrEdit;
        }

        public void AddAdvertiseHostReplyComment(int userId, long advertiseId,
            string text, int? operatorId = null)
        {
            var advertise = Repository.Find(advertiseId);
            var user = Repository.Find<User, int>(userId);
            var operatorUser = operatorId == null ? null :
                Repository.Find<User, int>((int)operatorId);
            var guestComment = advertise.Comments.FirstOrDefault(f =>
                  f.SenderUserID == userId && f.type == Comment.CommentType.advertise);
            var hostReply = guestComment.HostReply;
            if (hostReply != null) //edit
            {
                hostReply.Text = text;
                hostReply.Status = (int)Comment.CommentStatus.ready;
                hostReply.LastModifyDate = DateTime.Now;
                hostReply.LastModifyDatetick = DateTime.Now.Ticks;
                hostReply.Operator = operatorUser;
                advertise.LastModifiedDate = DateTime.Now;
                Repository.Update(advertise);
                Repository.Save();
            }
            else //add
            {
                var comment = new Comment();
                comment.SenderUser = user;
                comment.Operator = operatorUser;
                comment.Status = Comment.CommentStatus.ready;
                comment.type = Comment.CommentType.advertiseHostReply;
                comment.Text = text;
                comment.CreateDate = DateTime.Now;
                comment.LastModifyDate = DateTime.Now;
                comment.LastModifyDatetick = DateTime.Now.Ticks;
                comment.Operator = operatorUser;
                comment.Advertise = advertise;
                guestComment.HostReply = comment;
                advertise.LastModifiedDate = DateTime.Now;
                Repository.Update(advertise);
                Repository.Save();
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
        }

        public Dictionary<string, string> GetRulesDictionary(long id)
        {
            var advertise = Repository.Find(id);
            var dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(advertise.RequiredEvidence))
            {
                dictionary.Add("مدارک مورد نیاز", advertise.RequiredEvidence);
            }
            dictionary.Add("استعمال دخانیات", (bool)advertise.Smoking ? "مجاز" : "ممنوع");
            dictionary.Add("گرفتن مهمانی", (bool)advertise.Party ? "مجاز" : "ممنوع");
            dictionary.Add("آوردن حیوانات خانگی", (bool)advertise.Pets ? "مجاز" : "ممنوع");
            if (!string.IsNullOrEmpty(advertise.OtherRules))
            {
                dictionary.Add("سایر شرایط و قوانین اقامتگاه", advertise.OtherRules);
            }
            return dictionary;
        }

        public void DeleteExtrinsicReserves(long advertiseId, string from_date, string to_date, bool withLastDay = false)
        {
            var acc = Repository.Find(advertiseId);
            var fromGregorian = DateTimeUtility.PersianDateToGregorian(from_date);
            var toGregorian = DateTimeUtility.PersianDateToGregorian(to_date);
            if (withLastDay)
            {
                toGregorian = toGregorian.AddDays(1);
            }
            Repository.RemoveChildren<ExtrinsicReserve, long,
                IQueryable<ExtrinsicReserve>>(advertiseId, "ExtrinsicReserves",
                q => q.Where(w => w.StartDate >= fromGregorian &&
                w.StartDate < toGregorian).AsQueryable());
            Repository.Save();
            mediator.Send(new UpdateAdvertiseOccupiedCommand(advertiseId));
        }

        public bool ReserveRequest(long advertiseId, int userId, string startDate,
            string endDate, int guestCount, out string msg, out long reserveId)
        {
            if (guestCount < 1)
            {
                msg = "لطفا تعداد مهمان را وارد کنید";
                reserveId = 0;
                return false;
            }
            if (startDate == endDate)
            {
                msg = "تاریخ ورود و خروج نمیتوانند یکی باشند";
                reserveId = 0;
                return false;
            }
            var advertise = Repository.Find(advertiseId);
            var user = Repository.Find<User, int>(userId);
            if (advertise.Status != AdvertiseStatus.Published)
            {
                msg = "متاسفانه این اقامتگاه در حال حاضر از دسترس خارج است. لطفا اقامتگاه دیگری انتخاب نمایید.";
                reserveId = 0;
                return false;
            }

            long without_discount_price, couponCalculationPrice;
            var days = DateTimeUtility.GetPersianDateRangeDays(startDate, endDate);
            var total_price = priceCalculator.CalculateReservePrice(advertise, startDate, endDate, guestCount,
                out without_discount_price, out couponCalculationPrice);
            long depositePrice;
            if (days == 1)
            {
                depositePrice = total_price;
            }
            else if (days > 3)
            {
                depositePrice = (long)Math.Round(total_price * 0.3f);
            }
            else
            {
                var deposite = (long)Math.Round((double)total_price / (double)days);
                depositePrice = (long)(Math.Max(Math.Round(deposite / 1000f, 0), 1) * 1000);
            }
            Reserve reserve = new Reserve()
            {
                Advertise = advertise,
                GuestUser = user,
                HostUser = advertise.User,
                StartDate = DateTimeUtility.PersianDateToGregorian(startDate),
                EndDate = DateTimeUtility.PersianDateToGregorian(endDate),
                CreateDate = DateTime.Now,
                HostResponseDate = DateTime.Now,
                NumberOfGuests = guestCount,
                TotalPrice = total_price,
                DepositPrice = depositePrice,
                CouponCalculationPrice = couponCalculationPrice
            };
            reserve.InstantReserve = advertise.IsReserveInstant(reserve.StartDate, reserve.EndDate);
            reserve.Status = reserve.InstantReserve ? Reserve.ReserveStatus.WaitForReserve :
                Reserve.ReserveStatus.WaitForResponse;
            if (user.Reserves.Count(c => c.Status == ReserveStatus.WaitForResponse) >= 3)
            {
                msg = "شما نمیتوانید همزمان بیشتر از 3 درخواست رزرو بدهید.";
                reserveId = 0;
                return false;
            }
            if (advertise.UnitCount < 1 &&
                user.UserHasSimilarReserve(advertiseId,
                    reserve.StartDate, reserve.EndDate))
            {
                msg = "شما یک درخواست مشابه برای این آگهی دارید، برای درخواست جدید درخواست قبلی را لغو کنید";
                reserveId = 0;
                return false;
            }
            advertise.Reserves.Add(reserve);
            Repository.Update(advertise);
            Repository.Save();
            var identityUser = userManager.FindByNameAsync(advertise.User.PhoneNumber).Result;
            var contact = new UserContactDTO()
            {
                UserMainMobile = advertise.User.GetNoticesPhoneNumber(),
                UserAppNotificationToken = advertise.User.AppNotificationToken,
                UserEmail = identityUser.Email,
                EmailConfirmed = identityUser.EmailConfirmed,
                UserFcmAppNotificationToken = advertise.User.FcmAppNotificationToken,
                UserNotificationToken = advertise.User.NotificationToken,
                Type = UserContactType.ReserveRequest,
                AdvertiseId = reserve.AdvertiseID.ToString(),
                //UserId = user.Id.ToString(),
                UserId = string.Format("{0:n0}", reserve.TotalPrice - (reserve.TotalPrice * 0.1f)), // به جای کد مهمان، در این فیلد سهم میزبان فرستاده می شود
                ReserveId = reserve.Id.ToString(),
                Extra1 = startDate,
                Extra2 = endDate + Environment.NewLine + "به مدت " + (reserve.EndDate - reserve.StartDate).TotalDays + " شب" +
                            Environment.NewLine + "مبلغ: " + string.Format("{0:n0}", reserve.TotalPrice) + " تومان",
                Extra3 = reserve.NumberOfGuests.ToString() + " نفر" + Environment.NewLine + "کد رزرو: " + reserve.Id
            };
            mediator.Enqueue(new SendMessageCommand(contact));
            mediator.Publish(new ReserveRequestEvent(reserve.Id));
            mediator.Enqueue(new UpdateAdvertiseScoreCommand(advertiseId));
            msg = reserve.InstantReserve ?
                        "لطفا مبلغ رزرو را پرداخت نمایید تا رزرو شما نهایی شود"
                        : "درخواست رزرو شما با موفقیت انجام شد. نتیجه درخواست به اطلاع شما خواهد رسید.";
            reserveId = reserve.Id;
            return true;
        }

        public IList<Advertise> GetNorouzAdvertises(int count)
        {
            var advertises = Repository.Query(q => q.Where(w =>
                w.NowruzPrice > 0 && w.Active &&
                w.Status == Advertise.AdvertiseStatus.Published &&
                w.HideInSearch == false && w.UnitCount < 1));
            return advertises.OrderByDescending(o => o.ResidenceScore).Take(count).ToList();
        }

        public void SetHygieneProtocol(long id, HygieneProtocolStatus value)
        {
            var acc = Repository.Find(id);
            acc.HygieneProtocol = value;
            Repository.Update(acc);
            Repository.Save();
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public void UpdateAlbumPhoto(long advertiseId)
        {
            var residence = Repository.Find(advertiseId);
            var photoIds = residence.Photos.Select(x => x.Id).ToList();
            residence.AlbumPhoto = residence.Photos.Count == 0 ? "," : ("," + string.Join(",", photoIds) + ",");
            Repository.Update(residence);
            Repository.Save();
        }

        public async Task<ServiceResult> UpdateCalendarAsync(AdvertiseUpdateCalendarRequest request)
        {
            var serviceResult = new ServiceResult();
            var advertise = Repository.Find(request.advertiseId);
            if (advertise == null)
            {
                serviceResult.AddError("advertise id is incorrect");
                return serviceResult;
            }
            if (request.userId > 0 && advertise.UserId != request.userId)
            {
                serviceResult.AddError("user id is incorrect");
                return serviceResult;
            }
            if (DateTimeUtility.IsValidPersianDate(request.fromDate) == false ||
                (string.IsNullOrEmpty(request.toDate) == false &&
                DateTimeUtility.IsValidPersianDate(request.toDate) == false))
            {
                serviceResult.AddError("date format is incorrect");
                return serviceResult;
            }
            if (string.IsNullOrEmpty(request.toDate) == false &&
                DateTimeUtility.IsStartDateLowerThanEndDate(request.fromDate, request.toDate) == false)
            {
                serviceResult.AddError("date is incorrect");
                return serviceResult;
            }

            var garegorianToDate = string.IsNullOrEmpty(request.toDate) ?
                DateTimeUtility.PersianDateToGregorian(request.fromDate) :
                DateTimeUtility.PersianDateToGregorian(request.toDate);
            request.toDate = DateTimeUtility.GregorianToPersianDate(garegorianToDate.AddDays(1));

            if (request.full)
            {
                if (request.userId == 0)
                {
                    request.userId = advertise.UserId;
                }
                return await InsertExtrinsicReserveDatesAsync(request, advertise.UnitCount);
            }
            else
            {
                return DeleteExtrinsicReserveDates(request);
            }
        }

        public async Task<ServiceResult<List<long>>> AddInstantReserveDates(long residenceId, string fromDate, string toDate, int userId)
        {
            var serviceResult = new ServiceResult<List<long>>();
            var residence = await Repository.FindAsync(residenceId);
            //if (residence == null || residence.UserID != userId)
            //{
            //    serviceResult.AddError("user is incorrect");
            //    return serviceResult;
            //}
            var selectedPersianDates = DateTimeUtility.PersianDateRangeToList(fromDate, toDate, true, false);
            var instantReservePersianDates = residence.InstantReserveDates.Select(s => DateTimeUtility.GregorianToPersianDate(s.Date));
            foreach (var item in selectedPersianDates)
            {
                if (instantReservePersianDates.Any(x => x == item) == false)
                {
                    residence.InstantReserveDates.Add(new InstantReserveDate()
                    {
                        Date = DateTimeUtility.PersianDateToGregorian(item)
                    });
                }
            }
            Repository.Update(residence);
            Repository.Save();
            serviceResult.Result = residence.InstantReserveDates.Select(x => DateTimeUtility.DateValueOfJS(x.Date)).ToList();
            return serviceResult;
        }

        public async Task<ServiceResult<List<long>>> DeleteInstantReserveDates(long residenceId, string fromDate, string toDate, int userId)
        {
            var serviceResult = new ServiceResult<List<long>>();
            var residence = await Repository.FindAsync(residenceId);
            //if (residence == null || residence.UserID != userId)
            //{
            //    serviceResult.AddError("user is incorrect");
            //    return serviceResult;
            //}
            var selectedPersianDates = DateTimeUtility.PersianDateRangeToList(fromDate, toDate, true, false);
            foreach (var item in residence.InstantReserveDates.ToList())
            {
                var persianDate = DateTimeUtility.GregorianToPersianDate(item.Date);
                if (selectedPersianDates.Any(x => x == persianDate))
                {
                    residence.InstantReserveDates.Remove(item);
                }
            }
            Repository.Update(residence);
            Repository.Save();
            serviceResult.Result = residence.InstantReserveDates.Select(x => DateTimeUtility.DateValueOfJS(x.Date)).ToList();
            return serviceResult;
        }

        public async Task<ServiceResult> UpdatePricesAsync(ResidenceMainPricesDTO request, int adminId = 0)
        {
            var serviceResult = new ServiceResult();
            var residence = await Repository.FindAsync(request.residenceId);
            if (residence == null)
            {
                serviceResult.AddError("اقامتگاه یافت نشد");
                return serviceResult;
            }
            if (request.dailyPrice < 30000 || request.holidayPrice < 30000 || request.peakHolidayPrice < 30000 ||
                request.norouzPrice < 0 || request.extraCapacityPrice < 0 || request.norouzExtraCapacityPrice < 0)
            {
                serviceResult.AddError("قیمت های وارد شده اشتباه است");
                return serviceResult;
            }
            var clonedResidence = residence.ShallowCopy();
            residence.DailyPrice = request.dailyPrice;
            residence.HolidayPrice = request.holidayPrice;
            residence.PeakHolidayPrice = request.peakHolidayPrice;
            residence.MonthlyPrice = request.monthlyPrice;
            residence.NowruzPrice = request.norouzPrice;
            residence.ExtraCapacityPrice = request.extraCapacityPrice;
            residence.NowruzExtraCapacityPrice = request.norouzExtraCapacityPrice;
            Repository.Update(residence);
            Repository.Save();
            await mediator.Publish(new ChangeAdvertisePriceEvent(residence.Id, residence.NowruzPrice != clonedResidence.NowruzPrice));
            await mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
            await mediator.Publish(new AdvertiseUpdateEvent(clonedResidence, residence,
                ActionLog.ActionSourceEnum.AdminPanel, adminId));
            return serviceResult;
        }

        public async Task UpdateVideoStatus(long residenceId, Advertise.VideoStatusEnum status)
        {
            var residence = await Repository.FindAsync(residenceId);
            residence.VideoStatus = status;
            Repository.Update(residence);
            Repository.Save();
            await mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
        }

        private async Task<Advertise.AdvertiseStatus> UpdateStatus(long residenceId, Advertise.AdvertiseStatus status)
        {
            var residence = await Repository.FindAsync(residenceId);
            var clonedResidence = residence.ShallowCopy();
            residence.Status = status;
            Repository.Update(residence);
            Repository.Save();
            await mediator.Publish(new ChangeAdvertiseStatusEvent(residenceId, clonedResidence.Status));
            await mediator.Publish(new ChangeAdvertiseActiveEvent(clonedResidence, residence));
            await mediator.Send(new RemoveAdvertiseCacheCommand(residence.Id));
            return residence.Status;
        }

        private async Task<ServiceResult> InsertExtrinsicReserveDatesAsync(AdvertiseUpdateCalendarRequest request,
            int advertiseCount)
        {
            var serviceResult = new ServiceResult();
            var checkResult = CheckSetAsOccupiedDateRange(request.advertiseId, request.fromDate, request.toDate);
            if (checkResult.Result != CheckSetOccupiedResult.OK &&
                checkResult.Result != CheckSetOccupiedResult.ContainsReserveRequest)
            {
                serviceResult.AddError("date is incorrect");
                return serviceResult;
            }

            await mediator.Send(new InsertExtrinsicReserveCommand(request.advertiseId,
                request.fromDate, request.toDate, request.actionSource, request.userId, advertiseCount));
            if (DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date) == request.fromDate)
            {
                UnsetTodayEmpty(request.advertiseId);
            }
            return serviceResult;
        }

        private ServiceResult DeleteExtrinsicReserveDates(AdvertiseUpdateCalendarRequest request)
        {
            var serviceResult = new ServiceResult();
            var checkResult = CheckUnsetOccupiedDateRange(request.advertiseId, request.fromDate, request.toDate);
            if (checkResult.Result != CheckUnsetOccupiedResult.OK)
            {
                serviceResult.AddError("date is incorrect");
                return serviceResult;

            }
            DeleteExtrinsicReserves(request.advertiseId, request.fromDate, request.toDate);
            if (DateTimeUtility.GregorianToPersianDate(DateTime.Now.Date) == request.fromDate)
            {
                SetAsTodayEmpty(request.advertiseId);
            }
            return serviceResult;
        }
    }
}
