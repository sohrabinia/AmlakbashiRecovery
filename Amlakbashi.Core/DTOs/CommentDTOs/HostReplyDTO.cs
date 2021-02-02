using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.CommentDTOs
{
    [Serializable]
    public class HostReplyDTO
    {
        public long advertiseID { get; set; }
        public int guestUserId { get; set; }
        public string text { get; set; }
        public long parentID { get; set; }
    }
}
