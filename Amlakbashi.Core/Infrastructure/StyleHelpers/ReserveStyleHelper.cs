using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Core.Infrastructure.StyleHelpers
{
    public class ReserveStyleHelper
    {
        public static string GetCallStateColor(int callState)
        {
            switch ((CallState)callState)
            {
                case CallState.Called:
                    return "#FF7F00";
                case CallState.Answered:
                    return "#34A853";
                default:
                    return "#242424";
            }
        }

        public static string GetReserveCategoryColor(ReserveCategory category)
        {
            switch (category)
            {
                case ReserveCategory.WaitForHostResponse:
                    return "#FABB17";
                case ReserveCategory.WaitForGuestPayment:
                    return "#FF7F00";
                case ReserveCategory.Reserved:
                    return "#34A853";
                case ReserveCategory.Finished:
                    return "#4485F2";
                case ReserveCategory.Unsuccessful:
                    return "#EA4335";
                default:
                    return "#242424";
            }
        }

        public static string GetStatusColor(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.WaitForResponse:
                case ReserveStatus.CancelRequestByGuest:
                case ReserveStatus.CancelRequestByHost:
                case ReserveStatus.WaitForReserve:
                case ReserveStatus.CashPay:
                    return "#FF7F00";
                case ReserveStatus.Rejected:
                    return "#EA4335";
                case ReserveStatus.Reserved:
                case ReserveStatus.Started:
                case ReserveStatus.Completed:
                    return "#34A853";
                case ReserveStatus.CanceledByGuest:
                case ReserveStatus.CanceledBySystem:
                case ReserveStatus.CanceledByHost:
                    return "#EA4335";
                case ReserveStatus.Deleted:
                    return "#C4C4C4";
                default:
                    return "#242424";
            }
        }

        public static string GetHostResponseColor(int status)
        {
            switch ((HostResponseEnum)status)
            {
                case HostResponseEnum.None:
                    return "#242424";
                case HostResponseEnum.Accepted:
                    return "#34A853";
                case HostResponseEnum.Rejected:
                case HostResponseEnum.RejectedPrice:
                case HostResponseEnum.RejectedHomeFull:
                    return "#EA4335";
                case HostResponseEnum.NoInternet:
                    return "#FF7F00";
                default:
                    return "";
            }
        }
    }
}
