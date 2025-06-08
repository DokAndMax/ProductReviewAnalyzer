namespace ProductReviewAnalyzer.TableauExporterService.Features.Logging;

public class LogEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AnalysisId { get; init; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public string Result { get; init; } = default!;
    public string? Details { get; init; }
}
