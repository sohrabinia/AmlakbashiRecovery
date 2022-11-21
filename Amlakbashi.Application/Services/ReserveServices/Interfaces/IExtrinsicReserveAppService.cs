using Amlakbashi.Core.Entities;
using System;

namespace Amlakbashi.Application.Services.ReserveServices.Interfaces
{
    public interface IExtrinsicReserveAppService
    {
        void Insert(long advertiseId, DateTime date, ActionLog.ActionSourceEnum actionSource, int doerUserID, int count = 1);
        void Insert(long advertiseId, string from_date, string to_date, ActionLog.ActionSourceEnum actionSource, int doerUserId, int count = 1);
    }
}
