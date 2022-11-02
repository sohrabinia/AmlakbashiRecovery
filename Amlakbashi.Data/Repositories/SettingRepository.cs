using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class SettingRepository : GenericRepository<Setting, int>
    {
        public SettingRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
