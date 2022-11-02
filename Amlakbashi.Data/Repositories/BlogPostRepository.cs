using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class BlogPostRepository : GenericRepository<BlogPost, int>
    {
        public BlogPostRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
