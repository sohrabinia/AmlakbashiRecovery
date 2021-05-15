namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    public abstract class BankingResultDTO
    {
        public bool hasError { get; set; }
        public int errorCode { get; set; }
        public string message { get; set; }
        public long referenceNumber { get; set; }
        public string ott { get; set; }
    }
}
