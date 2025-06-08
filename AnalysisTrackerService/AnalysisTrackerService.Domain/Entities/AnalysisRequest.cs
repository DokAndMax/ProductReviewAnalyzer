using ProductReviewAnalyzer.AnalysisTrackerService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Domain.Entities;

public class AnalysisRequest
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public RequestStatus Status { get; private set; } = RequestStatus.Pending;
    public string? DashboardUrl { get; private set; }

    public ICollection<AnalysisItem> Items { get; init; } = new List<AnalysisItem>();

    public void MarkCompleted(string dashboardUrl)
    {
        Status = RequestStatus.Success;
        DashboardUrl = dashboardUrl;
    }

    public void MarkFailed() => Status = RequestStatus.Failed;
}