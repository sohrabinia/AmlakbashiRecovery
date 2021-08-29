using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    [Serializable]
    public class AccommodationReportItemDTO
    {
        public int Rating { get; set; }
        public int CountReport { get; set; }
        public float FloatRating { get; set; }
        //public Dictionary<User, List<ReportItem>> ReportList { get; set; }
        public List<UserReportitemsDTO> ReportList { get; set; } = new List<UserReportitemsDTO>();
    }
}
