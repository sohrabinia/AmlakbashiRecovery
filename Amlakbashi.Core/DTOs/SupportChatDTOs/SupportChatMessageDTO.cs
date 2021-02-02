using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.SupportChatDTOs
{
    public class SupportChatMessageDTO
    {
        public long id { get; set; }
        public string text { get; set; }
        public string dateString { get; set; }
        public bool sent { get; set; }
        public bool read { get; set; }
        public int userId { get; set; }
        public long userPhotoId { get; set; }
        public string userName { get; set; }
        public bool self { get; set; }
    }
}
