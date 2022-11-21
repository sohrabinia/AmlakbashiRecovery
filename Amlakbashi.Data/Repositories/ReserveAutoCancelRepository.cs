using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ReserveAutoCancelRepository : GenericRepository<ReserveAutoCancel, long>
    {
        public ReserveAutoCancelRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
