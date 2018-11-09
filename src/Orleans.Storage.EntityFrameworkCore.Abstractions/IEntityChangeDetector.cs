namespace Orleans.Storage.EntityFrameworkCore
{
    /// <summary>
    /// 变动探测器
    /// </summary>
    public interface IEntityChangeDetector
    {
        /// <summary>
        /// 检查变动
        /// </summary>
        /// <param name="newEntry">新实体</param>
        /// <param name="oldEntry">旧实体</param>
        /// <returns></returns>
        IEntityChangeManager DetectChanges(object newEntry, object oldEntry,int versionNo);
    }
}
