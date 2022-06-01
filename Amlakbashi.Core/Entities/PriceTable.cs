using Amlakbashi.Core.Common.Entity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// قیمت های مشخص شده هر آگهی در تاریخ های مختلف
    /// </summary>
    public class PriceTable : Entity<int>, ISoftDelete
    {
        [Column("PriceTableID")]
        public override int Id { get; set; }
        public long AdvertiseID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Price { get; set; }
        public long UnixDate { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        [ForeignKey("AdvertiseID")]
        public virtual Advertise Advertise { get; set; }
    }
}
