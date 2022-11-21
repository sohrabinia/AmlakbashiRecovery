using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class PaymentRepository : GenericRepository<Payment, int>
    {
        public PaymentRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
