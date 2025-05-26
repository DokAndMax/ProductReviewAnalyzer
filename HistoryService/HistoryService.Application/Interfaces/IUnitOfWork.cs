namespace ProductReviewAnalyzer.HistoryService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}