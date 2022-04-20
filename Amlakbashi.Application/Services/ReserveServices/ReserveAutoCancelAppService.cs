using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
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
        public ReserveAutoCancelAppService(IRepository<ReserveAutoCancel, long> repository) : base(repository)
        {
        }

        public DateTime? GetReserveExpireTime(long reserveId)
        {
            var reserveAutoCancel = Repository.Query(q => q.FirstOrDefault(w => w.ReserveId == reserveId));
            return reserveAutoCancel?.ScheduledTime;
        }

        public void UpdateScheduledTime(long reserveId, int delayInMinute = 30)
        {
            var data = Repository.Query(q => q.FirstOrDefault(w => w.ReserveId == reserveId));
            if (data != null)
            {
                var delay = DateTimeUtility.DelayAvoidingNightTime(new TimeSpan(0, delayInMinute, 0));
                data.ScheduledTime = DateTime.Now.Add(delay);
                Repository.Update(data);
                Repository.Save();
            }
        }
    }
}
