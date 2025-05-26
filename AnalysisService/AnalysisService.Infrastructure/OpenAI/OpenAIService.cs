using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure.OpenAI;

internal sealed class OpenAIService(HttpClient http, IConfiguration cfg, ILogger<OpenAIService> logger)
    : IOpenAIService
{
    private readonly ILogger<OpenAIService> _logger = logger;
    private readonly string _model = cfg["OpenAI:Model"] ?? "gpt-4o-mini";

    public async Task<(Sentiment prodSent,
                       Sentiment storeSent,
                       IReadOnlyList<string> prodPros,
                       IReadOnlyList<string> prodCons,
                       IReadOnlyList<string> prodCategories,
                       IReadOnlyList<string> prodUsage,
                       IReadOnlyList<string> storePros,
                       IReadOnlyList<string> storeCons)>
        AnalyzeAsync(string reviewText, CancellationToken ct)
    {
        var req = new
        {
            model = _model,
            temperature = 0.2,
            messages = new[]
            {
                new { role = "system", content =
@"You are an e-commerce review analysis assistant.
Return ONLY valid minified JSON with the following schema:
{
  ""product"": {
    ""sentiment"": ""positive|neutral|negative"",
    ""pros"": [""string""...],
    ""cons"": [""string""...],
    ""categories"": [""string""...],
    ""usage_insights"": [""string""...]        // everyday usage, nuances, tips
  },
  ""store"": {
    ""sentiment"": ""positive|neutral|negative"",
    ""pros"": [""string""...],
    ""cons"": [""string""...]
  }
}" },
                new { role = "user",   content = reviewText }
            }
        };

        var res = await http.PostAsJsonAsync("/v1/chat/completions", req, ct);
        res.EnsureSuccessStatusCode();

        var root = await res.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: ct);
        var jsonStr = root!.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content").GetString()!;

        var parsed = JsonSerializer.Deserialize<JsonElement>(jsonStr);

        Sentiment Map(string s) => s switch
        {
            "positive" => Sentiment.Positive,
            "negative" => Sentiment.Negative,
            _ => Sentiment.Neutral
        };

        var prod = parsed.GetProperty("product");
        var store = parsed.GetProperty("store");

        IReadOnlyList<string> Arr(JsonElement e, string prop) =>
            e.GetProperty(prop).EnumerateArray().Select(x => x.GetString()!).ToList();

        return (
            Map(prod.GetProperty("sentiment").GetString()!),
            Map(store.GetProperty("sentiment").GetString()!),
            Arr(prod, "pros"),
            Arr(prod, "cons"),
            Arr(prod, "categories"),
            Arr(prod, "usage_insights"),
            Arr(store, "pros"),
            Arr(store, "cons"));
    }
}
