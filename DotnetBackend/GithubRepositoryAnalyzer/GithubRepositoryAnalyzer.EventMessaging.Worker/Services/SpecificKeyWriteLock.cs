using System.Collections.Concurrent;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker.Services;

public class SpecificKeyWriteLock
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> @locks = new();

    public SemaphoreSlim Lock(string key)
    {
        var semaphore = locks.GetOrAdd(key, _ => new SemaphoreSlim(1));
        semaphore.Wait();

        return semaphore;
    }

    public async Task<SemaphoreSlim> LockAsync(string key)
    {
        var semaphore = locks.GetOrAdd(key, _ => new SemaphoreSlim(1));
        await semaphore.WaitAsync();

        return semaphore;
    }

    public void Unlock(string key, SemaphoreSlim semaphoreSlim)
    {
        semaphoreSlim.Release();
        locks.TryRemove(key, out _);
    }
}
