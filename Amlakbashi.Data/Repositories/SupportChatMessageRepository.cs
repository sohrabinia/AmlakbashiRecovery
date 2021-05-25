using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class SupportChatMessageRepository : GenericRepository<SupportChatMessage, long>
    {
        public SupportChatMessageRepository(AmlakbashiDB _context) : base(_context)
        {

        }
    }
}
