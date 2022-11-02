using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ReserveRepository : GenericRepository<Reserve, long>
    {
        public ReserveRepository(AmlakbashiDB context): base(context)
        {
        }
    }
}
