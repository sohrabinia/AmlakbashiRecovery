using Amlakbashi.Core.Common.Repository;

namespace Amlakbashi.Core.Common.AppService
{
    public abstract class BaseAppService<TEntity, TKey> where TEntity : Entity<TKey>
    {
        protected readonly IRepository<TEntity, TKey> Repository;
        public BaseAppService(IRepository<TEntity, TKey> repository)
        {
            Repository = repository;
        }
    }
}
