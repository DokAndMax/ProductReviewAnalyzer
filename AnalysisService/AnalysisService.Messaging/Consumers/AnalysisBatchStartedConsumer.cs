using MassTransit;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Consumers;

public class AnalysisBatchStartedConsumer : IConsumer<AnalysisBatchStartedConsumer>
{
    public async Task Consume(ConsumeContext<AnalysisBatchStartedConsumer> ctx)
    {
    }
}