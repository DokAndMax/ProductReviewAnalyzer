namespace ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

public sealed class ProductAnalysis(
    Sentiment sentiment,
    double sentimentScore,
    string summary,
    IReadOnlyList<string> emotions,
    IReadOnlyList<string> keywords,
    IReadOnlyList<AspectItem> pros,
    IReadOnlyList<AspectItem> cons,
    IReadOnlyList<UsageInsightItem> usageInsights,
    IReadOnlyList<AspectSentimentItem> aspectSentiments
        )
{
    public Sentiment Sentiment { get; init; } = sentiment;
    public double SentimentScore { get; init; } = sentimentScore;
    public string Summary { get; init; } = summary;

    public IReadOnlyList<string> Emotions { get; init; } = emotions;
    public IReadOnlyList<string> Keywords { get; init; } = keywords;

    public IReadOnlyList<AspectItem> Pros { get; init; } = pros;
    public IReadOnlyList<AspectItem> Cons { get; init; } = cons;
    public IReadOnlyList<UsageInsightItem> UsageInsights { get; init; } = usageInsights;
    public IReadOnlyList<AspectSentimentItem> AspectSentiments { get; init; } = aspectSentiments;
}
