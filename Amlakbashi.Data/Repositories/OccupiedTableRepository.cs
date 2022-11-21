using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class OccupiedTableRepository : GenericRepository<OccupiedTable, long>
    {
        public OccupiedTableRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
