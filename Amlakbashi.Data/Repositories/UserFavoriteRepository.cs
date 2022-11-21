using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class UserFavoriteRepository : GenericRepository<UserFavorite, int>
    {
        public UserFavoriteRepository(AmlakbashiDB context): base(context)
        {
        }
    }
}
