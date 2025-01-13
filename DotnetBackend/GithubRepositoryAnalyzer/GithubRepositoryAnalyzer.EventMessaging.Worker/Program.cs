using System.Text.Json.Serialization;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.EventMessaging.Worker;
using GithubRepositoryAnalyzer.EventMessaging.Worker.Diagnostics;
using GithubRepositoryAnalyzer.EventMessaging.Worker.GithubUserSearchService;
using GithubRepositoryAnalyzer.EventMessaging.Worker.RepositorySearchService;
using GithubRepositoryAnalyzer.Kernel.Extensions;
using MassTransit;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SearchRepositoryOnUserNetworkConsumer>();
    x.AddConsumer<SearchRepositoriesConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
        
        cfg.UseMessageRetry(retryConfig =>
        {
            retryConfig.Interval(2, TimeSpan.FromSeconds(1));
        });
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["ConnectionSettings:Redis"]!));

builder.Services.AddStackExchangeRedisCache(
    options =>
    {
        options.Configuration = builder.Configuration["ConnectionSettings:Redis"]!;
    });

builder.Services.AddHttpClient();
builder.Services.AddRedisCacheStorage<GithubUser>();
builder.Services.AddRedisCacheStorage<List<string>>();
builder.Services.AddRedisCacheStorage<SearchRepositoryResult>();
builder.Services.AddScoped<IGithubUserSearchService, GithubUserSearchService>();
builder.Services.AddScoped<IRepositorySearchService, RepositorySearchService>();

builder.AddOpenTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();