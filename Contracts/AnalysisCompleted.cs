namespace ProductReviewAnalyzer.Contracts;

public record AnalysisCompleted(
    Guid AnalysisId,
    long ReviewId,
    string GoodsId,
    string Store,
    decimal ProductSentiment,
    decimal StoreSentiment,
    IReadOnlyList<string> ProductPros,
    IReadOnlyList<string> ProductCons,
    IReadOnlyList<string> ProductCategories,
    IReadOnlyList<string> ProductUsageInsights,
    IReadOnlyList<string> StorePros,
    IReadOnlyList<string> StoreCons,
    DateTime AnalyzedAtUtc);
