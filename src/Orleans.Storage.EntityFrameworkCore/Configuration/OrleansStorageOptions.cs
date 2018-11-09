using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Storage.EntityFrameworkCore
{
    public class OrleansStorageOptions
    {
        public Action<ComparisonConfig> CompareNetObjectsConfig { get; set; }
    }
}
