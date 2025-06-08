namespace ProductReviewAnalyzer.TableauExporterService.Features.Logging;

public interface ILogRepository
{
    Task InsertAsync(LogEntry entry, CancellationToken ct);
}
