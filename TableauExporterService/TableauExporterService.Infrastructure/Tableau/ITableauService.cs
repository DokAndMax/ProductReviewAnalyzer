namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau;

public interface ITableauService
{
    Task PushDatasetAsync(string projectName, string datasourceName, string jsonDataset, CancellationToken ct);
}