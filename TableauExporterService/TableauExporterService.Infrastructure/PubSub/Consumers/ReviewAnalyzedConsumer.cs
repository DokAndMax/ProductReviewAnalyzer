using MassTransit;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.Contracts.Events;
using ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.PubSub.Consumers;

public sealed class ReviewAnalyzedConsumer(
    IAnalysisBuffer buffer,
    ILogger<ReviewAnalyzedConsumer> logger) : IConsumer<ReviewAnalyzed>
{
    private readonly IAnalysisBuffer buffer = buffer;
    private readonly ILogger<ReviewAnalyzedConsumer> logger = logger;

    public Task Consume(ConsumeContext<ReviewAnalyzed> context)
    {
        var msg = context.Message;
        buffer.Add(msg);
        logger.LogInformation("Buffered ReviewAnalyzed for RequestId {RequestId}, ProductId {ProductId}",
            msg.RequestId, msg.ProductId);
        return Task.CompletedTask;
    }
}
