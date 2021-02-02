using Amlakbashi.Core.Common.ContactEngines;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Infrastructure.UserContact
{
    public class NotificationContact : INotificationContact
    {
        public void SendMessage(UserContactDTO contactDTO)
        {
            string title = "", body = "", click_action = "";
            List<NotificationButton> buttons = null;
            switch (contactDTO.Type)
            {
                case UserContactType.confirm:
                    title = "تایید آگهی در املاک باشی";
                    body = string.Format("آگهی با کد {0} تایید و منتشر شد. برای ویرایش آگهی خود کلیک کنید", contactDTO.AdvertiseId);
                    click_action = "/post/accomodationmanager?id=" + contactDTO.AdvertiseId;
                    break;
                case UserContactType.GuestCancelRequestSent:
                    title = "درخواست لغو رزرو";
                    body = string.Format("درخواست لغو رزرو کد {0} توسط میزبان. لطفا با میزبان مذاکره کنید", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "discussion_with_host",
                            Title = "مذاکره با میزبان",
                            Url = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId
            }
                    };
                    break;
                case UserContactType.GuestPayReserve:
                    title = "درخواست رزرو پذیرفته شد";
                    body = string.Format("درخواست رزرو {0} توسط میزبان پذیرفته شد. برای تکمیل رزرو پرداخت کنید.", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "pay_reserve",
                            Title = "پرداخت",
                            Url = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId
                        }
                    };
                    break;
                case UserContactType.GuestRefuseCancelReserveByHost:
                    title = "میزبان از لغو سفر منصرف شد";
                    body = string.Format("میزبان کد رزرو {0} از لغو سفر، منصرف شد", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.GuestReserveCanceled:
                    title = "سفر شما لغو شد";
                    body = string.Format("درخواست رزرو با کد {0} لغو شد", contactDTO.ReserveId);
                    click_action = "/";
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "open_home_page",
                            Title = "رزرو واحد دیگر",
                            Url = "/"
                        }
                    };
                    break;
                case UserContactType.GuestReserveCanceledByHost:
                    title = "سفر توسط میزبان لغو شد";
                    body = string.Format("درخواست رزرو با کد {0} توسط میزبان لغو شد", contactDTO.ReserveId);
                    click_action = "/";
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "open_home_page",
                            Title = "انتخاب مورد دیگر",
                            Url = "/"
                        }
                    };
                    break;
                case UserContactType.GuestReservedDepositePayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} برای شما با کد رزرو {1} رزرو شد. در شروع سفر مبلغ {2} تومان دیگر به میزبان پرداخت کنید و واحد را تحویل بگیرید.", contactDTO.AdvertiseId, contactDTO.ReserveId, contactDTO.RemainPrice);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.GuestReservedTotalPayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} برای شما با کد رزرو {1} رزرو شد.", contactDTO.AdvertiseId, contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.GuestReserveRejected:
                    title = "درخواست رزرو رد شد";
                    body = string.Format("درخواست رزرو اقامتگاه کد {0} با کد رزرو {1} توسط میزبان لغو شد. لطفا مورد دیگری انتخاب بفرمایید", contactDTO.AdvertiseId, contactDTO.ReserveId);
                    click_action = "/";
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "open_home_page",
                            Title = "انتخاب مورد دیگر",
                            Url = "/"
                        }
                    };
                    break;
                case UserContactType.GuestStayStarted:
                    title = "سفر شروع شد";
                    body = string.Format("سفر شما به اقامتگاه کد {0} با کد رزرو {1} شروع شد", contactDTO.AdvertiseId, contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.HostCancelRequestSent:
                    title = "درخواست لغو رزرو";
                    body = string.Format("درخواست لغو رزرو کد {0} توسط مهمان. لطفا با مهمان تماس بگیرید و با او مذاکره کنید", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "discussion_with_guest",
                            Title = "مذاکره با مهمان",
                            Url = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId
                        }
                    };
                    break;
                case UserContactType.HostReserveCanceled:
                    title = "سفر لغو شد";
                    body = string.Format("با درخواست لغو آگهی {0} با کد رزرو {1} موافقت شد.", contactDTO.AdvertiseId, contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.RefuseCancelReserve:
                    title = "مهمان از لغو سفر منصرف شد";
                    body = string.Format("مهمان کد رزرو {0} از لغو سفر، منصرف شد", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.NewReserveChatGuest:
                    title = "یک پیام جدید";
                    body = string.Format("پیام جدید از میزبان رزرو کد {0}. برای مشاهده کلیک کنید.", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "open_chat",
                            Title = "مشاهده",
                            Url = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId
            }
                    };
                    break;
                case UserContactType.NewReserveChatHost:
                    title = "یک پیام جدید";
                    body = string.Format("پیام جدید از مهمان رزرو کد {0}. برای مشاهده کلیک کنید.", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "open_chat",
                            Title = "مشاهده",
                            Url = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId
                        }
                    };
                    break;
                case UserContactType.HostReserveCashPay:
                    title = "تایید پرداخت نقدی";
                    body = string.Format("مهمان رزرو کد {0} اعلام کرده که مبلغ رزرو را به صورت نقدی پرداخت کرده. آیا تایید میکنید؟", contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "confirm_cash_pay_true",
                            Title = "بله",
                            Url = "/reserve/confirmcashpaybynotif?reserve_id=" + contactDTO.ReserveId +
                                "&payed=true"
                        },
                        new NotificationButton()
                        {
                            Name = "confirm_cash_pay_false",
                            Title = "خیر",
                            Url = "/reserve/confirmcashpaybynotif?reserve_id=" + contactDTO.ReserveId +
                                "&payed=false"
                        }
                    };
                    break;
                case UserContactType.HostReservedTotalPayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} کد رزرو {1} رزرو شد. مهمان کل مبلغ را پرداخت کرده است.", contactDTO.AdvertiseId, contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.HostReservedDepositePayed:
                    title = "رزرو انجام شد";
                    body = string.Format("اقامتگاه کد {0} کد رزرو {1} رزرو شد. بیعانه: {2} تومان - باقیمانده: {3} تومان", contactDTO.AdvertiseId, contactDTO.ReserveId, contactDTO.Price, contactDTO.RemainPrice);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.ReserveCanceledBySystem:
                    title = "درخواست شما لغو شد";
                    body = string.Format("درخواست رزرو با کد {0} بدلیل عدم پاسخگویی {1} لغو شد", contactDTO.ReserveId, contactDTO.DoerTitle);
                    click_action = "/";
                    //buttons = new List<NotificationButton>()
                    //{
                    //    new NotificationButton()
                    //    {
                    //        Name = "open_home_page",
                    //        Title = "رزرو واحد دیگر",
                    //        Url = "/"
                    //    }
                    //};
                    break;
                case UserContactType.ReserveRequest:
                    title = "درخواست رزرو از املاک باشی";
                    body = string.Format("کد آگهی {0} - {1} تا {2} - {3}", contactDTO.AdvertiseId, contactDTO.Extra1, contactDTO.Extra2, contactDTO.Extra3);
                    click_action = "/reserve/reserveitemmanager?category=" + Reserve.ReserveCategory.WaitForHostResponse;
                    buttons = new List<NotificationButton>()
                    {
                        new NotificationButton()
                        {
                            Name = "accept_reserve",
                            Title = "قبول ✓",
                            Url = "/reserve/reserveresponsebynotif?reserve_id="
                                + contactDTO.ReserveId + "&host_response=1"
                        },
                        new NotificationButton()
                        {
                            Name = "reject_reserve",
                            Title = "رد ⛌",
                            Url = "/reserve/reserveresponsebynotif?reserve_id="
                                + contactDTO.ReserveId + "&host_response=2"
                        },
                        new NotificationButton()
                        {
                            Name = "reject_reserve_occup",
                            Title = "رد بدلیل پر بودن ⛌",
                            Url = "/reserve/reserveresponsebynotif?reserve_id="
                                + contactDTO.ReserveId + "&host_response=4"
                        },
                        new NotificationButton()
                        {
                            Name = "reject_reserve_price",
                            Title = "رد بدلیل قیمت ⛌",
                            Url = "/reserve/reserveresponsebynotif?reserve_id="
                                + contactDTO.ReserveId + "&host_response=3"
                        }
                    };
                    break;
                case UserContactType.SiteClearingHost:
                    title = "تسویه رزرو";
                    body = string.Format("مبلغ {0} تومان بابت تسویه رزرو کد {1} به حساب شما واریز شد. شماره تراکنش: {2}.", contactDTO.Price, contactDTO.ReserveId, contactDTO.TransactionId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.SiteClearingHostWithCredit:
                    title = "تسویه رزرو";
                    body = string.Format("مبلغ {0} تومان بابت تسویه رزرو کد {1} به کیف پول شما واریز شد. شماره تراکنش: {2}.", contactDTO.Price, contactDTO.ReserveId, contactDTO.TransactionId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.SiteRefundGuest:
                    title = "عودت مبلغ رزرو";
                    body = string.Format("مبلغ {0} تومان بابت عودت رزرو کد {1} به حساب شما واریز شد. شماره تراکنش: {2}.", contactDTO.Price, contactDTO.ReserveId, contactDTO.TransactionId);
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.HostReserveRejectedForReserved:
                    title = "درخواست رزرو لغو شد";
                    body = string.Format("درخواست رزرو آگهی {0} با کد رزرو {1} به دلیل رزرو اقامتگاه دیگر توسط مهمان لغو شد.", contactDTO.AdvertiseId, contactDTO.ReserveId);
                    click_action = "/reserve/reserveitemmanager?category=" + Reserve.ReserveCategory.Unsuccessful;
                    break;
                case UserContactType.payment:
                    title = "رسید پرداخت اینترنتی";
                    body = string.Format("پرداخت شما با موفقیت انجام شد. شماره تراکنش {0}. باتشکر", contactDTO.TransactionId);
                    click_action = "/";
                    break;
                case UserContactType.UserCreditIncrease:
                    title = "رسید واریز به کیف پول";
                    body = string.Format("مبلغ {0} تومان بابت {1} به کیف پول شما واریز شد. شماره تراکنش: {2}", contactDTO.Price, contactDTO.CauseString, contactDTO.TransactionId);
                    click_action = "/";
                    break;
                case UserContactType.UserCreditDecrease:
                    title = "رسید برداشت از کیف پول";
                    body = string.Format("مبلغ {0} تومان بابت {1} از کیف پول شما برداشت شد. شماره تراکنش: {2}", contactDTO.Price, contactDTO.CauseString, contactDTO.TransactionId);
                    click_action = "/";
                    break;
                case UserContactType.FinishStay:
                    title = "پایان سفر";
                    body = string.Format("سفر شما به پایان رسید. برای امتیازدهی به میزبان کلیک کنید.");
                    click_action = "/reserve/reserveitemmanager?reserve_id=" + contactDTO.ReserveId;
                    break;
                case UserContactType.HostUpdatePrice:
                    title = "بروز رسانی قیمت ها";
                    body = string.Format("آقا/خانم {0} عزیز با توجه به نزدیک بودن تعطیلات قیمت های خود را بروزسانی فرمایید", contactDTO.Extra1);
                    click_action = "/post/accomodationmanager";
                    break;
                case UserContactType.PrizeCharge:
                    title = "کیف هدیه شما شارژ شد";
                    body = "کیف هدیه شما بابت معرفی مبلغ " + contactDTO.Extra1 + " تومان شارژ شد";
                    click_action = "/";
                    break;
                case UserContactType.CouponAppreciate:
                    title = "تخفیف قدرانی از همراهی شما";
                    body = "کاربر گرامی به منظور قدرانی از شما در رزرو بعدی " + contactDTO.Extra1 + " تخفیف مبلغ اولین روز رزرو را دریافت می کنید";
                    click_action = "/";
                    break;
                case UserContactType.CouponPresent:
                    title = "تخفیف ثبت نام در سایت با معرفی";
                    body = "کاربر گرامی به جهت ثبت نام با معرفی در املاک باشی در رزرو بعدی " + contactDTO.Extra2 + " تخفیف مبلغ اولین روز رزرو را دریافت می کنید";
                    click_action = "/";
                    break;
            }
            if (string.IsNullOrEmpty(contactDTO.UserNotificationToken) ||
                string.IsNullOrEmpty(title) ||
                string.IsNullOrEmpty(body))
            {
                return;
            }
            try
            {
                NotificationEngine.SendMessage(contactDTO.UserNotificationToken, title, body, click_action, buttons);
            }
            catch
            {
            }
        }

        public void SendNotification(string token, string title, string body, string click_action,
            List<NotificationButton> buttons = null)
        {
            try
            {
                NotificationEngine.SendMessage(token, title, body, click_action, buttons);
            }
            catch
            {
                // TODO Logger
            }
        }

        public void TestMessage(string token, string reserveId)
        {
            var title = "درخواست رزرو";
            var body = "کد آگهی 12345 - 12/11/97 تا 15/11/97 - 300000 تومان - ۵ نفر";
            var click_action = "/reserve/reserveitemmanager?category=" + Reserve.ReserveCategory.WaitForHostResponse;

            var buttons = new List<NotificationButton>()
            {
                new NotificationButton()
                {
                    Name = "accept_reserve",
                    Title = "قبول ✓",
                    Url = "/reserve/reserveresponsebynotif?reserve_id="
                        + reserveId + "&host_response=1"
                },
                new NotificationButton()
                {
                    Name = "reject_reserve",
                    Title = "رد ⛌",
                    Url = "/reserve/reserveresponsebynotif?reserve_id="
                        + reserveId + "&host_response=2"
                },
                new NotificationButton()
                {
                    Name = "reject_reserve_price",
                    Title = "رد بدلیل قیمت ⛌",
                    Url = "/reserve/reserveresponsebynotif?reserve_id="
                        + reserveId + "&host_response=3"
                },
                new NotificationButton()
                {
                    Name = "reject_reserve_occup",
                    Title = "رد بدلیل پر بودن ⛌",
                    Url = "/reserve/reserveresponsebynotif?reserve_id="
                        + reserveId + "&host_response=4"
                }
            };
            NotificationEngine.SendMessage(token, title, body, click_action, buttons);
        }

        public void SendMessageApplication(string token, string title, string body, string targetAction, string targetId)
        {
            NotificationEngine.SendMessageApplication(token, title, body, targetAction, targetId);
        }
    }
}
