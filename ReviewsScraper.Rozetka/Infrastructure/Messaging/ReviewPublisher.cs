using MassTransit;
using ProductReviewAnalyzer.Contracts;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Messaging;

public interface IReviewPublisher
{
    Task PublishAsync(Review review, CancellationToken ct);
}

public sealed class ReviewPublisher(IPublishEndpoint bus, ILogger<ReviewPublisher> logger) : IReviewPublisher
{
    public Task PublishAsync(Review review, CancellationToken ct)
    {
        var evt = new NewReviewReceived(
            review.Id,
            review.ProductId,
            review.UserTitle,
            review.Mark,
            review.Text,
            review.Dignity,
            review.Shortcomings,
            review.FromBuyer,
            review.CreatedAt);

        logger.LogDebug("Publishing NewReviewReceived for review {ReviewId}", review.Id);
        return bus.Publish(evt, ct);
    }
}