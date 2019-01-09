using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Runtime;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Orleans.Storage.EntityFrameworkCore
{
    public class RepositoryCore<TEntity, TPrimaryKey> : IRepositoryCore
        where TEntity : class, IStorageEntity
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEntityChangeDetector _changeDetector;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ILogger _logger;
        private readonly bool IsAutoUpdate = true;
        private readonly bool IsAutoDelete = true;
        private readonly bool IsAutoInsert = true;
        private TEntity SnapshotEntity;
        public RepositoryCore(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this._logger = serviceProvider.GetRequiredService<ILogger<RepositoryCore<TEntity, TPrimaryKey>>>();
            this._changeDetector = _serviceProvider.GetService<IEntityChangeDetector>();
            this._dbContextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();

        }
        public RepositoryCore(IServiceProvider serviceProvider, bool isAutoUpdate, bool isAutoDelete, bool isAutoInsert) : this(serviceProvider)
        {
            this.IsAutoDelete = isAutoDelete;
            this.IsAutoInsert = isAutoInsert;
            this.IsAutoUpdate = isAutoUpdate;
        }
        public async Task<object> AddAsync(object obj)
        {
            if (obj is TEntity entity)
            {
                TEntity e;
                if (this.IsAutoInsert)
                {
                    e = await this.AutoInsertAsync(entity);
                }
                else
                {
                    e = await this.GetRepository().InsertAsync(entity);
                }
                this.SaveSnapshot(e);
                return e;
            }
            else
            {
                throw new RepositoryException($"AddAsync：entity is not the same type as {typeof(TEntity).Name}");
            }
        }
        public Task ClearAsync(object obj)
        {
            if (obj is TEntity entity)
            {
                if (this.IsAutoDelete)
                {
                    return this.AutoDeleoteAsync(entity);
                }
                else
                {
                    return this.GetRepository().DeleteAsync(entity);
                }
            }
            else
            {
                throw new RepositoryException($"ClearAsync：entity is not the same type as {typeof(TEntity).Name}");
            }
        }
        public async Task<object> ModifyAsync(object id, object obj)
        {
            //修改数据
            if (obj is TEntity entity)
            {
                this.SetVersionNo(obj);
                TEntity e;
                try
                {
                    if (this.IsAutoUpdate)
                    {
                        e = await this.AutoUpdateAsync(entity);
                    }
                    else
                    {
                        e = await this.GetRepository().UpdateAsync(entity);
                    }
                    this.SaveSnapshot(e);
                    return e;
                }
                catch (Exception ex)
                {
                    var message = new
                    {
                        oldDate = this.SnapshotEntity,
                        newData = entity,
                        name = typeof(TEntity).Name
                    };
                    this._logger.LogError(ex, this.JsonSerialize(message));
                    throw new RepositoryException($"Modify  {typeof(TEntity).Name} database failed");
                }
            }
            else
            {
                throw new RepositoryException($"ModifyAsync：entity is not the same type as {typeof(TEntity).Name}");
            }
        }
        public async Task<object> ReadAsync(object id)
        {
            id = this.ConvertPrimaryKey(id);
            if (id == null)
            {
                return null;
            }

            var emtity = await this.GetRepository().GetAsync((TPrimaryKey)id);
            this.SaveSnapshot(emtity);
            return emtity;
        }

        public Task<TEntity> AutoInsertAsync(TEntity entity)
        {
            return this.DoAsync((db) =>
            {
                var e = db.Add(entity);
                db.SaveChanges();
                this.SaveSnapshot(e.Entity);
                return e.Entity;
            });
        }
        public Task<TEntity> AutoUpdateAsync(TEntity entity)
        {
            IEntityChangeManager changeManager = this.GetChangeManagerAsync(entity);
            return this.DoAsync((db) =>
            {
                db.Update(changeManager, entity);
                db.SaveChanges();
                return entity;
            });
        }
        public Task AutoDeleoteAsync(TEntity entity)
        {
            return this.DoAsync((db) =>
            {
                db.Delete(entity);
                int count = db.SaveChanges();
                return count;
            });
        }

        public Task<T> DoAsync<T>(Func<DbContext, T> func)
        {
            using (var db = _dbContextFactory.CreateDbContext<TEntity>())
            {
                var ret = func.Invoke(db);
                return Task.FromResult(ret);
            }
        }
        private IRepository<TEntity, TPrimaryKey> GetRepository()
        {
            var repository = this._serviceProvider.GetRequiredServiceByName<IRepository>(typeof(TEntity).Name);
            return (IRepository<TEntity, TPrimaryKey>)repository;
        }
        public void SetVersionNo(object entity)
        {
            if (entity == null) return;
            //判断是否需要设置版本号
            if (typeof(IStorageEntity).IsInstanceOfType(entity))
            {
                ((IStorageEntity)entity).VersionNo++;
            }
        }
        public void SaveSnapshot(TEntity entity)
        {
            if (entity == null)
                return;
            if (!this.IsAutoUpdate)
                return;
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, entity);
            stream.Position = 0;
            this.SnapshotEntity = (TEntity)formatter.Deserialize(stream);
        }

        public IEntityChangeManager GetChangeManagerAsync(TEntity newEntity)
        {
            if (this.SnapshotEntity.VersionNo != (newEntity.VersionNo - 1))
                throw new RepositoryException("Entity modification failed, version number is inconsistent");
            var changeManager = this._changeDetector.DetectChanges(newEntity, this.SnapshotEntity, newEntity.VersionNo);
            return changeManager;
        }

        private string JsonSerialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{typeof(TEntity).Name} Json Serialize failure ");
                return "";
            }
        }

        private object ConvertPrimaryKey(object id)
        {
            if (id.GetType() == typeof(TPrimaryKey))
                return id;

            if (id.GetType() == typeof(long) &&
                typeof(TPrimaryKey) == typeof(int))
            {
                return Convert.ToInt32(id); //转换成32 Int
            }
            return null;
        }
    }
}
