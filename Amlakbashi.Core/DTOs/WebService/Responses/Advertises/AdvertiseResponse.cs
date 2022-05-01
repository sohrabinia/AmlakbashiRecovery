using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises.AdvertiseParts;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseResponse
    {
        public long id { get; set; }
        public string title { get; set; }
        public bool active { get; set; }
        public bool favorite { get; set; }
        public string typeTitle { get; set; }
        public int hostId { get; set; }
        public string hostName { get; set; }
        public string hostCreateDate { get; set; }
        public float hostReponseRate { get; set; }
        public int capacity { get; set; }
        public int extraCapacity { get; set; }
        public int roomCount { get; set; }
        public int singleBedCount { get; set; }
        public int doubleBedCount { get; set; }
        public int wcCount { get; set; }
        public string description { get; set; }
        public int buildingArea { get; set; }
        public int landArea { get; set; }
        public string floor { get; set; }
        public int price { get; set; }
        public int extraCapacityPrice { get; set; }
        public AdvertiseRulesResponse rules { get; set; }
        public AdvertiseAddressResponse location { get; set; }
        public AdvertiseCommentResponse comments { get; set; }
        public List<string> amenities { get; set; } = new List<string>();
        public List<AdvertiseChildsResponse> units { get; set; } = new List<AdvertiseChildsResponse>();
        public List<string> imagesUrls { get; set; } = new List<string>();
        public string hostImageUrl { get; set; }

        public static implicit operator AdvertiseResponse(Advertise advertise)
        {
            var response = new AdvertiseResponse();
            response.id = advertise.Id;
            response.active = advertise.CanPublish();
            response.title = advertise.Title;
            response.typeTitle = AdvertiseMainLocalization.GetAdvertiseTypeUserString(advertise.TypeID);
            response.hostId = advertise.UserID;
            response.hostName = advertise.User.FullName;
            response.hostImageUrl = advertise.User.GetUserImageApiUrl();
            if (advertise.User.HostReserves.Any())
            {
                response.hostReponseRate = ((float)advertise.User.HostReserves.Where(x => x.HostResponse != Reserve.HostResponseEnum.None).Count()
                / (float)advertise.User.HostReserves.Count) * 100;
            }
            response.capacity = advertise.Capacity;
            response.extraCapacity = advertise.MoreThanCapacity;
            response.roomCount = advertise.Room;
            response.singleBedCount = advertise.SingleBed;
            response.doubleBedCount = advertise.DoublesBed;
            response.description = advertise.Description;
            response.buildingArea = advertise.Metrazh;
            response.landArea = advertise.LandArea;
            response.floor = advertise.Floor.ToString();
            response.price = advertise.BasePrice;
            response.extraCapacityPrice = advertise.MoreThanCapacityPrice;
            response.rules = new AdvertiseRulesResponse()
            {
                party = advertise.AllowParty,
                pets = advertise.AllowPets,
                smoking = advertise.AllowSmoking,
                otherRules = advertise.OtherRules,
                requiredEvidences = advertise.EvidenceRequired,
                reserveCancellationRules = AdvertiseMainLocalization.GetReserveCancelationRules(),
                nowruzReserveCancellationRules = AdvertiseMainLocalization.GetNowruzReserveCancelationRules()
            };
            response.location = new AdvertiseAddressResponse()
            {
                address = advertise.Address,
                latitude = advertise.Latitude,
                longitude = advertise.Longitude,
                province = advertise.RegionProvince?.PersianName,
                city = advertise.RegionCity?.PersianName,
                area = advertise.RegionArea?.PersianName
            };
            var rates = advertise.UserRatingDict();
            response.comments = new AdvertiseCommentResponse()
            {
                rateCount = rates.Count
            };
            if (rates.Count > 0)
            {
                response.comments.rate = (float)rates.Values.Select(s => s.Sum(x => x.Score)).Sum() / (float)rates.Values.Select(s => s.Count).Sum();
            }
            var detailedRatesTypes = Enum.GetValues(typeof(Comment.UserRatingType)) as Comment.UserRatingType[];
            foreach (var item in detailedRatesTypes)
            {
                var itemRates = advertise.ReportItems.Where(w => w.ReportID == (int)item);
                response.comments.detailedRates.Add(new AdvertiseRateItemResponse(ReportItem.GetUserRatingTypeString(item),
                    itemRates.Any() == false ? 0 : (float)itemRates.Average(x => x.Score)));
            }
            var comments = advertise.PublishedComments();
            response.comments.comments.AddRange(comments.Select(s => new AdvertiseCommentItemResponse()
            {
                comment = s.Text,
                date = StringUtility.EnglishNumberToPersian(DateTimeUtility.ConvertDate(s.CreateDate)),
                name = s.SenderUser.FullName,
                imageUrl = s.SenderUser.GetUserImageApiUrl()
            }).ToList());
            foreach (var item in advertise.GetActiveAmeneties())
            {
                response.amenities.Add(AdvertiseMainLocalization.GetPropertyTitle(item));
            }
            if (advertise.Mode == Advertise.AdvertiseMode.Parent)
            {
                response.units.AddRange(advertise.Childs.Select(x => new AdvertiseChildsResponse()
                {
                    id = x.Id,
                    roomCount = x.Room,
                    singleBedCount = x.SingleBed,
                    doubleBedCount = x.DoublesBed,
                    capacity = x.Capacity,
                    extraCapacity = x.MoreThanCapacity,
                    price = x.DailyPrice,
                    holidyPrice = x.HolidayPrice,
                    peakHolidayPrice = x.HolidayPikePrice,
                    extraCapacityPrice = x.MoreThanCapacityPrice
                }));
            }
            response.imagesUrls = advertise.GetImagesApiUrls();
            return response;
        }
    }
}
