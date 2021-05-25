using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ActionLogRepository : GenericRepository<ActionLog, long>
    {
        public ActionLogRepository(AmlakbashiDB _context) : base(_context)
        {

        }
    }
}
