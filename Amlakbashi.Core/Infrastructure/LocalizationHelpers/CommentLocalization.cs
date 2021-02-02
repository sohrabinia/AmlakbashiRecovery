using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class CommentLocalization
    {
        public static string GetCommentTypeTitle(int comment_type)
        {
            switch ((Comment.CommentType)comment_type)
            {
                case Comment.CommentType.advertise:
                    return "آگهی";
                case Comment.CommentType.post:
                    return "پست";
                case Comment.CommentType.advertiseHostReply:
                    return "پاسخ میزبان";
                default:
                    return "";
            }
        }
    }
}
