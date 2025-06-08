using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Domain.Entities;

public class ReviewAnalysis
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid RequestId { get; init; }
    public long ReviewId { get; init; }
    public long ProductId { get; init; }
    public string ProductTitle { get; init; } = default!;
    public string Store { get; init; } = default!;
    public string UserTitle { get; init; } = default!;
    public int? Mark { get; init; }
    public string? Dignity { get; init; }
    public string? Shortcomings { get; init; }
    public bool FromBuyer { get; init; }
    public DateTime ReviewCreatedAt { get; init; }
    public string RawText { get; init; } = default!;

    public Sentiment ProductSentiment { get; set; }
    public double ProductSentimentScore { get; set; }
    public string ProductSummary { get; set; } = default!;
    public IReadOnlyList<string> ProductEmotions { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ProductKeywords { get; set; } = Array.Empty<string>();

    public IReadOnlyList<AspectItem> ProductPros { get; set; } = Array.Empty<AspectItem>();
    public IReadOnlyList<AspectItem> ProductCons { get; set; } = Array.Empty<AspectItem>();
    public IReadOnlyList<UsageInsightItem> ProductUsageInsights { get; set; } = Array.Empty<UsageInsightItem>();
    public IReadOnlyList<AspectSentimentItem> ProductAspectSentiments { get; set; } = Array.Empty<AspectSentimentItem>();


    public Sentiment StoreSentiment { get; set; }
    public double StoreSentimentScore { get; set; }

    public IReadOnlyList<AspectItem> StorePros { get; set; } = Array.Empty<AspectItem>();
    public IReadOnlyList<AspectItem> StoreCons { get; set; } = Array.Empty<AspectItem>();

    public DateTime AnalysisCreatedAtUtc { get; init; } = DateTime.UtcNow;
}