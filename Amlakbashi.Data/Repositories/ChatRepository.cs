using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ChatRepository : GenericRepository<Chat, long>
    {
        public ChatRepository(AmlakbashiDB context): base(context)
        {
        }
    }
}
