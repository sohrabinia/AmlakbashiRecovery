using Amlakbashi.Core.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    [SoftDelete(EntityDefaults.IsDeleted)]
    public class SupportChat : Entity<long>, ISoftDelete
    {
        public int? UserID { get; set; }
        public int? SupporterID { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastMessageTime { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("SupporterID")]
        public virtual User Supporter { get; set; }

        [JsonIgnore]
        public virtual ICollection<SupportChatMessage> Messages { get; set; }

        public enum AutoQuestion
        {
            questionHowToReserve = 0,
            questionHowToContactHost = 1,
            questionCheckInCheckout = 2,
            questionEvidence = 3,
            questionCancelReserveRules = 4,
            questionPaymentGuest = 5,
            questionPaymentHost = 6
        }
    }
}
