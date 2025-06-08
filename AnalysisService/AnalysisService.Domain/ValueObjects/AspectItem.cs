namespace ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

public sealed class AspectItem(string text, string category, double sentimentScore)
{
    public string Text { get; init; } = text;
    public string Category { get; init; } = category;
    public double SentimentScore { get; init; } = sentimentScore;
}
