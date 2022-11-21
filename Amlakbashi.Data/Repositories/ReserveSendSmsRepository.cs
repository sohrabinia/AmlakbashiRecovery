using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class ReserveSendSmsRepository : GenericRepository<ReserveSendSms, long>
    {
        public ReserveSendSmsRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
