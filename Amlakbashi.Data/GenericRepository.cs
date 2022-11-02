using Amlakbashi.Core;
using Amlakbashi.Core.Common.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Amlakbashi.Data
{
    public class GenericRepository<T, K> : IRepository<T, K> where T : Entity<K>
    {
        private AmlakbashiDB context = null;
        private DbSet<T> dbSet = null;

        public GenericRepository(AmlakbashiDB context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public T Find(K id)
        {
            return dbSet.Find(id);
        }

        public async Task<T> FindAsync(K id)
        {
            return await dbSet.FindAsync(id);
        }

        public TEntity Find<TEntity, TKey>(TKey id) where TEntity : Entity<TKey>, new()
        {
            return context.Set<TEntity>().Find(id);
        }

        public void Reload(T entity)
        {
            context.Entry(entity).Reload();
        }

        public async Task ReloadAsync(T entity)
        {
            await context.Entry(entity).ReloadAsync();
        }

        public R Query<R>(Func<IQueryable<T>, R> query)
        {
            return query(dbSet);
        }

        public IQueryable<TEntity> Query<TEntity, TKey>(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) where TEntity : Entity<TKey>
        {
            var childrenDbSet = context.Set<TEntity>();
            return query(childrenDbSet);
        }

        public void Insert(T obj)
        {
            dbSet.Add(obj);
        }

        public async Task InsertAsync(T obj)
        {
            await dbSet.AddAsync(obj);
        }

        public void Insert(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public async Task InsertAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public void Update(T obj)
        {
            dbSet.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(K id)
        {
            T existing = dbSet.Find(id);
            dbSet.Remove(existing);
        }

        public async Task DeleteAsync(K id)
        {
            T existing = await dbSet.FindAsync(id);
            dbSet.Remove(existing);
        }

        public void Delete(Expression<Func<T, bool>> query)
        {
            var data = dbSet.Where(query);
            dbSet.RemoveRange(data);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public void RemoveChildren<TChild, TChildKey, R>(K id, string collectionName,Func<IEnumerable<TChild>, R> query) where TChild : Entity<TChildKey>, new()
        {
            var parent = dbSet.Find(id);
            var childrenCollection = (typeof(T).GetProperty(collectionName)
                .GetValue(parent) as ICollection<TChild>);
            var children = query(childrenCollection) as IQueryable<TChild>;
            var childDbSet = context.Set<TChild>();
            foreach (var child in children.ToList())
            {
                childDbSet.Remove(child);
            }
        }
    }
}
