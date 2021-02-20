
using Amlakbashi.Core.Common.Entity;
using Amlakbashi.Core.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class ExtrinsicReserve : Entity<long>, IReserve, ISoftDelete
    {
        public int NotifierUserID { get; set; }
        public long AdvertiseID { get; set; }
        public int HostUserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("AdvertiseID")]
        public virtual Advertise Advertise { get; set; }

        [ForeignKey("NotifierUserID")]
        public virtual User NotifierUser { get; set; }

        [ForeignKey("HostUserID")]
        public virtual User HostUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
