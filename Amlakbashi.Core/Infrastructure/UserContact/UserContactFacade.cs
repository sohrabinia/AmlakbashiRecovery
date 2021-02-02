using Amlakbashi.Core.Common.ContactEngines;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace Amlakbashi.Core.Infrastructure.UserContact
{

    public class UserContactFacade : IUserContactFacade
    {
        private static List<string> AdminMobiles = new List<string>()
        {
            "09191613134",
            "09356172126",
            "09102600350",
            "09121197156",
            "09360263804",
            "09198155019",
            "09216813826",
            "09354086894",
            "09196218216",
            "09912097905",
            "09052932348",
            "09199075074",
            "09365966647"
        };

        private readonly ISmsContact sms;
        private readonly INotificationContact notification;
        private readonly IAppNotificationContact appNotification;
        private readonly IEmailContact email;
        public UserContactFacade(ISmsContact sms,
            INotificationContact notification,
            IAppNotificationContact appNotification,
            IEmailContact email)
        {
            this.sms = sms;
            this.notification = notification;
            this.appNotification = appNotification;
            this.email = email;
        }

        public void SendMessage(UserContactDTO contactDTO)
        {
#if DEBUG
            var mobile = PhoneUtility.InternationalNumberToLocal(contactDTO.UserMainMobile);
            if (AdminMobiles.Contains(mobile) == false)
                return;
#endif
            if (!string.IsNullOrEmpty(contactDTO.UserFcmAppNotificationToken) ||
                !string.IsNullOrEmpty(contactDTO.UserAppNotificationToken) ||
                !string.IsNullOrEmpty(contactDTO.UserNotificationToken))
            {
                if (!string.IsNullOrEmpty(contactDTO.UserFcmAppNotificationToken))
                {
                    contactDTO.FcmNotification = true;
                    appNotification.SendMessage(contactDTO);
                }
                else if (!string.IsNullOrEmpty(contactDTO.UserAppNotificationToken))
                {
                    appNotification.SendMessage(contactDTO);
                }
                else
                {
                    notification.SendMessage(contactDTO);
                }

                switch (contactDTO.Type)
                {
                    case UserContactType.NewReserveChatHost:
                    case UserContactType.NewReserveChatGuest:
                    case UserContactType.CouponAppreciate:
                    case UserContactType.CouponPresent:
                    case UserContactType.PrizeCharge:
                        break;
                    default:
                        SendMessageClassic(true, contactDTO);
                        break;
                }
            }
            else
            {
                SendMessageClassic(true, contactDTO);
            }
        }

        public void SendMessageClassic(bool initial, UserContactDTO contactDTO)
        {
#if DEBUG
            var mobile = PhoneUtility.InternationalNumberToLocal(contactDTO.UserMainMobile);
            if (AdminMobiles.Contains(mobile) == false)
                return;
#endif
            bool isMessageRequired = false;
            switch (contactDTO.Type)
            {
                case UserContactType.GuestPayReserve:
                    isMessageRequired = contactDTO.ReserveStatus == Reserve.ReserveStatus.WaitForReserve;
                    break;
                case UserContactType.HostReserveCashPay:
                    isMessageRequired = contactDTO.ReserveStatus == Reserve.ReserveStatus.CashPay;
                    break;
                case UserContactType.ReserveRequest:
                    isMessageRequired = contactDTO.ReserveStatus == Reserve.ReserveStatus.WaitForResponse;
                    break;
                default:
                    isMessageRequired = false;
                    break;
            }
            if (initial || isMessageRequired)
            {
                if ((User.LoginPriorites)contactDTO.UserLoginPriority == User.LoginPriorites.Mobile)
                {
                    sms.SendMessage(contactDTO);
                }
                else
                {
                    email.SendMessage(contactDTO);
                }
            }
        }

        public void SendReserveRequestCall(User user, long advertiseId)
        {
#if DEBUG
            var mobile = PhoneUtility.InternationalNumberToLocal(user.MainMobile);
                if (AdminMobiles.Contains(mobile) == false)
                    return;
#endif
            sms.SendReserveRequestCall(user, advertiseId);
        }

        public void SendPayReserveCall(User user, long advertiseId)
        {
#if DEBUG
            var mobile = PhoneUtility.InternationalNumberToLocal(user.MainMobile);
            if (AdminMobiles.Contains(mobile) == false)
                return;
#endif
            sms.SendPayReserveCall(user, advertiseId);
        }

        public void TestNotification(string token, long reserveId)
        {
            notification.TestMessage(token, reserveId.ToString());
        }

        public void SendNotification(string token, string title, string body, string click_action,
            List<NotificationButton> buttons = null)
        {
#if DEBUG
            return;
#endif
            notification.SendNotification(token, title, body, click_action, buttons);
        }

        public void SendTemplateSms(string mobile, string template)
        {
#if DEBUG
            if (AdminMobiles.Contains(mobile) == false)
                    return;
#endif
            sms.SendTemplate(mobile, template);
        }

        public void SendVerificationSms(string localNumber, string code)
        {
            sms.SendVerification(localNumber, code);
        }

        public void SendNotificationApplication(string token, string title, string body, string targetAction, string targetId)
        {
            notification.SendMessageApplication(token, title, body, targetAction, targetId);
        }
    }
}