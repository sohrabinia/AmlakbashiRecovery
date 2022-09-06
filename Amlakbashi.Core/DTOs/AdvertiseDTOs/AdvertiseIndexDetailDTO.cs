using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.DTOs.AdvertiseDTOs
{
    public class AdvertiseIndexDetailDTO
    {
        public long Id { get; set; }
        public Advertise.AdvertiseMode Mode { get; set; }
        public long? ParentId { get; set; }
        public Advertise.AdvertiseStatus Status { get; set; }
        public bool Available { get; set; }
        public string CreateDate { get; set; }
        public int DailyPrice { get; set; }
        public long AdvertiseScore { get; set; }
        public int WebVisit { get; set; }
        public string NorouzMinReserveDate { get; set; }
        public int SupportInfoCount { get; set; }
        public string UserFullName { get; set; } = "کاربر حذف شده";
        public string CityPersianName { get; set; }
        public int UserId { get; set; }

        public static implicit operator AdvertiseIndexDetailDTO(Advertise advertise)
        {
            return new AdvertiseIndexDetailDTO()
            {
                Id = advertise.Id,
                Mode = advertise.Mode,
                ParentId = advertise.ParentId,
                UserId = advertise.UserId,
                CreateDate = DateTimeUtility.ConvertDate(advertise.CreateDate).ToString(),
                DailyPrice = advertise.DailyPrice,
                AdvertiseScore = advertise.ResidenceScore,
                WebVisit = advertise.View,
                NorouzMinReserveDate = advertise.MinReserveDateForNowruz < 1 ? "-" :
                    DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(advertise.MinReserveDateForNowruz)).Replace(",", "/"),
                Status = advertise.Status,
                Available = advertise.Active,
                SupportInfoCount = advertise.GetSupportInfoList().Length
            };
        }
    }
}
