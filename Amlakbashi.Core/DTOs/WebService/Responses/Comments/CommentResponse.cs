using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Comments
{
    public class CommentResponse
    {
        public long id { get; set; }
        public string userName { get; set; }
        public string date { get; set; }
        public string comment { get; set; }
        public string userImageUrl { get; set; }
        public string residencyTitle { get; set; }
        public string residencyImageUrl { get; set; }
        public string hostReplyComment { get; set; }

        public static implicit operator CommentResponse(Comment comment)
        {
            return new CommentResponse()
            {
                id = comment.Id,
                userName = comment.SenderUser.FullName,
                date = DateTimeUtility.GregorianToPersianDate(comment.CreateDate),
                comment = comment.Text,
                userImageUrl = "",
                residencyTitle = comment.Advertise.Title,
                residencyImageUrl = "",
                hostReplyComment = comment.HostReply?.Text
            };
        }
    }
}
