﻿using System.Diagnostics;
using OpenTelemetry.Context.Propagation;

namespace GithubRepositoryAnalyzer.Diagnostics;

public static class RabbitMqDiagnostics
{
    public static readonly string ActivitySourceName = "RabbitMQ";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    public static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
}
