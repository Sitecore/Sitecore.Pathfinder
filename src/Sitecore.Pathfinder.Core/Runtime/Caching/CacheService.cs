// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Runtime.Caching;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Runtime.Caching
{
    [Export(typeof(ICacheService))]
    public class CacheService : ICacheService
    {
        [NotNull]
        private readonly MemoryCache _cache;

        public CacheService()
        {
            _cache = new MemoryCache("CacheService");
        }

        public T Get<T>(string cacheKey)
        {
            var value = _cache.Get(cacheKey);
            if (value == null)
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
            _cache.Set(cacheKey, value, new CacheItemPolicy());
        }
    }
}
