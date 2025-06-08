namespace ProductReviewAnalyzer.AnalysisService.Application.DTOs;

public record ReviewAnalyzedData(
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

    IReadOnlyList<AspectItemData> ProductPros,
    IReadOnlyList<AspectItemData> ProductCons,
    IReadOnlyList<UsageInsightItemData> ProductUsageInsights,
    IReadOnlyList<AspectSentimentItemData> ProductAspectSentiments,

    IReadOnlyList<AspectItemData> StorePros,
    IReadOnlyList<AspectItemData> StoreCons,

    DateTime AnalyzedAtUtc
)
{
    public ReviewAnalyzedData() :
        this(Guid.Empty, -1, -1, string.Empty, DateTime.MinValue, -1, -1, string.Empty, [], [], [], [], [], [], [],
            [], DateTime.MinValue) {}
}