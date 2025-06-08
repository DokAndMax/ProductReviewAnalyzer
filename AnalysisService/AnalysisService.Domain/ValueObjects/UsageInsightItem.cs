namespace ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

public sealed class UsageInsightItem(string text, string category)
{
    public string Text { get; init; } = text;
    public string Category { get; init; } = category;
}
