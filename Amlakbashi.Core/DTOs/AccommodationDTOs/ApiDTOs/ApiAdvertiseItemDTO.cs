using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiAdvertiseItemDTO
    {
        public long id { get; set; }
        public long key { get; set; }
        public string title { get; set; }
        public long image { get; set; }
        public long price { get; set; }
        public long norouzPrice { get; set; }
        public bool norouzSpecial { get; set; }
        public int room { get; set; }
        public int minCapacity { get; set; }
        public int maxCapacity { get; set; }
        public bool todayEmpty { get; set; }
        public int discountPercent { get; set; }
        public string discountDateString { get; set; }
        public bool elevator { get; set; }
        public bool pool { get; set; }
        public bool parking { get; set; }
        public int adType { get; set; }
        public int adPosition { get; set; }
        public string regionString { get; set; }
        public bool hasChild { get; set; }
        public float rating { get; set; }
        public bool instantReserveAvailable { get; set; }
        public int minReserveDays { get; set; }
        public int maxReserveDays { get; set; }

        public static implicit operator ApiAdvertiseItemDTO(Advertise advertise)
        {
            int discountPercent = 0;
            string discountDateString = "";
            var discountData = advertise.GetFirstDiscountData(true, true);
            if (discountData.Percent > 0)
            {
                discountDateString = discountData.DateString;
                discountPercent = discountData.Percent;
            }
            //var norouzPrice = advertise.Childs.Any() ?
            //        advertise.Childs.Min(x => x.NorouzPrice) :
            //        advertise.NorouzPrice;
            var norouzPrice = 0;

            var dto = new ApiAdvertiseItemDTO();
            dto.id = advertise.Id;
            dto.key = advertise.Id;
            dto.title = advertise.Title;
            dto.image = advertise.PhotoID == null ? 0 : (int)advertise.PhotoID;
            dto.price = advertise.BasePrice;
            dto.norouzPrice = norouzPrice;
            dto.norouzSpecial = norouzPrice > 0;
            dto.room = advertise.Room;
            dto.minCapacity = advertise.Childs.Any() ?
                advertise.Childs.Min(x => x.Capacity) :
                advertise.Capacity;
            dto.maxCapacity = advertise.Childs.Any() ?
                advertise.Childs.Min(x => x.Capacity + x.MoreThanCapacity) :
                advertise.Capacity + advertise.MoreThanCapacity;
            dto.adType = ConvertAdvertiseType((int)advertise.TypeID);
            dto.adPosition = ConvertAdvertisePosition((int)advertise.Position);
            dto.elevator = advertise.Elevator == null ? false : (bool)advertise.Elevator;
            dto.pool = (bool)advertise.Pool;
            dto.parking = advertise.Parking != Advertise.ParkingItems.Unset &&
                advertise.Parking != Advertise.ParkingItems.NoParking;
            dto.todayEmpty = advertise.TodayIsEmpty || advertise.Childs.Any(x => x.TodayIsEmpty);
            dto.discountPercent = discountPercent;
            dto.discountDateString = discountDateString;
            dto.regionString = advertise.LocationString;
            dto.rating = advertise.AverageUserRating;
            dto.hasChild = advertise.Mode == Advertise.AdvertiseMode.Parent;
            dto.instantReserveAvailable = advertise.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Confirmed;
            dto.minReserveDays = advertise.MinReserveDays;
            dto.maxReserveDays = advertise.MaxReserveDays;
            return dto;
        }

        public static int ConvertAdvertiseType(int advertiseType)
        {
            switch ((Advertise.AdvertiseType)advertiseType)
            {
                case Advertise.AdvertiseType.Apartment:
                    return 1;
                case Advertise.AdvertiseType.Villa:
                    return 2;
                case Advertise.AdvertiseType.HotelApartment:
                    return 3;
                case Advertise.AdvertiseType.House:
                    return 4;
                case Advertise.AdvertiseType.SuitAndRoom:
                    return 5;
                case Advertise.AdvertiseType.Hut:
                    return 6;
                case Advertise.AdvertiseType.TourismAccommodation:
                    return 7;
                case Advertise.AdvertiseType.Hotel:
                    return 8;
                case Advertise.AdvertiseType.Inn:
                    return 9;
                case Advertise.AdvertiseType.Camp:
                    return 10;
                case Advertise.AdvertiseType.Pansion:
                    return 11;
                case Advertise.AdvertiseType.Complex:
                    return 12;
                default:
                    return 0;
            }
        }

        public static int ConvertAdvertisePosition(int advertisePosition)
        {
            switch ((Advertise.PositionType)advertisePosition)
            {
                case Advertise.PositionType.sahel:
                    return 1;
                case Advertise.PositionType.jungle:
                    return 2;
                case Advertise.PositionType.koohestani:
                    return 3;
                case Advertise.PositionType.biaban:
                    return 4;
                case Advertise.PositionType.shahri:
                    return 5;
                case Advertise.PositionType.hoome:
                    return 6;
                case Advertise.PositionType.roostaee:
                    return 7;
                case Advertise.PositionType.dakhele_shahrak:
                    return 8;
                case Advertise.PositionType.ashayeri:
                    return 9;
                case Advertise.PositionType.SummerQuarter:
                    return 10;
                default:
                    return 0;
            }
        }
    }
}
