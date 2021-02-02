using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.CommentDTOs
{
    [Serializable]
    public class CommentDetailDTO
    {
        public CommentDetailDTO(List<CommentItemDTO> comments)
        {
            this.comments = comments;
        }
        public List<CommentItemDTO> comments { get; private set; }
    }
}
