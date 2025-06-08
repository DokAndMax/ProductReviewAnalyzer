namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;

public interface ITableauAuthenticator
{
    Task EnsureAuthenticatedAsync(CancellationToken ct);

    HttpClient HttpClient { get; }

    string AuthToken { get; }

    string SiteId { get; }
}