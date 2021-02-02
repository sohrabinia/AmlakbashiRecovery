using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.CommentDTOs
{
    [Serializable]
    public class CommentOverviewDTO
    {
        public CommentOverviewDTO(long advertiseId, long parentAdvertiseId, string url, long photoId,
            string title, string typeString, string parentTitle,
            string selfTitle, int commentCount, int newCommentCount,
            float overallScore, int rateCount, ScoreDetailDTO scoreDetail)
        {
            this.advertiseId = advertiseId;
            this.parentAdvertiseId = parentAdvertiseId;
            this.url = url;
            this.photoId = photoId;
            this.title = title;
            this.typeString = typeString;
            this.parentTitle = parentTitle;
            this.selfTitle = selfTitle;
            this.commentCount = commentCount;
            this.newCommentCount = newCommentCount;
            this.overallScore = overallScore;
            this.rateCount = rateCount;
            this.scoreDetail = scoreDetail;
        }
        public long advertiseId { get; private set; }
        public long parentAdvertiseId { get; private set; }
        public string url { get; private set; }
        public long photoId { get; private set; }
        public string title { get; private set; }
        public string typeString { get; private set; }
        public string parentTitle { get; private set; }
        public string selfTitle { get; private set; }
        public int commentCount { get; private set; }
        public int newCommentCount { get; private set; }
        public float overallScore { get; private set; }
        public int rateCount { get; private set; }
        public ScoreDetailDTO scoreDetail { get; private set; }
    }
}
