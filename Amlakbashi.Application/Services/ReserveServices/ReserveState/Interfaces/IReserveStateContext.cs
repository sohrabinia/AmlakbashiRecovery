using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces
{
    public interface IReserveStateContext : IReserveActions
    {
        IReserveStateContext UseReserve(long reserveId);
        bool SetStatus(ReserveStatus status, bool sendSms,
            ActionLog.ActionSourceEnum actionSource, int doerUserId, bool force = false);
    }
}
