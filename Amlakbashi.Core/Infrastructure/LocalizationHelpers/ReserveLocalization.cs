using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class ReserveLocalization
    {
        public static string GetHostResponseString(int status)
        {
            switch ((HostResponseEnum)status)
            {
                case HostResponseEnum.None:
                    return "بدون پاسخ";
                case HostResponseEnum.Accepted:
                    return "پذیرفته شده";
                case HostResponseEnum.Rejected:
                    return "رد شده";
                case HostResponseEnum.RejectedPrice:
                    return "رد شده بدلیل قیمت";
                case HostResponseEnum.RejectedHomeFull:
                    return "رد شده بدلیل پر بودن";
                case HostResponseEnum.NoInternet:
                    return "عدم دسترسی به اینترنت";
                default:
                    return "";
            }
        }

        public static string GetReserveCategoryTitle(ReserveCategory category,
            Reserve.ReserveManagerSelectType selectType = Reserve.ReserveManagerSelectType.All)
        {
            switch (selectType)
            {
                case Reserve.ReserveManagerSelectType.Guest:
                    switch (category)
                    {
                        case ReserveCategory.WaitForHostResponse:
                            return "استعلام از میزبان";
                        case ReserveCategory.WaitForGuestPayment:
                            return "منتظر پرداخت شما";
                        default:
                            return "";
                    }
                case Reserve.ReserveManagerSelectType.Host:
                    switch (category)
                    {
                        case ReserveCategory.WaitForHostResponse:
                            return "منتظر تایید شما";
                        case ReserveCategory.WaitForGuestPayment:
                            return "منتظر پرداخت مهمان";
                        default:
                            return "";
                    }
                default:
                    switch (category)
                    {
                        case ReserveCategory.Reserved:
                            return " رزرو شده";
                        case ReserveCategory.Finished:
                            return "پایان سفر  ";
                        case ReserveCategory.Unsuccessful:
                            return "ناموفق";
                        default:
                            return "";
                    }
            }
        }

        public static string GetStatusString(int status, StatusStringType type,
            long reserveId = -1, HostResponseEnum hostResponse = HostResponseEnum.None)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.WaitForResponse:
                    switch (type)
                    {
                        case StatusStringType.Guest:
                        case StatusStringType.Site:
                            return "در انتظار پاسخ میزبان";
                        case StatusStringType.Host:
                            return "در انتظار پاسخ شما";
                        default:
                            return "";
                    }
                case ReserveStatus.WaitForReserve:
                    switch (type)
                    {
                        case StatusStringType.Guest:
                            return "در انتظار پرداخت شما";
                        case StatusStringType.Host:
                        case StatusStringType.Site:
                            return "در انتظار پرداخت مهمان";
                        default:
                            return "";
                    }
                case ReserveStatus.Rejected:
                    switch (type)
                    {
                        case StatusStringType.Guest:
                            return "رد شده توسط میزبان";
                        case StatusStringType.Host:
                            if (reserveId > 0)
                            {
                                switch (hostResponse)
                                {
                                    case HostResponseEnum.RejectedPrice:
                                        return "رد شده توسط شما بدلیل قیمت";
                                    case HostResponseEnum.RejectedHomeFull:
                                        return "رد شده توسط شما بدلیل پر بودن";
                                    default:
                                        return "رد شده توسط شما";
                                }
                            }
                            else
                            {
                                return "رد شده توسط شما";
                            }
                        case StatusStringType.Site:
                            return "رد شده";
                        default:
                            return "";
                    }
                case ReserveStatus.CashPay:
                    switch (type)
                    {
                        case StatusStringType.Guest:
                            return "در انتظار تایید پرداخت نقدی توسط میزبان";
                        case StatusStringType.Host:
                            return "در انتظار تایید پرداخت نقدی توسط شما";
                        case StatusStringType.Site:
                            return "در انتظار تایید پرداخت نقدی";
                        default:
                            return "";
                    }
                case ReserveStatus.Reserved:
                    switch (type)
                    {
                        case StatusStringType.Host:
                        case StatusStringType.Guest:
                            return "رزرو شد";
                        case StatusStringType.Site:
                            return "رزرو شده";
                        default:
                            return "";
                    }
                case ReserveStatus.Started:
                    switch (type)
                    {
                        case StatusStringType.Host:
                        case StatusStringType.Guest:
                            return "سفر شروع شد";
                        case StatusStringType.Site:
                            return "شروع سفر";
                        default:
                            return "";
                    }
                case ReserveStatus.Completed:
                    switch (type)
                    {
                        case StatusStringType.Host:
                        case StatusStringType.Guest:
                            return "سفر تمام شد";
                        case StatusStringType.Site:
                            return "پایان سفر";
                        default:
                            return "";
                    }
                case ReserveStatus.CancelRequestByGuest:
                    switch (type)
                    {
                        case StatusStringType.Guest:
                            return "درخواست لغو شما ارسال شد";
                        case StatusStringType.Host:
                        case StatusStringType.Site:
                            return "درخواست لغو توسط مهمان";
                        default:
                            return "";
                    }
                case ReserveStatus.CancelRequestByHost:
                    switch (type)
                    {
                        case StatusStringType.Host:
                            return "درخواست لغو شما ارسال شد";
                        case StatusStringType.Guest:
                        case StatusStringType.Site:
                            return "درخواست لغو توسط میزبان";
                        default:
                            return "";
                    }
                case ReserveStatus.CanceledByGuest:
                    switch (type)
                    {
                        case StatusStringType.Guest:
                            return "درخواست توسط شما لغو شد";
                        case StatusStringType.Host:
                            return "درخواست توسط مهمان لغو شد";
                        case StatusStringType.Site:
                            return "لغو شده توسط مهمان";
                        default:
                            return "";
                    }
                case ReserveStatus.CanceledByHost:
                    switch (type)
                    {
                        case StatusStringType.Guest:
                            return "درخواست توسط میزبان لغو شد";
                        case StatusStringType.Host:
                            return "درخواست توسط شما لغو شد";
                        case StatusStringType.Site:
                            return "لغو شده توسط میزبان";
                        default:
                            return "";
                    }
                case ReserveStatus.CanceledBySystem:
                    return "لغو شده توسط سیستم";
                case ReserveStatus.Deleted:
                    return "پاک شده";
                default:
                    return "";
            }
        }

        public static string GetReserveCancelReasonsTitle(Reserve.ReserveCancelReasons reason)
        {
            switch (reason)
            {
                case ReserveCancelReasons.Guest_Guest_TripCancellation:
                    return "لغو شدن سفر";
                case ReserveCancelReasons.Guest_Guest_NotHavingEvidence:
                    return "نداشتن مدارک هویتی";
                case ReserveCancelReasons.Guest_Guest_IncorrectNumberOfGuests:
                    return "تعداد نفرات اشتباه";
                case ReserveCancelReasons.Guest_Guest_ChangeTripDate:
                    return "تغییر زمان سفر";
                case ReserveCancelReasons.Guest_Host_IncorrectResidenceInfo:
                    return "مغایرت اقامتگاه با اطلاعات سایت";
                case ReserveCancelReasons.Guest_Host_IncorrectHost:
                    return "موجه نبودن میزبان";
                case ReserveCancelReasons.Guest_Host_DirtyResidence:
                    return "کثیفی اقامتگاه";
                case ReserveCancelReasons.Host_Host_LowPrice:
                    return "قیمت پایین";
                case ReserveCancelReasons.Host_Host_ResidenceFull:
                    return "پر بودن اقامتگاه";
                case ReserveCancelReasons.Host_Host_ResidenceRebuilding:
                    return "بازسازی اقامتگاه";
                case ReserveCancelReasons.Host_Guest_NotHavingEvidence:
                    return "عدم ارائه مدارک هویتی توسط مهمان";
                case ReserveCancelReasons.Host_Guest_IncorrectNumberOfGuest:
                    return "درست نبودن تعداد نفرات مهمان";
                case ReserveCancelReasons.Host_Guest_IncorrectGuest:
                    return "موجه نبودن مهمان";
                default:
                    return string.Empty;
            }
        }
    }
}
