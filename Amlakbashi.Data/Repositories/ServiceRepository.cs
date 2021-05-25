using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ServiceRepository : GenericRepository<Service, int>
    {
        public ServiceRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
