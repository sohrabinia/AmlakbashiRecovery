using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Data.Repositories
{
    public class SettingRepository : GenericRepository<Setting, int>
    {
        public SettingRepository(AmlakbashiDB _context) : base(_context)
        {
        }
    }
}
