using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class AdvertiseReportRepository : GenericRepository<AdvertiseReport, int>
    {
        public AdvertiseReportRepository(IDbContext _context) : base(_context)
        {
        }
    }
}
