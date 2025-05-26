namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.DTOs;

public record ReviewDto(
    long Id,
    long ProductId,
    string UserTitle,
    int Mark,
    string Text,
    bool FromBuyer,
    DateTime CreatedAt);