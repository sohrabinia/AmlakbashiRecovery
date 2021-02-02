using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.UserContact.Interfaces
{
    public interface ISmsContact
    {
        void SendMessage(UserContactDTO contactDTO);
        void SendReserveRequestCall(User user, long advertiseId);
        void SendPayReserveCall(User user, long advertiseId);
        void SendTemplate(string mobile, string template);
        void SendVerification(string localNumber, string code);
    }
}
