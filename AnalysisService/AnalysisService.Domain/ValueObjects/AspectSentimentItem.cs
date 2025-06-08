namespace ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

public sealed class AspectSentimentItem(string aspect, Sentiment sentiment, double sentimentScore)
{
    public string Aspect { get; init; } = aspect;
    public Sentiment Sentiment { get; init; } = sentiment;
    public double SentimentScore { get; init; } = sentimentScore;
}
