namespace ProductReviewAnalyzer.Contracts.DTOs;

public sealed class UsageInsightItemDto(string text, string category)
{
    public string Text { get; init; } = text;
    public string Category { get; init; } = category;
}
