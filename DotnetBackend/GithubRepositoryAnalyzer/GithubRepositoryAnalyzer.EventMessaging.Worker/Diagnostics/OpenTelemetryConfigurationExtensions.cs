using System.Reflection;
using MassTransit.Logging;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker.Diagnostics;

public static class OpenTelemetryConfigurationExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        const string serviceName = "GithubRepositoryAnalyzer.EventMessaging.Worker";
        
        var otlpEndpoint = new Uri(builder.Configuration.GetValue<string>("OTLP_Endpoint")!);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resurce =>
            {
                resurce
                    .AddService(serviceName)
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("service.version",
                            Assembly.GetExecutingAssembly().GetName().Version!.ToString())
                    });
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddNpgsql()
                    .AddRedisInstrumentation()
                    .AddOtlpExporter(option =>
                        option.Endpoint = otlpEndpoint);
            })
            .WithMetrics(metric =>
                metric
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(ApplicationDiagnostics.Meter.Name)
                    .AddOtlpExporter(options =>
                        options.Endpoint = otlpEndpoint)
            )
            .WithLogging(
                logging =>
                    logging.AddOtlpExporter(options =>
                        options.Endpoint = otlpEndpoint));

        return builder;
    }
}