using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ProductReviewAnalyzer.Common.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContextWithMigrator<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> configureDb)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(configureDb);
            services.AddScoped<IDatabaseMigrator, DatabaseMigrator<TContext>>();
            return services;
        }
    }
}