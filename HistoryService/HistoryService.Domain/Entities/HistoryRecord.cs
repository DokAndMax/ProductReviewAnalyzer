using ProductReviewAnalyzer.HistoryService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.HistoryService.Domain.Entities;

public class HistoryRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Url { get; init; } = default!;
    public DateTime RequestedAtUtc { get; init; }
    public RequestStatus Status { get; set; }
}