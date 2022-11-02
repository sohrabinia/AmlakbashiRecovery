using System;

namespace Amlakbashi.Application.Services.ReserveServices.Interfaces
{
    public interface IReserveAutoCancelAppService
    {
        DateTime? GetReserveExpireTime(long reserveId);
        void UpdateScheduledTime(long reserveId, int delayInMinute = 30);
    }
}
