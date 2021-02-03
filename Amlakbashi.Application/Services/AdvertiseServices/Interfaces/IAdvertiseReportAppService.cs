using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IAdvertiseReportAppService : IAppService<AdvertiseReport, int>
    {
        bool Insert(AdvertiseReport item, out List<string> msg);
        bool Update(AdvertiseReport item, out List<string> msg);
    }
}
