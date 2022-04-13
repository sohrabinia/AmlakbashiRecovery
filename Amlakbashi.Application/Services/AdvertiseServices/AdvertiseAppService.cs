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
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs;
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

        public AdvertiseListResponse Filter(AdvertiseListRequest request)
        {
            var category = Repository.Find<DynamicCategory, int>(request.categoryId);
            var advertises = category.Advertises.AsQueryable();

            advertises = advertiseFilter.FilterPhrase(advertises, request.phrase);
            if (request.area > 0)
            {
                advertises = advertises.Where(x => x.Area == request.area);
            }
            if (request.locationTypes != null && request.locationTypes.Any())
            {
                advertises = advertises.Where(a => request.locationTypes.Contains(a.Position));
            }
            if (request.parking)
            {
                advertises = advertises.Where(a => a.Parking != ParkingItems.NoParking);
            }
            if (request.capacity > 0)
            {
                advertises = advertises.Where(a => a.Capacity >= request.capacity ||
                    a.Capacity + a.MoreThanCapacity >= request.capacity);
            }
            if (request.roomCount > 0)
            {
                advertises = advertises.Where(w => w.Room == request.roomCount);
            }
            if (request.bedCount > 0)
            {
                advertises = advertises.Where(x => x.SingleBed + (x.DoublesBed * 2) == request.bedCount);
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
                advertises = advertises.Where(a => a.AllowPets == true);
            }
            if (request.party)
            {
                advertises = advertises.Where(a => a.AllowParty == true);
            }
            if (request.smoking)
            {
                advertises = advertises.Where(a => a.AllowSmoking == true);
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
                advertises = advertises.Where(a => a.NorouzPrice > 0);
            }
            if (request.instantReserve)
            {
                advertises = advertises.Where(a => a.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed);
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
                orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(x => x.TodayIsEmpty);
            }
            if (request.residencyType != AdvertiseType.None && request.residencyType != AdvertiseType.All)
            {
                orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(x => x.TypeID == request.residencyType);
            }
            if (request.capacity > 0)
            {
                orderedAdvertiseList = orderedAdvertiseList.ThenBy(x => x.Capacity);
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
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.HolidayPikePrice);
                            break;
                        case priceRangeTypes.Monthly:
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.RentPrice);
                            break;
                        case priceRangeTypes.Norouz:
                            orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.NorouzPrice);
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
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.HolidayPikePrice);
                            break;
                        case priceRangeTypes.Monthly:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.RentPrice);
                            break;
                        case priceRangeTypes.Norouz:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.NorouzPrice);
                            break;
                        default:
                            orderedAdvertiseList = orderedAdvertiseList.ThenBy(a => a.BasePrice);
                            break;
                    }
                    break;
                case SortOrder.UserRate:
                    orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.AverageUserRating);
                    break;
                case SortOrder.Clean:
                    orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.TidinessUserRating);
                    break;
                default:
                    orderedAdvertiseList = orderedAdvertiseList.ThenByDescending(a => a.AdvertiseScore);
                    break;
            }

            var pagedList = orderedAdvertiseList.ToPagedList(request.page, request.pageItemCount);
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
                itemResponse.favourited = request.UserFavorites.Any(x => x.AdvertiseID == item.Id);
                response.advertiseList.Add(itemResponse);
            }
            return response;
        }

        public IList<Advertise> Filter(string id)
        {
            return Repository.Query(q => q.Where(x => x.Id.ToString().Contains(id) &&
                x.Status == AdvertiseStatus.Published && x.Available)).OrderByDescending(x => x.AmlakbashiScore).Take(5).ToList();
        }

        public IList<Advertise> GetAdvertisesByUserId(int userId, bool includeCommentsAndReports = false)
        {
            if (includeCommentsAndReports)
            {
                return Repository.Query(q => q.Include(i => i.Parent).Include(i => i.Comments).Include(i => i.ReportItems)
                    .Where(w => w.UserID == userId && w.Comments.Any(a => a.Status == Comment.CommentStatus.publish)).ToList());
            }
            return Repository.Query(q => q.Where(w => w.UserID == userId).ToList());
        }

        public IList<Advertise> GetNotChildAdvertisesByUserId(int userId)
        {
            return Repository.Query(q => q.Include(i => i.Childs).Where(w =>
                w.Status != AdvertiseStatus.Deleted &&
                w.Status != AdvertiseStatus.NotCompleted &&
                w.UserID == userId && w.Mode != AdvertiseMode.Child).ToList());
        }

        public IList<long> GetAdvertiseIdsByUserId(int userId)
        {
            return Repository.Query(q => q.Where(w => w.UserID == userId).Select(s => s.Id).ToList());
        }

        public IList<Advertise> GetAdvertisesByStatus(AdvertiseStatus status, bool haveSlug = false)
        {
            if (haveSlug)
            {
                return Repository.Query(q => q.Where(a => a.Status == AdvertiseStatus.Published &&
                    !string.IsNullOrEmpty(a.Slug) && a.Province > 0).ToList());
            }
            return Repository.Query(q => q.Where(w => w.Status == status).ToList());
        }

        public IList<Advertise> GetInstantReserveAdvertisesByUserId(int userId, InstantReserveStatusEnum instantStatus)
        {
            return Repository.Query(q => q.Where(x => x.UserID == userId &&
                  x.InstantReserveStatus == instantStatus).ToList());
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
                return Repository.Query(q => q.Where(w => w.InstantReserveStatus == InstantReserveStatusEnum.Confirmed)
                    .OrderByDescending(o => o.AverageUserRating).Take(count)).ToList();
            }
            return Repository.Query(q => q.OrderByDescending(o => o.AverageUserRating).Take(count)).ToList();
        }

        public List<string> GetAdvertiseTags(Advertise advertise)
        {
            var tags = new List<string>();
            tags.Add(AdvertiseMainLocalization.GetAdvertiseTypeUserString(advertise.TypeID));
            if (advertise.Room > 0)
            {
                tags.Add($"{advertise.Room} خوابه");
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

        public Advertise Find(long id, int statusLowerThan)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id && (int)f.Status < statusLowerThan &&
                f.Count == 0));
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
            model = model.Where(w => w.Mode != AdvertiseMode.Child);
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
                model = model.Where(w => w.Id == dto.Id || w.Childs.Any(x => x.Id == dto.Id));
            }
            if (dto.UserId != -1)
            {
                model = model.Where(w => w.UserID == dto.UserId);
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
            if (dto.InstatntReserveStatus > -1)
            {
                model = model.Where(x => x.InstantReserveStatus == (Advertise.InstantReserveStatusEnum)dto.InstatntReserveStatus);
            }
            if (string.IsNullOrEmpty(dto.MinReserveNorouzFromDate) == false)
            {
                var gregorianDate = DateTimeUtility.PersianDateToGregorian(
                        StringUtility.PersianNumberToEnglish(dto.MinReserveNorouzFromDate).Replace('/', ','));
                var minReserveNorouzDateUnix = DateTimeUtility.DateValueOfJS(gregorianDate);
                model = model.Where(x => x.unixNorouzMinRequestDate >= minReserveNorouzDateUnix);
            }
            if (dto.Area > -1)
            {
                model = model.Where(x => x.Area == dto.Area);
            }
            else if (dto.City > -1)
            {
                model = model.Where(x => x.City == dto.City);
            }
            else if (dto.Province > -1)
            {
                model = model.Where(x => x.Province == dto.Province);
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
            if (dto.Sort == "contact")
                model = model.OrderByDescending(a => a.ContactClick).ThenByDescending(a => a.WebVisit);
            else if (dto.Sort == "modify")
                model = model.OrderByDescending(a => a.LastModifyDate).ThenByDescending(a => a.CreateDate);
            else if (dto.Sort == "click")
                model = model.OrderByDescending(a => a.WebVisit).ThenByDescending(a => a.ContactClick);
            else if (dto.Sort == "score")
                model = model.OrderByDescending(a => a.AdvertiseScore);
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
                model = model.Where(w => w.UserID == userid);
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
            IQueryable<Advertise> query = Repository.Query(q => q.Where(x => x.UserID > 0));
            if (defaultProvince)
            {
                query = query.Where(x => x.Province == province);
            }
            if (area > 0)
            {
                query = query.Where(x => x.Area == area);
            }
            else if (city > 0)
            {
                query = query.Where(x => x.City == city);
            }
            else if (province > 0)
            {
                query = query.Where(x => x.Province == province);
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
            IQueryable<Advertise> query = Repository.Query(q => q.Where(x => x.UserID > 0));
            if (fromDate != null && toDate != null)
            {
                query = query.Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate);
            }

            if (area > 0)
            {
                query = query.Where(x => x.Area == area);
            }
            else if (city > 0)
            {
                query = query.Where(x => x.City == city);
            }
            else if (province > 0)
            {
                query = query.Where(x => x.Province == province);
            }

            if (adtype > 0 && adtype != (int)Advertise.AdvertiseType.All)
            {
                query = query.Where(x => x.TypeID == (Advertise.AdvertiseType)adtype);
            }

            if (userId > 0)
                query = query.Where(x => x.UserID == userId);

            return query.ToList();
        }

        public void Edit(Advertise editedAd)
        {
            var advertise = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedAd.Id));
            advertise.Title = editedAd.Title;
            advertise.MetaTitle = editedAd.MetaTitle;
            advertise.MetaDescription = editedAd.MetaDescription;
            advertise.Slug = editedAd.Slug;
            advertise.UserID = editedAd.UserID;
            if (advertise.Childs != null && advertise.Childs.Any())
            {
                foreach (var child in advertise.Childs)
                {
                    child.UserID = editedAd.UserID;
                }
            }
            advertise.Overview = editedAd.Overview;
            advertise.WebVisit = editedAd.WebVisit;
            advertise.ContactClick = editedAd.ContactClick;
            if (advertise.AmlakbashiScore != editedAd.AmlakbashiScore)
            {
                advertise.AdvertiseScore += (editedAd.AmlakbashiScore - advertise.AmlakbashiScore);
                advertise.AmlakbashiScore = editedAd.AmlakbashiScore;
            }
            advertise.Description = editedAd.Description;
            Repository.Update(advertise);
            Repository.Save();
            mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
            mediator.Send(new RemoveCategoryItemCacheCommand(advertise.Id));
        }

        public void UpdateAccView(long accId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == accId));
            acc.WebVisit += 1;
            acc.Overview += 1;
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
            foreach (var item in acc.Childs.Where(w => w.Available == true))
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

        public AdvertiseDirector GetBasicForm(long id, out bool isEdit, out int level)
        {
            Advertise acc = null;
            level = 0;
            if (id > 0)
            {
                acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
                level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : (acc.OwnershipType < 1 ? 3 : 4));
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
            data.LastModifyDate = DateTime.Now;
            level = 1;

            if (data.Id > 0)
            {
                level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : (acc.OwnershipType < 1 ? 3 : 4));
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
            data.UserID = userId;
            data.Available = true;
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
                    if (data.Position != acc.Position)
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
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : (acc.OwnershipType < 1 ? 3 : 4));
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
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : (acc.OwnershipType < 1 ? 3 : 4));
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
            if (!acc.Photos.Select(s => s.Id).SequenceEqual(photoIds) || acc.PhotoID != photoPart.PhotoID)
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
            acc.LastModifyDate = DateTime.Now;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new CreateAdvertiseGeneralEvent(acc.Id));
            mediator.Publish(new ChangeAdvertiseAddressEvent(shallowAcc, acc));
            if (removedPhotoIds.Any())
            {
                mediator.Send(new RemovePhotosByFileIdsCommand(acc.Id, removedPhotoIds)).Wait();
            }
            mediator.Send(new RenameAdvertisePhotosCommand(acc.Id)).Wait();
            mediator.Send(new GenerateThumbImageCommand(acc.Id, acc.PhotoID,
                    acc.Photos.Select(s => s.Id).ToList())).Wait();
            return director;
        }

        public AdvertiseDirector GetExtraForm(long id, out bool isEdit, out int level)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            isEdit = acc != null && !(acc.Status == AdvertiseStatus.NotCompleted || acc.Status == AdvertiseStatus.Unset);
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : (acc.OwnershipType < 1 ? 3 : 4));
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
            level = acc.Status != AdvertiseStatus.NotCompleted ? 4 : acc.TypeID == AdvertiseType.None ? 1 : (string.IsNullOrEmpty(acc.Title) ? 2 : (acc.OwnershipType < 1 ? 3 : 4));
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
                licenseFileId = mediator.Send(new UpdateAdvertiseLicenseFileCommand(uploadedLicenseFile, data.Id, acc.UserID, data.LicenseFileId)).Result;
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
            acc.LastModifyDate = DateTime.Now;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeAdvertisePriceEvent(acc.Id, acc.NorouzPrice != oldAcc.NorouzPrice));
            if (acc.Mode == AdvertiseMode.Parent)
            {
                mediator.Publish(new ChangeAdvertiseRulesEvent(acc.Id));
            }
            return director;
        }

        private void UpdateLicenseFile(IFormFile licenseFile, long advertiseId)
        {

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
                hotel.Count = 0;
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
                    changeNorouzPrice = child.NorouzPrice != oldChild.NorouzPrice;
                    child.LastModifyDate = DateTime.Now;
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
                    data.LastModifyDate = DateTime.Now;
                    data.UserID = userId;
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
                    changeNorouzPrice = child.NorouzPrice != oldChild.NorouzPrice;
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
                    child.LastModifyDate = DateTime.Now;
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
                        mediator.Send(new GenerateThumbImageCommand(child.Id, child.PhotoID,
                            child.Photos.Select(s => s.Id).ToList())).Wait();
                    }
                }
                else
                {
                    data.CreateDate = DateTime.Now;
                    data.LastModifyDate = DateTime.Now;
                    data.UserID = userId;
                    data.Province = parent.Province;
                    data.City = parent.City;
                    data.Area = parent.Area;
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
                        mediator.Send(new GenerateThumbImageCommand(data.Id, data.PhotoID,
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
                licenseFileId = mediator.Send(new UpdateAdvertiseLicenseFileCommand(uploadedLicenseFile, data.Id, data.UserID, data.LicenseFileId)).Result;
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
                    mediator.Send(new GenerateThumbImageCommand(data.Id, acc.PhotoID,
                            acc.Photos.Select(s => s.Id).ToList())).Wait();
                }
                mediator.Publish(new CreateAdvertiseGeneralEvent(acc.Id, true));
            }

            if (type == DirectorType.Extra || type == DirectorType.ComplexUnit ||
                type == DirectorType.HotelUnit)
            {
                mediator.Publish(new ChangeAdvertisePriceEvent(acc.Id, acc.NorouzPrice != shallowData.NorouzPrice));
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
            acc.unixNorouzMinRequestDate = dateUnix;
            foreach (var child in acc.Childs)
            {
                child.unixNorouzMinRequestDate = dateUnix;
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
            acc.Available = isAvailable;
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

        public AdvertiseStatus ToggleSuspension(long id)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var prevAcc = acc.ShallowCopy();
            if (acc.Status == AdvertiseStatus.Archived)
            {
                acc.Status = AdvertiseStatus.Published;
                Repository.Update(acc);
                Repository.Save();
                mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
            }
            else if (acc.Status == AdvertiseStatus.Published)
            {
                acc.Status = AdvertiseStatus.Archived;
                Repository.Update(acc);
                Repository.Save();
                mediator.Publish(new ChangeAdvertiseStatusEvent(id, prevAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(prevAcc, acc));
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return acc.Status;
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
            acc.TodayIsEmpty = true;
            Repository.Update(acc);
            Repository.Save();
            mediator.Send(new RemoveCategoryItemCacheCommand(acc.Id));
        }

        public void UnsetTodayEmpty(long id)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.TodayIsEmpty = false;
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

        public void RequestInstantReserve(long id,
            bool ignoreMsg, int userId,
            int doerUserId, ActionLog.ActionSourceEnum actionSource,
            User.InstantReserveAccessEnum currInstantReserveAccess,
            out bool needMsg)
        {
            using (var tran = new TransactionScope())
            {
                needMsg = false;
                var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
                if (currInstantReserveAccess == User.InstantReserveAccessEnum.Verified)
                {
                    if (acc.InstantReserveStatus != InstantReserveStatusEnum.Confirmed)
                    {
                        acc.InstantReserveStatus = InstantReserveStatusEnum.Confirmed;
                        Repository.Update(acc);
                        Repository.Save();
                        mediator.Publish(new ChangeInstantReserveStatusEvent(id, acc.UserID,
                                acc.InstantReserveStatus, actionSource, doerUserId));
                    }
                }
                else
                {
                    if (ignoreMsg || currInstantReserveAccess == User.InstantReserveAccessEnum.Requested)
                    {
                        var currInstantReserveStatus = acc.InstantReserveStatus;
                        if (currInstantReserveStatus != InstantReserveStatusEnum.Requested)
                        {
                            acc.InstantReserveStatus = InstantReserveStatusEnum.Requested;
                            Repository.Update(acc);
                            Repository.Save();
                            mediator.Publish(new ChangeInstantReserveStatusEvent(id, acc.UserID,
                                acc.InstantReserveStatus, actionSource, doerUserId));
                        }
                    }
                    else
                    {
                        needMsg = true;
                    }
                }
                tran.Complete();
            }
        }

        public void CancelInstantReserve(long id, int userId, int doerUserId,
            ActionLog.ActionSourceEnum actionSource)
        {
            using (var tran = new TransactionScope())
            {
                var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
                acc.InstantReserveStatus = InstantReserveStatusEnum.None;
                Repository.Update(acc);
                Repository.Save();
                mediator.Publish(new ChangeInstantReserveStatusEvent(id,
                    userId, acc.InstantReserveStatus, actionSource, doerUserId));
                tran.Complete();
            }
        }

        public int GetInstantReserveCancelCount(int userId)
        {
            var advertises = Repository.Query(q => q.Where(x => x.UserID == userId));
            return advertises == null || advertises.Any() == false ? 0 :
                advertises.Sum(x => x.InstantReserveCancels);
        }

        public string GetInstantReserveBanReason(long id)
        {
            var allAccs = Repository.Query(q => q);
            var acc = allAccs.FirstOrDefault(x => x.Id == id);
            var accs = allAccs.Where(x => x.UserID == acc.UserID);
            var countCancel = accs.Sum(x => x.InstantReserveCancels);
            return "شما " + countCancel + " مورد لغو رزرو داشته اید و نمیتوانید از این امکان استفاده کنید.";
        }

        public void SetStayDuration(long id, int min, int max)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.MinReserveDays = min;
            acc.MaxReserveDays = max;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeStayDurationEvent(id));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public void SetNorouzPrice(long id, int norouzPrice, int overCapacityPrice = 0)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.NorouzPrice = norouzPrice;
            acc.NorouzOverCapacityPrice = overCapacityPrice;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeNorouzPriceEvent(id));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public PriceInputDTO GetPrices(long id)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var director = new AdvertiseDirector(acc, DirectorType.AdvertisePage);
            PriceInputDTO result = director.GetAdvertisePart<PricePart>();
            return result;
        }

        public bool SetPrices(long id, PriceInputDTO prices, out Dictionary<string, string> errors)
        {
            string msg;
            PricePart part = new PricePart();
            PropertyCopier<PriceInputDTO, PricePart>.CopyInsensetive(prices, part);
            part.HolidayPikePrice = prices.pikeHolidayPrice;
            if (part.Validate(out errors, out msg) == false)
                return false;
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            PropertyCopier<PricePart, Advertise>.Copy(part, acc);
            Repository.Update(acc);
            Repository.Save();
            if (acc.Id > 0)
            {
                mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
                mediator.Publish(new ChangeAdvertisePriceEvent(acc.Id));
            }
            return true;
        }

        public void SetMaxInstantReserveStart(long id, int maxInstantReserveStart)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.MaxInstantReserveStart = maxInstantReserveStart;
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeMaxInstantReserveStartEvent(id));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public void AddToAdvertiseVisit(long id)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.WebVisit += 1;
            acc.Overview += 1;
            Repository.Update(acc);
            Repository.Save();
        }

        public IList<Advertise> GetAdvertiseRelatedItems(long id, int count = 4)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            var publishedState = AdvertiseStatus.Published;
            var accs = Repository.Query(q => q.Where(w =>
                w.Status == publishedState && w.Available &&
                w.Count == 0 && w.UserID != acc.UserID));

            var price_max_difference = acc.DailyPrice * 0.25f;//25 percent of difference to the Advertise

            var model = accs.
                OrderByDescending(x => x.Area == acc.Area).
                ThenByDescending(x => x.City == acc.City).
                ThenByDescending(x => x.Province == acc.Province).
                ThenByDescending(x => x.CountryDirection == acc.CountryDirection).
                ThenByDescending(x => x.TypeID == acc.TypeID).//sort by advertise type
                ThenByDescending(x => Math.Abs(x.DailyPrice - acc.DailyPrice) <= price_max_difference).
                ThenByDescending(x => x.DailyPrice >= acc.DailyPrice).
                ThenBy(x => Math.Abs(x.DailyPrice - acc.DailyPrice) <= price_max_difference ? 0 : x.DailyPrice).
                ThenByDescending(x => x.AdvertiseScore).
                Take(count).ToList();
            return model;
        }

        public ApiAmenitiesGetDTO GetAmenitiesDTO(long id, out int userId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            userId = acc.UserID;
            ApiAmenitiesGetDTO dto = acc;
            var parent = Repository.Query(q => q.FirstOrDefault(x => x.Childs.Any(y => y.Id == id)));
            var parentAdvertiseType = parent == null ? AdvertiseType.None : parent.TypeID;

            dto.heatingSystemSelectItem = new List<SelectItem>();
            dto.coolingSystemSelectItem = new List<SelectItem>();
            dto.wcSelectItem = new List<SelectItem>();
            dto.extraBlanketSelectItem = new List<SelectItem>();

            var heatingItems = Advertise.GetPropertyItems(Property.HeatingSystem, parentAdvertiseType) as HeatingSystemItems[];
            dto.heatingSystemSelectItem.AddRange(heatingItems.Select(s => new SelectItem((int)s,
                AdvertiseMainLocalization.GetPropertyValueTitle(s))));

            var coolingItems = Advertise.GetPropertyItems(Property.CoolingSystem, parentAdvertiseType) as CoolingSystemItems[];
            dto.coolingSystemSelectItem.AddRange(coolingItems.Select(s => new SelectItem((int)s,
                AdvertiseMainLocalization.GetPropertyValueTitle(s))));

            var wcItems = Advertise.GetPropertyItems(Property.WC, parentAdvertiseType) as WCItems[];
            dto.wcSelectItem.AddRange(wcItems.Select(s => new SelectItem((int)s,
                AdvertiseMainLocalization.GetPropertyValueTitle(s))));

            var extraBlanketItems = Advertise.GetPropertyItems(Property.ExtraBlanketCount, parentAdvertiseType) as ExtraBlanketCountItems[];
            dto.extraBlanketSelectItem.AddRange(extraBlanketItems.Select(s => new SelectItem((int)s,
                AdvertiseMainLocalization.GetPropertyValueTitle(s))));

            return dto;
        }

        public bool UpdateAmenities(ApiAmenitiesDTO editedData, out Dictionary<string, string> errors, out string msg)
        {
            AmenitiesPart part = new AmenitiesPart();
            PropertyCopier<ApiAmenitiesDTO, AmenitiesPart>.CopyInsensetive(editedData, part);
            if (part.Validate(out errors, out msg) == false)
                return false;
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedData.id));
            var previousPoolFeature = acc.PoolFeatures;//TODO : this is temporary to ignore updating pool feature since this property is not implemented in android and ios application
            var shallowAcc = acc.ShallowCopy();
            PropertyCopier<AmenitiesPart, Advertise>.Copy(part, acc);
            acc.LastModifyDate = DateTime.Now;
            var director = new AdvertiseDirector(acc, DirectorType.Extra);
            var hasImportantChange = director.HasImpotantChange(shallowAcc);
            if (acc.Status != AdvertiseStatus.NotCompleted && acc.Status != AdvertiseStatus.FirstReady &&
                (hasImportantChange || acc.Status == AdvertiseStatus.NotVerified))
            {
                acc.Status = AdvertiseStatus.ReadyToPublish;
            }
            acc.PoolFeatures = previousPoolFeature;//TODO: this is temporary to ignore updating pool feature since this property is not implemented in android and ios application
            Repository.Update(acc);
            Repository.Save();
            if (hasImportantChange)
            {
                mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, shallowAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAcc, acc));
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return true;
        }

        public void UpdateExtraBlanketCount(long id, ExtraBlanketCountItems data)
        {
            var advertise = Repository.Find(id);
            advertise.ExtraBlanketCount = data;
            Repository.Update(advertise);
            Repository.Save();
            mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
        }

        public void UpdateElevator(long id, bool data)
        {
            var advertise = Repository.Find(id);
            advertise.Elevator = data;
            Repository.Update(advertise);
            Repository.Save();
            mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
        }

        public ApiPhotoDTO GetPhotoDTO(long id, out int accUserId)
        {
            var advertise = Repository.Query(q => q.FirstOrDefault(x => x.Id == id));
            if (advertise == null)
            {
                accUserId = 0;
                return null;
            }
            ApiPhotoDTO dto = new ApiPhotoDTO();
            accUserId = advertise.UserID;
            dto.id = advertise.Id;
            dto.mainPhoto = advertise.PhotoID == null ? 0 : (int)advertise.PhotoID;
            dto.album = advertise.Photos.Select(s => s.Id).ToList();
            if (advertise.PhotoID == null && advertise.Photos.Count > 0)
            {
                advertise.PhotoID = advertise.Photos.First().Id;
                Repository.Update(advertise);
                Repository.Save();
                mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
            }
            return dto;
        }

        public bool UpdatePhotos(ApiPhotoDTO editedData, string rootPath)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(x => x.Id == editedData.id));
            var shallowAcc = acc.ShallowCopy();
            var albumString = editedData.ConvertAlbumToString();
            if (acc.Photos.Select(s => s.Id).SequenceEqual(editedData.album) && acc.PhotoID == editedData.mainPhoto)
            {
                return true;
            }
            var removedPhotoIds = acc.Photos.Select(s => s.Id).Except(editedData.album).ToList();
            acc.Photos.Clear();
            foreach (var item in editedData.album)
            {
                var file = Repository.Find<File, long>(item);
                acc.Photos.Add(file);
            }
            acc.PhotoID = editedData.mainPhoto;
            acc.AlbumPhoto = editedData.ConvertAlbumToString();
            acc.LastModifyDate = DateTime.Now;
            var director = new AdvertiseDirector(acc, DirectorType.General);
            var hasImportantChange = director.HasImpotantChange(shallowAcc);
            if (acc.Status != AdvertiseStatus.NotCompleted && acc.Status != AdvertiseStatus.FirstReady &&
                (hasImportantChange || acc.Status == AdvertiseStatus.NotVerified))
            {
                acc.Status = AdvertiseStatus.ReadyToPublish;
            }
            Repository.Update(acc);
            Repository.Save();
            if (hasImportantChange)
            {
                mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, shallowAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAcc, acc));
            }
            if (removedPhotoIds.Any())
            {
                mediator.Send(new RemovePhotosByFileIdsCommand(acc.Id, removedPhotoIds)).Wait();
            }
            mediator.Send(new RenameAdvertisePhotosCommand(acc.Id)).Wait();
            mediator.Send(new GenerateThumbImageCommand(acc.Id, acc.PhotoID, editedData.album)).Wait();
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return true;
        }

        public ApiPositionDTO GetPositionDTO(long id, out int userId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            userId = acc.UserID;
            ApiPositionDTO dto = acc;
            dto.positionSelectItem = new List<SelectItem>();
            var positionItems = Advertise.GetPropertyItems(Property.Region) as PositionType[];
            dto.positionSelectItem.AddRange(positionItems.Select(s => new SelectItem((int)s,
                AdvertiseMainLocalization.GetPropertyValueTitle(s))));
            return dto;
        }

        public bool UpdatePositionDTO(ApiPositionDTO editedData, out Dictionary<string, string> errors)
        {
            string msg;
            PositionPart positionPart = new PositionPart();
            PropertyCopier<ApiPositionDTO, PositionPart>.CopyInsensetive(editedData, positionPart);
            if (positionPart.Validate(out errors, out msg) == false)
                return false;
            var addressPart = new AddressPart();
            PropertyCopier<ApiPositionDTO, AddressPart>.CopyInsensetive(editedData, addressPart);
            if (addressPart.Area != null && addressPart.Area < 1)
            {
                addressPart.Area = null;
            }
            if (addressPart.City != null && addressPart.City < 1)
            {
                addressPart.City = null;
            }
            if (addressPart.Province != null && addressPart.Province < 1)
            {
                addressPart.Province = null;
            }
            if (addressPart.Validate(out errors, out msg) == false)
                return false;
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedData.id));
            var shallowAcc = acc.ShallowCopy();
            PropertyCopier<PositionPart, Advertise>.Copy(positionPart, acc);
            PropertyCopier<AddressPart, Advertise>.Copy(addressPart, acc);
            var director = new AdvertiseDirector(acc, DirectorType.General);
            var hasImportantChange = director.HasImpotantChange(shallowAcc);
            if (acc.Status != AdvertiseStatus.NotCompleted && acc.Status != AdvertiseStatus.FirstReady &&
                (hasImportantChange || acc.Status == AdvertiseStatus.NotVerified))
            {
                acc.Status = AdvertiseStatus.ReadyToPublish;
            }
            acc.LastModifyDate = DateTime.Now;
            Repository.Update(acc);
            Repository.Save();
            if (acc.Mode == AdvertiseMode.Parent)
            {
                mediator.Publish(new ChangeAdvertisePositionEvent(acc.Id));
                mediator.Publish(new ChangeAdvertiseAddressEvent(shallowAcc, acc));
            }
            if (hasImportantChange)
            {
                mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, shallowAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAcc, acc));
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return true;
        }

        public ApiRulesDTO GetRulesDTO(long id, out int userId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            userId = acc.UserID;
            ApiRulesDTO dto = acc;
            return dto;
        }

        public bool UpdateRulesDTO(ApiRulesDTO editedData)
        {
            RulesPart part = new RulesPart();
            part.EvidenceRequired = editedData.evidenceRequired.value;
            part.OtherRules = editedData.otherRules.value;
            part.AllowParty = (bool)editedData.allowParty.value;
            part.AllowPets = (bool)editedData.allowPets.value;
            part.AllowSmoking = (bool)editedData.allowSmoking.value;
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedData.id));
            var shallowAcc = acc.ShallowCopy();
            PropertyCopier<RulesPart, Advertise>.Copy(part, acc);
            var director = new AdvertiseDirector(acc, DirectorType.Extra);
            var hasImportantChange = director.HasImpotantChange(shallowAcc);
            if (acc.Status != AdvertiseStatus.NotCompleted && acc.Status != AdvertiseStatus.FirstReady &&
                (hasImportantChange || acc.Status == AdvertiseStatus.NotVerified))
            {
                acc.Status = AdvertiseStatus.ReadyToPublish;
            }
            acc.LastModifyDate = DateTime.Now;
            Repository.Update(acc);
            Repository.Save();
            if (acc.Mode == AdvertiseMode.Parent)
            {
                mediator.Publish(new ChangeAdvertiseRulesEvent(acc.Id));
            }
            if (hasImportantChange)
            {
                mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, shallowAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAcc, acc));
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return true;
        }

        public ApiSpecificDTO GetSpecificDTO(long id, out int userId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            userId = acc.UserID;
            ApiSpecificDTO dto = acc;
            var parent = Repository.Query(q => q.FirstOrDefault(x => x.Childs.Any(y => y.Id == id)));
            var parentAdvertiseType = parent == null ? AdvertiseType.None : parent.TypeID;

            dto.parkingSelectItem = new List<SelectItem>();
            dto.floorSelectItem = new List<SelectItem>();
            var parkingItem = Advertise.GetPropertyItems(Property.Parking, parentAdvertiseType) as ParkingItems[];
            dto.parkingSelectItem.AddRange(parkingItem.Select(s => new SelectItem((int)s,
                AdvertiseMainLocalization.GetPropertyValueTitle(s))));
            var floorItem = Advertise.GetPropertyItems(Property.Floor, parentAdvertiseType) as FloorItems[];
            dto.floorSelectItem.AddRange(floorItem.Select(s => new SelectItem((int)s,
                AdvertiseMainLocalization.GetPropertyValueTitle(s))));
            return dto;
        }

        public bool UpdateSpecificDTO(ApiSpecificDTO editedData, bool hasChild, out List<string> errors)
        {
            if (editedData.Validate(hasChild, out errors) == false)
            {
                return false;
            }
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedData.id));
            var shallowAcc = acc.ShallowCopy();
            ApiSpecificDTO.CopyToAdvertise(editedData, acc);
            var director = new AdvertiseDirector(acc, DirectorType.General);
            var hasImportantChange = director.HasImpotantChange(shallowAcc);
            if (acc.Status != AdvertiseStatus.NotCompleted && acc.Status != AdvertiseStatus.FirstReady &&
                (hasImportantChange || acc.Status == AdvertiseStatus.NotVerified))
            {
                acc.Status = AdvertiseStatus.ReadyToPublish;
            }
            acc.LastModifyDate = DateTime.Now;
            acc.MoreThanCapacity = editedData.extraCapacity.value;
            Repository.Update(acc);
            Repository.Save();
            if (hasImportantChange)
            {
                mediator.Publish(new ChangeAdvertiseStatusEvent(acc.Id, shallowAcc.Status));
                mediator.Publish(new ChangeAdvertiseActiveEvent(shallowAcc, acc));
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return true;
        }

        public ApiHotelUnitDTO GetHotelUnitDTO(long id, out int userId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            userId = acc.UserID;
            ApiHotelUnitDTO dto = acc;
            return dto;
        }

        public bool UpdateHotelUnitDTO(ApiHotelUnitDTO editedData, out List<string> errors)
        {
            if (editedData.Validate(out errors) == false)
            {
                return false;
            }
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedData.id));
            acc.TypeID = (Advertise.AdvertiseType)editedData.typeId;
            acc.ParentAccType = (Advertise.AdvertiseType)Advertise.AdvertiseTypeToHeadType(editedData.typeId);
            acc.Title = editedData.title.value;
            acc.Capacity = editedData.capacity.value;
            acc.MoreThanCapacity = editedData.extraCapacity.value;
            acc.SingleBed = editedData.singleBed.value;
            acc.DoublesBed = editedData.doubleBed.value;
            acc.Count = editedData.count.value;
            acc.BlanketsAndMattresses = editedData.blanketsAndMattresses.value;
            acc.DailyPrice = editedData.dailyPrice.value;
            acc.HolidayPrice = editedData.holidayPrice.value;
            acc.HolidayPikePrice = editedData.pikeHolidayPrice.value;
            acc.MoreThanCapacityPrice = editedData.moreThanCapacityPrice.value;
            acc.LastModifyDate = DateTime.Now;
            Repository.Update(acc);
            Repository.Save();
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
            return true;
        }

        public ApiNorouzPriceDTO GetNorouzPriceDTO(long id, out int userId)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            userId = acc.UserID;
            ApiNorouzPriceDTO dto = new ApiNorouzPriceDTO();
            dto.id = acc.Id;
            dto.SetNorouzPrice(acc.NorouzPrice);
            dto.SetNorouzOverCapacityPrice(acc.NorouzOverCapacityPrice);
            return dto;
        }

        public void SetNorouzPrice(long id, int norouzPrice,
            int overCapacityPrice = 0, int buildNumber = 0)
        {
            var acc = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            acc.NorouzPrice = norouzPrice;
            if (buildNumber > 0)
            {
                acc.NorouzOverCapacityPrice = overCapacityPrice;
            }
            Repository.Update(acc);
            Repository.Save();
            mediator.Publish(new ChangeNorouzPriceEvent(id));
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }

        public IEnumerable<Advertise> GetMostViewedAdvertisesInCity(int city_id, int province_id, int type_id, int count)
        {
            var advertises = Repository.Query(q => q);
            if (city_id > 0)
            {
                advertises = advertises.Where(x => x.City == city_id && x.Status == AdvertiseStatus.Published &&
                x.Available && x.Count == 0 && !x.HideInCategory);
            }
            else if (province_id > 0)
            {
                advertises = advertises.Where(x => x.Province == province_id && x.Status == AdvertiseStatus.Published &&
                x.Available && x.Count == 0 && !x.HideInCategory);
            }
            if (type_id != (int)AdvertiseType.All)
            {
                advertises = advertises.Where(x => x.TypeID == (AdvertiseType)type_id);
            }
            return advertises.OrderByDescending(x => x.AdvertiseScore).Take(count).ToList();
        }

        public IEnumerable<Advertise> GetMostViewedAdvertisesByType(int type_id, int count)
        {
            var advertises = Repository.Query(q => q);
            advertises = advertises.Where(x => x.TypeID == (AdvertiseType)type_id &&
                x.Status == AdvertiseStatus.Published && x.Available && x.Count == 0 && !x.HideInCategory);
            return advertises.OrderByDescending(x => x.AdvertiseScore).Take(count).ToList();
        }

        // for Norouz - commented at AdvertiseApi.GetHomePageCarousels
        public IList<Advertise> GetMostViewedNorouzAdvertises(int count)
        {
            IQueryable<Advertise> advertises = Repository.Query(q => q.Where(
                x => x.NorouzPrice > 0 &&
                x.Status == AdvertiseStatus.Published &&
                x.Available && x.Count == 0 && !x.HideInCategory));
            return advertises.OrderByDescending(x => x.AdvertiseScore)
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
            guestsOutOfRange = numberOfGuests > advertise.Capacity + advertise.MoreThanCapacity;
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

        public void UpdateInstantReserveStatus(int userId, InstantReserveStatusEnum status, bool forRequested = false)
        {
            var data = Repository.Query(q => q.Where(w => w.UserID == userId));
            if (forRequested)
            {
                data = data.Where(w => w.InstantReserveStatus == InstantReserveStatusEnum.Requested);
            }
            foreach (var item in data)
            {
                item.InstantReserveStatus = status;
                foreach (var child in item.Childs)
                {
                    child.InstantReserveStatus = status;
                }
                Repository.Update(item);
                mediator.Send(new RemoveAdvertiseCacheCommand(item.Id));
            }
            Repository.Save();
        }

        public void UpdateInstantReserveStatus(long accId,
            InstantReserveStatusEnum status, int doerUserId,
            ActionSourceEnum actionSource)
        {
            mediator.Send(new UpdateInstantReserveStatusCommand(accId, status, doerUserId, actionSource));
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
            string startDate, string endDate, out string msg)
        {
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
            if (advertise.MinReserveDays > 0 && days < advertise.MinReserveDays)
            {
                msg = "برای رزرو این اقامتگاه باید حداقل " + advertise.MinReserveDays + "  شب اقامت کنید. برای اقامت " + days + " شبه میتوانید اقامتگاه های دیگر را رزرو کنید";
                return false;
            }
            if (advertise.MaxReserveDays > 0 && days > advertise.MaxReserveDays)
            {
                msg = "شما میتوانید حداکثر " + advertise.MaxReserveDays + "  شب در این اقامتگاه اقامت کنید. برای اقامت طولانی تر میتوانید اقامتگاه های دیگر را رزرو کنید";
                return false;
            }
            var todayUnix = DateTimeUtility.DateValueOfJS(DateTime.Now.Date);
            if (advertise.unixNorouzMinRequestDate > todayUnix &&
                DateTimeUtility.IsNorouz(DateTimeUtility.PersianDateRangeToList(startDate, endDate, true, false)))
            {
                var minDateString = DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(advertise.unixNorouzMinRequestDate));
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

            if (currentUserId > 0 && advertise.Count < 1 &&
                Repository.Find<User, int>(currentUserId).
                UserHasSimilarReserve(advertiseId,
                startDateGregorian, endDateGregorian))
            {
                msg = "شما یک درخواست مشابه برای این آگهی دارید، برای درخواست جدید درخواست قبلی را لغو کنید";
                return false;
            }
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
                    advertise.LastModifyDate = DateTime.Now;
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
                    advertise.LastModifyDate = DateTime.Now;
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
                advertise.LastModifyDate = DateTime.Now;
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
                advertise.LastModifyDate = DateTime.Now;
                Repository.Update(advertise);
                Repository.Save();
            }
            mediator.Send(new RemoveAdvertiseCacheCommand(advertise.Id));
        }

        public Dictionary<string, string> GetRulesDictionary(long id)
        {
            var advertise = Repository.Find(id);
            var dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(advertise.EvidenceRequired))
            {
                dictionary.Add("مدارک مورد نیاز", advertise.EvidenceRequired);
            }
            dictionary.Add("استعمال دخانیات", (bool)advertise.AllowSmoking ? "مجاز" : "ممنوع");
            dictionary.Add("گرفتن مهمانی", (bool)advertise.AllowParty ? "مجاز" : "ممنوع");
            dictionary.Add("آوردن حیوانات خانگی", (bool)advertise.AllowPets ? "مجاز" : "ممنوع");
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
            //var idsToDelete = acc.ExtrinsicReserves.Where(
            //    w => w.StartDate >= fromGregorian && w.StartDate <= toGregorian)
            //    .Select(s => s.Id).ToList();
            Repository.RemoveChildren<ExtrinsicReserve, long,
                IQueryable<ExtrinsicReserve>>(advertiseId, "ExtrinsicReserves",
                q => q.Where(w => w.StartDate >= fromGregorian &&
                w.StartDate < toGregorian).AsQueryable());
            Repository.Save();
            mediator.Send(new UpdateAdvertiseOccupiedCommand(advertiseId));
        }

        public bool ReserveRequest(long advertiseId, int userId, string startDate,
            string endDate, int guestCount, bool instantReserve, out string msg,
            out long reserveId)
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
            if (instantReserve)
            {
                instantReserve = advertise.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed;
            }
            if (instantReserve)
            {
                var formDateGregortian = DateTimeUtility.PersianDateToGregorian(startDate);
                instantReserve = formDateGregortian <= DateTime.Now.AddDays(advertise.MaxInstantReserveStart).Date;
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
                InstantReserve = instantReserve,
                CouponCalculationPrice = couponCalculationPrice
            };
            reserve.Status = instantReserve ? Reserve.ReserveStatus.WaitForReserve :
                Reserve.ReserveStatus.WaitForResponse;
            if (user.Reserves.Count(c => c.Status == ReserveStatus.WaitForResponse) >= 3)
            {
                msg = "شما نمیتوانید همزمان بیشتر از 3 درخواست رزرو بدهید.";
                reserveId = 0;
                return false;
            }
            if (advertise.Count < 1 &&
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
            var identityUser = userManager.FindByNameAsync(advertise.User.MainMobile).Result;
            var contact = new UserContactDTO()
            {
                UserMainMobile = advertise.User.MainMobile,
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
            msg = instantReserve ?
                        "لطفا مبلغ رزرو را پرداخت نمایید تا رزرو شما نهایی شود"
                        : "درخواست رزرو شما با موفقیت انجام شد. نتیجه درخواست به اطلاع شما خواهد رسید.";
            reserveId = reserve.Id;
            return true;
        }

        public IList<Advertise> GetNorouzAdvertises(int count)
        {
            var advertises = Repository.Query(q => q.Where(w =>
                w.NorouzPrice > 0 && w.Available &&
                w.Status == Advertise.AdvertiseStatus.Published &&
                w.HideInCategory == false && w.Count < 1));
            return advertises.OrderByDescending(o => o.AdvertiseScore).Take(count).ToList();
        }

        public void SetHygieneProtocol(long id, HygieneProtocolStatus value)
        {
            var acc = Repository.Find(id);
            acc.HygieneProtocol = value;
            Repository.Update(acc);
            Repository.Save();
            mediator.Send(new RemoveAdvertiseCacheCommand(acc.Id));
        }
    }
}
