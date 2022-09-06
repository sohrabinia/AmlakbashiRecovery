namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    public abstract class BankingResultDTO
    {
        public bool HasError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
