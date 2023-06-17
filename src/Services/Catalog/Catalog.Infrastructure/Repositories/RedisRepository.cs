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
        private readonly ConnectionMultiplexer _redis;

        public RedisRepository()
        {
            _db = _redis.GetDatabase();
        }

        /// <summary>
        /// Remove value from redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Remove(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Remove multiple values from redis
        /// </summary>
        /// <param name="keys"></param>
        public async Task Remove(RedisKey[] keys)
        {
            await _db.KeyDeleteAsync(keys);
        }

        /// <summary>
        /// Check if key is exist in redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Exists(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public TimeSpan? TimeToExpire(string key)
        {
            return _db.KeyTimeToLive(key);
        }


        /// <summary>
        /// Dispose DB connection
        /// </summary>
        public void Stop()
        {
            _redis.Dispose();
        }

        /// <summary>
        /// Add new record in redis 
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type T</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public async Task<bool> Add<T>(string key, T value, TimeSpan expiresAt) where T : class
        {
            var stringContent = SerializeContent(value);
            return await _db.StringSetAsync(key, stringContent, expiresAt);
        }

        /// <summary>
        /// Add new record in redis 
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public async Task<bool> Add(string key, object value, TimeSpan expiresAt)
        {
            var stringContent = SerializeContent(value);
            return await _db.StringSetAsync(key, stringContent, expiresAt);
        }

        /// <summary>
        /// Add new record in redis 
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type T</param>
        /// <returns>true or false</returns>
        public async Task<bool> Update<T>(string key, T value) where T : class
        {
            var stringContent = SerializeContent(value);
            return await _db.StringSetAsync(key, stringContent);
        }

        /// <summary>
        /// Get value of key, return one object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(string key) where T : class
        {

            try
            {
                RedisValue myString = await _db.StringGetAsync(key);
                if (myString.HasValue && !myString.IsNullOrEmpty)
                {
                    return DeserializeContent<T>(myString);
                }

                return null;
            }
            catch (Exception)
            {
                // Log Exception
                return null;
            }
        }

        ///// <summary>
        ///// Get all values of key, return list as you can send key in pattern format 
        ///// (article:*) get all articles.  
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public List<T> GetList<T>(string key) where T : class
        //{
        //    try
        //    {
        //        //var server = _redis.GetServer(host: RedisClientConfigurations.Url,
        //        //                              port: RedisClientConfigurations.Port);
        //        var keys = Connection.GetServer.Keys(_db.Database, key);
        //        var keyValues = _db.StringGet(keys.ToArray());

        //        var values = (from redisValue in keyValues
        //                      where redisValue.HasValue && !redisValue.IsNullOrEmpty
        //                      select DeserializeContent<T>(redisValue)).ToList();


        //        return values;
        //    }
        //    catch (Exception)
        //    {
        //        // Log Exception
        //        return null;
        //    }
        //}

        // serialize and Deserialize content in separate functions as redis can save value as array of binary. 
        // so, any time you need to change the way of handling value, do it here.

        private string SerializeContent(object value)
        {
            return JsonSerializer.Serialize(value);
        }

        private T DeserializeContent<T>(RedisValue myString)
        {
            return JsonSerializer.Deserialize<T>(myString);
        }
    }
}
