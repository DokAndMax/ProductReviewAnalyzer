using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.DTOs;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Publishers;

internal class ReviewAnalyzedPublisher(
    IPublishEndpoint bus,
    IMapper mapper,
    ILogger<ReviewAnalyzedPublisher> logger)
    : IReviewAnalyzedPublisher
{
    public Task PublishReviewAnalyzedAsync(ReviewAnalyzedData reviewAnalyzedData, CancellationToken ct)
    {
        var evt = mapper.Map<ReviewAnalyzed>(reviewAnalyzedData);

        logger.LogDebug("Publishing ReviewScraped for AnalysisBatchStarted {RequestId}", reviewAnalyzedData.RequestId);
        return bus.Publish(evt, ct);
    }
}