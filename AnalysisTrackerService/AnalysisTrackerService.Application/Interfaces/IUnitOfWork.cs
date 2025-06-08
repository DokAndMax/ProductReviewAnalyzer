namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}