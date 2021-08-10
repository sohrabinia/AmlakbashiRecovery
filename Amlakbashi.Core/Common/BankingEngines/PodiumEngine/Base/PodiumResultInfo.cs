namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base
{
    public abstract class PodiumResultInfo
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string MessageCode { get; set; }
        public string ErrorCode { get; set; }
    }
}