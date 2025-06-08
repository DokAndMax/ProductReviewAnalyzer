namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

public interface IAnalysisNotifier
{
    Task NotifyCompletedAsync(Guid userId,
        Guid requestId,
        string? dashboardUrl,
        int status,
        CancellationToken ct);
}