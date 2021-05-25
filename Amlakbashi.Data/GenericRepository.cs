using Amlakbashi.Core;
using Amlakbashi.Core.Common.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace Amlakbashi.Data
{
    public class GenericRepository<T, K> : IRepository<T, K> where T : Entity<K>
    {
        private AmlakbashiDB _context = null;
        private DbSet<T> dbSet = null;

        public GenericRepository(AmlakbashiDB _context)
        {
            this._context = _context;
            dbSet = _context.Set<T>();
        }

        public void Insert(T obj)
        {
            dbSet.Add(obj);
        }
        public void Insert(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }
        public void Update(T obj)
        {
            dbSet.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Attach(T obj)
        {
            dbSet.Attach(obj);
        }
        public void Delete(K id)
        {
            T existing = dbSet.Find(id);
            dbSet.Remove(existing);
        }

        public void Delete(Expression<Func<T, bool>> query)
        {
            var data = dbSet.Where(query);
            dbSet.RemoveRange(data);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public R Query<R>(Func<IQueryable<T>, R> query)
        {
            return query(dbSet);
        }
        public T Find(K id)
        {
            return dbSet.Find(id);
        }
        public TEntity Find<TEntity, TKey>(TKey id) where TEntity : Entity<TKey>, new()
        {
            return _context.Set<TEntity>().Find(id);
        }

        public void Reload(T entity)
        {
            _context.Entry(entity).Reload();
        }

        public IQueryable<TEntity> Query<TEntity, TKey>(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) where TEntity : Entity<TKey>
        {
            var childrenDbSet = _context.Set<TEntity>();
            return query(childrenDbSet);
        }

        public void RemoveChildren<TChild, TChildKey, R>(K id, string collectionName,Func<IEnumerable<TChild>, R> query) where TChild : Entity<TChildKey>, new()
        {
            var parent = dbSet.Find(id);
            var childrenCollection = (typeof(T).GetProperty(collectionName)
                .GetValue(parent) as ICollection<TChild>);
            var children = query(childrenCollection) as IQueryable<TChild>;
            var childDbSet = _context.Set<TChild>();
            foreach (var child in children.ToList())
            {
                childDbSet.Remove(child);
            }
        }
    }
}
