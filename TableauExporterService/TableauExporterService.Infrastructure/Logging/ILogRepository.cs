namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Logging;

public interface ILogRepository
{
    Task InsertAsync(LogEntry entry, CancellationToken ct);
}