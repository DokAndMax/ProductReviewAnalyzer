using Microsoft.EntityFrameworkCore;

namespace ProductReviewAnalyzer.Common.Persistence;

/// <summary>
/// Generic-реалізація IDatabaseMigrator для будь-якого DbContext.
/// </summary>
public class DatabaseMigrator<TContext>(TContext db) : IDatabaseMigrator
    where TContext : DbContext
{
    public Task MigrateAsync(CancellationToken cancellationToken = default)
        => db.Database.MigrateAsync(cancellationToken);
}