using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class AdvertiseRepository : GenericRepository<Advertise, long>
    {
        public AdvertiseRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
