using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.CommentDTOs
{
    [Serializable]
    public class CommentItemDTO
    {
        public CommentItemDTO(long commentId, int guestUserId, string name, long photoId, string dateString,
            float overallScore, string text, ScoreDetailDTO scoreDetail, CommentItemDTO reply = null)
        {
            this.commentId = commentId;
            this.guestUserId = guestUserId;
            this.name = name;
            this.photoId = photoId;
            this.dateString = dateString;
            this.text = text;
            this.overallScore = overallScore;
            this.scoreDetail = scoreDetail;
            this.reply = reply;
        }
        public long commentId { get; private set; }
        public int guestUserId { get; private set; }
        public string name { get; private set; }
        public long photoId { get; private set; }
        public string dateString { get; private set; }
        public float overallScore { get; private set; }
        public string text { get; private set; }
        public ScoreDetailDTO scoreDetail { get; private set; }
        public CommentItemDTO reply { get; private set; }
    }
}
