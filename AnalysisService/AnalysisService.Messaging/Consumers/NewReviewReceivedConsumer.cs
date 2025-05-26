using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.Commands;
using ProductReviewAnalyzer.Contracts;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Consumers;

public sealed class NewReviewReceivedConsumer(IMediator mediator, ILogger<NewReviewReceivedConsumer> logger) : IConsumer<NewReviewReceived>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<NewReviewReceivedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<NewReviewReceived> ctx)
    {
        var m = ctx.Message;
        _logger.LogInformation("Аналізуємо ReviewId={ReviewId}", m.Id);

        await _mediator.Send(new AnalyzeReviewCommand(
            m.Id,
            m.ProductId,
            m.UserTitle,
            m.Mark,
            m.Text,
            m.Dignity,
            m.Shortcomings,
            m.FromBuyer,
            m.CreatedAt), ctx.CancellationToken);

        _logger.LogInformation("Аналіз завершено ReviewId={ReviewId}", m.Id);
    }
}