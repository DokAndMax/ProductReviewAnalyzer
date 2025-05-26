using ProductReviewAnalyzer.HistoryService.Application.Interfaces;

namespace ProductReviewAnalyzer.HistoryService.Infrastructure.Persistence;

internal sealed class UnitOfWork(HistoryDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    public void Dispose() => db.Dispose();
}