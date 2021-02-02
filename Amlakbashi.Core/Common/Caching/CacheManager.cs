using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Caching
{
    public class CacheManager<T> : ICacheManager<T>
    {
        IRedisTypedClient<T> redisClient;
        public CacheManager(/*RedisManagerPool redisManager*/)
        {
            //var redis = redisManager.GetClient();
            //redisClient = redis.As<T>();
        }

        public T Set(T entity)
        {
            return redisClient.Store(entity);
        }

        public T Get(object id)
        {
            return redisClient.GetById(id);
        }

        public bool Remove(object id)
        {
            try
            {
                redisClient.DeleteById(id);
                return true;
            }
            catch(Exception exc)
            {
                return false;
            }
        }
    }
}
