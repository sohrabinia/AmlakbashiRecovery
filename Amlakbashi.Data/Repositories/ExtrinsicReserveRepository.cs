using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ExtrinsicReserveRepository : GenericRepository<ExtrinsicReserve, long>
    {
        public ExtrinsicReserveRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
