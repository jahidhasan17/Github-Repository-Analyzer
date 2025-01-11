using System.Diagnostics.Metrics;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker.Diagnostics;

public static class ApplicationDiagnostics
{
    private const string ServiceName = "GithubRepositoryAnalyzer.Api";
    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> SearchStartedCounter = Meter.CreateCounter<long>("search.created");
}