using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Storage.EntityFrameworkCore
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    public interface IRepositoryCore
    {
        /// <summary>
        /// 读取仓储
        /// </summary>
        /// <param name="id">标识Id</param>
        /// <returns></returns>
        Task<object> ReadAsync(object id);
        /// <summary>
        /// 写入仓储
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<object> AddAsync(object entity);
        /// <summary>
        /// 修改仓储
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<object> ModifyAsync(object id, object entity);
        /// <summary>
        /// 删除仓储
        /// </summary>
        /// <param name="entity">实体信息</param>
        /// <returns></returns>
        Task ClearAsync(object entity);
    }
}
