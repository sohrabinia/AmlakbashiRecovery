using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.Interfaces
{
    public interface IReserveAutoCancelAppService : IAppService<ReserveAutoCancel, long>
    {
    }
}
