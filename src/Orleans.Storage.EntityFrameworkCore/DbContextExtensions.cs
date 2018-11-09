using Microsoft.EntityFrameworkCore;
using Orleans.Storage.EntityFrameworkCore.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Orleans.Storage.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// 删除实体
        /// </summary>
        public static void Delete(this DbContext context, object entity)
        {
            if (entity == null)
                throw new RepositoryException("Delete entity Can not be empty");

            var Properties = entity.GetType().GetProperties()
               .Where(f => typeof(IStorageEntity).IsAssignableFrom(f.PropertyType)).ToList();
            if (Properties!=null && Properties.Count>0)
            {
                foreach (var item in Properties)
                {
                    var value = item.GetValue(entity);
                    Delete(context, value);
                }
            }

            //获取所有的继承IList的属性
            Properties = entity.GetType().GetProperties()
                  .Where(f => f.PropertyType.FullName.Contains("System.Collections.Generic.IList`1")
                  && typeof(IStorageEntity).IsAssignableFrom(f.PropertyType.GenericTypeArguments[0])).ToList();

            if (Properties != null && Properties.Count > 0)
            {
                foreach (var item in Properties)
                {
                    var values = item.GetValue(entity);
                    if (values == null)
                        continue;
                    foreach (var value in ((IList)values))
                    {
                        Delete(context, value);
                    }
                }
            }
            //标记删除状态
            context.Remove(entity);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static void Update(this DbContext context, IEntityChangeManager changeManager, object entry)
        {
            if (entry == null)
                throw new RepositoryException("Update newestEntity and originalEntity Can not be empty");

            //删除
            var removes = changeManager.GetDifferences(EntityChangeType.Remove);
            if (removes != null && removes.Count > 0)
            {
                foreach (var remove in removes)
                {
                    context.Delete(remove.OldEntity);
                }
            }

            List<int> addEntity = new List<int>();
            //遍历图形 设置对象修改或者添加
            context.ChangeTracker.TrackGraph(entry, e =>
            {
                int sourceHashCode = e.SourceEntry?.Entity.GetHashCode() ?? 0;
                int entityHashCode = e.Entry.Entity.GetHashCode();
                EntityDifference change = changeManager.GetDifference(entityHashCode);
                //如果上级添加，子级也添加
                if (addEntity.Contains(sourceHashCode))
                {
                    addEntity.Add(e.Entry.Entity.GetHashCode());
                    e.Entry.State = EntityState.Added;
                    return;
                }
                if (change == null)
                {
                    e.Entry.State = EntityState.Unchanged;
                    return;
                }
                if (change.Type == EntityChangeType.Addition)
                {
                    addEntity.Add(entityHashCode);
                    e.Entry.State = EntityState.Added;
                    return;
                }
                else
                {
                    e.Entry.State = EntityState.Unchanged;
                    foreach (var item in change.ChangePropertys)
                    {
                        e.Entry.Member(item.Name).IsModified = true;
                    }
                }
            });


        }

        /// <summary>
        /// 进行EF变动跟踪脱机处理，现在设置为Transient的，暂时无需调用此方法
        /// </summary>
        /// <param name="context"></param>
        private static void DetachedChangeTracker(this DbContext context)
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            //清除所有的EF的变动跟踪器
            foreach (var item in context.ChangeTracker.Entries().ToList())
            {
                try
                {
                    item.State = EntityState.Detached;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
