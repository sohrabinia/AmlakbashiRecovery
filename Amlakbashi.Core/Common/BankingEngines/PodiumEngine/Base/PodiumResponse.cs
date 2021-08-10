namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base
{
    public abstract class PodiumResponse<T> where T : PodiumResultInfo
    {
        public bool hasError { get; set; }
        public long messageId { get; set; }
        public long referenceNumber { get; set; }
        public int errorCode { get; set; }
        public string message { get; set; }
        public int count { get; set; }
        public string ott { get; set; }
        public PodiumResult result { get; set; }
        public T BankResult { get; set; }
    }

    public class PodiumResult
    {
        public string result { get; set; }
        public int statusCode { get; set; }
    }
}