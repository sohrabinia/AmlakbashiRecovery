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
    public class ReserveSendSmsAppService : AppServiceBase<ReserveSendSms, long>, IReserveSendSmsAppService
    {
        public ReserveSendSmsAppService(IRepository<ReserveSendSms, long> repository, ICacheManager<ReserveSendSms> cache) : base(repository, cache)
        {

        }
    }
}
