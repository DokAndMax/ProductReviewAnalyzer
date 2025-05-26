using MassTransit;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.Contracts;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Logging;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau;

namespace ProductReviewAnalyzer.TableauExporterService.Features.PubSub.Consumers;

public sealed class AnalysisCompletedConsumer(
    ITableauService tableau,
    ILogRepository loggerRepo,
    ILogger<AnalysisCompletedConsumer> logger)
    : IConsumer<AnalysisCompleted>
{
    public async Task Consume(ConsumeContext<AnalysisCompleted> ctx)
    {
        var evt = ctx.Message;
        logger.LogInformation("Exporting analysis {Id} to Tableau", evt.AnalysisId);

        var dataset = DatasetBuilder.Build(evt);

        try
        {
            await tableau.PushDatasetAsync("ProductReview", "ReviewAnalyses", dataset, ctx.CancellationToken);

            await loggerRepo.InsertAsync(new LogEntry
            {
                AnalysisId = evt.AnalysisId,
                Result = "Success"
            }, ctx.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to export {Id}", evt.AnalysisId);

            await loggerRepo.InsertAsync(new LogEntry
            {
                AnalysisId = evt.AnalysisId,
                Result = "Failure",
                Details = ex.Message
            }, ctx.CancellationToken);

            throw;
        }
    }
}