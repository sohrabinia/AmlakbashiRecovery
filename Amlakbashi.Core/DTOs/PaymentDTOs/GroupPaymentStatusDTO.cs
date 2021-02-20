using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using static Amlakbashi.Core.Entities.GroupPayment;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    public class GroupPaymentStatusDTO
    {
        public GroupPaymentStatus status { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public GroupPaymentStatusDTO(GroupPaymentStatus status)
        {
            this.status = status;
            title = PaymentLocalization.GetGroupPaymentStatusString(status);
            color = PaymentStyleHelper.GetGroupPaymentStatusColor(status);
        }
    }
}
