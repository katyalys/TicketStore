namespace Catalog.Domain.Interfaces
{
    public interface IRedisRepository
    {
        void ExpiredKeyNotification();
        Task<bool> RemoveAsync(string key);
        Task<bool> ExistsAsync(string key, TimeSpan expiresAt);
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresAt) where T : class;
        Task<bool> UpdateAsync<T>(string key, T value, TimeSpan expiresAt) where T : class;
        Task<T> GetAsync<T>(string key) where T : class;
        TimeSpan? TimeToExpire(string key);
        Task<Dictionary<string, T>> GetListAsync<T>() where T : class;
    }
}
