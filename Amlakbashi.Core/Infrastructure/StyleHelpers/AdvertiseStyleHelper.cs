using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.StyleHelpers
{
    public static class AdvertiseStyleHelper
    {
        public static string GetAdvertiseStatusColor(int status)
        {
            switch ((AdvertiseStatus)status)
            {
                case AdvertiseStatus.Published:
                    return "#34A853";
                case AdvertiseStatus.FirstReady:
                case AdvertiseStatus.ReadyToPublish:
                case AdvertiseStatus.NotCompleted:
                    return "#FF7F00";
                case AdvertiseStatus.Archived:
                case AdvertiseStatus.Deleted:
                case AdvertiseStatus.NotVerified:
                    return "#EA4335";
                default:
                    return "#242424";
            }
        }

        public static string GetInstantReserveStatusColor(InstantReserveStatusEnum status)
        {
            switch (status)
            {
                case InstantReserveStatusEnum.None:
                    return "#4485F2";
                case InstantReserveStatusEnum.Requested:
                    return "#FF7F00";
                case InstantReserveStatusEnum.Confirmed:
                    return "#34A853";
                default:
                    return "";
            }
        }
    }
}
