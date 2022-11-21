using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class BankCardRepository : GenericRepository<BankCard, int>
    {
        public BankCardRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
