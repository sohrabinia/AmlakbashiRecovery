using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.User;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Common.Utilities;
using static Amlakbashi.Core.Entities.Comment;
using Amlakbashi.Core.Infrastructure.StyleHelpers;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs
{
    [Serializable]
    public class AccommodationManagerDTO
    {
        public InstantReserveAccessEnum instantReserveAccess { get; set; }
        public IEnumerable<DashboardAccDTO> accList { get; set; }
        public static AccommodationManagerDTO Generate(User user, List<Advertise> input)
        {
            var data = new AccommodationManagerDTO();
            data.instantReserveAccess = user.InstantReserveAccess;
            data.accList = new List<DashboardAccDTO>();
            var hotelAccTypes = GetHotelTypes();
            var verifiedStates = new List<AdvertiseStatus>() {
                AdvertiseStatus.Published,
                AdvertiseStatus.Archived
            };
            var allNotVerifyReasons = (NotVerifyReasonsEnum[])Enum.GetValues(typeof(NotVerifyReasonsEnum));
            foreach (var item in input)
            {
                var hasChildren = item.Childs.Any();
                var _capacityString = "";
                if (!hasChildren)
                {
                    _capacityString = item.MoreThanCapacity > 0 ?
                        item.Capacity + " تا " + (item.Capacity +
                        item.MoreThanCapacity) + " مهمان" :
                        item.Capacity + " مهمان";
                }
                var _notVerifyReasons = new List<string>();
                if (item.Status == AdvertiseStatus.NotVerified &&
                    !string.IsNullOrEmpty(item.NotVerifyReasons))
                {
                    var strPropertiesSelected = "," + item.NotVerifyReasons + ",";
                    foreach (var reason in allNotVerifyReasons)
                    {
                        if (strPropertiesSelected.Contains(string.Format(",{0},", (int)reason)))
                        {
                            _notVerifyReasons.Add(AdvertiseMainLocalization.GetNotVerifyReasonTitle((int)reason));
                        }
                    }
                }
                var _unavailableDates = new List<long>();
                var _extrinsicDates = new List<long>();
                if (!item.Childs.Any())
                {
                    //_unavailableDates = ReserveDepend.GetAdvertiseUnavailableDates(item.AdvertiseID,
                    //    ReserveDepend.OccupiedSelectType.ForFrom,
                    //    ReserveDepend.OccupiedSource.All, item, reserves, occupiedTables).ConvertAll(x => x.Replace(",", "/"));
                    _unavailableDates = item.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s)).ToList();
                    _extrinsicDates = item.ExtrinsicReserves.Select(s => DateTimeUtility.DateValueOfJS(s.StartDate)).ToList();
                }
                var _mainType = !hasChildren ? AccMainType.Single :
                        (hotelAccTypes.Contains(item.TypeID) ?
                        AccMainType.Hotel : AccMainType.Complex);
                var _isVerified = verifiedStates.Contains(item.Status);
                var _isActivated = item.Status == AdvertiseStatus.Published;
                var _hotelRooms = new List<DashboardHotelRoomDTO>();
                var _apartmentUnits = new List<DashboardComplexUnitDTO>();
                var _suitUnits = new List<DashboardComplexUnitDTO>();
                var _villaUnits = new List<DashboardComplexUnitDTO>();
                var _houseUnits = new List<DashboardComplexUnitDTO>();
                var _hutUnits = new List<DashboardComplexUnitDTO>();
                var todayUnix = DateTimeUtility.DateValueOfJS(DateTime.Now.Date);
                switch (_mainType)
                {
                    case AccMainType.Hotel:
                        var unitTitle = (AdvertiseType)item.TypeID == AdvertiseType.Camp ? "چادر" :
                            ((AdvertiseType)item.TypeID == AdvertiseType.TourismAccommodation ? "واحد" : "اتاق");
                        foreach (var child in item.Childs)
                        {
                            //var _ud = ReserveDepend.GetAdvertiseUnavailableDates(
                            //    item.AdvertiseID,
                            //    ReserveDepend.OccupiedSelectType.ForFrom,
                            //    ReserveDepend.OccupiedSource.All, child, reserves, occupiedTables).ConvertAll(x => x.Replace(",", "/"));
                            var _ud = child.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s)).ToList();

                            _hotelRooms.Add(new DashboardHotelRoomDTO()
                            {
                                id = child.Id,
                                title = child.Title,
                                todayIsEmpty = child.TodayIsEmpty,
                                todayIsFull = !child.TodayIsEmpty && _ud.Contains(todayUnix),
                                photoUrl = string.Format("/file/accthumblarge?accid={0}&fileid={1}", child.Id, child.PhotoID == null ? 0 : child.PhotoID),
                                roomUnitString = unitTitle,
                                unavailableDates = _ud,
                                anyComment = child.Comments.Any(a =>
                                    a.type == CommentType.advertise
                                    && a.Status == CommentStatus.publish),
                                newCommentCount = child.Comments.Count(a =>
                                    a.type == CommentType.advertise
                                    && a.Status == CommentStatus.publish
                                    && a.SeenByHost == false),
                                discounts = child.DiscountTables.Select(s => (DiscountDTO)s).ToList(),
                                isVerified = _isVerified,
                                isActivated = _isVerified
                            });
                        }
                        break;
                    case AccMainType.Complex:
                        foreach (var child in item.Childs)
                        {
                            var _ud = child.OccupiedDates().Select(s => DateTimeUtility.DateValueOfJS(s)).ToList();
                            var childData = new DashboardComplexUnitDTO()
                            {
                                id = child.Id,
                                title = child.Title,
                                todayIsEmpty = child.TodayIsEmpty,
                                todayIsFull = !child.TodayIsEmpty && _ud.Contains(todayUnix),
                                photoUrl = string.Format("/file/accthumblarge?accid={0}&fileid={1}", child.Id, child.PhotoID == null ? 0 : child.PhotoID),
                                previewUrl = string.Format("/accomodation/preview?id={0}", child.Id),
                                floor = (FloorItems)child.Floor,
                                floorString = "طبقه: " + AdvertiseMainLocalization.GetPropertyValueTitle((FloorItems)child.Floor),
                                roomCount = child.Room,
                                anyComment = child.Comments.Any(a =>
                                    a.type == CommentType.advertise
                                    && a.Status == CommentStatus.publish),
                                newCommentCount = child.Comments.Count(a =>
                                    a.type == CommentType.advertise
                                    && a.Status == CommentStatus.publish
                                    && a.SeenByHost == false),
                                userRating = child.AverageUserRating,
                                unavailableDates = _ud,
                                discounts = child.DiscountTables.Select(s => (DiscountDTO)s).ToList(),
                                isVerified = _isVerified,
                                isActivated = _isVerified
                            };
                            switch (child.TypeID)
                            {
                                case AdvertiseType.Apartment:
                                    _apartmentUnits.Add(childData);
                                    break;
                                case AdvertiseType.Villa:
                                    _villaUnits.Add(childData);
                                    break;
                                case AdvertiseType.SuitAndRoom:
                                    _suitUnits.Add(childData);
                                    break;
                                case AdvertiseType.House:
                                    _houseUnits.Add(childData);
                                    break;
                                case AdvertiseType.Hut:
                                    _hutUnits.Add(childData);
                                    break;
                            }
                        }
                        break;
                }
                var result = new DashboardAccDTO()
                {
                    id = item.Id,
                    title = item.Title,
                    mainType = _mainType,
                    isVerified = _isVerified,
                    isActivated = _isActivated,
                    photoUrl = string.Format("/file/accthumblarge?accid={0}&fileid={1}", item.Id, item.PhotoID == null ? 0 : item.PhotoID),
                    previewUrl = string.Format("/accomodation/preview?id={0}", item.Id),
                    editUrl = string.Format("/accomodation/accbasicform?id={0}", item.Id),
                    status = item.Status,
                    statusString = AdvertiseMainLocalization.GetAdvertiseStatusString((int)item.Status, true),
                    statusColor = AdvertiseStyleHelper.GetAdvertiseStatusColor((int)item.Status),
                    typeString = AdvertiseMainLocalization.GetAdvertiseTypeUserString(item.TypeID),
                    minReserveDays = item.MinReserveDays,
                    maxReserveDays = item.MaxReserveDays,
                    instantReserveStatus = item.InstantReserveStatus,
                    maxInstantReserveStart = item.MaxInstantReserveStart,
                    capacityString = _capacityString,
                    todayIsEmpty = item.AnyChildrenOrSelfIsEmpty,
                    todayIsFull = !item.AnyChildrenOrSelfIsEmpty && _unavailableDates.Contains(todayUnix),
                    locationString = item.LocationString,
                    notVerifyReasons = _notVerifyReasons,
                    anyComment = item.Comments.Any(a =>
                                    a.type == CommentType.advertise
                                    && a.Status == CommentStatus.publish),
                    newCommentCount = item.Comments.Count(a =>
                                    a.type == CommentType.advertise
                                    && a.Status == CommentStatus.publish
                                    && a.SeenByHost == false),
                    userRating = item.AverageUserRating,
                    unavailableDates = _unavailableDates,
                    extrinsicDates = _extrinsicDates,
                    discounts = _mainType != AccMainType.Single ? new List<DiscountDTO>() : item.DiscountTables.Select(s => (DiscountDTO)s).ToList(),
                    hotelRooms = _hotelRooms,
                    apartmentUnits = _apartmentUnits,
                    suitUnits = _suitUnits,
                    villaUnits = _villaUnits,
                    houseUnits = _houseUnits,
                    hutUnits = _hutUnits
                };
                (data.accList as List<DashboardAccDTO>).Add(result);
            }
            return data;
        }
    }
}
