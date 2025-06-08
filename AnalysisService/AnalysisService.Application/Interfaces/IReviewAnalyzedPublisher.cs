using ProductReviewAnalyzer.AnalysisService.Application.DTOs;

namespace ProductReviewAnalyzer.AnalysisService.Application.Interfaces;

public interface IReviewAnalyzedPublisher
{
    Task PublishReviewAnalyzedAsync(ReviewAnalyzedData reviewAnalyzedData, CancellationToken ct);
}
