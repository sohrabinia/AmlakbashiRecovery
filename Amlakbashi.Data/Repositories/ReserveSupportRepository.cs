using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ReserveSupportRepository : GenericRepository<ReserveSupport, int>
    {
        public ReserveSupportRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
