using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Comments
{
    public class CommentHostSubmitRequest
    {
        public long commentId { get; set; }
        public string text { get; set; }

        [BindNever]
        public int userId { get; set; }
    }
}
