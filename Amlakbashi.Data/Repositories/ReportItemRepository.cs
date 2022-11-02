using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ReportItemRepository : GenericRepository<ReportItem, long>
    {
        public ReportItemRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
