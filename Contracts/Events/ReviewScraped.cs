namespace ProductReviewAnalyzer.Contracts.Events;

public record ReviewScraped(
    Guid RequestId,
    long ReviewId,
    string ProductTitle,
    string Store,
    long ProductId,
    string UserTitle,
    int? Mark,
    string Text,
    string? Dignity,
    string? Shortcomings,
    bool FromBuyer,
    DateTime CreatedAt)
{
    public ReviewScraped() :
        this(Guid.Empty, -1, string.Empty, string.Empty, -1, string.Empty, -1,
            string.Empty, string.Empty, string.Empty, false, DateTime.MinValue)
    { }
}