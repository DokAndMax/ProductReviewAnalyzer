using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Buffer;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Logging;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau.HyperFile;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.PubSub;
using ProductReviewAnalyzer.TableauExporterService.Features.Logging;
using ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer;
using ProductReviewAnalyzer.TableauExporterService.Features.Tableau;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddHttpClient();

        services.AddSingleton<IAnalysisBuffer, InMemoryAnalysisBuffer>();

        services.AddSingleton<ILogRepository, MongoLogRepository>();

        services.AddScoped<IHyperFileSchemaFactory, HyperFileSchemaFactory>();
        services.AddScoped<IHyperFileWriter, HyperFileWriter>();
        services.AddScoped<IHyperFileBuilder, HyperFileBuilder>();


        services.AddMessaging(cfg);

        return services;
    }
}
