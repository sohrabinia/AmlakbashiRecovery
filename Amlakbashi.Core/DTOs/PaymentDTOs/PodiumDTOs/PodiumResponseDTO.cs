namespace Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs
{
    public abstract class PodiumResponseDTO
    {
        public bool HasError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public ExpenditurePaymentErrorAgent ErrorAgent { get; set; }
    }

    public enum ExpenditurePaymentErrorAgent
    {
        Unset,
        Podium,
        PasargadBank
    }
}
