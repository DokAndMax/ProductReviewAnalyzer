namespace ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

public sealed class ReviewAnalysisResult(ProductAnalysis product, StoreAnalysis store)
{
    public ProductAnalysis Product { get; init; } = product;
    public StoreAnalysis Store { get; init; } = store;
}