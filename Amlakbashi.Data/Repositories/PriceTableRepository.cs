using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class PriceTableRepository : GenericRepository<PriceTable, int>
    {
        public PriceTableRepository(IDbContext _context) : base(_context)
        {
        }
    }
}
