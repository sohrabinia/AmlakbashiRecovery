using Amlakbashi.Core.Common.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class OccupiedTable : Entity<long>, ISoftDelete
    {
        [Column("OccupiedTableID")]
        public override long Id { get; set; }
        public long AdvertiseID { get; set; }

        // TODO: fix this
        [ForeignKey("AdvertiseID")]
        public Advertise Advertise;
        public long? ReserveID { get; set; }
        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }
        public long? ExtrinsicReserveID { get; set; }
        [ForeignKey("ExtrinsicReserveID")]
        public virtual ExtrinsicReserve ExtrinsicReserve { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }
    }
}
