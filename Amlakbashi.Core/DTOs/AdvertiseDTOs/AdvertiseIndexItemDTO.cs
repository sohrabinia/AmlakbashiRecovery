using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AdvertiseDTOs
{
    public class AdvertiseIndexItemDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int DailyPrice { get; set; }
        public long AdvertiseScore { get; set; }
        public int WebVisit { get; set; }
        public long UnixNorouzMinReserveDate { get; set; }
        public int SupportInfoCount { get; set; }
        public Advertise.AdvertiseStatus Status { get; set; }
        public string UserPhoneNumber { get; set; } = "کاربر حذف شده";
        public long UserScore { get; set; } = 0;
        public string CityPersianName { get; set; }

        public static implicit operator AdvertiseIndexItemDTO (Advertise advertise)
        {
            var dto = new AdvertiseIndexItemDTO()
            {
                Id = advertise.Id,
                Title = advertise.Title,
                UserId = advertise.UserID,
                CreateDate = advertise.CreateDate,
                DailyPrice = advertise.DailyPrice,
                AdvertiseScore = advertise.AdvertiseScore,
                WebVisit = advertise.WebVisit,
                UnixNorouzMinReserveDate = advertise.unixNorouzMinRequestDate,
                Status = advertise.Status,
                SupportInfoCount = advertise.GetSupportInfoList().Length,
            };
            return dto;
        }
    }
}
