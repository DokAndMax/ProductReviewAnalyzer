using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Application.Interfaces;

public interface IReviewPublisher
{
    Task PublishAsync(Guid requestId, ReviewScrapedData review, CancellationToken ct);
    Task PublishScrapingCompletedAsync(Guid requestId, long productId, int totalComments, CancellationToken ct);
}