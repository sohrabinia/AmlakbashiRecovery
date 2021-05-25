using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class PostRepository : GenericRepository<Post, long>
    {
        public PostRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
