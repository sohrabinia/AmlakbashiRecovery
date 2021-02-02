using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class OccupiedTableRepository : GenericRepository<OccupiedTable, long>
    {
        public OccupiedTableRepository(IDbContext _context) : base(_context)
        {
        }
    }
}
