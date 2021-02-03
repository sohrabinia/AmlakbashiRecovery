using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IPriceTableAppService : IAppService<PriceTable, int>
    {
        bool SetAccommodationPriceInDate(long accId, string fromPersianDate, string toPersianDate, int price, out string msg);
    }
}
