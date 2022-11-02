using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class RegionRepository : GenericRepository<Region, int>
    {
        public RegionRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
