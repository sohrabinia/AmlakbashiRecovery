using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class AdvertiseRepository : GenericRepository<Advertise, long>
    {
        public AdvertiseRepository(IDbContext _context) : base(_context)
        {
        }
    }
}
