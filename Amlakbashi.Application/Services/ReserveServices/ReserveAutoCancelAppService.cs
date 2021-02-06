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
    public class ReserveAutoCancelAppService : AppServiceBase<ReserveAutoCancel, long>, IReserveAutoCancelAppService
    {
        public ReserveAutoCancelAppService(IRepository<ReserveAutoCancel, long> repository, ICacheManager<ReserveAutoCancel> cache) : base(repository, cache)
        {
        }
    }
}
