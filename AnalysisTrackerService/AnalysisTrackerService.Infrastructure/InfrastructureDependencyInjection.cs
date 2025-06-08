using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisTrackerService.Infrastructure.Persistence;
using ProductReviewAnalyzer.Common.Persistence;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var conn = cfg.GetConnectionString("SqlServer")
                   ?? throw new InvalidOperationException("SqlServer connection string not configured");

        services.AddDbContextWithMigrator<AnalysisDbContext>(o =>
            o.UseSqlServer(conn, sql => sql.EnableRetryOnFailure()));

        services.AddScoped<IAnalysisRepository, AnalysisRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}