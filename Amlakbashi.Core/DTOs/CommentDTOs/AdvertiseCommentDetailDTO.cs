using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.CommentDTOs
{
    [Serializable]
    public class AdvertiseCommentDetailDTO
    {
        public CommentOverviewDTO Overview { get; set; }
        public CommentDetailDTO Detail { get; set; }
    }
}
