using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Specification.TicketsSpecifications;

namespace Catalog.Infrastructure.Repositories
{
    public class RedisRepository: IRedisRepository
    {

        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;
        private readonly string _keyPrefix = "basket:";

        public RedisRepository(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        public async Task<bool> Remove(string key)
        {
            return await _db.KeyDeleteAsync(ApplyKeyPrefix(key));
        }

        public async Task<bool> Exists(string key, TimeSpan expiresAt)
        {
            if (await _db.KeyExistsAsync(ApplyKeyPrefix(key)))
            {
                return true;
            }

            return await _db.StringSetAsync(ApplyKeyPrefix(key), string.Empty, expiresAt);
        }

        public TimeSpan? TimeToExpire(string key)
        {
            return _db.KeyTimeToLive(ApplyKeyPrefix(key));
        }

        public async Task<bool> Add<T>(string key, T value, TimeSpan expiresAt) where T : class
        {
            var stringContent = SerializeContent(value);

            return await _db.StringSetAsync(ApplyKeyPrefix(key), stringContent, expiresAt);
        }

        public async Task<bool> Update<T>(string key, T value, TimeSpan expiresAt) where T : class
        {
            var stringContent = SerializeContent(value);

            return await _db.StringSetAsync(ApplyKeyPrefix(key), stringContent, expiresAt);
        }

        public async Task<T> Get<T>(string key) where T : class
        {
            RedisValue myString = await _db.StringGetAsync(ApplyKeyPrefix(key));
            if (myString.HasValue && !myString.IsNullOrEmpty)
            {
                return DeserializeContent<T>(myString);
            }

            return null;
        }

        public async Task<Dictionary<string, T>> GetList<T>() where T : class
        {
            var keys = _redis.GetServer("localhost", 6379).Keys(pattern: "basket:*");

            var keyValues = await _db.StringGetAsync(keys.ToArray());

            var result = new Dictionary<string, T>();

            int i = 0;
            foreach (var key in keys)
            {
                var keyValue = keyValues[i];
                if (keyValue.HasValue && !keyValue.IsNullOrEmpty)
                {
                    var keyString = key.ToString().Substring("basket:".Length);
                    var value = DeserializeContent<T>(keyValue);
                    result.Add(keyString, value);
                }
                i++;
            }

            return result;
        }

        private string SerializeContent(object value)
        {
            return JsonSerializer.Serialize(value);
        }

        private T DeserializeContent<T>(RedisValue myString)
        {
            return JsonSerializer.Deserialize<T>(myString);
        }

        private RedisKey ApplyKeyPrefix(RedisKey key)
        {
            return _keyPrefix + key;
        }
    }
}
