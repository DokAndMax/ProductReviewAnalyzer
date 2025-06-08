namespace ProductReviewAnalyzer.AnalysisService.Application.DTOs;

public sealed class AspectItemData
{
    public string Text { get; init; } = default!;
    public string Category { get; init; } = default!;
    public double SentimentScore { get; init; }

    public AspectItemData(string text, string category, double sentimentScore)
    {
        Text = text;
        Category = category;
        SentimentScore = sentimentScore;
    }
}
