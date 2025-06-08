using ProductReviewAnalyzer.AnalysisService.Application;
using ProductReviewAnalyzer.AnalysisService.Infrastructure;
using ProductReviewAnalyzer.AnalysisService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddMessaging(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")
               ?? throw new InvalidOperationException("Postgres connection string not configured"), name: "postgres")
    .AddRabbitMQHealthChecks(builder.Configuration);


var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();