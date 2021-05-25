using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ServicePostRepository : GenericRepository<ServicePost, int>
    {
        public ServicePostRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
