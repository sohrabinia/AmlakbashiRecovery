using Amlakbashi.Core.Common.ContactEngines;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.UserContact.Interfaces
{
    public interface IUserContactFacade
    {
        void SendMessage(UserContactDTO contactDTO);
        void SendMessageClassic(bool initial, UserContactDTO contactDTO);
        void SendReserveRequestCall(User user, long advertiseId);
        void SendPayReserveCall(User user, long advertiseId);
        void TestNotification(string token, long reserveId);
        void SendNotification(string token, string title, string body, string click_action,
            List<NotificationButton> buttons = null);
        void SendTemplateSms(string mobile, string template);
        void SendVerificationSms(string localNumber, string code);
        void SendNotificationApplication(string token, string title, string body, string targetAction, string targetId);
    }
}
