using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer;
using ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Buffer;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ITableauAuthenticator, TableauAuthenticator>();
builder.Services.AddScoped<IDatasourcePublisher, DatasourcePublisher>();
builder.Services.AddScoped<IWorkbookPublisher, WorkbookPublisher>();

builder.Services.AddSingleton<IAnalysisBuffer, InMemoryAnalysisBuffer>();

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("Mongo")
        ?? throw new InvalidOperationException("Mongo connection string not configured")));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ProductReviewAnalyzer.TableauExporterService.Features.Publishing.PublishToTableauCommand).Assembly);
});

builder.Services
    .AddHealthChecks()
    .AddMongoDb(
        clientFactory: sp => sp.GetRequiredService<IMongoClient>(),
        name: "mongo",
        failureStatus: HealthStatus.Unhealthy
    );

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();