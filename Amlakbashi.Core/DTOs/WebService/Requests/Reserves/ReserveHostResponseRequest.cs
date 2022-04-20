using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Reserves
{
    public class ReserveHostResponseRequest
    {
        public Reserve.HostResponseEnum hostResponse { get; set; }
        public long reserveId { get; set; }
    }
}
