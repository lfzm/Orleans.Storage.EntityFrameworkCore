using KellermanSoftware.CompareNetObjects;
using System;

namespace Orleans.Storage.EntityFrameworkCore
{
    public class OrleansStorageOptions
    {
        public Action<ComparisonConfig> CompareNetObjectsConfig { get; set; }
    }
}
