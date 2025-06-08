using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;

public interface IOpenAIService
{
    Task<ReviewAnalysisResult> AnalyzeAsync(string reviewText, CancellationToken ct);
}