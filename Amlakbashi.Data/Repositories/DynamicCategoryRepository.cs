using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class DynamicCategoryRepository : GenericRepository<DynamicCategory, int>
    {
        public DynamicCategoryRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
