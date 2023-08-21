using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisIntegraionWithAPI.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _cacheDb;
        private readonly IConnectionMultiplexer _redis;


        public CacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _cacheDb = _redis.GetDatabase();
        }
        public async Task<T> GetData<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public async Task<object> RemoveData(string key)
        {
            var isExists = await _cacheDb.KeyExistsAsync(key);
            if (isExists)
            {
                return await _cacheDb.KeyDeleteAsync(key);
            }
            return false;
        }

        public async Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expirtyTime);
        }
    }
}