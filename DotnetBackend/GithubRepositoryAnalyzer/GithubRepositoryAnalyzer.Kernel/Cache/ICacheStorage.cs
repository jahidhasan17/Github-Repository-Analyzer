namespace GithubRepositoryAnalyzer.Kernel.Cache;

public interface ICacheStorage<TValue>
{
    bool TryGetValue(string key, out TValue value);
    
    void AddOrUpdate(string key, TValue value);

    IEnumerable<TValue> GetValuesByPattern(string pattern);
}