using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices
{
    public class InstantReserveAutoCancelAppService : AppServiceBase<InstantReserveAutoCancel, long>, IInstantReserveAutoCancelAppService
    {
        public InstantReserveAutoCancelAppService(IRepository<InstantReserveAutoCancel, long> repository, ICacheManager<InstantReserveAutoCancel> cache) : base(repository, cache)
        {

        }
    }
}
