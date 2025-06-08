namespace ProductReviewAnalyzer.WebApp.Shared.Models;

public record CreateRequestDto(Guid UserId, IReadOnlyList<string> ProductUrls);