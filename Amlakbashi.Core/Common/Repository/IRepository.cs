using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Amlakbashi.Core.Common.Repository
{
    public interface IRepository<T, K> where T : Entity<K>
    {
        R Query<R>(Func<IQueryable<T>, R> queryMethod);
        IQueryable<TEntity> Query<TEntity, TKey>(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) where TEntity : Entity<TKey>;
        void Insert(T obj);
        void Insert(IEnumerable<T> entities);
        void Update(T obj);
        void Attach(T obj);
        void Delete(K id);
        void Delete(Expression<Func<T, bool>> query);
        void Save();
        TEntity Find<TEntity, TKey>(TKey id) where TEntity : Entity<TKey>, new();
        void Reload(T entity);
        T Find(K id);
        void RemoveChildren<TChild, TChildKey, R>(K id, string collectionName, Func<IEnumerable<TChild>, R> query) where TChild : Entity<TChildKey>, new();
    }
}
