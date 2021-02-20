using static Amlakbashi.Core.Entities.GroupPayment;

namespace Amlakbashi.Core.Infrastructure.StyleHelpers
{
    public static class PaymentStyleHelper
    {
        public static string GetGroupPaymentStatusColor(GroupPaymentStatus groupPaymentStatus)
        {
            switch (groupPaymentStatus)
            {
                case GroupPaymentStatus.ReadyToPay:
                    return "#edf7ee";
                case GroupPaymentStatus.WithError:
                    return "#f4ebe3";
                case GroupPaymentStatus.Excluded:
                    return "#f2e4e3";
                default:
                    return "";
            }
        }
    }
}
