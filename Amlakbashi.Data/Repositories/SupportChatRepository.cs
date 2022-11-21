using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class SupportChatRepository : GenericRepository<SupportChat, long>
    {
        public SupportChatRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
