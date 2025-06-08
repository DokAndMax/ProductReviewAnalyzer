using ProductReviewAnalyzer.Contracts.Events;
using System.Text.Json;

namespace ProductReviewAnalyzer.TableauExporterService.Features.BatchDataset;

public static class BatchDatasetBuilder
{
    public static string Build(
        Guid requestId,
        ICollection<long> productsId,
        IReadOnlyList<ReviewAnalyzed> analyses)
    {
        var payload = new
        {
            RequestId = requestId,
            ProductIds = productsId,
            ReviewCount = analyses.Count,
            Reviews = analyses.Select(a => new
            {
                a.RequestId,
                a.ReviewId,
                a.ProductId,
                a.Store,
                a.CreatedAtUtc,
                a.ProductSentimentScore,
                a.StoreSentimentScore,
                a.ProductSummary,
                a.ProductEmotions,              // List<string>
                a.ProductKeywords,              // List<string>
                a.ProductPros,                  // List<AspectItemDto>
                a.ProductCons,                  // List<AspectItemDto>
                a.ProductUsageInsights,         // List<UsageInsightItemDto>
                a.ProductAspectSentiments,      // List<AspectSentimentItemDto>
                a.StorePros,                    // List<AspectItemDto>
                a.StoreCons,                    // List<AspectItemDto>
                a.AnalyzedAtUtc
            })
        };

        return JsonSerializer.Serialize(payload);
    }
}
