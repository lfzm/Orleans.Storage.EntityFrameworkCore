using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Options;
using Orleans.Storage.EntityFrameworkCore.Model;

namespace Orleans.Storage.EntityFrameworkCore.ChangeDetector
{
    public class EntityChangeDetector : IEntityChangeDetector
    {
        private readonly IEntityChangeManagerFactory changeManagerFactory;
        private readonly OrleansStorageOptions options;

        public EntityChangeDetector(IEntityChangeManagerFactory _changeManagerFactory, IOptions<OrleansStorageOptions> options)
        {
            this.changeManagerFactory = _changeManagerFactory;
            this.options = options.Value;
        }
        public IEntityChangeManager DetectChanges(object newEntry, object oldEntry, int versionNo)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = int.MaxValue;
            compareLogic.Config.CompareStaticFields = false;//静态字段不比较
            compareLogic.Config.CompareStaticProperties = false;//静态属性不比较
            compareLogic.Config.Caching = true;

            if (options != null && options.CompareNetObjectsConfig != null)
            {
                options.CompareNetObjectsConfig(compareLogic.Config);
            }

            var result = compareLogic.Compare(oldEntry, newEntry);
            var entityChange = new EntityChange(oldEntry, newEntry, versionNo);
            return this.changeManagerFactory.Create(entityChange, result);
        }
    }
}
