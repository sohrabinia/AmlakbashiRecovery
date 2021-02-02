using Amlakbashi.Core.Common.ContactEngines;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;

namespace Amlakbashi.Core.Infrastructure.UserContact
{
    public class AppNotificationContact : IAppNotificationContact
    {
        public void SendMessage(UserContactDTO contactDTO)
        {
            string title = "", body = "", target_action = "", target_id = "0";
            switch (contactDTO.Type)
            {
                case UserContactType.confirm:
                    title = "تایید آگهی در املاک باشی";
                    body = string.Format("آگهی با کد {0} تایید و منتشر شد. برای ویرایش آگهی خود کلیک کنید",  contactDTO.AdvertiseId);
                    target_action = "AccomodationManager";
                    target_id =  contactDTO.AdvertiseId;
                    break;
                case UserContactType.GuestCancelRequestSent:
                    title = "درخواست لغو رزرو";
                    body = string.Format("درخواست لغو رزرو کد {0} توسط میزبان. لطفا با میزبان مذاکره کنید",  contactDTO.ReserveId);
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.GuestPayReserve:
                    title = "درخواست رزرو پذیرفته شد";
                    body = string.Format("درخواست رزرو {0} توسط میزبان پذیرفته شد. برای تکمیل رزرو پرداخت کنید.",  contactDTO.ReserveId);
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.GuestRefuseCancelReserveByHost:
                    title = "میزبان از لغو سفر منصرف شد";
                    body = string.Format("میزبان کد رزرو {0} از لغو سفر، منصرف شد",  contactDTO.ReserveId);
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.GuestReserveCanceled:
                    title = "سفر شما لغو شد";
                    body = string.Format("درخواست رزرو با کد {0} لغو شد",  contactDTO.ReserveId);
                    break;
                case UserContactType.GuestReserveCanceledByHost:
                    title = "سفر توسط میزبان لغو شد";
                    body = string.Format("درخواست رزرو با کد {0} توسط میزبان لغو شد",  contactDTO.ReserveId);
                    break;
                case UserContactType.GuestReservedDepositePayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} برای شما با کد رزرو {1} رزرو شد. در شروع سفر مبلغ {2} تومان دیگر به میزبان پرداخت کنید و واحد را تحویل بگیرید.",  contactDTO.AdvertiseId,  contactDTO.ReserveId, contactDTO.RemainPrice);
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.GuestReservedTotalPayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} برای شما با کد رزرو {1} رزرو شد.",  contactDTO.AdvertiseId,  contactDTO.ReserveId);
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.GuestReserveRejected:
                    title = "درخواست رزرو رد شد";
                    body = string.Format("درخواست رزرو اقامتگاه کد {0} با کد رزرو {1} توسط میزبان لغو شد. لطفا مورد دیگری انتخاب بفرمایید",  contactDTO.AdvertiseId,  contactDTO.ReserveId);
                    break;
                case UserContactType.GuestStayStarted:
                    title = "سفر شروع شد";
                    body = string.Format("سفر شما به اقامتگاه کد {0} با کد رزرو {1} شروع شد",  contactDTO.AdvertiseId,  contactDTO.ReserveId);
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.HostCancelRequestSent:
                    title = "درخواست لغو رزرو";
                    body = string.Format("درخواست لغو رزرو کد {0} توسط مهمان. لطفا با مهمان تماس بگیرید و با او مذاکره کنید",  contactDTO.ReserveId);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.HostReserveCanceled:
                    title = "سفر لغو شد";
                    body = string.Format("با درخواست لغو آگهی {0} با کد رزرو {1} موافقت شد.",  contactDTO.AdvertiseId,  contactDTO.ReserveId);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.RefuseCancelReserve:
                    title = "مهمان از لغو سفر منصرف شد";
                    body = string.Format("مهمان کد رزرو {0} از لغو سفر، منصرف شد",  contactDTO.ReserveId);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.NewReserveChatGuest:
                    title = "یک پیام جدید";
                    body = string.Format("پیام جدید از میزبان رزرو کد {0}. برای مشاهده کلیک کنید.",  contactDTO.ReserveId);
                    target_action = "ChatGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.NewReserveChatHost:
                    title = "یک پیام جدید";
                    body = string.Format("پیام جدید از مهمان رزرو کد {0}. برای مشاهده کلیک کنید.",  contactDTO.ReserveId);
                    target_action = "ChatHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.HostReserveCashPay:
                    title = "تایید پرداخت نقدی";
                    body = string.Format("مهمان رزرو کد {0} اعلام کرده که مبلغ رزرو را به صورت نقدی پرداخت کرده. آیا تایید میکنید؟",  contactDTO.ReserveId);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.HostReservedTotalPayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} کد رزرو {1} رزرو شد. مهمان کل مبلغ را پرداخت کرده است.",  contactDTO.AdvertiseId,  contactDTO.ReserveId);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.HostReservedDepositePayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} کد رزرو {1} رزرو شد. بیعانه: {2} تومان - باقیمانده: {3} تومان",  contactDTO.AdvertiseId,  contactDTO.ReserveId, contactDTO.Price, contactDTO.RemainPrice);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.ReserveCanceledBySystem:
                    title = "درخواست شما لغو شد";
                    body = string.Format("درخواست رزرو با کد {0} بدلیل عدم پاسخگویی {1} لغو شد",  contactDTO.ReserveId, contactDTO.DoerTitle);
                    break;
                case UserContactType.ReserveRequest:
                    title = "درخواست رزرو از املاک باشی";
                    body = string.Format("کد آگهی {0} - {1} تا {2} - {3}",  contactDTO.AdvertiseId, contactDTO.Extra1, contactDTO.Extra2, contactDTO.Extra3);
                    target_action = "Reserve.ReserveCategoryHost";
                    target_id = ((int)Reserve.ReserveCategory.WaitForHostResponse).ToString();
                    break;
                case UserContactType.SiteClearingHost:
                    title = "تسویه رزرو";
                    body = string.Format("مبلغ {0} تومان بابت تسویه رزرو کد {1} به حساب شما واریز شد. شماره تراکنش: {2}.", contactDTO.Price,  contactDTO.ReserveId, contactDTO.TransactionId);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.SiteClearingHostWithCredit:
                    title = "تسویه رزرو";
                    body = string.Format("مبلغ {0} تومان بابت تسویه رزرو کد {1} به کیف پول شما واریز شد. شماره تراکنش: {2}.", contactDTO.Price,  contactDTO.ReserveId, contactDTO.TransactionId);
                    target_action = "ReserveListHost";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.SiteRefundGuest:
                    title = "عودت مبلغ رزرو";
                    body = string.Format("مبلغ {0} تومان بابت عودت رزرو کد {1} به حساب شما واریز شد. شماره تراکنش: {2}.", contactDTO.Price,  contactDTO.ReserveId, contactDTO.TransactionId);
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.HostReserveRejectedForReserved:
                    title = "درخواست رزرو لغو شد";
                    body = string.Format("درخواست رزرو آگهی {0} با کد رزرو {1} به دلیل رزرو اقامتگاه دیگر توسط مهمان لغو شد.",  contactDTO.AdvertiseId,  contactDTO.ReserveId);
                    target_action = "Reserve.ReserveCategoryHost";
                    target_id = ((int)Reserve.ReserveCategory.Unsuccessful).ToString();
                    break;
                case UserContactType.payment:
                    title = "رسید پرداخت اینترنتی";
                    body = string.Format("پرداخت شما با موفقیت انجام شد. شماره تراکنش {0}. باتشکر", contactDTO.TransactionId);
                    break;
                case UserContactType.UserCreditIncrease:
                    title = "رسید واریز به کیف پول";
                    body = string.Format("مبلغ {0} تومان بابت {1} به کیف پول شما واریز شد. شماره تراکنش: {2}", contactDTO.Price, contactDTO.CauseString, contactDTO.TransactionId);
                    break;
                case UserContactType.UserCreditDecrease:
                    title = "رسید برداشت از کیف پول";
                    body = string.Format("مبلغ {0} تومان بابت {1} از کیف پول شما برداشت شد. شماره تراکنش: {2}", contactDTO.Price, contactDTO.CauseString, contactDTO.TransactionId);
                    break;
                case UserContactType.FinishStay:
                    title = "پایان سفر";
                    body = string.Format("سفر شما به پایان رسید. برای امتیازدهی به میزبان کلیک کنید.");
                    target_action = "ReserveListGuest";
                    target_id =  contactDTO.ReserveId;
                    break;
                case UserContactType.PrizeCharge:
                    title = "کیف هدیه شما شارژ شد";
                    body = "کیف هدیه شما بابت معرفی مبلغ " + contactDTO.Extra1 + " تومان شارژ شد";
                    break;
                case UserContactType.CouponAppreciate:
                    title = "تخفیف قدرانی از همراهی شما";
                    body = "کاربر گرامی به منظور قدرانی از شما در رزرو بعدی " + contactDTO.Extra1 + " تخفیف مبلغ اولین روز رزرو را دریافت می کنید";
                    break;
                case UserContactType.CouponPresent:
                    title = "تخفیف ثبت نام در سایت با معرفی";
                    body = "کاربر گرامی به جهت ثبت نام با معرفی در املاک باشی در رزرو بعدی " + contactDTO.Extra2 + " تخفیف مبلغ اولین روز رزرو را دریافت می کنید";
                    break;
                case UserContactType.HostUpdatePrice:
                    title = "بروز رسانی قیمت ها";
                    body = string.Format("آقا/خانم {0} عزیز با توجه به نزدیک بودن تعطیلات قیمت های خود را بروزسانی فرمایید", contactDTO.Extra1);
                    target_action = "AccomodationManager";
                    break;
            }

            if ((contactDTO.FcmNotification && string.IsNullOrEmpty(contactDTO.UserFcmAppNotificationToken)) ||
                (!contactDTO.FcmNotification && string.IsNullOrEmpty(contactDTO.UserAppNotificationToken)) ||
                string.IsNullOrEmpty(title) ||
                string.IsNullOrEmpty(body))
            {
                return;
            }

            try
            {
                if (contactDTO.FcmNotification)
                {
                    NotificationEngine.SendMessageApplication(contactDTO.UserFcmAppNotificationToken, title, body,
                    target_action, target_id);
                }
                else
                {
                    AppNotificationEngine.SendMessage(contactDTO.UserAppNotificationToken, title, body, "normal",
                    target_action, target_id);
                }
            }
            catch
            {
            }
        }
    }
}
