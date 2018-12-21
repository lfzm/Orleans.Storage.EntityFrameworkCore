using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Orleans.Storage.EntityFrameworkCore
{
    public interface IStorageBuilder
    {
        IServiceCollection Service { get; }

        IStorageBuilder Configure(Action<OrleansStorageOptions> config);

        IStorageBuilder AddDbContextFactory<TFactory>() 
            where TFactory : IDbContextFactory;

        IStorageBuilder AddRepository<TRepository, TEntity, TPrimaryKey>(bool isAutoUpdate = true, bool isAutoDelete = true, bool isAutoInsert = true)
          where TRepository : class, IRepository
          where TEntity : class, IStorageEntity;
    }
}
