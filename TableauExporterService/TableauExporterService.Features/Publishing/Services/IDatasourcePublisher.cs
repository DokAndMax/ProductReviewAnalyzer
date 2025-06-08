namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;

public interface IDatasourcePublisher
{
    Task<string> PublishHyperAsync(
        string projectName,
        string datasourceName,
        byte[] hyperBytes,
        CancellationToken cancellationToken);
}