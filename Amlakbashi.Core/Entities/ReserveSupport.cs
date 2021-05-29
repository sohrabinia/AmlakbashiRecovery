using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// ذخیره اطلاعات مربوط به پشتیبانی یک رزرو
    /// مثلا اینکه چه پشتیبانی چه رزروی رو داره پشتیبانی میکنه و زمان شروع پشتیبانی و ...
    /// </summary>
    public class ReserveSupport : Entity<int>, ISoftDelete
    {
        [Column("ReserveSupportID")]
        public override int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public int? SupporterID { get; set; }
        public int GuestID { get; set; }
        public DateTime JourneyStartDate { get; set; }
        public DateTime? StartSupportDate { get; set; }
        public DateTime? LastSupporterActionDate { get; set; }
        public string ReservesWaitingForSupport { get; set; }
        public string ReservesSupporting { get; set; }
        public string ReservesSimilar { get; set; }
        public string TransferReason { get; set; }
        public SupportStatus Status { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("GuestID")]
        public virtual User Guest { get; set; }

        [ForeignKey("SupporterID")]
        public virtual User Supporter { get; set; }

        public long[] GetReserveIds(SupportReserveStatus status)
        {
            var ids_string = GetReserveIdsByStatus(status);
            if (string.IsNullOrEmpty(ids_string))
            {
                return new long[0];
            }
            return Array.ConvertAll(ids_string.Split(','), x => long.Parse(x));
        }
        public long[] GetAllReserveIds()
        {
            return GetReserveIds(SupportReserveStatus.Similar).Concat(
                GetReserveIds(SupportReserveStatus.Supporting)).Concat(
                GetReserveIds(SupportReserveStatus.WaitingForSupport)).Distinct().ToArray();
        }
        public void AddReserveId(long id, SupportReserveStatus status)
        {
            var ids_string = GetReserveIdsByStatus(status);
            if (string.IsNullOrEmpty(ids_string))
            {
                ids_string = id.ToString();
            }
            else
            {
                var current_reserve_ids = GetReserveIds(status);
                if (!(current_reserve_ids.Length > 0 && current_reserve_ids.Last() == id))
                {
                    ids_string += ("," + id.ToString());
                }
            }
            switch (status)
            {
                case SupportReserveStatus.WaitingForSupport:
                    ReservesWaitingForSupport = ids_string;
                    break;
                case SupportReserveStatus.Supporting:
                    ReservesSupporting = ids_string;
                    break;
                case SupportReserveStatus.Similar:
                    ReservesSimilar = ids_string;
                    break;
            }
        }

        public void AddReserveId(long[] ids, SupportReserveStatus status)
        {
            foreach (var id in ids)
            {
                AddReserveId(id, status);
            }
        }

        private string GetReserveIdsByStatus(SupportReserveStatus status)
        {
            switch (status)
            {
                case SupportReserveStatus.WaitingForSupport:
                    return ReservesWaitingForSupport;
                case SupportReserveStatus.Supporting:
                    return ReservesSupporting;
                case SupportReserveStatus.Similar:
                    return ReservesSimilar;
                default:
                    return null;
            }
        }

        public static string GetSupportStatusString(SupporterStatus status)
        {
            switch (status)
            {
                case SupporterStatus.Free:
                    return "بدون پشتیبان";
                case SupporterStatus.SupportingByOthers:
                case SupporterStatus.SupportingByYou:
                    return "در حال پشتیبانی";
                case SupporterStatus.Done:
                    return "انجام شده";
                case SupporterStatus.Expired:
                    return "منقضی شده";
                default:
                    return "-";
            }
        }

        public static string GetSupportStatusColor(SupporterStatus status)
        {
            switch (status)
            {
                case SupporterStatus.Free:
                    return "#242424";
                case SupporterStatus.SupportingByOthers:
                case SupporterStatus.SupportingByYou:
                    return "#4485F2";
                case SupporterStatus.Done:
                    return "#34A853";
                case SupporterStatus.Expired:
                    return "#EA4335";
                default:
                    return "-";
            }
        }

        public enum SupportReserveStatus
        {
            WaitingForSupport = 0,
            Supporting = 1,
            Similar = 2
        }

        public enum SupportStatus
        {
            WaitingForSupport = 0,
            Supporting = 1,
            Expired = 2,
            Transfered = 3,
            Done = 4,
        }

        public enum SupporterStatus
        {
            None = 0,
            Free = 1,
            SupportingByOthers = 2,
            SupportingByYou = 3,
            Done = 4,
            Expired = 5
        }
    }
}
