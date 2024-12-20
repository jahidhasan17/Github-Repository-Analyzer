using System.Text.Json.Serialization;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.EventMessaging.Worker;
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
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:5002"));

builder.Services.AddStackExchangeRedisCache(
    options =>
    {
        options.Configuration = "localhost:5002";
    });

builder.Services.AddHttpClient();
builder.Services.AddRedisCacheStorage<GithubUser>();
builder.Services.AddRedisCacheStorage<List<string>>();
builder.Services.AddRedisCacheStorage<SearchRepositoryResult>();
builder.Services.AddScoped<IGithubUserSearchService, GithubUserSearchService>();
builder.Services.AddScoped<IRepositorySearchService, RepositorySearchService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();