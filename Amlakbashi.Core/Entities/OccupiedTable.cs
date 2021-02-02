using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class OccupiedTable : Entity<long>
    {
        [Column("OccupiedTableID")]
        public override long Id { get; set; }
        public long AdvertiseID { get; set; }
        [ForeignKey("AdvertiseID")]
        public Advertise Advertise;
        public long? ReserveID { get; set; }
        [ForeignKey("ReserveID")]
        public Reserve Reserve { get; set; }
        public long? ExtrinsicReserveID { get; set; }
        [ForeignKey("ExtrinsicReserveID")]
        public ExtrinsicReserve ExtrinsicReserve { get; set; }
        public DateTime Date { get; set; }
    }
}
