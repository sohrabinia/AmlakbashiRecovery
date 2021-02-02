using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Amlakbashi.Core.Common.Repository
{
    public interface IDbContext
    {
        int SaveChanges();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry Entry(object entity);
    }
}
