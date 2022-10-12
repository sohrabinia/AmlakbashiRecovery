using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System.Linq;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class AccommodationCardDTO
    {
        public long AdvertiseID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long DailyPrice { get; set; }
        public long NorouzPrice { get; set; }
        public bool Favourited { get; set; }
        public int Room { get; set; }
        public int MinCapacity { get; set; }
        public int MaxCapacity { get; set; }
        public Advertise.AdvertiseType AdvertiseType { get; set; }
        public string AdvertiseTypeString { get; set; }
        public int DiscountPercent { get; set; }
        public string DiscountDateString { get; set; }
        public bool TodayIsEmpty { get; set; }
        public string Slug { get; set; }
        public long PhotoID { get; set; }
        public string RegionString { get; set; }
        public float OverallRate { get; set; }
        public int RateCount { get; set; }
        public bool HasChild { get; set; }
        public bool instantReserveAvailable { get; set; }
        public int minReserveDays { get; set; }
        public int maxReserveDays { get; set; }
        public string address { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }

        public static implicit operator AccommodationCardDTO(Advertise advertise)
        {
            var dto = new AccommodationCardDTO();
            int discountPercent = 0;
            string discountDateString = "";
            var discount = advertise.GetFirstDiscountData(true, true);
            if (discount.Percent > 0)
            {
                discountPercent = discount.Percent;
                discountDateString = discount.DateString;
            }
            bool has_child = advertise.Childs.Any();
            dto.AdvertiseID = advertise.Id;
            dto.Title = advertise.Title;
            dto.Description = advertise.Description;
            dto.Slug = advertise.Slug;
            dto.DailyPrice = advertise.BasePrice;
            dto.NorouzPrice = advertise.NowruzPrice;
            dto.AdvertiseType = advertise.TypeID;
            dto.AdvertiseTypeString = !has_child ? null :
                AdvertiseMainLocalization.GetAdvertiseTypePersianNameForUser(advertise.TypeID);
            dto.Room = advertise.RoomCount;
            dto.MinCapacity = has_child ? advertise.Childs.Max(m => m.Capacity) : advertise.Capacity;
            dto.MaxCapacity = has_child ? dto.MinCapacity : advertise.Capacity + advertise.ExtraCapacity;
            dto.TodayIsEmpty = advertise.EmptyTonight || (has_child && advertise.Childs.Any(x => x.EmptyTonight));
            dto.OverallRate = advertise.AverageUsersScore;
            //RateCount = rate_count,
            dto.RateCount = 0; //TODO replace with proper value
            dto.PhotoID = advertise.MainPhotoId == null ? 0 : (int)advertise.MainPhotoId;
            dto.HasChild = has_child;
            dto.instantReserveAvailable = advertise.InstantReserveStatus == Advertise.InstantReserveStatusEnum.Permanent;
            dto.minReserveDays = advertise.MinReserveDuration;
            dto.maxReserveDays = advertise.MaxReserveDuration;
            dto.address = advertise.Address;
            dto.DiscountPercent = discountPercent;
            dto.DiscountDateString = discountDateString;
            dto.provinceName = advertise.RegionProvince.PersianName;
            dto.cityName = advertise.RegionCity.PersianName;
            dto.areaName = advertise.AreaId != null ? advertise.RegionArea.PersianName : null;
            dto.RegionString = RegionLocalization.GetAccItemRegionString(dto.provinceName, dto.cityName, dto.areaName,
                        (int)advertise.CountryDirection);
            return dto;
        }
    }
}
