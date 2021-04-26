using Amlakbashi.Core.Common.ContactEngines;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Infrastructure.UserContact
{
    public class EmailContact : IEmailContact
    {
        public void SendMessage(UserContactDTO contactDTO)
        {
            if (contactDTO.EmailConfirmed == false)
            {
                return;
            }
            switch (contactDTO.Type)
            {
                case UserContactType.confirm:
                    SendEmail(EmailSenderDepartment.Support, contactDTO.UserEmail,
                        "تایید آگهی",
                        "<div>" + "آگهی با کد " +
                         contactDTO.AdvertiseId + " تایید و منتشر شد." + "</div>" +
                        "<div>" + "برای ویرایش آگهی خود به حساب کاربریتان مراجعه کنید :" + "</div>" +
                        "<a href='https://bit.ly/2T6ZAM1'>ورود به حساب کاربری</a>"
                        );
                    break;
                case UserContactType.GuestCancelRequestSent:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "درخواست لغو رزرو",
                        "<div>" + "مهمان گرامی ، درخواست کنسل شدن رزرو اقامتگاه با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " توسط میزبان داده شده. " + "</div>");
                    break;
                case UserContactType.GuestPayReserve:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "درخواست رزرو پذیرفته شد",
                        "<div>" + "مهمان گرامی، درخواست رزرو شما برای آگهی با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " توسط میزبان پذیرفته شد." + "</div>" +
                        "<div>" + "جهت پرداخت و تکمیل رزرو به لینک زیر مراجعه فرمایید" + "</div>" +
                        "<div>" + GeneralData.WebsiteUrl + "/reserve/reserveitemmanager?category=1" + "</div>"
                        );
                    break;
                case UserContactType.GuestRefuseCancelReserveByHost:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "میزبان از لغو سفر منصرف شد",
                        "<div>" + "مهمان گرامی ، میزبان اقامتگاه با کد آگهی " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " از کنسل کردن رزرو منصرف شد." + "</div>"
                        );
                    break;
                case UserContactType.GuestReserveCanceled:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "درخواست رزرو شما لغو شد",
                        "<div>" + "درخواست رزرو شما برای آگهی با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " لغو شد." + "</div>"
                        );
                    break;
                case UserContactType.GuestReserveCanceledByHost:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "درخواست رزرو شما لغو شد",
                        "<div>" + "درخواست رزرو شما برای آگهی با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " توسط میزبان لغو شد." + "</div>"
                        );
                    break;
                case UserContactType.GuestReservedDepositePayed:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "رزرو انجام شد",
                        "<div>" + "مهمان گرامی ، اقامتگاه با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " برای شما رزرو شد." + "</div>" +
                        "<div>" + "شما مبلغ " +  contactDTO.Price + " تومان به عنوان بیعانه پرداخت کرده اید." + "</div>" +
                        "<div>" + "لطفا در شروع سفر به قسمت رزرو در پنل کاربری خود مراجعه کنید و با پرداخت بقیه مبلغ به میزان " + contactDTO.RemainPrice + " تومان اقامتگاه را تحویل بگیرید." + "</div>" +
                        "<div>" + "شماره تلفن میزبان : " + contactDTO.AudienceMobile + "</div>"
                        );
                    break;
                case UserContactType.GuestReservedTotalPayed:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "رزرو انجام شد",
                        "<div>" + "مهمان گرامی ، اقامتگاه با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " برای شما رزرو شد." + "</div>" +
                        "<div>" + "شما کل مبلغ را پرداخت کرده اید." + "</div>" +
                        "<div>" + "لطفا در هنگام رسیدن به مقصد وارد قسمت رزرو در پنل کاربری خود در سایت املاک باشی شوید و دکمه شروع سفر را بزنید." + "</div>" +
                        "<div>" + "شماره تلفن میزبان : " + contactDTO.AudienceMobile + "</div>"
                        );
                    break;
                case UserContactType.GuestReserveRejected:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "رد درخواست رزرو",
                        "<div>" + "متاسفانه درخواست رزرو شما به اقامتگاه با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " توسط میزبان رد شد.  مورد دیگری انتخاب بفرمایید." + "</div>" +
                        "<div>" + "https://bit.ly/2E3yJH0" + "</div>"
                        );
                    break;
                case UserContactType.GuestStayStarted:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "سفر شما آغاز شد",
                        "<div>" + "مهمان گرامی ، سفر شما به اقامتگاه با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " آغاز شد. لطفا در پایان سفر به اقامتگاه امتیاز دهید." + "</div>"
                        );
                    break;
                case UserContactType.HostCancelRequestSent:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "درخواست لفو توسط مهمان",
                        "<div>" + "میزبان گرامی ، درخواست کنسل شدن رزرو اقامتگاه با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " توسط مهمان داده شده. لطفا منتظر اعلام نتیجه درخواست کنسلی باشید." + "</div>"
                        );
                    break;
                case UserContactType.HostReserveCanceled:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "رزرو مورد نظر لغو شد",
                        "<div>" + "میزبان گرامی ، با درخواست کنسلی آگهی با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " موافقت شد. درخواست رزرو مورد نظر لغو شد." + "</div>"
                        );
                    break;
                case UserContactType.RefuseCancelReserve:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "مهمان از لغو سفر منصرف شد",
                        "<div>" + "میزبان گرامی ، مهمان اقامتگاه شما با کد آگهی " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " از کنسل کردن رزرو منصرف شد." + "</div>"
                        );
                    break;
                case UserContactType.NewReserveChatGuest:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "چت جدید",
                        "<div>" + "شما از طرف میزبان آگهی " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " با کد کاربری " + contactDTO.UserId + " یک چت جدید دارید. به حساب کاربری خود در املاک باشی مراجعه فرمایید." + "</div>"
                        );
                    break;
                case UserContactType.NewReserveChatHost:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "چت جدید",
                        "<div>" + "میزبان گرامی ، شما از طرف مهمان برای آگهی " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " با کد کاربری " + contactDTO.UserId + " یک چت جدید دارید. به حساب کاربری خود در املاک باشی مراجعه فرمایید." + "</div>"
                        );
                    break;
                case UserContactType.HostReserveCashPay:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "تایید پرداخت نقدی",
                        "<div>" + "مهمان اقامتگاه با کد آگهی " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " اعلام کرده که مبلغ تسویه را به صورت نقدی پرداخت کرده. وارد حساب خود و تایید فرمایید." + "</div>"
                        );
                    break;
                case UserContactType.HostReservedTotalPayed:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "اقامتگاه شما رزرو شد",
                        "<div>" + "میزبان گرامی ، آگهی شما با کد آگهی " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " رزرو شد." + "</div>" +
                        "<div>" + "کل مبلغ توسط مهمان پرداخت شده است." + "</div>" +
                        "<div>" + "هیچ مبلغی از مهمان دریافت نکنید." + "</div>" +
                        "<div>" + "شماره تلفن مهمان : " + contactDTO.AudienceMobile + "</div>"
                        );
                    break;
                case UserContactType.HostReservedDepositePayed:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "اقامتگاه شما رزرو شد",
                        "<div>" + "میزبان گرامی ، آگهی شما با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " رزرو شد." + "</div>" +
                        "<div>" + "مبلغ " +  contactDTO.Price + " تومان به عنوان بیعانه توسط مهمان پرداخت شده." + "</div>" +
                        "<div>" + "باقیمانده مبلغ " + contactDTO.RemainPrice + " تومان میباشد که باید توسط مهمان قبل از تحویل اقامتگاه پرداخت شود." + "</div>" +
                        "<div>" + "شماره تلفن مهمان : " + contactDTO.AudienceMobile + "</div>"
                        );
                    break;
                case UserContactType.ReserveCanceledBySystem:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "درخواست رزرو لغو شد",
                        "<div>" + "درخواست رزرو آگهی با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " به دلیل عدم پاسخگویی " + contactDTO.DoerTitle + " لغو شد." + "</div>"
                        );
                    break;
                case UserContactType.ReserveRequest:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                                "درخواست رزرو",
                                "<div>" + "درخواست رزرو برای آگهی با کد " +
                                 contactDTO.AdvertiseId + " : " + "</div>" +
                                "<div>" + "از تاریخ " + contactDTO.Extra1 + " تا تاریخ " + contactDTO.Extra2 + "</div>" +
                                "<div>" +
                                "تعداد مهمان ها: " + contactDTO.Extra3 +
                                "</div>" + "<div>" +
                                "برای پاسخ روی لینک زیر کلیک کنید: " +
                                "</div>" + "<div>" +
                                "<a href='" + GeneralData.WebsiteUrl + "/reserve/reserveitemmanager?category=0'>" +
                                "پاسخ به درخواست رزرو" + "</a>" +
                                "</div>"
                                );
                    break;
                case UserContactType.SiteClearingHost:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "تسویه رزرو",
                        "<div>" + "مبلغ " +  contactDTO.Price + " تومان بایت تسویه رزرو آگهی با کد " +  contactDTO.AdvertiseId + " به حساب شما واریز شد." + "</div>" +
                        "<div>" + "کد رزرو : " +  contactDTO.ReserveId + "</div>" +
                        "<div>" + "شماره تراکنش : " +  contactDTO.TransactionId + "</div>"
                        );
                    break;
                case UserContactType.SiteClearingHostWithCredit:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "تسویه رزرو",
                        "<div>" + "مبلغ " +  contactDTO.Price + " تومان بایت تسویه رزرو آگهی با کد " +  contactDTO.AdvertiseId + " به کیف پول شما واریز شد." + "</div>" +
                        "<div>" + "کد رزرو : " +  contactDTO.ReserveId + "</div>" +
                        "<div>" + "شماره تراکنش : " +  contactDTO.TransactionId + "</div>"
                        );
                    break;
                case UserContactType.SiteRefundGuest:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                      "عودت مبلغ رزرو",
                      "<div>" + "مبلغ " +  contactDTO.Price + " تومان بایت عودت رزرو آگهی با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " به حساب شما واریز شد." + "</div>" +
                      "<div>" + "شماره تراکنش : " +  contactDTO.TransactionId + "</div>"
                      );
                    break;
                case UserContactType.HostReserveRejectedForReserved:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "درخواست رزرو لغو شد",
                        "<div>" + "میزبان گرامی، درخواست رزرو اقامتگاه با کد " +  contactDTO.AdvertiseId + " و کد رزرو " +  contactDTO.ReserveId + " به دلیل رزرو اقامتگاه دیگر توسط مهمان لغو شد." + "</div>"
                        );
                    break;
                case UserContactType.payment:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                        "رسید پرداخت اینترنتی",
                        "<div>" + "پرداخت شما با موفقیت انجام شد ." + "</div>" +
                        "<div>" + "شماره تراکنش : " +  contactDTO.TransactionId + "</div>" +
                        "<div>" + "با تشکر" + "</div>"
                        );
                    break;
                case UserContactType.UserCreditIncrease:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                      "رسید واریز به کیف پول شما",
                      "<div>" + "مبلغ " +  contactDTO.Price + " تومان بابت " + contactDTO.CauseString + " به کیف پول شما واریز شد." + "</div>" +
                      "<div>" + "شماره تراکنش : " +  contactDTO.TransactionId + "</div>"
                      );
                    break;
                case UserContactType.UserCreditDecrease:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                      "رسید برداشت از کیف پول شما",
                      "<div>" + "مبلغ " +  contactDTO.Price + " تومان بابت " + contactDTO.CauseString + " از کیف پول شما برداشت شد." + "</div>" +
                      "<div>" + "شماره تراکنش : " +  contactDTO.TransactionId + "</div>"
                      );
                    break;
                case UserContactType.HostUpdatePrice:
                    SendEmail(EmailSenderDepartment.Support,  contactDTO.UserEmail,
                      "بروز رسانی قیمت ها",
                      "<div>" + "آقا/خانم " + contactDTO.Extra1 + " عزیز " + "با توجه به نزدیک بودن تعطیلات قیمت های خود را بروز رسانی فرمایید." + "</div>"
                      );
                    break;
            }
        }

        private void SendEmail(EmailSenderDepartment department, string email, string title, string text)
        {
            try
            {
                if (EmailUtility.ValidateEmail(email))
                {
                    EmailEngine.SendEmail(department, new List<string>() { email }, title, text);
                }
                else
                {
                    // TODO logger
                }
            }
            catch
            {
                // TODO logger
            }
        }
    }
}
