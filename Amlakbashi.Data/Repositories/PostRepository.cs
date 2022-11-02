using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class PostRepository : GenericRepository<Post, long>
    {
        public PostRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
