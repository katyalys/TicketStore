using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Interfaces
{
    public interface IRedisRepository
    {
        Task<bool> Remove(string key);
        Task<bool> Exists(string key, TimeSpan expiresAt);
        Task<bool> Add<T>(string key, T value, TimeSpan expiresAt) where T : class;
        Task<bool> Update<T>(string key, T value, TimeSpan expiresAt) where T : class;
        Task<T> Get<T>(string key) where T : class;
        TimeSpan? TimeToExpire(string key);
        Task<Dictionary<string, T>> GetList<T>() where T : class;
    }
}
