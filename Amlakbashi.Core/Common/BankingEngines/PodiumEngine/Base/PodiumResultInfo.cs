namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base
{
    public abstract class PodiumResultInfo
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int RsCode { get; set; }
    }
}