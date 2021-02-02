using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class GroupPayment : Entity<int>
    {
        [Column("GroupPaymentID")]
        public override int Id { get; set; }
        public long TotalPrice { get; set; }
        public long PaidPrice { get; set; }
        public string ReserveIds { get; set; }
        public int StatusInt { get; set; }
        public int CountPayments { get; set; }
        public int CountFailedPayments { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PayDate { get; set; }
        public string PayListUrl { get; set; }
        public string PayResultUrl { get; set; }
        public int DownloadCount { get; set; }

        public GroupPayment Init(IEnumerable<long> reserveIds, long totalPrice, string payListUrl)
        {
            this.TotalPrice = totalPrice;
            this.CountPayments = reserveIds.Count();
            this.CreateDate = DateTime.Now;
            this.PayListUrl = payListUrl;
            this.SetReserveIds(reserveIds);
            return this;
        }

        public void PaymentDone(string payResultUrl, long paidPrice, int countFailedPayments)
        {
            this.PayResultUrl = payResultUrl;
            this.PaidPrice = paidPrice;
            this.CountFailedPayments = countFailedPayments;
            this.PayDate = DateTime.Now;
            this.Status = PaymentStatus.Paid;
        }

        public void CancelPayment()
        {
            this.Status = PaymentStatus.Canceled;
        }

        public PaymentStatus Status
        {
            get
            {
                return (PaymentStatus)StatusInt;
            }
            set
            {
                StatusInt = (int)value;
            }
        }

        public string StatusString
        {
            get
            {
                switch (Status)
                {
                    case PaymentStatus.ReadyToPay:
                        return "در انتظار پرداخت";
                    case PaymentStatus.Paid:
                        return "پرداخت شده";
                    case PaymentStatus.Canceled:
                        return "لغو شده";
                    default:
                        return "";
                }
            }
        }

        public string StatusColor
        {
            get
            {
                switch (Status)
                {
                    case PaymentStatus.ReadyToPay:
                        return "#FF7F00";
                    case PaymentStatus.Paid:
                        return "#34A853";
                    case PaymentStatus.Canceled:
                        return "#EA4335";
                    default:
                        return "#242424";
                }
            }
        }

        public enum PaymentStatus
        {
            ReadyToPay = 0,
            Paid = 1,
            Canceled = 2
        }

        public long[] GetReserveIds()
        {
            if (string.IsNullOrEmpty(ReserveIds))
            {
                return new long[0];
            }
            return Array.ConvertAll(ReserveIds.Split(','), x => long.Parse(x));
        }

        public void SetReserveIds(IEnumerable<long> ids)
        {
            var ids_string = ReserveIds;
            foreach (var id in ids)
            {
                if (string.IsNullOrEmpty(ids_string))
                {
                    ids_string = id.ToString();
                }
                else
                {
                    ids_string += ("," + id.ToString());
                }
            }
            ReserveIds = ids_string;
        }

        public enum GroupPaymentStatus
        {
            ReadyToPay = 0,
            WithError = 1,
            Excluded = 2
        }
    }
}
