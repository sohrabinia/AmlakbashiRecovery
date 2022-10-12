using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class UpdateInstantReserveDatesRequest
    {
        public long residenceId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}
