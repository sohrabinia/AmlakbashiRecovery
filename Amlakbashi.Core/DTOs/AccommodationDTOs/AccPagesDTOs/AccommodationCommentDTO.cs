using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class AccommodationCommentDTO
    {
        public Comment Comment { get; set; }
        public Comment ReplyComment { get; set; }
        public User User { get; set; }
        public List<ReportItem> ReportItems { get; set; }
    }
}
