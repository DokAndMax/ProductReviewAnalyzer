namespace ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;

public record ReviewDto(
    long Id,
    IReadOnlyList<long> ProductIds,
    string UserTitle,
    int? Mark,
    string Text,
    bool FromBuyer,
    DateTime CreatedAt);