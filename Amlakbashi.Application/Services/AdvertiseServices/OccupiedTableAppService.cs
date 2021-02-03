using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using Amlakbashi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class OccupiedTableAppService : AppServiceBase<OccupiedTable, long>, IOccupiedTableAppService
    {
        public OccupiedTableAppService(IRepository<OccupiedTable, long> repository, ICacheManager<OccupiedTable> cache) : base(repository, cache)
        {
        }

        public IList<OccupiedTable> GetByAccId(long accId, DateTime fromDate)
        {
            return Repository.Query(q => q.Where(w => w.AdvertiseID == accId && w.Date >= fromDate).ToList());
        }
    }
}
