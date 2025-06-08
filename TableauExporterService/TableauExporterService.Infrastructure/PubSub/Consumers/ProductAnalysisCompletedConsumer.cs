using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.Contracts.Events;
using ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer;
using ProductReviewAnalyzer.TableauExporterService.Features.BatchDataset;
using ProductReviewAnalyzer.TableauExporterService.Features.Logging;
using ProductReviewAnalyzer.TableauExporterService.Features.Publishing;
using MediatR;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.PubSub.Consumers;

public sealed class ProductAnalysisCompletedConsumer(
    IAnalysisBuffer buffer,
    IMediator mediator,
    ILogRepository logRepo,
    IHttpClientFactory httpFactory,
    ILogger<ProductAnalysisCompletedConsumer> logger
) : IConsumer<ProductAnalysisCompleted>
{
    private readonly IAnalysisBuffer buffer = buffer;
    private readonly IMediator mediator = mediator;
    private readonly ILogRepository logRepo = logRepo;
    private readonly IHttpClientFactory httpFactory = httpFactory;
    private readonly ILogger<ProductAnalysisCompletedConsumer> logger = logger;

    public async Task Consume(ConsumeContext<ProductAnalysisCompleted> context)
    {
        var msg = context.Message;
        var requestId = msg.RequestId;
        var productsId = msg.ProductsId;

        var analyses = buffer.Drain(requestId);
        if (analyses.Count == 0)
        {
            logger.LogWarning("No buffered analyses found for RequestId {RequestId}", requestId);
            return;
        }

        logger.LogInformation("Building & publishing batch for RequestId {RequestId} ({Count} reviews)",
            requestId, analyses.Count);

        string dashboardUrl;
        try
        {
            var hyperBytes = await mediator.Send(
                new BuildBatchDatasetCommand(requestId, productsId, analyses),
                context.CancellationToken);

            var datasourceName = $"Product_{requestId}_Reviews";
            dashboardUrl = await mediator.Send(
                new PublishToTableauCommand(requestId, hyperBytes, datasourceName),
                context.CancellationToken);

            await logRepo.InsertAsync(new LogEntry
            {
                AnalysisId = requestId,
                Result = "Success",
                Details = $"Published dashboard URL: {dashboardUrl}"
            }, context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish dashboard for RequestId {RequestId}", requestId);

            await logRepo.InsertAsync(new LogEntry
            {
                AnalysisId = requestId,
                Result = "Failure",
                Details = ex.Message
            }, context.CancellationToken);

            await NotifyTrackerAsync(requestId, false, null, context.CancellationToken);
            throw;
        }

        await NotifyTrackerAsync(requestId, true, dashboardUrl, context.CancellationToken);
    }

    private async Task NotifyTrackerAsync(Guid requestId, bool isSuccess, string? dashboardUrl, CancellationToken ct)
    {
        try
        {
            var client = httpFactory.CreateClient();
            client.BaseAddress = new Uri("http://requestsservice:8080/");

            var payload = new
            {
                DashboardUrl = dashboardUrl,
                IsSuccess = isSuccess
            };

            var response = await client.PostAsJsonAsync($"/internal/requests/{requestId}/complete", payload, ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("NotifyTracker: POST to {Url} returned {StatusCode}",
                    client.BaseAddress + $"/internal/requests/{requestId}/complete",
                    response.StatusCode);
            }
            else
            {
                logger.LogInformation("NotifyTracker: Successfully notified AnalysisTrackerService for {RequestId}", requestId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while notifying AnalysisTrackerService for {RequestId}", requestId);
        }
    }
}
