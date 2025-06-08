using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;
using System.Net.Http.Headers;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure.OpenAI.Http;

internal sealed class OpenAIClient : IOpenAIClient
{
    private readonly HttpClient http;
    private readonly ILogger<OpenAIClient> logger;
    private readonly string apiKey;

    public OpenAIClient(
        HttpClient http,
        IConfiguration configuration,
        ILogger<OpenAIClient> logger)
    {
        this.http = http;
        this.logger = logger;
        apiKey = configuration["OpenAI:ApiKey"]
                  ?? throw new ArgumentException("OpenAI:ApiKey is not configured");

        this.http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> GetRawResponseAsync(string jsonRequestPayload, CancellationToken cancellationToken = default)
    {
        using var content = new StringContent(jsonRequestPayload);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        logger.LogDebug("Posting to OpenAI endpoint...");
        using var response = await http
            .PostAsync("/v1/chat/completions", content, cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
        logger.LogDebug(
            "OpenAI HTTP status: {StatusCode}, Body: {BodySnippet}",
            response.StatusCode,
            raw.Length > 200 ? raw[..200] + "…" : raw);

        response.EnsureSuccessStatusCode();
        return raw;
    }
}
