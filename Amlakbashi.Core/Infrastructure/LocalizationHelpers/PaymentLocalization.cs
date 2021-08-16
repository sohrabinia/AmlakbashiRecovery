using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.GroupPayment;
using static Amlakbashi.Core.Entities.ReservePayment;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class PaymentLocalization
    {
        public static string GetPaymentMethodString(int payment_method)
        {
            switch ((ReservePaymentMethod)payment_method)
            {
                case ReservePaymentMethod.EPay:
                    return "پرداخت اینترنتی";
                case ReservePaymentMethod.AmlakbashiCredit:
                    return "کیف پول";
                case ReservePaymentMethod.BankCard:
                    return "کارت به کارت";
                case ReservePaymentMethod.Podium:
                    return "پادیوم";
                default:
                    return "";
            }
        }

        public static string GetPaymentDatabaseGroupString(ReservePaymentType payment_type)
        {
            switch (payment_type)
            {
                case ReservePaymentType.GuestDeposite:
                case ReservePaymentType.GuestClearing:
                    return "Reserve_Guest";
                case ReservePaymentType.SiteDepositeToHost:
                case ReservePaymentType.SiteClearingToHost:
                case ReservePaymentType.SiteRefundToGuest:
                    return "Reserve_Site";
                default:
                    return "";
            }
        }

        public static string GetPaymentTypePersianString(string payment_string)
        {
            switch (payment_string)
            {
                case "Reserve_GuestDeposite":
                    return "پرداخت بیعانه توسط مهمان";
                case "Reserve_GuestClearing":
                    return "تسویه رزرو توسط مهمان";
                case "Reserve_SiteDepositeToHost":
                    return "پرداخت بیعانه به میزبان توسط سایت";
                case "Reserve_SiteClearingToHost":
                    return "پرداخت تسویه میزبان توسط سایت";
                case "Reserve_SiteRefundToGuest":
                    return "عودت مبلغ به مهمان توسط سایت";
                default:
                    return "";
            }
        }

        public static string GetPaymentTypePersianString(int payment_type)
        {
            switch ((ReservePaymentType)payment_type)
            {
                case ReservePaymentType.GuestDeposite:
                    return "پرداخت بیعانه توسط مهمان";
                case ReservePaymentType.GuestClearing:
                    return "تسویه رزرو توسط مهمان";
                case ReservePaymentType.SiteDepositeToHost:
                    return "پرداخت بیعانه به میزبان توسط سایت";
                case ReservePaymentType.SiteClearingToHost:
                    return "پرداخت تسویه میزبان توسط سایت";
                case ReservePaymentType.SiteRefundToGuest:
                    return "عودت مبلغ به مهمان توسط سایت";
                case ReservePaymentType.WaitingForPodium:
                    return "پرداخت ناموفق پادیوم";
                default:
                    return "";
            }
        }
        public static string GetGroupPaymentStatusString(GroupPaymentStatus groupPaymentStatus)
        {
            switch (groupPaymentStatus)
            {
                case GroupPaymentStatus.ReadyToPay:
                    return "آماده پرداخت";
                case GroupPaymentStatus.WithError:
                    return "دارای مشکل";
                case GroupPaymentStatus.Excluded:
                    return "مستثنی شده";
                default:
                    return "";
            }
        }
    }
}
