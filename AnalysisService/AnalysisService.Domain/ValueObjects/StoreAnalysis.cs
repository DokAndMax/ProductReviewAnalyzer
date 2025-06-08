namespace ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

public sealed class StoreAnalysis(
    Sentiment sentiment,
    double sentimentScore,
    IReadOnlyList<AspectItem> pros,
    IReadOnlyList<AspectItem> cons)
{
    public Sentiment Sentiment { get; init; } = sentiment;
    public double SentimentScore { get; init; } = sentimentScore;

    public IReadOnlyList<AspectItem> Pros { get; init; } = pros;
    public IReadOnlyList<AspectItem> Cons { get; init; } = cons;
}
