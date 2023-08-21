using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisIntegraionWithAPI.Services
{
    public interface ICacheService
    {
        Task<T> GetData<T>(string key);
        Task<bool> SetData<T>(string key, T value , DateTimeOffset expirationTime);
        Task<object> RemoveData(string key);

    }
}