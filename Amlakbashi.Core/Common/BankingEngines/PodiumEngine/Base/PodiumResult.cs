namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base
{
    public abstract class PodiumResult<T> where T : PodiumResultInfo
    {
        public bool hasError { get; set; }
        public long messageId { get; set; }
        public long referenceNumber { get; set; }
        public int errorCode { get; set; }
        public string message { get; set; }
        public int count { get; set; }
        public string ott { get; set; }
        public T result { get; set; }
    }
}