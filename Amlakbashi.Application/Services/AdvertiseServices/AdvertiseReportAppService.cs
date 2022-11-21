using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using Amlakbashi.Data;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    internal class AdvertiseReportAppService : BaseAppService<AdvertiseReport, int>, IAdvertiseReportAppService
    {
        public AdvertiseReportAppService(IRepository<AdvertiseReport, int> repository) : base(repository)
        {
        }

        public bool Insert(AdvertiseReport item, out List<string> msg)
        {
            if (item.Validate(out msg))
            {
                Repository.Insert(item);
                Repository.Save();
                return true;
            }
            return false;
        }

        public bool Update(AdvertiseReport item, out List<string> msg)
        {
            if (item.Validate(out msg))
            {
                var existedItem = Repository.Query(q => q.FirstOrDefault(f => f.Id == item.Id));
                existedItem.Reason = item.Reason;
                existedItem.ReasonString = item.ReasonString;
                Repository.Update(item);
                Repository.Save();
                return true;
            }
            return false;
        }
    }
}
