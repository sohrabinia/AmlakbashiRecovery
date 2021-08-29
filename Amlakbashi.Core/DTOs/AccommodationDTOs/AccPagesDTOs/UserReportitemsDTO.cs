using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class UserReportitemsDTO
    {
        public User User { get; set; }
        public List<ReportItem> ReportItems { get; set; }
    }
}
