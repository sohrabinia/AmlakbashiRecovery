using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using static Amlakbashi.Core.Entities.ActionLog;

namespace Amlakbashi.Application.Services.ReserveServices.Interfaces
{
    public interface IExtrinsicReserveAppService : IAppService<ExtrinsicReserve, long>
    {
        void Insert(long advertiseId, DateTime date, ActionSourceEnum actionSource, int doerUserID, int count = 1);
        void Insert(long advertiseId, string from_date, string to_date, ActionSourceEnum actionSource, int doerUserId, int count = 1);
    }
}
