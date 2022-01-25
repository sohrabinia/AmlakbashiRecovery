using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AdvertiseDTOs
{
    public class AdvertiseIndexDTO
    {
        public List<AdvertiseIndexItemDTO> AdvertiseList { get; set; }
        public PagingDTO PagingInfo { get; set; }
        public int Page { get; set; } = 1;
        public long Id { get; set; } = 0;
        public Advertise.AdvertiseStatus Status { get; set; } = Advertise.AdvertiseStatus.Unset;
        public int Type { get; set; } = -1;
        public int UserId { get; set; } = -1;
        public string Sort { get; set; } = "score";
        public int InstatntReserveStatus { get; set; } = -1;
        public string MinReserveNorouzFromDate { get; set; } = "";
        public int ImageCountMin { get; set; } = 0;
        public int ImageCountMax { get; set; } = 0;
        public int Province { get; set; } = -1;
        public int City { get; set; } = -1;
        public int Area { get; set; } = -1;
        public int HygieneProtocolStatus { get; set; } = -1;
        public Advertise.ParkingItems Parking { get; set; } = Advertise.ParkingItems.Unset;
    }
}
