using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class SupportChatRepository : GenericRepository<SupportChat, long>
    {
        public SupportChatRepository(AmlakbashiDB _context) : base(_context)
        {

        }
    }
}
