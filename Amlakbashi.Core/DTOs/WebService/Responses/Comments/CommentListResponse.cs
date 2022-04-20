using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Comments
{
    public class CommentListResponse
    {
        public IList<CommentResponse> comments { get; set; } = new List<CommentResponse>();
        public PagingInfo pagingInfo { get; set; }
    }
}
