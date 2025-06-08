using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure.Persistence;

public static class MartenExtensions
{
    public static IServiceCollection AddMartenStore(this IServiceCollection services, IConfiguration cfg)
    {
        var conn = cfg.GetConnectionString("Postgres")
                   ?? throw new InvalidOperationException("Postgres connection string not configured");

        services.AddMarten(opts =>
        {
            opts.Connection(conn);
            opts.AutoCreateSchemaObjects = AutoCreate.All;
        });

        return services;
    }
}