using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

namespace ProductReviewAnalyzer.AnalysisTrackerService.API.Services;

internal sealed class HttpUrlDispatchService(HttpClient http, IConfiguration cfg, ILogger<HttpUrlDispatchService> log)
    : IUrlDispatchService
{
    public async Task DispatchAsync(Guid requestId, string store, string productUrl, CancellationToken ct)
    {
        var baseUrl = cfg[$"Scrapers:{store}:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            log.LogWarning("Scraper base URL not configured for {Store}", store);
            return;
        }

        var endpoint = $"{baseUrl}/api/scraper/{store}/reviews";
        var payload = new { Url = productUrl, RequestId = requestId };

        var resp = await http.PostAsJsonAsync(endpoint, payload, ct);
        if (!resp.IsSuccessStatusCode)
        {
            log.LogError("Dispatch to {Endpoint} failed: {Status}", endpoint, resp.StatusCode);
        }
    }
}