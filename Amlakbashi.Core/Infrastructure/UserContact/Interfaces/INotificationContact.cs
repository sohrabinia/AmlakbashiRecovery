using Amlakbashi.Core.Common.ContactEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.UserContact.Interfaces
{
    public interface INotificationContact
    {
        void SendMessage(UserContactDTO contactDTO);
        void SendNotification(string token, string title, string body, string click_action,
            List<NotificationButton> buttons = null);
        void TestMessage(string token, string reserveId);
        void SendMessageApplication(string token, string title, string body, string targetAction, string targetId);
    }
}
