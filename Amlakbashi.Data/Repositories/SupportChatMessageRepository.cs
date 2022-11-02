using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class SupportChatMessageRepository : GenericRepository<SupportChatMessage, long>
    {
        public SupportChatMessageRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
