using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class OccupiedTableRepository : GenericRepository<OccupiedTable, long>
    {
        public OccupiedTableRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
