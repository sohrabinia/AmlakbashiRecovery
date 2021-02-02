using Amlakbashi.Core.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    [SoftDelete(EntityDefaults.IsDeleted)]
    public class Comment : Entity<long>, ISoftDelete
    {
        [Column("CommentID")]
        public override long Id { get; set; }
        public long AdvertiseID { get; set; }
        public long PostID { get; set; }
        public long? ParentID { get; set; }
        public int SenderUserID { get; set; }
        public int? RecieverUserID { get; set; }
        public CommentType type { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public long LastModifyDatetick { get; set; }
        public CommentStatus Status { get; set; }
        public string SuspendReason { get; set; }
        public bool SeenByHost { get; set; } //type=advertise only
        public int Score { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public long? HostReplyId { get; set; }
        [ForeignKey("HostReplyId")]
        public Comment HostReply { get; set; }
        public int? OperatorID { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("AdvertiseID")]
        public virtual Advertise Advertise { get; set; }

        [ForeignKey("SenderUserID")]
        public virtual User SenderUser { get; set; }

        [ForeignKey("RecieverUserID")]
        public virtual User RecieverUser { get; set; }

        [ForeignKey("OperatorID")]
        public virtual User Operator { get; set; }

        [ForeignKey("ParentID")]
        public virtual Comment Parent { get; set; }

        [JsonIgnore]
        public virtual ICollection<Comment> Childs { get; set; }

        public enum CommentType
        {
            advertise = 0,
            advertiseHostReply = 2,
            post = 1
        }

        public enum CommentStatus
        {
            ready = 0,
            publish = 2,
            suspend = 3,
            delete = 4
        }

        public enum UserRatingType
        {
            Tidiness = 1,
            HostBehaviour = 2,
            Position = 3,
            InfoCorrectness = 4,
            Safety = 5,
            PriceWorth = 6
        }

        public static string GetUserRatingTypeString(UserRatingType ratingType)
        {
            switch (ratingType)
            {
                case UserRatingType.Tidiness:
                    return "پاکیزگی اقامتگاه";
                case UserRatingType.HostBehaviour:
                    return "برخورد میزبان";
                case UserRatingType.Position:
                    return "موقعیت اقامتگاه";
                case UserRatingType.InfoCorrectness:
                    return "صحت مطالب";
                case UserRatingType.Safety:
                    return "امنیت اقامتگاه";
                case UserRatingType.PriceWorth:
                    return "ارزش نسبت به قیمت";
                default:
                    return "";
            }
        }
    }
}
