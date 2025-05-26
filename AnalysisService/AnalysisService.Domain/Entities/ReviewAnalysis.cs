using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Domain.Entities;

public class ReviewAnalysis
{
    // первинний ключ документа Marten
    public Guid Id { get; init; } = Guid.NewGuid();

    // дані самого відгуку
    public long ReviewId { get; init; }
    public long ProductId { get; init; }
    public string Store { get; init; } = default!;
    public string UserTitle { get; init; } = default!;
    public int? Mark { get; init; }
    public string? Dignity { get; init; }
    public string? Shortcomings { get; init; }
    public bool FromBuyer { get; init; }
    public DateTime ReviewCreatedAt { get; init; }
    public string RawText { get; init; } = default!;

    public Sentiment ProductSentiment { get; set; }
    public Sentiment StoreSentiment { get; set; }

    public IReadOnlyList<string> ProductPros { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ProductCons { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ProductCategories { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ProductUsageInsights { get; set; } = Array.Empty<string>();

    public IReadOnlyList<string> StorePros { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> StoreCons { get; set; } = Array.Empty<string>();

    public DateTime AnalysisCreatedAtUtc { get; init; } = DateTime.UtcNow;
}