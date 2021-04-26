using System;
using Microsoft.Extensions.Caching.Memory;

namespace PidPlugin.Cache
{
    public class MemoryCacheAccessor : ICacheAccessor
    {
        private readonly IMemoryCache memoryCache;

        public MemoryCacheAccessor(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache ??
                throw new ArgumentNullException(nameof(memoryCache));
        }

        public bool TryGetValue(string key, out object value)
        {
            return this.memoryCache.TryGetValue(key, out value);
        }

        public void Set(string key, object value, TimeSpan expirationTime)
        {
            System.Diagnostics.Debug.WriteLine("SETTING");
            this.memoryCache.Set(key, value, expirationTime);
        }
    }
}
