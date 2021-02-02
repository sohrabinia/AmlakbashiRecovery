using System;
using static Amlakbashi.Core.Entities.AdvertiseReport;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class AdvertiseReportDTO 
    {
        public int id { get; set; }
        public long accId { get; set; }
        public ReportReason reason { get; set; }
        public string reasonString { get; set; }
    }
}
