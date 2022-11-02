using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class CreditTransactionRepository : GenericRepository<CreditTransaction, long>
    {
        public CreditTransactionRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
