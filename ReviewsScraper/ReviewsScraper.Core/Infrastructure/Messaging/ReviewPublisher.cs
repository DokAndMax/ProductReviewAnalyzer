using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.Contracts.Events;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.Interfaces;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Messaging;

public sealed class ReviewPublisher(
    IPublishEndpoint bus,
    IMapper mapper,
    ILogger<ReviewPublisher> logger)
    : IReviewPublisher
{
    public Task PublishAsync(Guid requestId, ReviewScrapedData review, CancellationToken ct)
    {
        var evt = mapper.Map<ReviewScraped>((review, requestId));

        logger.LogDebug("Publishing ReviewScraped for review {ReviewId} (AnalysisRequest {RequestId})", review.ReviewId, requestId);
        return bus.Publish(evt, ct);
    }

    public Task PublishScrapingCompletedAsync(Guid requestId, long productId, int totalComments, CancellationToken ct)
    {
        var evt = new ScrapingCompleted(requestId, productId, totalComments);

        logger.LogInformation("Publishing ScrapingCompleted for product {ProductId} (AnalysisRequest {RequestId}) – {Count} comments", productId, requestId, totalComments);
        return bus.Publish(evt, ct);
    }
}

