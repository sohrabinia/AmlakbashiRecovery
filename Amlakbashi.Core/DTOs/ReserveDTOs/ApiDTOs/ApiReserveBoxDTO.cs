using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.ReserveDTOs.ApiDTOs
{
    [Serializable]
    public class ApiReserveBoxDTO
    {
        public int count { get; set; }
        public string[] infoMessages { get; set; }
        public bool hasChat { get; set; }
        public List<long> reserveIds { get; set; }
    }
}
