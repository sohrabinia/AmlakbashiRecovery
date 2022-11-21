using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Repository
{
    public interface IRepository<T, K> where T : Entity<K>
    {
        T Find(K id);
        Task<T> FindAsync(K id);
        TEntity Find<TEntity, TKey>(TKey id) where TEntity : Entity<TKey>, new();
        void Reload(T entity);
        Task ReloadAsync(T entity);
        R Query<R>(Func<IQueryable<T>, R> queryMethod);
        IQueryable<TEntity> Query<TEntity, TKey>(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) where TEntity : Entity<TKey>;
        void Insert(T obj);
        Task InsertAsync(T obj);
        void Insert(IEnumerable<T> entities);
        Task InsertAsync(IEnumerable<T> entities);
        void Update(T obj);
        void Delete(K id);
        Task DeleteAsync(K id);
        void Delete(Expression<Func<T, bool>> query);
        void Save();
        Task SaveAsync();
        void RemoveChildren<TChild, TChildKey, R>(K id, string collectionName, Func<IEnumerable<TChild>, R> query) where TChild : Entity<TChildKey>, new();
    }
}
