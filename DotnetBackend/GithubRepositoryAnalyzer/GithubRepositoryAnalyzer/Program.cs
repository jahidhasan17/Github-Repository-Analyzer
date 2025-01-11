using System.Text.Json.Serialization;
using GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.Config.Extensions;
using GithubRepositoryAnalyzer.Diagnostics;
using GithubRepositoryAnalyzer.Domain.Extensions;
using GithubRepositoryAnalyzer.Domain.Services;
using GithubRepositoryAnalyzer.Kernel.Extensions;
using MassTransit;
using StackExchange.Redis;
using SearchRepositoryResult = GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer.SearchRepositoryResult;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.AddDatabaseServices();
builder.Services.AddCors();
builder.Services.AddControllers()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
builder.Services.AddServices();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["ConnectionSettings:Redis"]!));

builder.Services.AddStackExchangeRedisCache(
    options =>
    {
        options.Configuration = builder.Configuration["ConnectionSettings:Redis"]!;
    });

builder.Services.AddRedisCacheStorage<SearchRepositoryResult>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.AddOpenTelemetry();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseCors(policyBuilder => policyBuilder
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapControllers();
    });

using var scope = app.Services.CreateScope();
var databaseMigrateService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrateService>();
await databaseMigrateService.MigrateAsync();

app.Run();