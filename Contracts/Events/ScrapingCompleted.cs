namespace ProductReviewAnalyzer.Contracts.Events;

public record ScrapingCompleted(
    Guid RequestId,
    long ProductId,
    int TotalComments);
