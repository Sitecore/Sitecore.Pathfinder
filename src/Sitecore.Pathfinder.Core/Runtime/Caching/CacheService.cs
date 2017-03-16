// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Runtime.Caching
{
    [Export(typeof(ICacheService))]
    public class CacheService : ICacheService
    {
        [NotNull]
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public T Get<T>(string cacheKey)
        {
            object value;
                
            if (!_cache.TryGetValue(cacheKey, out value))
            {
                return default(T);
            }

            if (value is T)
            {
                return (T)value;
            }

            return default(T);
        }

        public void Set(string cacheKey, object value)
        {
            _cache[cacheKey] = value;
        }
    }
}
