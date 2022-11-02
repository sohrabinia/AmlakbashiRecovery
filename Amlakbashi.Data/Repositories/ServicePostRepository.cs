using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ServicePostRepository : GenericRepository<ServicePost, int>
    {
        public ServicePostRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
