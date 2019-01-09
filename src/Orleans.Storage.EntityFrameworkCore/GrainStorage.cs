using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace Orleans.Storage.EntityFrameworkCore
{
    public class GrainStorage : IGrainStorage
    {
        public const string DefaultName = EFStorage.DefaultName;
        private IServiceProvider ServiceProvider;
        private IRepositoryCore repositoryStorage;
        private readonly ILogger Logger;
        public GrainStorage(IServiceProvider serviceProvider, ILogger<GrainStorage> logger)
        {
            ServiceProvider = serviceProvider;
            this.Logger = logger;
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            await this.GetRepository(grainState).ClearAsync(grainState.State);
            grainState.State = null;
            this.SetETag(grainState);
        }
        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            object id = this.GetPrimaryKeyObject(grainReference);
            grainState.State = await this.GetRepository(grainState).ReadAsync(id);
            this.SetETag(grainState);
        }
        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainState == null)
                throw new RepositoryException("修改的状态对象不能为空");

            if (grainState.ETag.Equals("0"))
                grainState.State = await this.GetRepository(grainState).AddAsync(grainState.State);
            else
            {
                try
                {
                    object id = this.GetPrimaryKeyObject(grainReference);
                    grainState.State = await this.GetRepository(grainState).ModifyAsync(id, grainState.State);
                }
                catch (Exception ex)
                {
                    await this.ReadStateAsync(grainType, grainReference, grainState);
                    throw ex;
                }
            }
            this.SetETag(grainState);
        }

        private void SetETag(IGrainState grainState)
        {
            if (grainState.State == null)
                grainState.ETag = "0";

            else
                grainState.ETag = "1";
        }
        private IRepositoryCore GetRepository(IGrainState grainState)
        {
            if (repositoryStorage != null)
                return repositoryStorage;

            repositoryStorage = ServiceProvider.GetServiceByName<IRepositoryCore>(grainState.State.GetType().Name);
            if (repositoryStorage == null)
                throw new RepositoryException(string.Format("{0} State Repository Unrealized", grainState.State.GetType().Name));
            else
                return repositoryStorage;
        }

        /// <summary>
        /// 获取 PrimaryKey
        /// </summary>
        /// <param name="grainReference"></param>
        /// <returns></returns>
        public object GetPrimaryKeyObject(IAddressable addressable)
        {
            var key = addressable.GetPrimaryKeyString();
            if (key != null)
                return key;
            if (addressable.IsPrimaryKeyBasedOnLong())
            {
                var key1 = addressable.GetPrimaryKeyLong();
                if (key1 > 0)
                    return key1;
            }
            return addressable.GetPrimaryKey();

        }
    }


}
