using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class BankCardRepository : GenericRepository<BankCard, int>
    {
        public BankCardRepository(AmlakbashiDB _context) : base(_context)
        {

        }
    }
}
