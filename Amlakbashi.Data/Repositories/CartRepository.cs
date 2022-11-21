using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class CartRepository : GenericRepository<Cart, long>
    {
        public CartRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
