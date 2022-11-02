using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class AdvertiseRepository : GenericRepository<Advertise, long>
    {
        public AdvertiseRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
