using KellermanSoftware.CompareNetObjects;
using Orleans.Storage.EntityFrameworkCore.Model;

namespace Orleans.Storage.EntityFrameworkCore
{
    /// <summary>
    /// 状态管理器工厂接口
    /// </summary>
    public interface IEntityChangeManagerFactory
    {
        /// <summary>
        /// 创建状态管理器
        /// </summary>
        /// <param name="comparisonResult">比较结果</param>
        /// <returns></returns>
        IEntityChangeManager Create(EntityChange entityChange, ComparisonResult comparisonResult);
    }
}
