namespace ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;

public record ReviewScrapedData(
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
    public ReviewScrapedData() :
        this(-1, string.Empty, string.Empty, -1, string.Empty, -1, string.Empty, string.Empty, string.Empty, false, DateTime.MinValue)
    { }
}
