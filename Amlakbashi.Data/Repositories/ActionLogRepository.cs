using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ActionLogRepository : GenericRepository<ActionLog, long>
    {
        public ActionLogRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
