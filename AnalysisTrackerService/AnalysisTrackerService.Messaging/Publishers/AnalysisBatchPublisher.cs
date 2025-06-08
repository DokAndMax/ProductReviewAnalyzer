using MassTransit;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Messaging.Publishers;

internal class AnalysisBatchPublisher(
    IPublishEndpoint bus,
    ILogger<AnalysisBatchPublisher> logger)
    : IAnalysisBatchPublisher
{
    public Task PublishBatchStartedAsync(Guid requestId, int totalProducts, CancellationToken ct)
    {
        var evt = new AnalysisBatchStarted(
            requestId,
            totalProducts);

        logger.LogDebug("Publishing ReviewScraped for AnalysisBatchStarted {AnalysisId}", requestId);
        return bus.Publish(evt, ct);
    }
}
