using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.UserContact.Interfaces
{
    public interface IAppNotificationContact
    {
        void SendMessage(UserContactDTO contactDTO);
    }
}
