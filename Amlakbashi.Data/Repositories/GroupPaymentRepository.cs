using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class GroupPaymentRepository : GenericRepository<GroupPayment, int>
    {
        public GroupPaymentRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
