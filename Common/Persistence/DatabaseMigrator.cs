using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProductReviewAnalyzer.Common.Persistence;

public class DatabaseMigrator<TContext>(TContext db) : IDatabaseMigrator
    where TContext : DbContext
{
    private readonly TContext db = db;

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        var databaseCreator = db.Database.GetService<IRelationalDatabaseCreator>();

        if (!await databaseCreator.ExistsAsync(cancellationToken))
        {
            await db.Database.MigrateAsync(cancellationToken);
            return;
        }

        var pending = await db.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pending.Any())
        {
            await db.Database.MigrateAsync(cancellationToken);
        }
    }
}