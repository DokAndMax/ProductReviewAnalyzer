using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Logging;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddHttpClient<ITableauService, TableauService>(client =>
        {
            client.BaseAddress = new Uri(cfg["Tableau:BaseUrl"] ?? "https://tableau.example.com");
            client.DefaultRequestHeaders.Add("X-Tableau-Auth", cfg["Tableau:ApiToken"]);
        });

        services.AddSingleton<ILogRepository, MongoLogRepository>();

        return services;
    }
}