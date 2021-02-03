using Amlakbashi.Core.Common.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class Post : Entity<long>, ISoftDelete
    {
        [Column("PostID")]
        public override long Id{ get; set; }
        public string Title { get; set; }
        public long FileID { get; set; }
        public string Abstract { get; set; }
        public string Description { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public long PhotoID { get; set; }
        public string Link { get; set; }
        public int Status { get; set; }
        public int UserID { get; set; }
        public bool IsDeleted { get; set; }

        public void SetStatus(PostStatus status)
        {
            Status = (int)status;
        }

        public PostStatus GetStatus()
        {
            return (PostStatus)Status;
        }

        public enum PostStatus
        {
            Suspend = 0,
            Published = 1,
            Deleted = 2,
            ReadyToPublish = 3
        }
    }
}
