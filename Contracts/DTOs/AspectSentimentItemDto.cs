namespace ProductReviewAnalyzer.Contracts.DTOs;

public sealed class AspectSentimentItemDto(string aspect, string sentiment, double sentimentScore)
{
    public string Aspect { get; init; } = aspect;
    public string Sentiment { get; init; } = sentiment;
    public double SentimentScore { get; init; } = sentimentScore;
}
