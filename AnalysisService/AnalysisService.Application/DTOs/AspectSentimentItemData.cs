namespace ProductReviewAnalyzer.AnalysisService.Application.DTOs;

public sealed class AspectSentimentItemData
{
    public string Aspect { get; init; } = default!;
    public string Sentiment { get; init; } = default!;
    public double SentimentScore { get; init; }

    public AspectSentimentItemData(string aspect, string sentiment, double sentimentScore)
    {
        Aspect = aspect;
        Sentiment = sentiment;
        SentimentScore = sentimentScore;
    }
}
