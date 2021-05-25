using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class RegionRepository : GenericRepository<Region, int>
    {
        public RegionRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
