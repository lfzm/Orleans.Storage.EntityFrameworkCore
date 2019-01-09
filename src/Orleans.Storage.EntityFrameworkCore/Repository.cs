using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Orleans.Storage.EntityFrameworkCore
{
    public abstract class Repository<TEntity, TPrimaryKey, TDbContext> : Repository<TDbContext>, IRepository<TEntity, TPrimaryKey>
         where TEntity : class, IStorageEntity
         where TDbContext : DbContext
    {
        public Repository(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> GetAsync(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }


    public abstract class Repository<TDbContext>
          where TDbContext : DbContext
    {
        protected readonly IDbContextFactory<TDbContext> dbContextFactory;
        protected readonly IServiceProvider serviceProvider;
        public Repository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.dbContextFactory = this.serviceProvider.GetRequiredService<IDbContextFactory<TDbContext>>();
        }
        public Task<T> DoAsync<T>(Func<TDbContext, T> func)
        {
            using (var db = dbContextFactory.CreateDbContext())
            {
                var rest = func.Invoke(db);
                return Task.FromResult(rest);
            }
        }
        public Task DoAsync(Action<TDbContext> action)
        {
            using (var db = dbContextFactory.CreateDbContext())
            {
                action.Invoke(db);
                return Task.CompletedTask;
            }
        }
        public T Do<T>(Func<TDbContext, T> func)
        {
            using (var db = dbContextFactory.CreateDbContext())
            {
                return func.Invoke(db);
            }
        }
        public void Do(Action<TDbContext> action)
        {
            using (var db = dbContextFactory.CreateDbContext())
            {
                action.Invoke(db);
            }
        }
    }
}
