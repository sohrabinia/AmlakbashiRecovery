using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.ReserveDTOs.ApiDTOs
{
    [Serializable]
    public class ApiChatItemDTO
    {
        public long id { get; set; }
        public string text { get; set; }
        public string timeString { get; set; }
        public bool self { get; set; }
        public bool read { get; set; }
        public bool sent { get; set; }
        public long profileImageId { get; set; }
        public string profileName { get; set; }
    }
}
