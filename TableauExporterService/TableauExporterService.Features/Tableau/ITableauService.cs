namespace ProductReviewAnalyzer.TableauExporterService.Features.Tableau;

public interface ITableauService
{
    Task<string> PushDatasetAsync(string projectName, string datasourceName, string jsonDataset, CancellationToken ct);
}