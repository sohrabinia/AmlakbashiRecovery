using Amlakbashi.Core.Entities;

namespace Amlakbashi.Data.Repositories
{
    public class FileRepository : GenericRepository<File, long>
    {
        public FileRepository(AmlakbashiDB context) : base(context)
        {
        }
    }
}
