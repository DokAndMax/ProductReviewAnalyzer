namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

public interface IAnalysisBatchPublisher
{
    Task PublishBatchStartedAsync(Guid requestId, int totalProducts, CancellationToken ct);
}
