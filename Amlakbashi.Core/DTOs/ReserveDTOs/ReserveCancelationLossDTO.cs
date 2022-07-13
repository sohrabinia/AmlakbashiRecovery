using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveCancelationLossDTO
    {
        public long SitePortion { get; set; }
        public long HostPortion { get; set; }
        public long GuestPortion { get; set; }
    }
}
