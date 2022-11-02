using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ReservePaymentRepository : GenericRepository<ReservePayment, long>
    {
        public ReservePaymentRepository(AmlakbashiDB context): base(context)
        {
        }
    }
}
