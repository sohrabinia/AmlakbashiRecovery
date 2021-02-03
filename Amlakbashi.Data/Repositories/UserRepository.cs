using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Data.Repositories
{
    public class UserRepository: GenericRepository<User, int>
    {
        public UserRepository(IDbContext _context): base(_context)
        {
            
        }
    }
}
