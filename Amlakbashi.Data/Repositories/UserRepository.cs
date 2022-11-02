using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class UserRepository: GenericRepository<User, int>
    {
        public UserRepository(AmlakbashiDB context): base(context)
        {
        }
    }
}
