using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau;

internal sealed class TableauService : ITableauService
{
    private readonly HttpClient _http;
    private readonly ILogger<TableauService> _logger;
    private readonly string _siteId;
    private readonly AsyncRetryPolicy _policy;

    public TableauService(HttpClient http, IConfiguration cfg, ILogger<TableauService> logger)
    {
        _http = http;
        _logger = logger;
        _siteId = cfg["Tableau:SiteId"] ?? "";

        _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt * 2),
                (ex, ts, retry, ctx) =>
                {
                    _logger.LogWarning(ex,
                        "Error pushing to Tableau (attempt {Retry}/{Max})", retry, 3);
                });
    }

    public async Task PushDatasetAsync(
        string projectName,
        string datasourceName,
        string jsonDataset,
        CancellationToken ct)
    {
        await _policy.ExecuteAsync(async _ =>
        {
            var req = new
            {
                datasourceName,
                projectName,
                jsonDataset
            };

            var res = await _http.PostAsJsonAsync(
                $"/api/3.19/sites/{_siteId}/data", req, ct);

            res.EnsureSuccessStatusCode();
            _logger.LogInformation("Dataset for {DatasourceName} pushed to Tableau", datasourceName);
        }, ct);
    }
}