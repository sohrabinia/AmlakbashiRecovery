using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Amlakbashi.Core.Common.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache cache;
        public CacheManager(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public T Set<T>(string key, T value)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };

            cache.SetString(key, JsonConvert.SerializeObject(value), options);
            return value;
        }

        public T Get<T>(string key)
        {
            var value = cache.GetString(key);

            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }
    }

    public enum CacheNames
    {
        CategoryItem
    }
}
