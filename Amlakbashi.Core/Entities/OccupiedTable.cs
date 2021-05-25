using Amlakbashi.Core.Common.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// روز های پر آگهی ها
    /// برای استعلام پربودن اقامتگاه استفاده می شود
    /// بعد از پر شدن هر اقامتگاه به هر دلیل (رزرو یا تقویم)، اطلاعات این انتیتی بطور خودکار بروز رسانی می شود
    /// </summary>
    public class OccupiedTable : Entity<long>, ISoftDelete
    {
        [Column("OccupiedTableID")]
        public override long Id { get; set; }
        public long AdvertiseID { get; set; }

        [ForeignKey("AdvertiseID")]
        public virtual Advertise Advertise { get; set; }
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
