using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class ReserveAutoCancel : Entity<long>
    {
        public override long Id { get; set; }
        public DateTime ScheduledTime { get; set; }
        public long ReserveId { get; set; }
        public bool SendSms { get; set; }
        public bool Force { get; set; }
    }
}
