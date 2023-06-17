using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Interfaces
{
    public interface IRedisRepository
    {
        Task<bool> Remove(string key);
        Task Remove(RedisKey[] keys);
        Task<bool> Exists(string key);
        public void Stop();
        Task<bool> Add<T>(string key, T value, TimeSpan expiresAt) where T : class;
        Task<bool> Add(string key, object value, TimeSpan expiresAt);
        Task<bool> Update<T>(string key, T value) where T : class;
        Task<T> Get<T>(string key) where T : class;
        TimeSpan? TimeToExpire(string key);
    }
}
