using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;
using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Application.Services;

public sealed class OpenAIOrchestrator(
    IOpenAIPromptBuilder promptBuilder,
    IOpenAIClient client,
    IOpenAIResponseParser responseParser,
    ILogger<OpenAIOrchestrator> logger) : IOpenAIService
{
    private readonly IOpenAIPromptBuilder _promptBuilder = promptBuilder;
    private readonly IOpenAIClient _client = client;
    private readonly IOpenAIResponseParser _responseParser = responseParser;
    private readonly ILogger<OpenAIOrchestrator> _logger = logger;

    public async Task<ReviewAnalysisResult> AnalyzeAsync(
        string reviewText,
        CancellationToken cancellationToken = default)
    {
        var payload = _promptBuilder.BuildPayload(reviewText);

        _logger.LogInformation("Sending request to OpenAI …");
        var rawJson = await _client.GetRawResponseAsync(payload, cancellationToken);
        _logger.LogDebug("Received raw JSON from OpenAI: {RawJson}", rawJson);

        var result = _responseParser.Parse(rawJson);
        _logger.LogInformation(
            "Parsed OpenAI response into ReviewAnalysisResult: ProductSentiment={ProductSentiment}, StoreSentiment={StoreSentiment}",
            result.Product.Sentiment,
            result.Store.Sentiment);

        return result;
    }
}
