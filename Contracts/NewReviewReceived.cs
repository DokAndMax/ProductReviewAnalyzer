namespace ProductReviewAnalyzer.Contracts;

public record NewReviewReceived(
    long Id,
    long ProductId,
    string UserTitle,
    int? Mark,
    string Text,
    string? Dignity,
    string? Shortcomings,
    bool FromBuyer,
    DateTime CreatedAt);