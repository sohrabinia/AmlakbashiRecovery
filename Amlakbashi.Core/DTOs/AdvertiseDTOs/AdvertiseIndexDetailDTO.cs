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
        public Advertise.AdvertiseStatus Status { get; set; }
        public string CreateDate { get; set; }
        public int DailyPrice { get; set; }
        public long AdvertiseScore { get; set; }
        public int WebVisit { get; set; }
        public string NorouzMinReserveDate { get; set; }
        public int SupportInfoCount { get; set; }
        public string UserFullName { get; set; } = "کاربر حذف شده";
        public long UserScore { get; set; } = 0;
        public string CityPersianName { get; set; }
        public int UserId { get; set; }

        public static implicit operator AdvertiseIndexDetailDTO(Advertise advertise)
        {
            return new AdvertiseIndexDetailDTO()
            {
                Id = advertise.Id,
                UserId = advertise.UserID,
                CreateDate = DateTimeUtility.ConvertDate(advertise.CreateDate).ToString(),
                DailyPrice = advertise.DailyPrice,
                AdvertiseScore = advertise.AdvertiseScore,
                WebVisit = advertise.WebVisit,
                NorouzMinReserveDate = advertise.unixNorouzMinRequestDate < 1 ? "-" :
                    DateTimeUtility.GregorianToPersianDate(DateTimeUtility.JSValueToDate(advertise.unixNorouzMinRequestDate)).Replace(",", "/"),
                Status = advertise.Status,
                SupportInfoCount = advertise.GetSupportInfoList().Length
            };
        }
    }
}
