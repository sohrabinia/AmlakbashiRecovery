using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ServiceRepository : GenericRepository<Service, int>
    {
        public ServiceRepository(IDbContext _context) : base(_context)
        {
        }
    }
}
