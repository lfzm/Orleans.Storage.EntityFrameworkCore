using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.Storage.EntityFrameworkCore;
using Orleans.Storage.EntityFrameworkCore.ChangeDetector;
using System;

namespace Orleans
{
    public static class RepositorySiloBuilderExtensions
    {
        /// <summary>
        /// 添加存储服务
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="builer"><see cref="StorageBuilder"/> 配置</param>
        /// <param name="storageName">Storage的名称</param>
        /// <returns></returns>
        public static IServiceCollection AddEFStorage(this IServiceCollection services, Action<IStorageBuilder> builer, string storageName = GrainStorage.DefaultName)
        {
            //配置差异对比服务
            services.TryAddSingleton<IEntityChangeDetector, EntityChangeDetector>();
            services.TryAddSingleton<IEntityChangeManagerFactory, EntityChangeManagerFactory>();
      
            //配置Orleans 的存储配置
            services.AddTransientNamedService<IGrainStorage, GrainStorage>(storageName);

            //对应实体的仓储配置
            var builder = new StorageBuilder(services);
            builer.Invoke(builder);
            return services;
        }

    }
}
