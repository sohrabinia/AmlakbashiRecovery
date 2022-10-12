using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Advertises
{
    public class UpdatePermanentInstantReserveRequest
    {
        public long residenceId { get; set; }
        public bool active { get; set; }
    }
}
