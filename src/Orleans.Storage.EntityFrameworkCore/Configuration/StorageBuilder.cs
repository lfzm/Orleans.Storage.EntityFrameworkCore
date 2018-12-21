using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using System;

namespace Orleans.Storage.EntityFrameworkCore
{
    public class StorageBuilder : IStorageBuilder
    {
        public StorageBuilder(IServiceCollection service)
        {
            this.Service = service;
        }
        public IServiceCollection Service { get; }

        public IStorageBuilder AddDbContextFactory<TFactory>() where TFactory : IDbContextFactory
        {
            Service.AddSingleton(typeof(IDbContextFactory),typeof(TFactory));
            Service.AddSingleton(typeof(IDbContextFactory<>), typeof(TFactory));
            return this;
        }

        public IStorageBuilder AddRepository<TRepository, TEntity, TPrimaryKey>(bool isAutoUpdate = true, bool isAutoDelete = true, bool isAutoInsert = true)
          where TEntity : class, IStorageEntity
          where TRepository : class, IRepository
        {
            string name = typeof(TEntity).Name;
            Service.AddTransientNamedService<IRepository, TRepository>(name);
            Service.AddTransientNamedService<IRepositoryCore>(name, (sp, key) =>
            {
                return new RepositoryCore<TEntity, TPrimaryKey>(sp, isAutoUpdate, isAutoDelete, isAutoInsert);
            });
            return this;
        }

   

        public IStorageBuilder Configure(Action<OrleansStorageOptions> config)
        {
            this.Service.Configure<OrleansStorageOptions>(config);
            return this;
        }
    }
}
