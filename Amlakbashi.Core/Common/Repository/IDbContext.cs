using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Amlakbashi.Core.Common.Repository
{
    public interface IDbContext
    {
        int SaveChanges();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry Entry(object entity);
    }
}
