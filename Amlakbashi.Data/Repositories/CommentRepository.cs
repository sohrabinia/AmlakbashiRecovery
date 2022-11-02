using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class CommentRepository : GenericRepository<Comment, long>
    {
        public CommentRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
