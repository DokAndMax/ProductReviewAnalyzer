using MassTransit;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Consumers;

public class ScrapingCompletedConsumer : IConsumer<ScrapingCompleted>
{
    public async Task Consume(ConsumeContext<ScrapingCompleted> ctx)
    {
    }
}