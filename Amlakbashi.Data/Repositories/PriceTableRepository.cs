using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class PriceTableRepository : GenericRepository<PriceTable, int>
    {
        public PriceTableRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
