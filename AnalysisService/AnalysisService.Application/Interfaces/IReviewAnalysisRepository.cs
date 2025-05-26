using ProductReviewAnalyzer.AnalysisService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisService.Application.Interfaces;

public interface IReviewAnalysisRepository
{
    Task InsertAsync(ReviewAnalysis entity, CancellationToken ct);
}