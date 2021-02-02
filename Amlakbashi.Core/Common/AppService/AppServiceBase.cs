using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Repository;

namespace Amlakbashi.Core.Common.AppService
{
    public class AppServiceBase<TEntity, TKey> : IAppService<TEntity,TKey> where TEntity : Entity<TKey>
    {
        protected readonly IRepository<TEntity, TKey> Repository;
        protected readonly ICacheManager<TEntity> Cache;
        
        public AppServiceBase(IRepository<TEntity, TKey> repository,
            ICacheManager<TEntity> cache)
        {
            Repository = repository;
            Cache = cache;
        }

    }
}
