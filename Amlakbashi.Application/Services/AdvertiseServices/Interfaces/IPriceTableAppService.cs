using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IPriceTableAppService : IAppService<PriceTable, int>
    {
        bool SetAccommodationPriceInDate(long accId, string fromPersianDate, string toPersianDate, int price, out string msg);
        ServiceResult UpdateAdvertiseManualPrices(AdvertiseUpdatePriceRequest request);
    }
}
