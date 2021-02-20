using Amlakbashi.Core.Common.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Amlakbashi.Core.Entities
{
    public class DiscountTable : Entity<int>, ISoftDelete
    {
        [Column("DiscountTableID")]
        public override int Id { get; set; }
        public long AdvertiseID { get; set; }
        [ForeignKey("AdvertiseID")]
        [JsonIgnore]
        public virtual Advertise Advertise { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Percent { get; set; }
        public bool IsDeleted { get; set; }

        public bool Validate(out List<string> msg)
        {
            msg = new List<string>();
            if (Percent <= 0 || Percent > 100)
            {
                msg.Add("میزان تخفیف باید عددی بین 0 و 100 باشد");
            }
            if (From == null)
            {
                msg.Add("تاریخ شروع تخفیف را وارد کنید");
            }
            if (To == null)
            {
                msg.Add("تاریخ پایان تخفیف را وارد کنید");
            }
            if (To < From)
            {
                msg.Add("تاریخ پایان نباید از تاریخ شروع کمتر باشد");
            }
            return msg.Any() == false;
        }
    }
}
