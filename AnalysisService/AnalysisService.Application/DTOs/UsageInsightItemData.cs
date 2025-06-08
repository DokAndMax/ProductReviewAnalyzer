namespace ProductReviewAnalyzer.AnalysisService.Application.DTOs;

public sealed class UsageInsightItemData(string text, string category)
{
    public string Text { get; init; } = text;
    public string Category { get; init; } = category;
}
