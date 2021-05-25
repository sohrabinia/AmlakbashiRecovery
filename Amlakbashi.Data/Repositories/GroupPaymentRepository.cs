using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Data.Repositories
{
    public class GroupPaymentRepository : GenericRepository<GroupPayment, int>
    {
        public GroupPaymentRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
