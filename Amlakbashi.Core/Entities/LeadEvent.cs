using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    [Table("LeadEvents")]
    public class LeadEvent : Entity<long>
    {
        public override long Id { get; set; }
        public long ResidenceId { get; set; }
        public int HostUserId { get; set; }
        public int? GuestUserId { get; set; }
        public string EventType { get; set; } // e.g. "ShowMobile", "Inquiry", "ClickCall"
        public string DeduplicationKey { get; set; }
        public string ClientIp { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ResidenceId))]
        public virtual Advertise Residence { get; set; }

        [ForeignKey(nameof(HostUserId))]
        public virtual User HostUser { get; set; }

        [ForeignKey(nameof(GuestUserId))]
        public virtual User GuestUser { get; set; }
    }
}
