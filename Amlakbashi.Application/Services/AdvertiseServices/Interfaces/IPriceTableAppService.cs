using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IPriceTableAppService
    {
        bool SetAccommodationPriceInDate(long accId, string fromPersianDate, string toPersianDate, int price, out string msg);
        ServiceResult UpdateAdvertiseManualPrices(AdvertiseUpdatePriceRequest request);
    }
}
