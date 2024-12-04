namespace GithubRepositoryAnalyzer.EventMessaging.Worker.Services;

public interface ICacheStorage<TValue>
{
    bool TryGetValue(string key, out TValue value);
    
    void AddOrUpdate(string key, TValue value);
    
    TValue GetOrAdd(string key, Func<TValue> initializationFunc);
}