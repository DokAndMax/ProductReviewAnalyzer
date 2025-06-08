using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.Common.Persistence;
using ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Persistence;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReviewsScraperInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        var conn = config.GetConnectionString("SqlServer")
                   ?? throw new InvalidOperationException("SqlServer connection string not configured");

        services.AddDbContextWithMigrator<ReviewsDbContext>(opt => opt.UseSqlServer(conn, options =>
        {
            options.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }));

        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}