using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveIndexSupportInfoDTO
    {
        public long Id { get; set; }
        public int HostCallState { get; set; }
        public int GuestCallState { get; set; }
        public int SupportInfoCount { get; set; }
        public List<string> SupportInfoList { get; set; }
    }
}
