using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Application.Interfaces;

/// <summary>
/// Абстракція над викликом ChatGPT / Assistants API
/// </summary>
public interface IOpenAIService
{
    Task<(Sentiment prodSent,
            Sentiment storeSent,
            IReadOnlyList<string> prodPros,
            IReadOnlyList<string> prodCons,
            IReadOnlyList<string> prodCategories,
            IReadOnlyList<string> prodUsage,
            IReadOnlyList<string> storePros,
            IReadOnlyList<string> storeCons)>
        AnalyzeAsync(string reviewText, CancellationToken ct);
}