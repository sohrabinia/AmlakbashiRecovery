using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using StackExchange.Redis;
using log4net;

namespace Amlakbashi.Core.Common.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache cache;
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly ILog logger;
        public CacheManager(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer, ILog logger)
        {
            this.cache = cache;
            this.connectionMultiplexer = connectionMultiplexer;
            this.logger = logger;
        }

        public T Set<T>(string key, T value)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30),
                    SlidingExpiration = TimeSpan.FromDays(1)
                };
                cache.SetString(key, JsonConvert.SerializeObject(value), options);
                return value;
            }
            catch (Exception exc)
            {
                logger.Error("CacheManager.Set", exc);
                return default;
            }
            
        }

        public T Get<T>(string key)
        {
            try
            {
                var value = cache.GetString(key);
                if (value != null)
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
                return default;
            }
            catch (Exception exc)
            {
                logger.Error("CacheManager.Get", exc);
                return default;
            }
            
        }

        public void Remove(string key)
        {
            try
            {
                cache.Remove(key);
            }
            catch (Exception exc)
            {
                logger.Error("CacheManager.Remove", exc);
            }
            
        }

        public void Clear()
        {
            try
            {
                var endpoints = connectionMultiplexer.GetEndPoints();
                var server = connectionMultiplexer.GetServer(endpoints[0]);
                server.FlushAllDatabases();
            }
            catch (Exception exc)
            {
                logger.Error("CacheManager.Clear", exc);
            }
        }
    }

    public enum CacheNames
    {
        Category_Item_
    }
}
