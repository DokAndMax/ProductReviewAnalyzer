namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;

public interface IWorkbookPublisher
{
    Task<string> PublishWorkbookAsync(
        string workbookName,
        string datasourceContentUrl,
        string datasourceName,
        bool overwrite,
        CancellationToken cancellationToken);
}
