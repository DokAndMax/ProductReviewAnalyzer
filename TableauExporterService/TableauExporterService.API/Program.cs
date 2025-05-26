using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using ProductReviewAnalyzer.TableauExporterService.Features.PubSub.Consumers;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<AnalysisCompletedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ")
                 ?? throw new InvalidOperationException("RabbitMQ connection string not configured"));
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("Mongo")!));

builder.Services
    .AddHealthChecks()
    .AddMongoDb(
        clientFactory: sp => sp.GetRequiredService<IMongoClient>(),
        name: "mongo",
        failureStatus: HealthStatus.Unhealthy
    );

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();