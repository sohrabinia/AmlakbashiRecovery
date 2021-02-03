using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Redis;
using ServiceStack.DataAnnotations;

namespace Amlakbashi.Data.Repositories
{
    public class BlogPostRepository : GenericRepository<BlogPost, int>
    {
        public BlogPostRepository(IDbContext _context) : base(_context)
        {

        }
    }
}
