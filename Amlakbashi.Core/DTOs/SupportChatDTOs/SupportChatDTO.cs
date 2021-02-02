using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.SupportChatDTOs
{
    public class SupportChatDTO
    {
        public long id { get; set; }
        public int userId { get; set; }
        public string userTitle { get; set; }
        public long userPhotoId { get; set; }
        public string userName { get; set; }
        public int supporterId { get; set; }
        public long supporterPhotoId { get; set; }
        public string supporterName { get; set; }
        public long reserveId { get; set; }
        public long advertiseId { get; set; }
        public long reserveSupporterId { get; set; }
        public long reserveSupporterPhotoId { get; set; }
        public string reserveSupporterName { get; set; }
        public int newMessageCount { get; set; }
    }
}
