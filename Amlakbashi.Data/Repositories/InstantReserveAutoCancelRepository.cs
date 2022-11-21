using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class InstantReserveAutoCancelRepository : GenericRepository<InstantReserveAutoCancel, long>
    {
        public InstantReserveAutoCancelRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
