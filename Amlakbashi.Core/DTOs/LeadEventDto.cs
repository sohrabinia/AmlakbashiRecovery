using System;

namespace Amlakbashi.Core.DTOs
{
    public class LeadEventDto
    {
        public long ResidenceId { get; set; }
        public int HostUserId { get; set; }
        public int? GuestUserId { get; set; }
        public string EventType { get; set; }
        public string DeduplicationKey { get; set; }
        public string ClientIp { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
