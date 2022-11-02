using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ServiceRepository : GenericRepository<Service, int>
    {
        public ServiceRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
