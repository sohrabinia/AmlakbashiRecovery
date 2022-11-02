using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Application.Services.ActionLogServices.Interfaces
{
    public interface IActionLogAppService
    {
        IList<ActionLog> Filter(int userId, int actionType, int actionSource, long relatedId);
        ActionLog Find(long id);
    }
}
