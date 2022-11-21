using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IAdvertiseReportAppService
    {
        bool Insert(AdvertiseReport item, out List<string> msg);
        bool Update(AdvertiseReport item, out List<string> msg);
    }
}
