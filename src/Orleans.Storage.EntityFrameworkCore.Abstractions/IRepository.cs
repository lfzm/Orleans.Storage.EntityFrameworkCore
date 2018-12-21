using System.Threading.Tasks;
using System;

namespace Orleans.Storage.EntityFrameworkCore
{
    public interface IRepository:IDisposable
    {

    }
    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : class, IStorageEntity
    {
        /// <summary>
        /// 获取仓储
        /// </summary>
        /// <param name="id">标识ID</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(TPrimaryKey id);
        /// <summary>
        /// 修改存储
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        /// <summary>
        /// 插入存储
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);
        /// <summary>
        /// 删除存储
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);
    }
}
