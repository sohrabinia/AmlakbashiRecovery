using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services
{
    internal class CartAppService : AppServiceBase<Cart, long>, ICartAppService
    {
        public CartAppService(IRepository<Cart, long> repository) : base(repository)
        {
        }

        public IList<Cart> Filter(int status = -1, int uid = -1, long refid = -1)
        {
            var model = Repository.Query(q => q);
            if (refid > 0)
            {
                model = model.Where(w => w.Payment.RefID == refid);
            }
            if (uid != -1)
                model = model.Where(c => c.UserID == uid);

            if (status != -1)
                model = model.Where(c => c.Status == status);

            return model.OrderByDescending(u => u.PayDate).ToList();
        }
    }
}
