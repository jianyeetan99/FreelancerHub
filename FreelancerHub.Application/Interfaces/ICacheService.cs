namespace FreelancerHub.Application.Interfaces;

public interface ICacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,  TimeSpan? slidingExpiry = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);
}