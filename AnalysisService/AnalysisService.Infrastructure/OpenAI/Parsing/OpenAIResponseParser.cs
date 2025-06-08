using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;
using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;
using System.Text.Json;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure.OpenAI.Parsing;

internal sealed class OpenAIResponseParser(ILogger<OpenAIResponseParser> logger) : IOpenAIResponseParser
{
    private readonly ILogger<OpenAIResponseParser> logger = logger;

    public ReviewAnalysisResult Parse(string rawJson)
    {
        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;

        var contentElement = root
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content");

        var jsonString = contentElement.GetString() ?? throw new InvalidOperationException("Empty content");

        logger.LogDebug("Parsing inner JSON content from Chat response…");

        JsonElement parsedInner;
        try
        {
            parsedInner = JsonSerializer.Deserialize<JsonElement>(jsonString);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to parse nested JSON: {JsonString}", jsonString);
            throw;
        }

        var productEl = parsedInner.GetProperty("product");
        var productSentiment = MapSentiment(productEl.GetProperty("sentiment").GetString()!);
        var productScore = productEl.GetProperty("sentiment_score").GetDouble();
        var productSummary = productEl.GetProperty("summary").GetString()!;

        var productEmotions = productEl
            .GetProperty("emotions")
            .EnumerateArray()
            .Select(x => x.GetString()!)
            .ToList();

        var productKeywords = productEl
            .GetProperty("keywords")
            .EnumerateArray()
            .Select(x => x.GetString()!)
            .ToList();

        var productPros = ParseAspectList(productEl.GetProperty("pros"));
        var productCons = ParseAspectList(productEl.GetProperty("cons"));
        var usageInsights = ParseUsageList(productEl.GetProperty("usage_insights"));
        var aspectSentiments = ParseAspectSentimentList(productEl.GetProperty("aspect_sentiments"));

        var productAnalysis = new ProductAnalysis(
            productSentiment,
            productScore,
            productSummary,
            productEmotions,
            productKeywords,
            productPros,
            productCons,
            usageInsights,
            aspectSentiments
        );

        var storeEl = parsedInner.GetProperty("store");
        var storeSentiment = MapSentiment(storeEl.GetProperty("sentiment").GetString()!);
        var storeScore = storeEl.GetProperty("sentiment_score").GetDouble();
        var storePros = ParseAspectList(storeEl.GetProperty("pros"));
        var storeCons = ParseAspectList(storeEl.GetProperty("cons"));

        var storeAnalysis = new StoreAnalysis(
            storeSentiment,
            storeScore,
            storePros,
            storeCons
        );

        return new ReviewAnalysisResult(productAnalysis, storeAnalysis);
    }

    private static Sentiment MapSentiment(string s) => s switch
    {
        "positive" => Sentiment.Positive,
        "negative" => Sentiment.Negative,
        _ => Sentiment.Neutral
    };

    private static List<AspectItem> ParseAspectList(JsonElement arrayElement)
    {
        var list = new List<AspectItem>();
        foreach (var item in arrayElement.EnumerateArray())
        {
            var text = item.GetProperty("text").GetString()!;
            var category = item.GetProperty("category").GetString()!;
            var score = item.GetProperty("sentiment_score").GetDouble();
            list.Add(new AspectItem(text, category, score));
        }
        return list;
    }

    private static List<UsageInsightItem> ParseUsageList(JsonElement arrayElement)
    {
        var list = new List<UsageInsightItem>();
        foreach (var item in arrayElement.EnumerateArray())
        {
            var text = item.GetProperty("text").GetString()!;
            var category = item.GetProperty("category").GetString()!;
            list.Add(new UsageInsightItem(text, category));
        }
        return list;
    }

    private static List<AspectSentimentItem> ParseAspectSentimentList(JsonElement arrayElement)
    {
        var list = new List<AspectSentimentItem>();
        foreach (var item in arrayElement.EnumerateArray())
        {
            var aspect = item.GetProperty("aspect").GetString()!;
            var sentiment = MapSentiment(item.GetProperty("sentiment").GetString()!);
            var score = item.GetProperty("sentiment_score").GetDouble();
            list.Add(new AspectSentimentItem(aspect, sentiment, score));
        }
        return list;
    }
}
