using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class AdvertiseReportRepository : GenericRepository<AdvertiseReport, int>
    {
        public AdvertiseReportRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
