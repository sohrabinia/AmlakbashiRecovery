using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface ICartAppService : IAppService<Cart, long>
    {
        IList<Cart> Filter(int status = -1, int uid = -1, long refid = -1);
    }
}
