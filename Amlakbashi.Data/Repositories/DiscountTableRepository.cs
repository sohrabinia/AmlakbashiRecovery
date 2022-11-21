using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class DiscountTableRepository : GenericRepository<DiscountTable, int>
    {
        public DiscountTableRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
