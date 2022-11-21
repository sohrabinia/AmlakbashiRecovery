using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class PriceTableRepository : GenericRepository<PriceTable, int>
    {
        public PriceTableRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
