using Microsoft.AspNetCore.SignalR;
using ProductReviewAnalyzer.AnalysisTrackerService.API.Hubs;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

namespace ProductReviewAnalyzer.AnalysisTrackerService.API.Services;

internal sealed class SignalRAnalysisNotifier(IHubContext<AnalysisHub> hub) : IAnalysisNotifier
{
    public Task NotifyCompletedAsync(Guid userId, Guid requestId, string? dashboardUrl, int status, CancellationToken ct)
        => hub.Clients.User(userId.ToString())
            .SendAsync("RequestCompleted", requestId, dashboardUrl, status, ct);
}