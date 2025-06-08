using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.Commands;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Consumers;

public sealed class ReviewScrapedConsumer(IMediator mediator, ILogger<ReviewScrapedConsumer> logger) : IConsumer<ReviewScraped>
{
    private readonly IMediator mediator = mediator;
    private readonly ILogger<ReviewScrapedConsumer> logger = logger;

    public async Task Consume(ConsumeContext<ReviewScraped> ctx)
    {
        var m = ctx.Message;

        try
        {
            logger.LogInformation(
                "Processing review: ReviewId={ReviewId}", m.ReviewId);

            await mediator.Send(new AnalyzeReviewCommand(
                m.RequestId,
                m.ReviewId,
                m.ProductId,
                m.ProductTitle,
                m.Store,
                m.UserTitle,
                m.Mark,
                m.Text,
                m.Dignity,
                m.Shortcomings,
                m.FromBuyer,
                m.CreatedAt), ctx.CancellationToken);

            logger.LogInformation(
                "Analysis completed: ReviewId={ReviewId}", m.ReviewId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error analyzing review: ReviewId={ReviewId}, ProductId={ProductId}, Store={Store}",
                m.ReviewId, m.ProductId, m.Store);
        }
    }
}