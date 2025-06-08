namespace ProductReviewAnalyzer.AnalysisTrackerService.Domain.Entities;

public class AnalysisItem
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid RequestId { get; set; }

    public AnalysisRequest Request { get; set; } = default!;

    public string ProductUrl { get; set; } = default!;
}