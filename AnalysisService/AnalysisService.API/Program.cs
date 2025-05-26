using ProductReviewAnalyzer.AnalysisService.Application;
using ProductReviewAnalyzer.AnalysisService.Infrastructure;
using ProductReviewAnalyzer.AnalysisService.Messaging;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddMessaging(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!, name: "postgres")
    .AddRabbitMQ(sp =>
    {
        var cfg = sp.GetRequiredService<IConfiguration>();
        var rabbitUri = cfg.GetConnectionString("RabbitMQ")!;
        var factory = new ConnectionFactory
        {
            Uri = new Uri(rabbitUri)
        };
        return factory.CreateConnectionAsync();
    }, name: "rabbitmq");

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();