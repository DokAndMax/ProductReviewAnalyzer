namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

public interface IUrlDispatchService
{
    Task DispatchAsync(Guid requestId, string store, string productUrl, CancellationToken ct);
}