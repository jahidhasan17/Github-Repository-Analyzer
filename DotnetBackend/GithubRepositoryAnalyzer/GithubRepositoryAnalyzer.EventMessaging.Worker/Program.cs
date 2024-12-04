using System.Text.Json.Serialization;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.EventMessaging.Worker;
using GithubRepositoryAnalyzer.EventMessaging.Worker.Services;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
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

builder.Services.AddHttpClient();
builder.Services.AddRedisCacheStorage<GithubUser>();
builder.Services.AddRedisCacheStorage<List<string>>();
builder.Services.AddRedisCacheStorage<SearchRepositoryResult>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();