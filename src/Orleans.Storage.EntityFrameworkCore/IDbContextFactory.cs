using Microsoft.EntityFrameworkCore;

namespace Orleans.Storage.EntityFrameworkCore
{
    public interface IDbContextFactory
    {
        DbContext CreateDbContext();
    }

    public interface IDbContextFactory<TDbContext>:IDbContextFactory
        where TDbContext:DbContext
    {
        new TDbContext  CreateDbContext();
    }
}
