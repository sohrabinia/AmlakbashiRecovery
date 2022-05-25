using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiAdvertiseDetailDTO
    {
        public long id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string capacity_string { get; set; }
        public int capacity { get; set; }
        public int extraCapacity { get; set; }
        public List<ApiAdvertiseDetailDTO> children_apartment { get; set; }
        public List<ApiAdvertiseDetailDTO> children_suit { get; set; }
        public List<ApiAdvertiseDetailDTO> children_house { get; set; }
        public List<ApiAdvertiseDetailDTO> children_villa { get; set; }
        public List<ApiAdvertiseDetailDTO> children_hut { get; set; }
        public List<ApiAdvertiseDetailDTO> children_hotel { get; set; }
        public int count { get; set; }
        public string images { get; set; }
        public string regionString { get; set; }
        public string address { get; set; }
        public bool allowParty { get; set; }
        public bool allowPets { get; set; }
        public bool allowSmoking { get; set; }
        public string evidenceRequired { get; set; }
        public string otherRules { get; set; }
        public int area { get; set; }
        public int landArea { get; set; }
        public bool? elevator { get; set; }
        public bool? pool { get; set; }
        public bool? sauna { get; set; }
        public bool? jacuzzi { get; set; }
        public bool? bathroom { get; set; }
        public bool? wifi { get; set; }
        public bool? washingMachine { get; set; }
        public bool? microwaveOven { get; set; }
        public bool? soundSystem { get; set; }
        public bool? golf { get; set; }
        public bool? poolTable { get; set; }
        public bool? foosball { get; set; }
        public bool? hairdryer { get; set; }
        public bool? tv { get; set; }
        public bool? oven { get; set; }
        public bool? refrigerator { get; set; }
        public bool? kitchenHood { get; set; }
        public bool? kitchenUtensils { get; set; }
        public bool? teaMaker { get; set; }
        public int blanketsAndMattresses { get; set; }
        public string extraBlanketCountString { get; set; }
        public string heatingSystemString { get; set; }
        public string coolingSystemString { get; set; }
        public string wcString { get; set; }
        public string parkingString { get; set; }
        public int room { get; set; }
        public string floorString { get; set; }
        public int singleBed { get; set; }
        public int doubleBed { get; set; }
        public long dailyPrice { get; set; }
        public long norouzPrice { get; set; }
        public long rentPrice { get; set; }
        public long holidayPrice { get; set; }
        public long pikeHolidayPrice { get; set; }
        public long moreThanCapacityPrice { get; set; }
        public ApiUserRatingItemDTO userRatings { get; set; }
        public int totalReserveCount { get; set; }
        public int userRatingCount { get; set; }
        public float userRatingOverallScore { get; set; }
        public bool isComplex { get; set; }
        public bool isHotel { get; set; }
        public int typeId { get; set; }
        public string typeString { get; set; }
        public List<ApiUserCommentItemDTO> userComments { get; set; }
        public bool favourited { get; set; }
        public string host_name { get; set; }
        public long host_image { get; set; }
        public bool reserveAvailable { get; set; }
        public string commentNotVerifyReason { get; set; }
        public string websiteUrl { get; set; }
        public bool instantReserveAvailable { get; set; }
        public int maxInstantReserveStart { get; set; }
        public int norouzOverCapacityPrice { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string rulesHeaderText { get; set; }
        public string[] rulesParagraphs { get; set; }

        public static ApiAdvertiseDetailDTO Generate(int userId, Advertise advertise,
            bool isHotelItem = false, bool favourited = false,
            bool getAllComments = false, string commentNotVerifyReason = "")
        {
            string images;
            List<Advertise> children;
            ApiUserRatingItemDTO _userRatings;
            List<ApiUserCommentItemDTO> _userComments;
            int _userRatingCount = 0;
            int _totalReserveCount = 0;
            bool _isComplex = false;
            bool _isHotel = false;
            string _rawUrl = null;
            List<ApiAdvertiseDetailDTO> _children_hotel;
            if (isHotelItem)
            {
                images = "";
                children = new List<Advertise>();
                _userRatings = new ApiUserRatingItemDTO();
                _userComments = new List<ApiUserCommentItemDTO>();
                _children_hotel = new List<ApiAdvertiseDetailDTO>();
            }
            else
            {
                children = advertise.Childs.Where(
                    x => x.Available).ToList();
                var images_list = advertise.Photos.Select(s => s.Id.ToString()).ToList();
                var main_image = advertise.PhotoID.ToString();
                images_list.Remove(main_image);
                images_list.Insert(0, main_image);
                images = string.Join(",", images_list);

                _userRatingCount = advertise.ReportItems.Count;
                _totalReserveCount = advertise.SuccessfullReserves().Count();
                _userRatings = new ApiUserRatingItemDTO()
                {
                    tidiness = (float)advertise.ReportItems.Where(w => w.ReportID == 1).Select(s => s.Score).DefaultIfEmpty().Average(),
                    hostBehaviour = (float)advertise.ReportItems.Where(w => w.ReportID == 2).Select(s => s.Score).DefaultIfEmpty().Average(),
                    position = (float)advertise.ReportItems.Where(w => w.ReportID == 3).Select(s => s.Score).DefaultIfEmpty().Average(),
                    infoCorrectness = (float)advertise.ReportItems.Where(w => w.ReportID == 4).Select(s => s.Score).DefaultIfEmpty().Average(),
                    safety = (float)advertise.ReportItems.Where(w => w.ReportID == 5).Select(s => s.Score).DefaultIfEmpty().Average(),
                    priceWorth = (float)advertise.ReportItems.Where(w => w.ReportID == 6).Select(s => s.Score).DefaultIfEmpty().Average(),
                };
                _userComments = new List<ApiUserCommentItemDTO>();
                List<Comment> comments;
                if (getAllComments)
                {
                    comments = advertise.Comments.ToList();
                }
                else
                {
                    comments = advertise.Comments.Take(3).ToList();
                }
                User user;
                IQueryable<ReportItem> advertiseReportItems = advertise.ReportItems.AsQueryable();
                float average;
                ApiUserRatingItemDTO ratingDetail;
                if (getAllComments)
                {
                    var temp_user_ids = comments.Select(x => x.SenderUserID).Distinct().ToList();
                    var user_ids = advertiseReportItems.Where(
                        x => !temp_user_ids.Contains(x.UserID)).Select(x => x.UserID).Distinct().ToList();
                    foreach (var uid in user_ids)
                    {
                        var ritems = advertiseReportItems.Where(x => x.UserID == uid);
                        var newestItem = ritems.OrderByDescending(x => x.LastModifyDate).First();
                        comments.Add(new Comment()
                        {
                            AdvertiseID = advertise.Id,
                            SenderUserID = uid,
                            RecieverUserID = advertise.UserID,
                            Id = 1000000 + newestItem.Id,
                            LastModifyDate = newestItem.LastModifyDate,
                            Text = ""
                        });
                    }
                }
                var hostUser = advertise.User;
                foreach (var comment in comments)
                {
                    user = comment.SenderUser;
                    ratingDetail = ApiUserRatingItemDTO.Generate(advertise,
                        comment.SenderUserID, out average);
                    var reply = comment.HostReply;
                    ApiUserCommentItemDTO userCommentReply = null;
                    if (reply != null && reply.Status == Comment.CommentStatus.publish)
                    {
                        userCommentReply = new ApiUserCommentItemDTO()
                        {
                            id = reply.Id,
                            comment = reply.Text,
                            photoId = hostUser.PhotoID == null ? 0 : (long)hostUser.PhotoID,
                            fullName = hostUser.FullName,
                            firstName = hostUser.FirstName,
                            score = 0,
                            date = reply.LastModifyDate,
                            ratingDetail = null,
                            persianDate = StringUtility.EnglishNumberToPersian(DateTimeUtility.ConvertDate(reply.LastModifyDate))
                        };
                    }
                    _userComments.Add(new ApiUserCommentItemDTO()
                    {
                        id = comment.Id,
                        comment = comment.Text,
                        photoId = user == null || user.PhotoID == null ? 0 : (long)user.PhotoID,
                        fullName = user == null ? null : user.FullName,
                        firstName = user == null ? null : user.FirstName,
                        score = average,
                        ratingDetail = ratingDetail,
                        date = comment.LastModifyDate,
                        persianDate = StringUtility.EnglishNumberToPersian(DateTimeUtility.ConvertDate(comment.LastModifyDate)),
                        reply = userCommentReply
                    });
                }
                _userComments = _userComments.
                    OrderBy(x => string.IsNullOrEmpty(x.comment)).
                    ThenByDescending(x => x.date).ToList();
                _isComplex = advertise.Childs.Count > 0 &&
                    advertise.Childs.ElementAt(0).Count == 0;
                _isHotel = !_isComplex && advertise.Childs.Count > 0;
                if (_isHotel)
                {
                    _children_hotel = children.Select(s => Generate(userId, s, true)).ToList();
                }
                else
                {
                    _children_hotel = new List<ApiAdvertiseDetailDTO>();
                }
                _rawUrl = AdvertiseUrlLocalization.SlugToAdvertiseUrl(advertise.Slug);
            }
            var host_user = advertise.User;

            double lat = 0, lng = 0;
            if (advertise.Latitude != 0 && advertise.Longitude != 0)
            {
                var rnd = new Random();
                var OldRange = (1 - 0);
                var NewRange = (advertise.Latitude + 0.0017 - advertise.Latitude - 0.0017);
                var NewValue = (((rnd.NextDouble() - 0) * NewRange) / OldRange) + advertise.Latitude - 0.0017;
                lat = NewValue;
                NewRange = (advertise.Longitude + 0.0017 - advertise.Longitude - 0.0017);
                NewValue = (((rnd.NextDouble() - 0) * NewRange) / OldRange) + advertise.Longitude - 0.0017;
                lng = NewValue;
            }
            string[] _rulesParagraphs = new string[5];
            var _rulesHeaderText = "";
            return new ApiAdvertiseDetailDTO()
            {
                id = advertise.Id,
                title = advertise.Title,
                favourited = favourited,
                address = advertise.Address,
                allowParty = advertise.AllowParty,
                allowPets = advertise.AllowPets,
                allowSmoking = advertise.AllowSmoking,
                area = advertise.Metrazh,
                bathroom = advertise.Bathroom,
                blanketsAndMattresses = advertise.BlanketsAndMattresses,
                count = advertise.Count,
                dailyPrice = advertise.DailyPrice,
                description = advertise.Description,
                doubleBed = advertise.DoublesBed,
                singleBed = advertise.SingleBed,
                elevator = advertise.Elevator == null ? false : (bool)advertise.Elevator,
                pool = advertise.Pool,
                poolTable = advertise.PoolTable,
                golf = advertise.Golf,
                foosball = advertise.Foosball,
                hairdryer = advertise.Hairdryer,
                kitchenHood = advertise.KitchenHood,
                kitchenUtensils = advertise.KitchenUtensils,
                evidenceRequired = advertise.EvidenceRequired,
                otherRules = advertise.OtherRules,
                jacuzzi = advertise.Jacuzzi,
                sauna = advertise.Sauna,
                oven = advertise.Oven,
                landArea = advertise.LandArea,
                holidayPrice = advertise.HolidayPrice,
                pikeHolidayPrice = advertise.HolidayPikePrice,
                moreThanCapacityPrice = advertise.MoreThanCapacityPrice,
                //norouzPrice = advertise.Childs.Any() ?
                //    advertise.Childs.Min(x => x.NorouzPrice) :
                //    advertise.NorouzPrice,
                norouzPrice = 0,
                rentPrice = advertise.RentPrice,
                soundSystem = advertise.SoundSystem,
                refrigerator = advertise.Refrigerator,
                teaMaker = advertise.TeaMaker,
                washingMachine = advertise.WashingMachine,
                microwaveOven = advertise.MicrowaveOven,
                tv = advertise.TV,
                wifi = advertise.Wifi,
                capacity_string = advertise.Capacity < 1 ? "" : (advertise.MoreThanCapacity > 0 ? advertise.Capacity +
                " تا " + (advertise.Capacity + advertise.MoreThanCapacity) +
                " مهمان" : advertise.Capacity + " مهمان"),
                images = images,
                coolingSystemString = advertise.CoolingSystem < 0 ? "" :
                    AdvertiseMainLocalization.GetPropertyValueTitle(
                    (CoolingSystemItems)advertise.CoolingSystem),
                heatingSystemString = advertise.HeatingSystem < 0 ? "" :
                    AdvertiseMainLocalization.GetPropertyValueTitle(
                    (HeatingSystemItems)advertise.HeatingSystem),
                parkingString = advertise.Parking < 0 ? "ندارد" : AdvertiseMainLocalization.GetPropertyValueTitle(
                (ParkingItems)advertise.Parking),
                regionString = RegionLocalization.GetAccItemRegionString(
                    advertise.Province != null ? advertise.RegionProvince.PersianName : "",
                    advertise.City != null ? advertise.RegionCity.PersianName : "",
                    advertise.Area != null ? advertise.RegionArea.PersianName : "",
                    (int)advertise.CountryDirection),
                extraBlanketCountString = (ExtraBlanketCountItems)advertise.ExtraBlanketCount == ExtraBlanketCountItems.Unset ?
                    "" : AdvertiseMainLocalization.GetPropertyValueTitle(
                    advertise.ExtraBlanketCount),
                wcString = AdvertiseMainLocalization.GetPropertyValueTitle(
                    advertise.WC),
                floorString = advertise.Floor == FloorItems.Unset ?
                    "" : AdvertiseMainLocalization.GetPropertyValueTitle(
                    advertise.Floor),
                room = advertise.Room,
                children_apartment = children.Where(x => x.TypeID ==
                    AdvertiseType.Apartment).Select(s =>
                    Generate(userId, s)).ToList(),
                children_suit = children.Where(x => x.TypeID ==
                    AdvertiseType.SuitAndRoom).Select(s =>
                    Generate(userId, s)).ToList(),
                children_house = children.Where(x => x.TypeID ==
                    AdvertiseType.House).Select(s =>
                    Generate(userId, s)).ToList(),
                children_villa = children.Where(x => x.TypeID ==
                    AdvertiseType.Villa).Select(s =>
                    Generate(userId, s)).ToList(),
                children_hut = children.Where(x => x.TypeID ==
                    AdvertiseType.Hut).Select(s =>
                    Generate(userId, s)).ToList(),
                children_hotel = _children_hotel,
                userRatings = _userRatings,
                totalReserveCount = _totalReserveCount,
                userRatingCount = _userRatingCount,
                userRatingOverallScore = advertise.AverageUserRating,
                userComments = _userComments,
                isComplex = _isComplex,
                isHotel = _isHotel,
                typeId = (int)advertise.TypeID,
                capacity = advertise.Capacity,
                extraCapacity = advertise.MoreThanCapacity,
                host_name = host_user.LastName != null ? host_user.LastName : "",
                host_image = host_user.PhotoStatus == (int)Entities.User.UserPhotoState.publish ? (host_user.PhotoID == null ? 0 : (long)host_user.PhotoID) : 0,
                typeString = AdvertiseMainLocalization.GetAdvertiseTypePersianString((int)advertise.TypeID),
                reserveAvailable = advertise.Status == AdvertiseStatus.Published && advertise.Available,
                commentNotVerifyReason = commentNotVerifyReason,
                websiteUrl = isHotelItem ? null : GeneralData.WebsiteUrl + _rawUrl,
                instantReserveAvailable = advertise.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed,
                maxInstantReserveStart = advertise.MaxInstantReserveStart,
                //norouzOverCapacityPrice = advertise.NorouzOverCapacityPrice,
                latitude = lat,
                longitude = lng,
                rulesHeaderText = _rulesHeaderText,
                rulesParagraphs = _rulesParagraphs
            };
        }
    }
}
