using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.Common.Persistence;
using ProductReviewAnalyzer.HistoryService.Application.Interfaces;
using ProductReviewAnalyzer.HistoryService.Infrastructure.Persistence;

namespace ProductReviewAnalyzer.HistoryService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var conn = cfg.GetConnectionString("SqlServer")
                   ?? throw new InvalidOperationException("SqlServer connection string not configured");

        services.AddDbContextWithMigrator<HistoryDbContext>(o =>
            o.UseSqlServer(conn, sql => sql.EnableRetryOnFailure()));

        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}