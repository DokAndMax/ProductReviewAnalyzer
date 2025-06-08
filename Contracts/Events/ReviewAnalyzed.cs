using ProductReviewAnalyzer.Contracts.DTOs;

namespace ProductReviewAnalyzer.Contracts.Events;

public record ReviewAnalyzed(
    Guid RequestId,
    long ReviewId,
    long ProductId,
    string Store,
    DateTime CreatedAtUtc,

    decimal ProductSentimentScore,
    decimal StoreSentimentScore,

    string ProductSummary,

    IReadOnlyList<string> ProductEmotions,
    IReadOnlyList<string> ProductKeywords,

    IReadOnlyList<AspectItemDto> ProductPros,
    IReadOnlyList<AspectItemDto> ProductCons,
    IReadOnlyList<UsageInsightItemDto> ProductUsageInsights,
    IReadOnlyList<AspectSentimentItemDto> ProductAspectSentiments,

    IReadOnlyList<AspectItemDto> StorePros,
    IReadOnlyList<AspectItemDto> StoreCons,

    DateTime AnalyzedAtUtc
)
{
    public ReviewAnalyzed() :
        this(Guid.Empty, -1, -1, string.Empty,
            DateTime.MinValue, -1, -1,
            string.Empty, [], [], [], [], [], [], [],
            [], DateTime.MinValue)
    { }
}