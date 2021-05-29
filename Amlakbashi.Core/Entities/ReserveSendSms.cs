using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// ارسال گروهی پیامک های رزرو ها در یک فاصله زمانی مشخص
    /// </summary>
    public class ReserveSendSms : Entity<long>
    {
        public override long Id { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool initial { get; set; }
        public int userId { get; set; }
        public int type { get; set; }
        public string advertise_id { get; set; }
        public string user_id { get; set; }
        public string reserve_id { get; set; }
        public string transaction_id { get; set; }
        public string audience_mobile { get; set; }
        public string price { get; set; }
        public string remain_price { get; set; }
        public string doer_title { get; set; }
        public string cause_string { get; set; }
        public string code { get; set; }
        public string extra_1 { get; set; }
        public string extra_2 { get; set; }
        public string extra_3 { get; set; }
    }
}
