using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class PrizeCreditTransactionRepository : GenericRepository<PriceTable, int>
    {
        public PrizeCreditTransactionRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
