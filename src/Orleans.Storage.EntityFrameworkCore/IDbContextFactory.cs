using Microsoft.EntityFrameworkCore;

namespace Orleans.Storage.EntityFrameworkCore
{
    public interface IDbContextFactory
    {
        DbContext CreateDbContext<TEntity>();
    }

    public interface IDbContextFactory<TDbContext>: IDbContextFactory
    {
        TDbContext CreateDbContext();
    }
}
